using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

using SqlConn = System.Data.SQLite.SQLiteConnection;
using SqlCmd = System.Data.SQLite.SQLiteCommand;
using SqlReader = System.Data.SQLite.SQLiteDataReader;
using System.Data;

namespace ECM
{
    public static class MapDatabase
    {
        static SqlConn sqlConnection = null;

        static string mapDatabasePath = "Resources/Database/eveMap.db";

        static Dictionary<long, EveStation> m_Stations = new Dictionary<long, EveStation>();

        public static Dictionary<long, EveStation> Stations
        {
            get
            {
                return m_Stations;
            }
        }

        private static bool OpenDatabase()
        {
            sqlConnection = new SqlConn(string.Format("Data Source={0};version=3;", mapDatabasePath));
            sqlConnection.Open();

            SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA cache_size=5000";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA count_changes=OFF";
            cmd.ExecuteNonQuery();

            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "PRAGMA temp_store=MEMORY";
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            return true;
        }

        private static void CloseDatabase()
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open) return;

            sqlConnection.Close();
        }

        public static void LoadMap()
        {
            OpenDatabase();

            LoadStations();

            CloseDatabase();
        }

        private static void LoadStations()
        {
            // Get Stations
            string selectCmd = "SELECT * FROM staStations";

            SqlCmd cmd = sqlConnection.CreateCommand();
            cmd.CommandText = selectCmd;
            SqlReader row = cmd.ExecuteReader();

            while (row.Read())
            {
                long stationID = Convert.ToInt64(row[0]);
                string stationName = row[1].ToString();
                long systemID = Convert.ToInt64(row[2]);
                long corpID = Convert.ToInt64(row[3]);
                long opsID = Convert.ToInt64(row[4]);
                int opsFlag = Convert.ToInt32(row[5]);
                int rentCost = Convert.ToInt32(row[6]);
                float maxVol = Convert.ToSingle(row[7]);
                double x = Convert.ToDouble(row[8]);
                double y = Convert.ToDouble(row[9]);
                double z = Convert.ToDouble(row[10]);
                float refEff = Convert.ToSingle(row[11]);
                float refTake = Convert.ToSingle(row[12]);

                EveStation newStation = new EveStation();

                newStation.Name = stationName;
                newStation.ID = stationID;
                newStation.SolarSystemID = systemID;
                newStation.CorporationID = corpID;
                newStation.OperationID = opsID;
                newStation.OperationsFlag = opsFlag;
                newStation.OfficeRentCost = rentCost;
                newStation.MaxShipVolume = maxVol;
                newStation.X = x;
                newStation.Y = y;
                newStation.Z = z;
                newStation.ReprocessingEfficiency = refEff;
                newStation.ReprocessingStationTake = refTake;

                m_Stations.Add(newStation.ID, newStation);
            }

            row.Close();
        }
    }
}
