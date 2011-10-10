using System;
using System.Collections.Generic;
using System.Data.SQLite;
using EveApi;

namespace ECM
{
    public static class AccountDatabase
    {
        static SQLiteConnection sqlConnection;
        static string AccountDatabasePath = "Resources/Database/ecmAccounts.db";

        public static bool OpenDatabase()
        {
            if(System.IO.File.Exists(AccountDatabasePath) == false)
            {
                SQLiteConnection.CreateFile(AccountDatabasePath);
            }

            sqlConnection = new SQLiteConnection(string.Format("Data Source={0};version=3", AccountDatabasePath));
            sqlConnection.Open();
            
            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA cache_size=5000";
            cmd.ExecuteNonQuery();
            
            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA count_changes=OFF";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA temp_store=MEMORY";
            cmd.ExecuteNonQuery();

            CreateAccountsTables();
            
            cmd.Dispose();
            return true;
        }

        public static void CloseDatabase()
        {
            if(sqlConnection.State != System.Data.ConnectionState.Open) return;
            
            sqlConnection.Close();
        }

        #region Create Tables
        private static void CreateAccountsTables()
        {
            string createCmd = @"CREATE TABLE IF NOT EXISTS ecmAccounts(KeyID TEXT PRIMARY KEY, VCode TEXT, Expires TEXT, Access TEXT, LogonCount INTEGER, MinPlayed INTEGER,
                                                                        CreateDate TEXT, PaidUntil TEXT)";

            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = createCmd;
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ecmCharacters (
                                    ID                INT     PRIMARY KEY,
                                    AccountID         INT     REFERENCES ecmAccounts ( KeyID ),
                                    AutoUpdate        BOOLEAN,
                                    Name              TEXT,
                                    Race              TEXT,
                                    Bloodline         TEXT,
                                    Ancestry          TEXT,
                                    AccountBalance    REAL,
                                    Skillpoints       INT,
                                    ShipName          TEXT,
                                    ShipTypeID        INT,
                                    ShipTypeName      TEXT,
                                    CorporationID     INT,
                                    Corporation       TEXT,
                                    CorporationDate   TEXT,
                                    AllianceID        INT,
                                    Alliance          TEXT,
                                    AllianceDate      TEXT,
                                    LastKnownLocation TEXT,
                                    SecurityStatus    REAL,
                                    Birthday          TEXT,
                                    Gender            TEXT,
                                    CloneName         TEXT,
                                    CloneSkillpoints  INT,
                                    Intelligence      INT,
                                    Memory            INT,
                                    Perception        INT,
                                    Willpower         INT,
                                    Charisma          INT
                                );";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ecmImplants (
                                    ImplantName  TEXT PRIMARY KEY,
                                    ImplantValue INT
                                );";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ecmCharacterImplants (
                                    CharacterID INT  REFERENCES ecmCharacters ( ID ) MATCH FULL,
                                    ImplantName TEXT REFERENCES ecmImplants ( ImplantName ) MATCH FULL
                                );";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ecmCharacterCertificates (
                                    Records       INTEGER PRIMARY KEY AUTOINCREMENT,
                                    CharacterID   INT     REFERENCES ecmCharacters ( ID ) MATCH FULL,
                                    CertificateID INT
                                );";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ecmCharacterSkills (
                                    Record      INTEGER PRIMARY KEY AUTOINCREMENT,
                                    CharacterID INT     REFERENCES ecmCharacters ( ID ) MATCH FULL,
                                    SkillTypeID INT,
                                    SkillLevel  INT,
                                    Skillpoints INT
                                );";
            cmd.ExecuteNonQuery();
        }
        #endregion

        public static void AddAccount(Account toAdd)
        {
            string createCmd = @"INSERT OR REPLACE INTO ecmAccounts(KeyID, VCode, Expires, Access, LogonCount, MinPlayed, CreateDate, PaidUntil)
                                    VALUES(@KeyID, @VCode, @Expires, @Access, @LogonCount, @MinPlayed, @CreateDate, @PaidUntil)";
            bool mustClose = false;

            if(sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenDatabase();
                mustClose = true;
            }

            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = createCmd;

            cmd.Parameters.AddWithValue("@KeyID", toAdd.KeyID);
            cmd.Parameters.AddWithValue("@VCode", toAdd.VCode);
            cmd.Parameters.AddWithValue("@Expires", toAdd.Expires.ToString());
            cmd.Parameters.AddWithValue("@Access", toAdd.KeyAccess);
            cmd.Parameters.AddWithValue("@LogonCount", toAdd.LogonCount);
            cmd.Parameters.AddWithValue("@MinPlayed", toAdd.LogonMinutes);
            cmd.Parameters.AddWithValue("@CreateDate", toAdd.CreateDate.ToString());
            cmd.Parameters.AddWithValue("@PaidUntil", toAdd.PaidUntil.ToString());

            cmd.ExecuteNonQuery();

            if(mustClose)
            {
                CloseDatabase();
            }
        }

        public static List<Account> GetAllAccounts()
        {
            string selectCmd = "SELECT * FROM ecmAccounts";
            bool mustClose = false;
            List<Account> accounts = new List<Account>();

            if(sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenDatabase();
                mustClose = true;
            }

            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;

            SQLiteDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    string keyID = reader["KeyID"].ToString();
                    string vCode = reader["VCode"].ToString();
                    EveApi.ApiKeyMask access = (EveApi.ApiKeyMask)Enum.Parse(typeof(EveApi.ApiKeyMask), reader["Access"].ToString());
                    DateTime expires = Convert.ToDateTime(reader["Expires"].ToString());

                    Account newAcc = new Account(keyID, vCode, access, expires);

                    newAcc.LogonCount = Convert.ToInt32(reader["LogonCount"].ToString());
                    newAcc.LogonMinutes = Convert.ToInt32(reader["MinPlayed"].ToString());
                    newAcc.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
                    newAcc.PaidUntil = Convert.ToDateTime(reader["PaidUntil"].ToString());

                    accounts.Add(newAcc);

                    // get Characters for account
                    GetAccountCharacters(newAcc);
                }
            }

            if(mustClose)
            {
                CloseDatabase();
            }

            return accounts;
        }

        public static void RemoveAccount (Account toRemove)
        {
            throw new NotImplementedException ();
        }

        internal static void AddCharacter(Character charToAdd)
        {
            // Insert/Update the character
            string createCmd = @"INSERT OR REPLACE INTO ecmCharacters(ID,
                                    AccountID,
                                    AutoUpdate,
                                    Name,
                                    Race,
                                    Bloodline,
                                    Ancestry,
                                    AccountBalance,
                                    Skillpoints,
                                    ShipName,
                                    ShipTypeID,
                                    ShipTypeName,
                                    CorporationID,
                                    Corporation,
                                    CorporationDate,
                                    AllianceID,
                                    Alliance,
                                    AllianceDate,
                                    LastKnownLocation,
                                    SecurityStatus,
                                    Birthday,
                                    Gender,
                                    CloneName,
                                    CloneSkillpoints,
                                    Intelligence,
                                    Memory,
                                    Perception,
                                    Willpower,
                                    Charisma)
                                VALUES(@ID,
                                    @AccountID,
                                    @AutoUpdate,
                                    @Name,
                                    @Race,
                                    @Bloodline,
                                    @Ancestry,
                                    @AccountBalance,
                                    @Skillpoints,
                                    @ShipName,
                                    @ShipTypeID,
                                    @ShipTypeName,
                                    @CorporationID,
                                    @Corporation,
                                    @CorporationDate,
                                    @AllianceID,
                                    @Alliance,
                                    @AllianceDate,
                                    @LastKnownLocation,
                                    @SecurityStatus,
                                    @Birthday,
                                    @Gender,
                                    @CloneName,
                                    @CloneSkillpoints,
                                    @Intelligence,
                                    @Memory,
                                    @Perception,
                                    @Willpower,
                                    @Charisma)";
            bool mustClose = false;

            if(sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenDatabase();
                mustClose = true;
            }

            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = createCmd;

            cmd.Parameters.AddWithValue("@ID", charToAdd.ID);
            cmd.Parameters.AddWithValue("@AccountID", charToAdd.Account.KeyID);
            cmd.Parameters.AddWithValue("@AutoUpdate", charToAdd.AutoUpdate);
            cmd.Parameters.AddWithValue("@Name", charToAdd.Name);
            cmd.Parameters.AddWithValue("@Race", charToAdd.Race);
            cmd.Parameters.AddWithValue("@Bloodline", charToAdd.Bloodline);
            cmd.Parameters.AddWithValue("@Ancestry", charToAdd.Ancestry);
            cmd.Parameters.AddWithValue("@AccountBalance", charToAdd.AccountBalance);
            cmd.Parameters.AddWithValue("@Skillpoints", charToAdd.SkillPoints);
            cmd.Parameters.AddWithValue("@ShipName", charToAdd.ShipName);
            cmd.Parameters.AddWithValue("@ShipTypeID", charToAdd.ShipTypeID);
            cmd.Parameters.AddWithValue("@ShipTypeName", charToAdd.ShipTypeName);
            cmd.Parameters.AddWithValue("@CorporationID", charToAdd.CorporationID);
            cmd.Parameters.AddWithValue("@Corporation", charToAdd.Corporation);
            cmd.Parameters.AddWithValue("@CorporationDate", charToAdd.CorporationDate);
            cmd.Parameters.AddWithValue("@AllianceID", charToAdd.AllianceID);
            cmd.Parameters.AddWithValue("@Alliance", charToAdd.Alliance);
            cmd.Parameters.AddWithValue("@AllianceDate", charToAdd.AllianceDate);
            cmd.Parameters.AddWithValue("@LastKnownLocation", charToAdd.LastKnownLocation);
            cmd.Parameters.AddWithValue("@SecurityStatus", charToAdd.SecurityStatus);
            cmd.Parameters.AddWithValue("@Birthday", charToAdd.Birthday);
            cmd.Parameters.AddWithValue("@Gender", charToAdd.Gender);
            cmd.Parameters.AddWithValue("@CloneName", charToAdd.CloneName);
            cmd.Parameters.AddWithValue("@CloneSkillpoints", charToAdd.CloneSkillPoints);
            cmd.Parameters.AddWithValue("@Intelligence", charToAdd.Attributes.Intelligence);
            cmd.Parameters.AddWithValue("@Memory", charToAdd.Attributes.Memory);
            cmd.Parameters.AddWithValue("@Perception", charToAdd.Attributes.Perception);
            cmd.Parameters.AddWithValue("@Willpower", charToAdd.Attributes.Willpower);
            cmd.Parameters.AddWithValue("@Charisma", charToAdd.Attributes.Charisma);

            cmd.ExecuteNonQuery();

            // Add Implants
            // Link implants

            // Add Skills
            foreach(CharacterSkills skill in charToAdd.Skills)
            {
                cmd = sqlConnection.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO ecmCharacterSkills(CharacterID, SkillTypeID, SkillLevel, Skillpoints) VALUES (@CharacterID, @SkillTypeID, @SkillLevel, @Skillpoints)";

                cmd.Parameters.AddWithValue("@CharacterID", charToAdd.ID);
                cmd.Parameters.AddWithValue("@SkillTypeID", skill.ID);
                cmd.Parameters.AddWithValue("@SkillLevel", skill.Level);
                cmd.Parameters.AddWithValue("@Skillpoints", skill.Skillpoints);

                cmd.ExecuteNonQuery();
            }

            // Add Certificates
            foreach(CharacterCertificates cert in charToAdd.Certificates)
            {
                cmd = sqlConnection.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO ecmCharacterCertificates(CharacterID, CertificateID) VALUES (@CharacterID, @CertificateID)";

                cmd.Parameters.AddWithValue("@CharacterID", charToAdd.ID);
                cmd.Parameters.AddWithValue("@CertificateID", cert.ID);

                cmd.ExecuteNonQuery();
            }

            if(mustClose)
            {
                CloseDatabase();
            }
        }

        internal static void RemoveCharacter(Character charToRemove)
        {
            throw new NotImplementedException();
        }

        internal static void GetAccountCharacters(Account account)
        {
            string selectCmd = "SELECT * FROM ecmCharacters WHERE AccountID = " + account.KeyID;
            bool mustClose = false;

            if(sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenDatabase();
                mustClose = true;
            }

            SQLiteCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;

            SQLiteDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    string name = reader["Name"].ToString();
                    long charID = Convert.ToInt64(reader["ID"].ToString());
                    Character newChar = new Character(account, charID, name);

                    newChar.AutoUpdate = Convert.ToBoolean(reader["AutoUpdate"].ToString());
                    newChar.Race = reader["Race"].ToString();
                    newChar.Bloodline = reader["Bloodline"].ToString();
                    newChar.Ancestry = reader["Ancestry"].ToString();
                    string tmp = reader["AccountBalance"].ToString();
                    newChar.AccountBalance = Convert.ToDouble(tmp);
                    newChar.SkillPoints = Convert.ToInt32(reader["Skillpoints"].ToString());
                    newChar.ShipName = reader["ShipName"].ToString();
                    newChar.ShipTypeID = Convert.ToInt64(reader["ShipTypeID"].ToString());
                    newChar.ShipTypeName = reader["ShipTypeName"].ToString();
                    newChar.CorporationID = Convert.ToInt64(reader["CorporationID"].ToString());
                    newChar.Corporation = reader["Corporation"].ToString();
                    newChar.CorporationDate = Convert.ToDateTime(reader["CorporationDate"].ToString());
                    newChar.AllianceID = Convert.ToInt64(reader["AllianceID"].ToString());
                    newChar.Alliance = reader["Alliance"].ToString();
                    newChar.AllianceDate = Convert.ToDateTime(reader["AllianceDate"].ToString());
                    newChar.LastKnownLocation = reader["LastKnownLocation"].ToString();
                    newChar.SecurityStatus = Convert.ToDouble(reader["SecurityStatus"].ToString());
                    newChar.Birthday = Convert.ToDateTime(reader["Birthday"].ToString());
                    newChar.Gender = reader["Gender"].ToString();
                    newChar.CloneName = reader["CloneName"].ToString();
                    newChar.CloneSkillPoints = Convert.ToInt64(reader["CloneSkillpoints"].ToString());
                    newChar.Attributes.Intelligence = Convert.ToInt32(reader["Intelligence"].ToString());
                    newChar.Attributes.Memory = Convert.ToInt32(reader["Memory"].ToString());
                    newChar.Attributes.Perception = Convert.ToInt32(reader["Perception"].ToString());
                    newChar.Attributes.Willpower = Convert.ToInt32(reader["Willpower"].ToString());
                    newChar.Attributes.Charisma = Convert.ToInt32(reader["Charisma"].ToString());

                    account.AddCharacter(newChar);

                    // Get Implants
                    // Get Skills
                    // Get Certificates
                }
            }

            if(mustClose)
            {
                CloseDatabase();
            }
        }
    }

}

