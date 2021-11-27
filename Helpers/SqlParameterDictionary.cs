using System.Collections.Generic;

namespace Tafe_System
{
    public class SqlParameterDictionary : Dictionary<string, SqlParameterDetails>
    {
        public void AddParameter(string key, System.Data.SqlDbType type, int length)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails(type, length);
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, System.Data.SqlDbType type)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails(type, null);
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, string value, System.Data.SqlDbType type, int length)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails(type, length);
            parameterDetails.value = value;
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, string value, System.Data.SqlDbType type)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails(type, null);
            parameterDetails.value = value;
            this.Add(key, parameterDetails);
        }

    }
}
