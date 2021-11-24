
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    public class DatabaseConnection
    {
        private readonly SqlConnection connection;
        public SqlCommand objCommand;
        public SqlDataReader objDataReader;
        public SqlDataAdapter objDataAdapter = new SqlDataAdapter();

        private readonly KeyValuePair<string, SqlParameterDetails> saltParameter = new KeyValuePair<string, SqlParameterDetails>("@salt", new SqlParameterDetails(SqlDbType.VarChar, 255));
        private readonly KeyValuePair<string, SqlParameterDetails> enrolmentValidationResult = new KeyValuePair<string, SqlParameterDetails>("@enrolmentValidationResult", new SqlParameterDetails(SqlDbType.VarChar, 100));

        public DatabaseConnection()
        {
            connection = new SqlConnection("server=" + Environment.MachineName + ";" + "database=tafesystem;Trusted_Connection=yes");
            connection.Open();
        }

        private void SqlExceptionRegular(SqlException er)
        {
            switch (er.Errors[0].Number)
            {
                case 547:
                    System.Windows.MessageBox.Show("A foreign key was invalid:\n" + er.Errors[0].Message);
                    break;
                case 2627:
                    System.Windows.MessageBox.Show("An alternate or primary key was duplicate:\n" + er.Errors[0].Message);
                    break;
                default:
                    System.Windows.MessageBox.Show("An unknown error has occured\n" + er.Errors[0].Number + ": " + er.Errors[0].Message);
                    break;
            }
        }

        public string CreateSalt()
        {
            var saltBytes = new byte[128 / 8];

            using (var provider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        private string GetSalt(string userType, string userID)
        {
            SqlCommand command = new SqlCommand("tsp_Get" + userType + "Salt", connection);

            command.Parameters.Add("@" + userType + "id", SqlDbType.Int).Value = userID;

            return GetValueFromTable(ref command, saltParameter);
        }

        public string HashedPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        }


        public int LogUserIn(string userID, string password)
        {
            return VerifyPassword("Student", userID, password) != 0 ? 1 : VerifyPassword("Teacher", userID, password);
        }

        private int VerifyPassword(string userType, string userID, string password)
        {
            if (!string.IsNullOrWhiteSpace(userID) && !string.IsNullOrWhiteSpace(password))
            {
                string salt = GetSalt(userType, userID);
                string hashedPassword = HashedPassword(password, salt);

                using SqlCommand command = new SqlCommand("tsp_VerifyPassword", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@id", SqlDbType.Int).Value = userID;
                command.Parameters.Add("@password", SqlDbType.VarChar, 255).Value = hashedPassword;
                command.Parameters.Add("@completed", SqlDbType.Int).Direction = ParameterDirection.Output;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException er)
                {
                    SqlExceptionRegular(er);
                    return 0;
                }
                return int.Parse(command.Parameters["@completed"].Value.ToString());

            }
            return 0;
        }

        public bool AddUserToDatabase(SqlParameterDictionary parameterDictionary, string password, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, string removableSuffice, string successMessage, string addQuery)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                System.Windows.MessageBox.Show("Please enter a valid password");
                return false;
            }
            string salt = CreateSalt();
            parameterDictionary["@salt"].value = salt;
            parameterDictionary["@password"].value = HashedPassword(password, salt);

            return AddToDatabase(parameterDictionary, textBoxElements, comboBoxElementsValue, null, null, removableSuffice, successMessage, addQuery);
        }

        public bool UpdateUserInDatabase(KeyValuePair<string, SqlParameterDetails> primaryKey, SqlParameterDictionary parameterDictionary, string password, string getSaltQuery, WatermarkTextBox primaryKeyTextBox, WatermarkTextBox[] textboxElements, ComboBox[] comboBoxElementsValue, string removableSuffice, string commandQueryGetDetails, string commandQueryUpdateDetails, string successMessage)
        {
            primaryKey.Value.value = primaryKeyTextBox.Text;
            if (!string.IsNullOrWhiteSpace(password)) parameterDictionary["@password"].value = CreateNewPasswordForUser(password, primaryKey, getSaltQuery);

            return UpdateDatabase(commandQueryUpdateDetails, commandQueryGetDetails, primaryKey, parameterDictionary, primaryKeyTextBox, textboxElements, comboBoxElementsValue, null, null, removableSuffice, successMessage);
        }

        private List<SqlParameter> GenerateSQLParameters(SqlParameterDictionary parameterDictionary)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (KeyValuePair<string, SqlParameterDetails> keyandvalue in parameterDictionary)
            {
                parameters.Add(GenerateSQLParameter(keyandvalue));
            }
            return parameters;
        }

        public SqlParameter GenerateSQLParameter(KeyValuePair<string, SqlParameterDetails> keyandvalue)
        {

            SqlParameter parameter;

            if (keyandvalue.Value.type == SqlDbType.VarChar || keyandvalue.Value.type == SqlDbType.Char)
            {
                parameter = new SqlParameter(keyandvalue.Key, keyandvalue.Value.type, (int)keyandvalue.Value.length);
                parameter.Value = keyandvalue.Value.value;
            }
            else
            {
                parameter = new SqlParameter(keyandvalue.Key, keyandvalue.Value.type);
                if (int.TryParse(keyandvalue.Value.value, out int convertedValue))
                {
                    parameter.Value = convertedValue;
                }
            }

            if (string.Equals(keyandvalue.Value.value, "NULLVALUE"))
            {
                parameter.Value = DBNull.Value;
            }

            return parameter;
        }

        public bool ValidateEnrolment(SqlParameterDictionary enrolmentValidationParameters, WatermarkTextBox[] addEnrolmentTextBoxElements, out string result)
        {
            AddValuesToParameterDictionary(ref enrolmentValidationParameters, "E", addEnrolmentTextBoxElements, null, null, null);
            SqlCommand command = new SqlCommand("tsp_ValidateEnrolment", connection);

            foreach (SqlParameter parameter in GenerateSQLParameters(enrolmentValidationParameters)) command.Parameters.Add(parameter);

            result = GetValueFromTable(ref command, enrolmentValidationResult);

            return result == "Validated";
        }

        private void AddValuesToParameterDictionary(ref SqlParameterDictionary parameters, string removableSuffice, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, ComboBox[] comboBoxElementsIndex, CheckBox[] checkBoxElements)
        {
            if (textBoxElements != null)
            {
                foreach (WatermarkTextBox textBoxElement in textBoxElements)
                {
                    if (!string.IsNullOrEmpty(textBoxElement.Text))
                        parameters["@" + textBoxElement.Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower()].value = textBoxElement.Text;
                }
            }
            //All below values cannot be blank
            if (comboBoxElementsValue != null)
                for (int i = 0; i < comboBoxElementsValue.Length; i++) parameters["@" + comboBoxElementsValue[i].Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower()].value = comboBoxElementsValue[i].Text;
            if (comboBoxElementsIndex != null)
                for (int i = 0; i < comboBoxElementsIndex.Length; i++) parameters["@" + comboBoxElementsIndex[i].Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower()].value = (comboBoxElementsIndex[i].SelectedIndex + 1).ToString();
            if (checkBoxElements != null)
                for (int i = 0; i < checkBoxElements.Length; i++) parameters["@" + checkBoxElements[i].Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower()].value = checkBoxElements[i].IsChecked == true ? "1" : "0";

        }

        public bool AddToDatabase(SqlParameterDictionary parameterDictionary, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, ComboBox[] comboBoxElementsIndex, CheckBox[] checkBoxElements, string removableSuffice, string successMessage, string query)
        {
            if (textBoxElements != null)
            {
                foreach (WatermarkTextBox element in textBoxElements)
                {
                    if (string.IsNullOrWhiteSpace(element.Text))
                    {
                        System.Windows.MessageBox.Show("All fields require values");
                        return false;
                    }
                }
            }

            AddValuesToParameterDictionary(ref parameterDictionary, removableSuffice, textBoxElements, comboBoxElementsValue, comboBoxElementsIndex, checkBoxElements);

            if (!AddDictionaryToDatabase(parameterDictionary, query))
            {
                System.Windows.MessageBox.Show("Something went wrong");
                return false;
            }
            else
            {
                ClearUserInputFieldsAfterSuccessfulChange(successMessage, null, textBoxElements, comboBoxElementsValue, comboBoxElementsIndex, checkBoxElements);
                return true;
            }
        }

        public bool AddDictionaryToDatabase(SqlParameterDictionary parameterDictionary, string queryName)
        {
            SqlCommand command = new SqlCommand(queryName, connection);

            foreach (SqlParameter parameter in GenerateSQLParameters(parameterDictionary))
            {
                command.Parameters.Add(parameter);
            }
            command.CommandType = System.Data.CommandType.StoredProcedure;
            return ExecuteStoredProcedureQuery(ref command, true);
        }

        public bool UpdateDatabase(string commandQueryUpdateValues, string commandQueryGetValues, KeyValuePair<string, SqlParameterDetails> primaryKey, SqlParameterDictionary parameterDictionary, WatermarkTextBox primaryKeyTextBox, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, ComboBox[] comboBoxElementsIndex, CheckBox[] checkBoxElements, string removableSuffice, string successMessage)
        {
            primaryKey.Value.value = primaryKeyTextBox.Text;

            if (string.IsNullOrWhiteSpace(primaryKey.Value.value))
            {
                System.Windows.MessageBox.Show("Please enter a valid" + primaryKey.Key);
                return false;
            }

            ExecuteGetExistingValuesQuery(commandQueryGetValues, primaryKey, ref parameterDictionary);

            AddValuesToParameterDictionary(ref parameterDictionary, removableSuffice, textBoxElements, comboBoxElementsValue, comboBoxElementsIndex, checkBoxElements);

            if (UpdateDictionaryInDatabase(commandQueryUpdateValues, commandQueryGetValues, primaryKey, parameterDictionary))
            {
                ClearUserInputFieldsAfterSuccessfulChange(successMessage, primaryKeyTextBox, textBoxElements, comboBoxElementsValue, comboBoxElementsIndex, checkBoxElements);
                return true;
            }
            return false;


        }

        public bool UpdateDictionaryInDatabase(string commandQueryUpdateValues, string commandQueryGetValues, KeyValuePair<string, SqlParameterDetails> primaryKey, SqlParameterDictionary parameterDictionary)
        {
            using SqlCommand command = new SqlCommand(commandQueryUpdateValues, connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add(GenerateSQLParameter(primaryKey));
            foreach (SqlParameter parameter in GenerateSQLParameters(parameterDictionary)) command.Parameters.Add(parameter);
            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Primary key is invalid");
                return false;
            }
            catch (SqlException er)
            {
                SqlExceptionRegular(er);
                return false;
            }
        }

        private void ExecuteGetExistingValuesQuery(string commandQueryGetValues, KeyValuePair<string, SqlParameterDetails> primaryKey, ref SqlParameterDictionary parameterDictionary)
        {
            using SqlCommand command = new SqlCommand(commandQueryGetValues, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(GenerateSQLParameter(primaryKey));
            foreach (SqlParameter parameter in GenerateSQLParameters(parameterDictionary))
            {
                parameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(parameter);
            }
            try
            {
                command.ExecuteNonQuery();
                foreach (KeyValuePair<string, SqlParameterDetails> keyvaluepair in parameterDictionary)
                {
                    if (!(keyvaluepair.Key == "@password" && !string.IsNullOrWhiteSpace(keyvaluepair.Value.value)))
                        keyvaluepair.Value.value = command.Parameters[keyvaluepair.Key].Value.ToString();
                }
            }
            catch (SqlException er)
            {
                SqlExceptionRegular(er);
            }
        }

        public void CreateBridge(string bridgeQuery, ref KeyValuePair<string, SqlParameterDetails> key1, WatermarkTextBox key1TextBox, ref KeyValuePair<string, SqlParameterDetails> key2, WatermarkTextBox key2TextBox)
        {
            if (string.IsNullOrWhiteSpace(key1TextBox.Text) || string.IsNullOrWhiteSpace(key2TextBox.Text))
            {
                System.Windows.MessageBox.Show("Both fields require values");
                return;
            }
            if (ValidationHelper.ValidateOnlyIntegers(key1.Key, key1TextBox.Text) && ValidationHelper.ValidateOnlyIntegers(key2.Key, key2TextBox.Text))
            {
                key1.Value.value = key1TextBox.Text;
                key2.Value.value = key2TextBox.Text;

                if (CreateBridgeQuery(bridgeQuery, key1, key2))
                {
                    key1TextBox.Text = "";
                    key2TextBox.Text = "";
                    System.Windows.MessageBox.Show("Bridge Created Successfully");
                }
            }
        }

        public string FindBridge(string bridgeQuery, KeyValuePair<string, SqlParameterDetails> key1, KeyValuePair<string, SqlParameterDetails> key2)
        {
            FindBridgeQuery(bridgeQuery, key1, key2, out int foundBridge);
            return foundBridge == -1 ? key1.Key + " or " + key2.Key + " invalid" : foundBridge.ToString();
        }

        public bool CreateBridgeQuery(string bridgeQuery, KeyValuePair<string, SqlParameterDetails> key1, KeyValuePair<string, SqlParameterDetails> key2)
        {
            using SqlCommand command = new SqlCommand(bridgeQuery, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(GenerateSQLParameter(key1));
            command.Parameters.Add(GenerateSQLParameter(key2));
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                switch (ex.Errors[0].Number)
                {
                    case 547: // Foreign Key violation
                        System.Windows.MessageBox.Show("One or more primary key is invalid\n" + ex.Errors[0].Message);
                        break;
                    case 2627: // Alternate key violation

                        System.Windows.MessageBox.Show("Bridge already exists");

                        break;
                }
                return false;
            }
            return true;
        }

        private void FindBridgeQuery(string bridgeQuery, KeyValuePair<string, SqlParameterDetails> key1, KeyValuePair<string, SqlParameterDetails> key2, out int bridgeKey)
        {
            SqlCommand command = new SqlCommand(bridgeQuery, connection);

            command.Parameters.Add(GenerateSQLParameter(key1));
            command.Parameters.Add(GenerateSQLParameter(key2));
            command.Parameters.Add(new SqlParameter("@foundbridge", SqlDbType.Int)).Direction = ParameterDirection.Output;
            ExecuteStoredProcedureQuery(ref command, false);

            if (int.TryParse(command.Parameters["@foundbridge"].Value.ToString(), out int bridge))
            {
                bridgeKey = bridge;
            }
            else
            {
                bridgeKey = -1;
            }
            command.Dispose();
        }

        public string GetValueFromTable(string query, KeyValuePair<string, SqlParameterDetails> returnParameter)
        {
            SqlCommand command = new SqlCommand(query, connection);
            return GetValueFromTable(ref command, returnParameter);
        }



        public string GetValueFromTable(string query, KeyValuePair<string, SqlParameterDetails> primaryParameter, KeyValuePair<string, SqlParameterDetails> returnValue)
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add(GenerateSQLParameter(primaryParameter));

            return GetValueFromTable(ref command, returnValue);
        }


        public string GetValueFromTable(ref SqlCommand command, KeyValuePair<string, SqlParameterDetails> returnValue)
        {
            command.Parameters.Add(GenerateSQLParameter(returnValue)).Direction = System.Data.ParameterDirection.Output;
            ExecuteStoredProcedureQuery(ref command, false);
            try
            {
                return command.Parameters[returnValue.Key].Value.ToString();
            }
            finally
            {
                command.Dispose();
            }
        }

        public string GetValueFromTable(string query, SqlParameterDictionary parameterDictionary, KeyValuePair<string, SqlParameterDetails> returnValue)
        {
            SqlCommand command = new SqlCommand(query, connection);

            foreach (SqlParameter parameter in GenerateSQLParameters(parameterDictionary)) command.Parameters.Add(parameter);
            return GetValueFromTable(ref command, returnValue);
        }


        public string CreateNewPasswordForUser(string password, KeyValuePair<string, SqlParameterDetails> primaryKey, string userSaltQuery)
        {
            SqlCommand command = new SqlCommand(userSaltQuery, connection);
            command.Parameters.Add(GenerateSQLParameter(primaryKey));
            command.Parameters.Add(new SqlParameter("@salt", SqlDbType.VarChar, 255)).Direction = System.Data.ParameterDirection.Output;
            ExecuteStoredProcedureQuery(ref command, false);
            try
            {
                return HashedPassword(password, command.Parameters["@salt"].Value.ToString());
            }
            finally
            {
                command.Dispose();
            }

        }

        public bool RemoveRow(string deleteQuery, ref KeyValuePair<string, SqlParameterDetails> primaryKey, string successMessage, WatermarkTextBox primaryKeyTextBox, WatermarkTextBox[] textBoxes, ComboBox[] comboBoxesValue, ComboBox[] comboBoxIndex, CheckBox[] checkBoxes)
        {
            primaryKey.Value.value = primaryKeyTextBox.Text;

            if (string.IsNullOrWhiteSpace(primaryKey.Value.value))
            {
                System.Windows.MessageBox.Show("Must enter a " + primaryKey.Key);
                return false;
            }

            if (!ValidationHelper.ValidateNoSpaces(primaryKey.Value.value))
            {
                System.Windows.MessageBox.Show(primaryKey.Key + " must not have any spaces");
                return false;
            }
            if (ExecuteBasicQuery(deleteQuery, primaryKey))
            {
                ClearUserInputFieldsAfterSuccessfulChange(successMessage, primaryKeyTextBox, textBoxes, comboBoxesValue, comboBoxIndex, checkBoxes);
                return true;
            }
            else
            {
                return false;
            }

        }

        private void ClearUserInputFieldsAfterSuccessfulChange(string successMessage, WatermarkTextBox primaryKey, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, ComboBox[] comboBoxElementsIndex, CheckBox[] checkBoxElements)
        {
            System.Windows.MessageBox.Show(successMessage);
            ClearUserInputFields(primaryKey, textBoxElements, comboBoxElementsValue, comboBoxElementsIndex, checkBoxElements);
        }

        public void ClearUserInputFields(WatermarkTextBox primaryTextBox, WatermarkTextBox[] textBoxElements, ComboBox[] comboBoxElementsValue, ComboBox[] comboBoxElementsIndex, CheckBox[] checkBoxElements)
        {
            if (primaryTextBox != null) primaryTextBox.Text = "";
            if (textBoxElements != null) foreach (TextBox textBox in textBoxElements) textBox.Text = "";
            if (comboBoxElementsValue != null) foreach (ComboBox comboBox in comboBoxElementsValue) comboBox.SelectedIndex = 0;
            if (comboBoxElementsIndex != null) foreach (ComboBox comboBox in comboBoxElementsIndex) comboBox.SelectedIndex = 0;
            if (checkBoxElements != null) foreach (CheckBox checkBox in checkBoxElements) checkBox.IsChecked = false;
        }

        public void AutoFillExistingElements(DataGrid dataGrid, WatermarkTextBox primaryKeyTextBox, ref KeyValuePair<string, SqlParameterDetails> primaryKey, SqlParameterDictionary parameterDictionary, string getDetailsQuery, string removableSuffice, WatermarkTextBox[] watermarkTextBoxes, ComboBox[] comboBoxesValue, ComboBox[] comboBoxesIndex, CheckBox[] checkBoxes)
        {
            DataRowView dataRowView = (DataRowView)dataGrid.SelectedItem;
            if (dataRowView != null)
            {

                string selectedItem = dataRowView.Row[0].ToString();


                primaryKeyTextBox.Text = selectedItem;
                primaryKey.Value.value = selectedItem;

                ExecuteGetExistingValuesQuery(getDetailsQuery, primaryKey, ref parameterDictionary);

                foreach (KeyValuePair<string, SqlParameterDetails> keyvaluepair in parameterDictionary)
                {
                    System.Diagnostics.Debug.WriteLine(keyvaluepair.Key + " = " + keyvaluepair.Value.value);

                    if (watermarkTextBoxes != null)
                    {
                        foreach (WatermarkTextBox watermarkTextBox in watermarkTextBoxes)
                        {
                            if (string.Equals("@" + watermarkTextBox.Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower(), keyvaluepair.Key))
                            {
                                watermarkTextBox.Text = keyvaluepair.Value.value;
                            }
                        }
                    }

                    if (comboBoxesValue != null)
                    {
                        foreach (ComboBox comboBox in comboBoxesValue)
                        {
                            if (string.Equals("@" + comboBox.Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower(), keyvaluepair.Key))
                            {
                                comboBox.SelectedValue = keyvaluepair.Value.value;
                            }
                        }
                    }

                    if (comboBoxesIndex != null)
                    {
                        foreach (ComboBox comboBox in comboBoxesIndex)
                        {
                            if (string.Equals("@" + comboBox.Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower(), keyvaluepair.Key))
                            {
                                comboBox.SelectedIndex = int.Parse(keyvaluepair.Value.value) - 1;
                            }
                        }
                    }

                    if (checkBoxes != null)
                    {
                        foreach (CheckBox checkBox in checkBoxes)
                        {
                            if (string.Equals("@" + checkBox.Name.ToString().Replace("add" + removableSuffice, "").Replace("update" + removableSuffice, "").ToLower(), keyvaluepair.Key))
                            {
                                checkBox.IsChecked = keyvaluepair.Value.value == "True" ? true : false;
                            }
                        }

                    }
                }
            }
        }

        public void NewDataGridSelection(DataGrid dsetSelected, DataGrid dsetToChange, int rowNumber, KeyValuePair<string, SqlParameterDetails> primaryKey, string query)
        {
            DataRowView dataRowView = (DataRowView)dsetSelected.SelectedItem;
            if (dataRowView != null)
            {
                int selectedItem = Convert.ToInt32(dataRowView.Row[rowNumber]);

                primaryKey.Value.value = selectedItem.ToString();
                dsetToChange.ItemsSource = GetTableFromDatabase(query, primaryKey).DefaultView;
            }
        }

        public DataTable GetTableFromDatabase(string query)
        {
            return GetAppropriateDataTableFromStoredProcedure(query);
        }

        public DataTable GetTableFromDatabase(string query, KeyValuePair<string, SqlParameterDetails> primaryKey)
        {
            return GetAppropriateDataTableFromStoredProcedure(query, GenerateSQLParameter(primaryKey));
        }

        public DataTable GetTableFromDatabase(string query, SqlParameterDictionary parameterDictionary)
        {
            return GetAppropriateDataTableFromStoredProcedure(query, GenerateSQLParameters(parameterDictionary));
        }

        public bool ExecuteStoredProcedureQuery(ref SqlCommand command, bool dispose)
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException er)
            {
                SqlExceptionRegular(er);
                return false;
            }
            finally
            {
                if (dispose) command.Dispose();
            }
        }

        public DataTable StoredProcedureDataTable(SqlCommand command)
        {
            DataTable dataTable = new DataTable();
            ExecuteStoredProcedureQuery(ref command, false);
            objDataAdapter.SelectCommand = command;
            objDataAdapter.Fill(dataTable);

            command.Dispose();
            return dataTable;
        }
        public DataTable GetAppropriateDataTableFromStoredProcedure(string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            return StoredProcedureDataTable(command);
        }

        public DataTable GetAppropriateDataTableFromStoredProcedure(string query, SqlParameter parameter)
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add(parameter);
            return StoredProcedureDataTable(command);
        }

        public DataTable GetAppropriateDataTableFromStoredProcedure(string query, List<SqlParameter> parameters)
        {
            SqlCommand command = new SqlCommand(query, connection);
            foreach (SqlParameter parameter in parameters) command.Parameters.Add(parameter);
            return StoredProcedureDataTable(command);
        }

        public bool ExecuteBasicQuery(string query, SqlParameterDictionary parameters)
        {
            SqlCommand command = new SqlCommand(query, connection);
            foreach (SqlParameter parameter in GenerateSQLParameters(parameters)) command.Parameters.Add(parameter);
            return ExecuteStoredProcedureQuery(ref command, true);

        }

        public bool ExecuteBasicQuery(string query, KeyValuePair<string, SqlParameterDetails> primaryKey)
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add(GenerateSQLParameter(primaryKey));
            return ExecuteStoredProcedureQuery(ref command, true);
        }

    }
}
