using System.Collections.Generic;

namespace Tafe_System
{
    public class SqlParameterDictionary : Dictionary<string, SqlParameterDetails>
    {
        public void AddParameter(string key, System.Data.SqlDbType type, int length)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails();
            parameterDetails.value = "";
            parameterDetails.type = type;
            parameterDetails.length = length;
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, System.Data.SqlDbType type)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails();
            parameterDetails.value = "";
            parameterDetails.type = type;
            parameterDetails.length = null;
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, string value, System.Data.SqlDbType type, int length)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails();
            parameterDetails.value = value;
            parameterDetails.type = type;
            parameterDetails.length = length;
            this.Add(key, parameterDetails);
        }

        public void AddParameter(string key, string value, System.Data.SqlDbType type)
        {
            SqlParameterDetails parameterDetails = new SqlParameterDetails();
            parameterDetails.value = value;
            parameterDetails.type = type;
            parameterDetails.length = null;
            this.Add(key, parameterDetails);
        }

    }
}
