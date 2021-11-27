using System.Data;

namespace Tafe_System
{
    public class SqlParameterDetails//Not a struct because values are mutable and intended to be after initialization 
    {
        public string value;
        public SqlDbType type;
        public int? length;

        public SqlParameterDetails(SqlDbType type, int? length)
        {
            this.type = type;
            this.length = length;
        }
    }
}
