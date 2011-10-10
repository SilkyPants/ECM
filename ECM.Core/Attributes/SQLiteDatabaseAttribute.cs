using System;

namespace ECM
{
    public class SQLiteDatabaseAttribute : Attribute
    {
        public string TableName
        {
            get;
            private set;
        }

        public string FieldName
        {
            get;
            private set;
        }

        public SQLiteDatabaseAttribute (string table, string field)
        {
            TableName = table;
            FieldName = field;
        }
    }
}

