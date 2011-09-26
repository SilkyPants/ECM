using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ECM.Core
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
            string createCmd = "CREATE TABLE IF NOT EXISTS ecmAccounts(KeyID TEXT PRIMARY KEY, VCode TEXT, Expires TEXT, Access TEXT)";

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
            string createCmd = "INSERT OR REPLACE INTO ecmAccounts(KeyID, VCode, Expires, Access) VALUES(@KeyID, @VCode, @Expires, @Access)";
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

                    accounts.Add(new Account(keyID, vCode, access, expires));
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

//            cmd.Parameters.AddWithValue("@KeyID", toAdd.KeyID);
//            cmd.Parameters.AddWithValue("@VCode", toAdd.VCode);
//            cmd.Parameters.AddWithValue("@Expires", toAdd.Expires.ToString());
//            cmd.Parameters.AddWithValue("@Access", toAdd.KeyAccess);

            cmd.ExecuteNonQuery();

            if(mustClose)
            {
                CloseDatabase();
            }
        }

        internal static void RemoveCharacter(Character charToRemove)
        {
            throw new NotImplementedException();
        }

        internal static List<Character> GetAccountCharacters(long accID)
        {
            throw new NotImplementedException();
        }
    }

}

