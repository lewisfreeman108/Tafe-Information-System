using System.Data;

namespace Tafe_System
{
    public class SqlParameterDetails
    {
        public string value;
        public System.Data.SqlDbType type;
        public int? length;

        public SqlParameterDetails() { }

        public SqlParameterDetails(SqlDbType type, int? length)
        {
            this.type = type;
            this.length = length;
        }
    }
}
