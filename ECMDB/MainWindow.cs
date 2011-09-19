using System;
using Gtk;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

public partial class MainWindow: Gtk.Window
{    
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();
    }
    
    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        Application.Quit ();
        a.RetVal = true;
    }

    IDbConnection dbConn;
    BackgroundWorker worker = new BackgroundWorker();
    string connString;
    SQLiteConnection outputSQLiteConn;

    private int maxRecord = 0;
    private int curRecord = 0;

    private string mySqlConnString = "Server={0};Database=evedb;Uid={1};Pwd={2};";
    private string msSqlConnStringLogin = "Data Source={0};Initial Catalog=evedb;User Id={1};Password={2}";
    private string msSqlConnStringIntegrated = "Data Source={0};Initial Catalog=evedb;Integrated Security=True";
    //private string sqliteConnString = "Data Source={0};";


    private void ServerTypeChanged(object sender, EventArgs e)
    {
//            if (rdoMSSQL.Checked)
//                txtSource.Text = ".\\SQLEXPRESS";
//            else if (rdoMySql.Checked)
//                txtSource.Text = "localhost";
//
//            chkIntegratedSec.Enabled = rdoMSSQL.Checked;
//            chkIntegratedSec.Visible = rdoMSSQL.Checked;
//            
//            btnOpenSQLiteDB.Enabled = rdoSQLite.Checked;
//            btnOpenSQLiteDB.Visible = rdoSQLite.Checked;
//
//            lblPort.Visible = rdoMySql.Checked;
//            txtPort.Visible = rdoMySql.Checked;
//            txtPort.Enabled = rdoMySql.Checked;
    }

    private void CreateDatabase(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        // Open real DB
//            if (rdoMySql.Checked)
//            {
//                connString = string.Format(mySqlConnString, txtSource.Text, txtUser.Text, txtPass.Text);
//                dbConn = new MySqlConnection(connString);
//            }
//            else if (rdoMSSQL.Checked)
//            {
//                if(chkIntegratedSec.Checked)
//                    connString = string.Format(msSqlConnStringIntegrated, txtSource.Text);
//                else
//                    connString = string.Format(msSqlConnStringLogin, txtSource.Text, txtUser.Text, txtPass.Text);
//
//                dbConn = new SqlConnection(connString);
//            }
//            else return;

        dbConn.Open();

        // Do some work
        //CreateAgentsDb();
        //CreateCharacterDb();
        //CreateCertificateDb();
        //CreateSkillDb();
        CreateItemDb();
        //CreateMapDb();
        //CreateMapObjectDb();

        dbConn.Close();
    }

    private void CreateMapDb()
    {
        string filename = "eveMap.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();

        // Create staStations table (the same but without the crap cols)
        Dictionary<string, string> columns = new Dictionary<string, string>();

        columns.Add("stationID", "int");
        columns.Add("stationName", "nvarchar(100)");
        columns.Add("solarSystemID", "int");
        columns.Add("corporationID", "int");
        columns.Add("operationID", "int");
        columns.Add("operationsFlag", "int");
        columns.Add("officeRentalCost", "int");
        columns.Add("maxShipVolumeDockable", "float");
        columns.Add("x", "float");
        columns.Add("y", "float");
        columns.Add("z", "float");
        columns.Add("reprocessingEfficiency", "float");
        columns.Add("reprocessingStationsTake", "float");
        //columns.Add("activityID", "int");

        CreateTable("staStations", columns);
        worker.ReportProgress(-1, "staStations");

        // Insert table data
        IDbCommand queryCmd = CreateDbCommand("SELECT stationID, stationName, solarSystemID, corporationID, operationID, officeRentalCost, " + 
                                   "maxShipVolumeDockable, x, y, z, reprocessingEfficiency, reprocessingStationsTake FROM staStations");

        DataSet data = new DataSet();
        DataAdapter adapter = CreateDataAdapter(queryCmd);

        adapter.Fill(data);

        int currRow = 0;
        int totalRows = data.Tables[0].Rows.Count;
        worker.ReportProgress(-2, totalRows);

        foreach(DataRow row in data.Tables[0].Rows)
        {
            StringBuilder insertCmdText = new StringBuilder();
            StringBuilder values = new StringBuilder();

            insertCmdText.AppendFormat("INSERT INTO staStations(");

            worker.ReportProgress(-3, ++currRow);

            int operationID = Convert.ToInt32(row[4]);

            bool prepend = false;
            int colIdx = 0;
            foreach (string column in columns.Keys)
            {
                if (prepend)
                {
                    insertCmdText.Append(", ");
                    values.Append(", ");
                }
                else prepend = true;

                insertCmdText.Append(column);

                if (column == "operationsFlag")
                {
                    values.Append(GetOperationServices(operationID));
                }
                else
                {
                    object rowItem = row[colIdx];

                    if (rowItem is string)
                    {
                        values.AppendFormat("\"{0}\"", rowItem);
                    }
                    else if (rowItem is DBNull)
                        values.Append("NULL");
                    else if (rowItem is bool)
                    {
                        bool val = Convert.ToBoolean(rowItem);
                        values.Append(val ? "1" : "0");
                    }
                    else
                        values.Append(rowItem);

                    colIdx++;
                }
            }

            insertCmdText.AppendFormat(") VALUES({0})", values);

            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = insertCmdText.ToString();
            comm.ExecuteNonQuery();

            float perc = (float)currRow / (float)totalRows;
            perc *= 100;

            worker.ReportProgress((int)perc);
        }

        // copy the rest
        CopyTable("mapRegions");
        CopyTable("mapRegionJumps");
        CopyTable("mapConstellations");
        CopyTable("mapConstellationJumps");
        CopyTable("mapSolarSystems");
        CopyTable("mapSolarSystemJumps");

        CopyTable("staOperations");

        outputSQLiteConn.Close();
    }

    private void CreateMapObjectDb()
    {
        string filename = "eveMapObjects.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();


        #region MapObjects
        // Create mapObjects table
        Dictionary<string, string> columns = new Dictionary<string, string>();

        columns.Add("itemID", "int");
        columns.Add("itemName", "nvarchar(100)");
        columns.Add("solarSystemID", "int");
        columns.Add("groupID", "int");
        columns.Add("x", "float");
        columns.Add("y", "float");
        columns.Add("z", "float");

        CreateTable("mapObjects", columns);
        worker.ReportProgress(-1, "mapObjects");

        // Insert table data
        IDbCommand queryCmd = CreateDbCommand("SELECT itemID, itemName, groupID, solarSystemID, x, y, z FROM mapDenormalize " +
                                              "WHERE (groupID > 5) AND (groupID < 15) ORDER BY solarSystemID");

        DataSet data = new DataSet();
        DataAdapter adapter = CreateDataAdapter(queryCmd);

        worker.ReportProgress(-4);
        adapter.Fill(data);

        IDataReader reader = data.CreateDataReader();

        int currRow = 0;
        int totalRows = data.Tables[0].Rows.Count;
        worker.ReportProgress(-2, totalRows);

        while (reader.Read())
        {
            StringBuilder insertCmdText = new StringBuilder();
            StringBuilder values = new StringBuilder();

            insertCmdText.AppendFormat("INSERT INTO mapObjects(");

            worker.ReportProgress(-3, ++currRow);

            bool prepend = false;
            foreach (string column in columns.Keys)
            {
                if (prepend)
                {
                    insertCmdText.Append(", ");
                    values.Append(", ");
                }
                else prepend = true;

                insertCmdText.Append(column);

                if (reader[column] is string)
                {
                    values.AppendFormat("\"{0}\"", reader[column]);
                }
                else if (reader[column] is DBNull)
                    values.Append("NULL");
                else if (reader[column] is bool)
                {
                    bool val = Convert.ToBoolean(reader[column]);
                    values.Append(val ? "1" : "0");
                }
                else
                    values.Append(reader[column]);
            }

            insertCmdText.AppendFormat(") VALUES({0})", values);

            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = insertCmdText.ToString();
            comm.ExecuteNonQuery();

            float perc = (float)currRow / (float)totalRows;
            perc *=100;

            worker.ReportProgress((int)perc);
        }

        reader.Close();
        #endregion

        outputSQLiteConn.Close();
    }

    private void CreateItemDb()
    {
        string filename = "eveItems.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();

        Dictionary<string,string> columns = new Dictionary<string,string>();

        columns.Add("typeID", "int");
        columns.Add("groupID", "smallint");
        columns.Add("typeName", "nvarchar(100)");
        columns.Add("description", "nvarchar(3000)");
        columns.Add("radius", "float");
        columns.Add("mass", "float");
        columns.Add("volume", "float");
        columns.Add("capacity", "float");
        columns.Add("portionSize", "int");
        columns.Add("raceID", "tinyint");
        columns.Add("basePrice", "money");
        columns.Add("published", "bit");
        columns.Add("marketGroupID", "smallint");
        columns.Add("chanceOfDuplicating", "float");

        CreateTable("invTypes", columns);
        worker.ReportProgress(-1, "invTypes");

        IDbCommand queryCmd = CreateDbCommand("SELECT invTypes.typeID, invTypes.groupID, invTypes.typeName, invTypes.description, invTypes.radius, invTypes.mass, " +
                                            "invTypes.volume, invTypes.capacity, invTypes.portionSize, invTypes.chanceOfDuplicating, invTypes.marketGroupID, " +
                                            "invTypes.published, invTypes.basePrice, invTypes.raceID " +
                                            "FROM invCategories INNER JOIN invGroups ON invCategories.categoryID = invGroups.categoryID INNER JOIN " +
                                            "invTypes ON invGroups.groupID = invTypes.groupID " +
                                            "WHERE(invCategories.categoryID > 3) AND (invCategories.categoryID <> 14) AND (invCategories.categoryID <> 16) AND " +
                                            "(invCategories.categoryID < 26) AND (invCategories.categoryID <> 34) AND (invCategories.categoryID < 49) OR " +
                                            "(invCategories.categoryID > 29)");

        DataSet data = new DataSet();
        DataAdapter adapter = CreateDataAdapter(queryCmd);

        adapter.Fill(data);


        IDataReader reader = data.CreateDataReader();

        int currRow = 0;
        int totalRows = data.Tables[0].Rows.Count;
        worker.ReportProgress(-2, totalRows);

        while (reader.Read())
        {
            StringBuilder insertCmdText = new StringBuilder();
            StringBuilder values = new StringBuilder();

            insertCmdText.AppendFormat("INSERT INTO invTypes(");

            worker.ReportProgress(-3, ++currRow);

            bool prepend = false;
            foreach (string column in columns.Keys)
            {
                if (prepend)
                {
                    insertCmdText.Append(", ");
                    values.Append(", ");
                }
                else prepend = true;

                insertCmdText.Append(column);

                if (reader[column] is string)
                {
                    string colString = reader[column].ToString().Replace("\'", "\'\'");;
                    values.AppendFormat("\'{0}\'", colString);
                }
                else if (reader[column] is DBNull)
                    values.Append("NULL");
                else if (reader[column] is bool)
                {
                    bool val = Convert.ToBoolean(reader[column]);
                    values.Append(val ? "1" : "0");
                }
                else
                    values.Append(reader[column]);
            }

            insertCmdText.AppendFormat(") VALUES({0})", values);

            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = insertCmdText.ToString();
            comm.ExecuteNonQuery();

            float perc = (float)currRow / (float)totalRows;
            perc *= 100;

            worker.ReportProgress((int)perc);
        }

        reader.Close();

        // Marketgroup Table
        columns = new Dictionary<string, string>();
        columns.Add("marketGroupID", "int");
        columns.Add("parentGroupID", "int");
        columns.Add("marketGroupName", "nvarchar(100)");
        columns.Add("description", "nvarchar(3000)");
        columns.Add("iconFile", "text");
        columns.Add("hasTypes", "bit");

        CreateTable("invMarketGroups", columns);
        worker.ReportProgress(-1, "invMarketGroups");


        queryCmd = CreateDbCommand("SELECT invmarketgroups.*, eveicons.iconFile FROM invmarketgroups LEFT JOIN eveIcons ON invmarketgroups.iconID = eveicons.iconID");

        data = new DataSet();
        adapter = CreateDataAdapter(queryCmd);

        adapter.Fill(data);

        reader = data.CreateDataReader();

        currRow = 0;
        totalRows = data.Tables[0].Rows.Count;
        worker.ReportProgress(-2, totalRows);

        while (reader.Read())
        {
            worker.ReportProgress(-3, ++currRow);
            
            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = "INSERT INTO invMarketGroups(marketGroupID, parentGroupID, marketGroupName, description, iconFile, hasTypes) VALUES (@marketGroupID, @parentGroupID, @marketGroupName, @description, @iconFile, @hasTypes)";

            comm.Parameters.AddWithValue("@marketGroupID", reader["marketGroupID"]);
            comm.Parameters.AddWithValue("@parentGroupID", reader["parentGroupID"]);
            comm.Parameters.AddWithValue("@marketGroupName", reader["marketGroupName"]);
            comm.Parameters.AddWithValue("@description", reader["description"]);
            comm.Parameters.AddWithValue("@iconFile", reader["iconFile"]);
            comm.Parameters.AddWithValue("@hasTypes", reader["hasTypes"]);

            comm.ExecuteNonQuery();

            float perc = (float)currRow / (float)totalRows;
            perc *= 100;

            worker.ReportProgress((int)perc);
        }

        reader.Close();

        // -------------------

        CopyTable("dgmAttributeCategories");
        CopyTable("dgmAttributeTypes");
        CopyTable("dgmEffects");
        CopyTable("dgmTypeAttributes");
        CopyTable("dgmTypeEffects");

        CopyTable("eveUnits");

        CopyTable("invBlueprintTypes");
        CopyTable("invCategories");
        CopyTable("invContrabandTypes");
        CopyTable("invGroups");
        CopyTable("invMetaTypes");
        CopyTable("invTypeMaterials");
        CopyTable("invTypeReactions");

        outputSQLiteConn.Close();
    }

    private void CreateSkillDb()
    {
        string filename = "eveSkills.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();


        worker.ReportProgress(-1, "invSkills");

        // Create Skill Table
        Dictionary<string, string> columns = new Dictionary<string,string>();
        
        columns.Add("typeID", "int");
        columns.Add("groupID", "smallint");
        columns.Add("typeName", "nvarchar(100)");
        columns.Add("description", "nvarchar(3000)");
        columns.Add("mass", "float");
        columns.Add("volume", "float");
        columns.Add("capacity", "float");
        columns.Add("portionSize", "int");
        columns.Add("raceID", "tinyint");
        columns.Add("basePrice", "float");
        columns.Add("published", "bit");
        columns.Add("marketGroupID", "smallint");
        columns.Add("chanceOfDuplicating", "float");

        CreateTable("invSkills", columns);

        IDbCommand queryCmd = CreateDbCommand("SELECT invTypes.typeID, invTypes.groupID, invTypes.typeName, invTypes.description, invTypes.mass, invTypes.radius, " +
                    "invTypes.volume, invTypes.capacity, invTypes.portionSize, invTypes.raceID, invTypes.basePrice, invTypes.published, invTypes.marketGroupID, " + 
                    "invTypes.chanceOfDuplicating FROM invCategories INNER JOIN invGroups ON invCategories.categoryID = invGroups.categoryID INNER JOIN " +
                    "invTypes ON invGroups.groupID = invTypes.groupID WHERE (invCategories.categoryID = 16)");

        DataSet data = new DataSet();
        DataAdapter adapter = CreateDataAdapter(queryCmd);

        adapter.Fill(data);


        IDataReader reader = data.CreateDataReader();

        int currRow = 0;
        int totalRows = data.Tables[0].Rows.Count;
        worker.ReportProgress(-2, totalRows);

        while (reader.Read())
        {
            StringBuilder insertCmdText = new StringBuilder();
            StringBuilder values = new StringBuilder();

            insertCmdText.AppendFormat("INSERT INTO invSkills(");

            worker.ReportProgress(-3, ++currRow);

            bool prepend = false;
            foreach (string column in columns.Keys)
            {
                if (prepend)
                {
                    insertCmdText.Append(", ");
                    values.Append(", ");
                }
                else prepend = true;

                insertCmdText.Append(column);

                if (reader[column] is string)
                {
                    values.AppendFormat("\"{0}\"", reader[column]);
                }
                else if (reader[column] is DBNull)
                    values.Append("NULL");
                else if (reader[column] is bool)
                {
                    bool val = Convert.ToBoolean(reader[column]);
                    values.Append(val ? "1" : "0");
                }
                else
                    values.Append(reader[column]);
            }

            insertCmdText.AppendFormat(") VALUES({0})", values);
            
            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = insertCmdText.ToString();
            comm.ExecuteNonQuery();

            float perc = (float)currRow / (float)totalRows;
            perc *=100;

            worker.ReportProgress((int)perc);
        }

        reader.Close();

        outputSQLiteConn.Close();
    }

    private void CreateTable(string tableName, Dictionary<string, string> columns)
    {
        StringBuilder createCmdText = new StringBuilder();
        createCmdText.AppendFormat("CREATE TABLE {0} (", tableName);

        bool prepend = false;
        string primary = "";
        foreach (string column in columns.Keys)
        {
            if (prepend)
                createCmdText.Append(", ");
            else
            {
                prepend = true;
                primary = column;
            }

            createCmdText.AppendFormat("{0} {1}", column, columns[column]);
        }

        createCmdText.AppendFormat(", PRIMARY KEY ({0}))", primary);

        SQLiteCommand comm = outputSQLiteConn.CreateCommand();
        comm.CommandText = createCmdText.ToString();
        comm.ExecuteNonQuery();
    }

    private void CreateCertificateDb()
    {
        string filename = "eveCertificates.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();

        CopyTable("crtCategories");
        CopyTable("crtCertificates");
        CopyTable("crtClasses");
        CopyTable("crtRecommendations");
        CopyTable("crtRelationships");

        outputSQLiteConn.Close();
    }

    private void CreateCharacterDb()
    {
        string filename = "eveCharacter.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();

        CopyTable("chrAncestries");
        CopyTable("chrBloodlines");
        CopyTable("chrFactions");
        CopyTable("chrRaces");

        outputSQLiteConn.Close();
    }

    private void CreateAgentsDb()
    {
        string filename = "eveAgents.db";

        if (File.Exists(filename))
            File.Delete(filename);

        System.Data.SQLite.SQLiteConnection.CreateFile(filename);

        outputSQLiteConn = new System.Data.SQLite.SQLiteConnection("data source=" + filename);
        outputSQLiteConn.Open();
        
        // First we make a better agents table
        IDbCommand cmd = CreateDbCommand("SELECT agtAgents.agentID, eveNames.itemName AS agentName, agtAgents.divisionID, agtAgents.corporationID, agtAgents.locationID,"+
                                         " agtAgents.agentTypeID FROM agtAgents INNER JOIN eveNames ON agtAgents.agentID = eveNames.itemID");

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        if (data.Tables.Count == 1)
        {
            worker.ReportProgress(-1, "agtAgents");

            SQLiteCommand comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = "CREATE TABLE agtAgents(agentID int NOT NULL, agentName text NOT NULL, divisionID tinyint, corporationID tinyint, locationID int, level tinyint, quality smallint, agentTypeID int, isLocator boolean);";
            comm.ExecuteNonQuery();

            string agentInsertCmd = "INSERT INTO agtAgents(agentID, agentName, divisionID, corporationID, locationID, level, quality, agentTypeID, isLocator)" +
                                    "VALUES({0}, \'{1}\', {2}, {3}, {4}, {5}, {6}, {7}, {8});";
            int record = 0;
            foreach (DataTable table in data.Tables)
            {
                worker.ReportProgress(-2, table.Rows.Count);
                foreach (DataRow row in table.Rows)
                {
                    worker.ReportProgress(-3, ++record);

                    float curr = record;
                    float max = table.Rows.Count;
                    float perc = curr / max;
                    perc *= 100;
                    worker.ReportProgress((int)perc);

                    int agentID = (int)row.ItemArray[0];
                    comm = outputSQLiteConn.CreateCommand();
                    comm.CommandText = string.Format(agentInsertCmd, row.ItemArray[0], row.ItemArray[1], row.ItemArray[2],
                                        row.ItemArray[3], row.ItemArray[4], GetAgentLevel(agentID),
                                        GetAgentQuality(agentID), row.ItemArray[5], IsAgentLocator(agentID));
                    comm.ExecuteNonQuery();
                }
            }
        }

        /// Then we want to copy the following
        /// agtResearchAgents
        /// and all the crpNPCCorp ones
        CopyTable("agtResearchAgents");
        CopyTable("crpNPCCorporations");
        CopyTable("crpNPCCorporationTrades");
        CopyTable("crpNPCCorporationResearchFields");
        CopyTable("crpNPCCorporationDivisions");

        outputSQLiteConn.Close();
    }

    private int GetAgentLevel(int agentID)
    {
        string cmdText = string.Format("SELECT v FROM agtConfig WHERE (agentID = {0}) AND (k = 'level');",
                                           agentID);

        IDbCommand cmd = dbConn.CreateCommand();
        cmd.Connection = dbConn;
        cmd.CommandText = cmdText;

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0 || data.Tables[0].Rows[0].ItemArray.Length == 0)
            return 0;

        int level = Convert.ToInt32(data.Tables[0].Rows[0].ItemArray[0]);

        return level;
    }

    private int GetAgentQuality(int agentID)
    {
        string cmdText = string.Format("SELECT v FROM agtConfig WHERE (agentID = {0}) AND (k = 'quality');",
                                           agentID);

        IDbCommand cmd = dbConn.CreateCommand();
        cmd.Connection = dbConn;
        cmd.CommandText = cmdText;

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0 || data.Tables[0].Rows[0].ItemArray.Length == 0)
            return 0;

        int quality = Convert.ToInt32(data.Tables[0].Rows[0].ItemArray[0]);

        return quality;
    }

    private int IsAgentLocator(int agentID)
    {
        string cmdText = string.Format("SELECT v FROM agtConfig WHERE (agentID = {0}) AND (k = 'agent.LocateCharacterService.enabled');",
                                        agentID);

        IDbCommand cmd = dbConn.CreateCommand();
        cmd.Connection = dbConn;
        cmd.CommandText = cmdText;

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0 || data.Tables[0].Rows[0].ItemArray.Length == 0)
            return 0;

        int locator = Convert.ToInt32(data.Tables[0].Rows[0].ItemArray[0]);

        return locator == 1 ? 1 : 0;
    }

    public long GetOperationServices(int operationID)
    {
        string cmdText = string.Format("SELECT serviceID FROM staOperationServices WHERE (operationID = {0})",
                                           operationID);

        IDbCommand cmd = dbConn.CreateCommand();
        cmd.Connection = dbConn;
        cmd.CommandText = cmdText;

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        long services = 0;

        foreach(DataRow row in data.Tables[0].Rows)
        {
            services += Convert.ToInt64(row.ItemArray[0]);
        }

        return services;
    }

    private void UpdateProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {
        if (e.ProgressPercentage >= 0)
        {
            pgbProgress.Fraction = e.ProgressPercentage;

            if (e.ProgressPercentage >= 100)
            {
                lblRecords.Text = "";
                lblStatus.Text = "Convert complete";
            }
        }
        else
        {
            if (e.ProgressPercentage == -1)
                lblStatus.Text = "Processing table " + e.UserState.ToString();
            else if (e.ProgressPercentage == -2)
                maxRecord = (int)e.UserState;
            else if (e.ProgressPercentage == -3)
                curRecord = (int)e.UserState;
            if (e.ProgressPercentage == -4)
                lblRecords.Text = "Querying Database...";
            else
                lblRecords.Text = string.Format("{0} of {1}", curRecord, maxRecord);
        }
    }

    private void WorkCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {

    }

    private void CopyTable(string tableName)
    {
        worker.ReportProgress(-1, tableName);
        string createTable = "CREATE TABLE IF NOT EXISTS " + tableName + "(";

        Console.WriteLine("Table Schema for {0} *****************************************", tableName);

        IDbCommand cmd = CreateDbCommand("SELECT * FROM " + tableName);
        IDataReader myReader = cmd.ExecuteReader(CommandBehavior.KeyInfo);

        //Retrieve column schema into a DataTable.
        DataTable schemaTable = myReader.GetSchemaTable();

        //For each field in the table...
        int i = 1;
        List<string> primaryKeyCols = new List<string>();
        foreach (DataRow columnDef in schemaTable.Rows)
        {
            // Remove the _ as SQLite doesn't play nice with them
            string colName = columnDef["ColumnName"].ToString();
            string colType;

            if (dbConn is MySqlConnection)
            {
                colType = GetSqlTypeString((Type)columnDef["DataType"]);
            }
            else
            {
                colType = columnDef["DataTypeName"].ToString();
            }

            int colTypeLength = int.Parse(columnDef["ColumnSize"].ToString());
            bool allowNull = bool.Parse(columnDef["AllowDBNull"].ToString());
            bool unique = bool.Parse(columnDef["IsUnique"].ToString());
            bool priKey = bool.Parse(columnDef["IsKey"].ToString());
            bool autoInc = bool.Parse(columnDef["IsAutoIncrement"].ToString());

            char c = colName[0];
            if (c >= 48 && c <= 57)
                colName = "X" + colName;

            if (colTypeLength > 1)
                colType += string.Format("({0})", colTypeLength);

            string colDef = string.Format("{0} {1}", colName, colType);

            if (priKey)
                primaryKeyCols.Add(colName);
            else if (unique)
                colDef += " UNIQUE";

            if (priKey && autoInc)
                colDef += " AUTOINCREMENT";

            if (!allowNull)
                colDef += " NOT NULL";

            if (i < schemaTable.Rows.Count)
                colDef += ", ";
            else
            {
                if (primaryKeyCols.Count > 0)
                {
                    colDef += ", PRIMARY KEY(";

                    foreach (string s in primaryKeyCols)
                    {
                        if (primaryKeyCols.IndexOf(s) > 0)
                            colDef += ", ";

                        colDef += s;
                    }

                    colDef += ")";
                }

                colDef += ")";
            }

            i++;

            createTable += colDef;
        }

        //Always close the DataReader and connection.
        myReader.Close();

        System.Data.SQLite.SQLiteCommand comm = outputSQLiteConn.CreateCommand();
        comm.CommandText = createTable;
        comm.ExecuteNonQuery();

        // Read in data

        DbDataAdapter adapter = CreateDataAdapter(cmd);
        DataSet data = new DataSet();
        adapter.Fill(data);

        DataTable table = data.Tables[0];

        string insertComm = string.Format("INSERT INTO {0} (", tableName);

        i = 1;
        foreach (DataColumn col in table.Columns)
        {
            string colName = col.ColumnName;
            char c = colName[0];
            if (c >= 48 && c <= 57)
                colName = "X" + colName;
            insertComm += colName;

            if (i < schemaTable.Rows.Count)
                insertComm += ", ";
            else
                insertComm += ")";

            i++;
        }


        int record = 0;
        worker.ReportProgress(-2, table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            worker.ReportProgress(-3, ++record);
            string insertVals = string.Format("VALUES (");
            i = 1;
            foreach (DataColumn col in table.Columns)
            {
                string insertVal = row[col.ColumnName] is DBNull ? "NULL" : row[col.ColumnName].ToString();

                if (col.DataType == typeof(String) || col.DataType == typeof(Boolean))
                {
                    string quote = insertVal.Contains("\"") ? "\'" : "\"";

                    insertVal = insertVal.Replace("\'", "\'\'");

                    insertVal = string.Format("{0}{1}{0}", quote, insertVal);
                }

                insertVals += insertVal;

                if (i < schemaTable.Rows.Count)
                    insertVals += ", ";
                else
                    insertVals += ")";

                i++;
            }
            float curr = record;
            float max = table.Rows.Count;
            float perc = curr / max;
            perc *= 100;
            worker.ReportProgress((int)perc);

            comm = outputSQLiteConn.CreateCommand();
            comm.CommandText = insertComm + insertVals;
            comm.ExecuteNonQuery();
        }

        myReader.Close();
    }

    private string GetSqlTypeString(Type type)
    {
        SqlParameter param;
        System.ComponentModel.TypeConverter tc;
        param = new SqlParameter();
        tc = System.ComponentModel.TypeDescriptor.GetConverter(param.DbType);
        if (tc.CanConvertFrom(type))
        {
            param.DbType = (DbType)tc.ConvertFrom(type.Name);
        }
        else
        {
            // try to forcefully convert
            try
            {
                param.DbType = (DbType)tc.ConvertFrom(type.Name);
            }
            catch (Exception e)
            {
                // ignore the exception
                Console.WriteLine(e.Message);
            }
        }
        return param.SqlDbType.ToString(); ;
    }

    private DbDataAdapter CreateDataAdapter(IDbCommand cmd)
    {
        if (dbConn is SqlConnection)
            return new SqlDataAdapter(cmd as SqlCommand);
        else if (dbConn is MySqlConnection)
            return new MySqlDataAdapter(cmd as MySqlCommand);
        else return null;
    }

    private IDbCommand CreateDbCommand(string commandText)
    {
        IDbCommand cmd = dbConn.CreateCommand();
        cmd.Connection = dbConn;
        cmd.CommandText = commandText;

        return cmd;
    }

    private void StartWorker(object sender, EventArgs e)
    {
        worker.RunWorkerAsync();
    }
}
