using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Tafe_System.AdminWindows;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class Students : Window
    {

        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;

        private readonly SearchStudentsMethods searchStudentMethods;

        private readonly List<SqlParameter> searchParameters = new List<SqlParameter>();

        private KeyValuePair<string, SqlParameterDetails> studentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary studentParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addStudentTextBoxElements;
        private readonly ComboBox[] addStudentComboBoxElementsValue;

        private KeyValuePair<string, SqlParameterDetails> enrolmentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@enrolmentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary enrolmentParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addEnrolmentTextBoxElements;
        private readonly ComboBox[] addEnrolmentComboBoxElementsValue;

        private readonly SqlParameterDictionary enrolmentValidationParameters = new SqlParameterDictionary();
        private readonly WatermarkTextBox[] enrolmentValidationTextBoxElements;

        public Students(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {

            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;

            searchStudentMethods = new SearchStudentsMethods(databaseConnection);

            InitializeComponent();

            searchSemester.IsEnabled = false;


            studentParameters.Add("@password", new SqlParameterDetails(SqlDbType.VarChar, 255));
            studentParameters.Add("@salt", new SqlParameterDetails(SqlDbType.VarChar, 255));
            studentParameters.Add("@firstname", new SqlParameterDetails(SqlDbType.VarChar, 50));
            studentParameters.Add("@surname", new SqlParameterDetails(SqlDbType.VarChar, 50));
            studentParameters.Add("@email", new SqlParameterDetails(SqlDbType.VarChar, 100));
            studentParameters.Add("@mobile", new SqlParameterDetails(SqlDbType.Char, 10));
            studentParameters.Add("@streetaddress", new SqlParameterDetails(SqlDbType.VarChar, 100));
            studentParameters.Add("@suburb", new SqlParameterDetails(SqlDbType.VarChar, 40));
            studentParameters.Add("@postcode", new SqlParameterDetails(SqlDbType.Char, 4));
            studentParameters.Add("@state", new SqlParameterDetails(SqlDbType.VarChar, 10));

            addStudentTextBoxElements = new WatermarkTextBox[] { addSFirstName, addSSurname, addSEmail, addSMobile, addSStreetAddress, addSSuburb, addSPostcode };
            addStudentComboBoxElementsValue = new ComboBox[] { addSState };

            enrolmentParameters.Add("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
            enrolmentParameters.Add("@offeringid", new SqlParameterDetails(SqlDbType.Int, null));
            enrolmentParameters.Add("@paymentmethod", new SqlParameterDetails(SqlDbType.VarChar, 20));
            enrolmentParameters.Add("@amountpaid", new SqlParameterDetails(SqlDbType.Money, null));


            addEnrolmentTextBoxElements = new WatermarkTextBox[] { addEAmountPaid, addEOfferingID, addEStudentID };
            addEnrolmentComboBoxElementsValue = new ComboBox[] { addEPaymentMethod };

            enrolmentValidationParameters.Add("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
            enrolmentValidationParameters.Add("@offeringid", new SqlParameterDetails(SqlDbType.Int, null));
            enrolmentValidationTextBoxElements = new WatermarkTextBox[] { addEStudentID, addEOfferingID };
        }

        public void Reset()
        {
            dsetStudents.ItemsSource = null;
            dsetEnrolments.ItemsSource = null;

            databaseConnection.ClearUserInputFields(updateSStudentID, addStudentTextBoxElements, addStudentComboBoxElementsValue, null, null);
            databaseConnection.ClearUserInputFields(updateEEnrolmentID, addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null);
        }

        private void btnSearchEnrolment_Click(object sender, RoutedEventArgs e)
        {
            SearchEnrolments();
        }

        private void SearchEnrolments()
        {
            searchParameters.Clear();

            if (!string.IsNullOrWhiteSpace(txtBoxSearchEnrolmentStudentID.Text))
            {
                searchParameters.Add(new SqlParameter("@studentid", txtBoxSearchEnrolmentStudentID.Text));
            }
            else
            {
                searchParameters.Add(new SqlParameter("@studentid", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchEnrolmentStudentID.Text))
            {
                searchParameters.Add(new SqlParameter("@courseid", txtBoxSearchEnrolmentCourseID.Text));
            }
            else
            {
                searchParameters.Add(new SqlParameter("@courseid", DBNull.Value));
            }

            if (cmbBoxSemester.SelectedIndex != 0)
            {
                searchParameters.Add(new SqlParameter("@semester", cmbBoxSemester.SelectedIndex + 1));
            }
            else
            {
                searchParameters.Add(new SqlParameter("@semester", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchEnrolmentYear.Text))
            {
                searchParameters.Add(new SqlParameter("@year", txtBoxSearchEnrolmentYear.Text));
            }
            else
            {
                searchParameters.Add(new SqlParameter("@year", DBNull.Value));
            }

            if (ValidationHelper.ValidateOnlyIntegers("Course ID", txtBoxSearchEnrolmentCourseID.Text) && ValidationHelper.ValidateOnlyIntegers("Student id", txtBoxSearchEnrolmentStudentID.Text) && ValidationHelper.ValidateOnlyIntegers("Year", txtBoxSearchEnrolmentYear.Text))
            {
                if (cmbBoxMode.SelectedIndex == 2)
                {
                    dsetEnrolments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure(chkBoxOutstandingPayment.IsChecked == true ? "tsp_GetOnlineEnrolmentsOutstandingPayment" : "tsp_GetOnlineEnrolmentsNoOutstandingPayment", searchParameters).DefaultView;
                }
                else if (cmbBoxMode.SelectedIndex == 0)
                {
                    if (ValidationHelper.ValidateNoIntegers("Location name", txtBoxSearchEnrolmentLocationName.Text))
                    {
                        searchParameters.Add(new SqlParameter("@locationname", DBNull.Value));
                        dsetEnrolments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure(chkBoxOutstandingPayment.IsChecked == true ? "tsp_GetEnrolmentsOutstandingPayment" : "tsp_GetEnrolmentsNoOutstandingPayment", searchParameters).DefaultView;
                    }
                }
                else
                {
                    if (ValidationHelper.ValidateNoIntegers("Location name", txtBoxSearchEnrolmentLocationName.Text))
                    {

                        searchParameters.Add(new SqlParameter("@locationname", txtBoxSearchEnrolmentLocationName.Text));
                        dsetEnrolments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure(chkBoxOutstandingPayment.IsChecked == true ? "tsp_GetEnrolmentsOustandingPayment" : "tsp_GetEnrolmentsNoOutstandingPayment", searchParameters).DefaultView;
                    }
                }
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("First name", addSFirstName.Text) && ValidationHelper.ValidateNoIntegers("Surname", addSSurname.Text) && ValidationHelper.ValidateIsEmail(addSEmail.Text) && ValidationHelper.ValidateIsMobile("Mobile number", addSMobile.Text) && ValidationHelper.ValidateNoIntegers("Suburb", addSSuburb.Text) && ValidationHelper.ValidateIsPostCode("Postcode", addSPostcode.Text))
            {
                if (databaseConnection.AddUserToDatabase(studentParameters, addSPassword.Text, addStudentTextBoxElements, addStudentComboBoxElementsValue, null, "S", "Successfully added student", "tsp_AddStudent"))
                {
                    addSPassword.Text = "";
                }

            }
        }


        private void btnUpdateStudent_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Student ID", updateSStudentID.Text) && ValidationHelper.ValidateNoIntegers("First name", addSFirstName.Text) && ValidationHelper.ValidateNoIntegers("Surname", addSSurname.Text) && ValidationHelper.ValidateIsEmail(addSEmail.Text) && ValidationHelper.ValidateIsMobile("Mobile number", addSMobile.Text) && ValidationHelper.ValidateNoIntegers("Suburb", addSSuburb.Text) && ValidationHelper.ValidateIsPostCode("Postcode", addSPostcode.Text))
            {
                databaseConnection.UpdateUserInDatabase(studentPrimaryKey, studentParameters, addSPassword.Text, "tsp_GetStudentSalt", updateSStudentID, addStudentTextBoxElements, addStudentComboBoxElementsValue, "S", "tsp_GetStudentDetails", "tsp_UpdateStudentDetails", "Successfully updated user");
            }
            addSPassword.Text = "";
        }

        private void btnAddEnrolment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Amount paid", addEAmountPaid.Text) && ValidationHelper.ValidateOnlyIntegers("Student ID", addEStudentID.Text) && ValidationHelper.ValidateOnlyIntegers("OfferingID", addEOfferingID.Text))
            {
                if (databaseConnection.ValidateEnrolment(enrolmentValidationParameters, enrolmentValidationTextBoxElements, out string result))
                {
                    databaseConnection.AddToDatabase(enrolmentParameters, addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null, "E", "Successfully added enrolment", "tsp_EnrolStudent");
                }
                else
                {
                    System.Windows.MessageBox.Show(result);
                }
            }
        }


        private void btnUpdateEnrolment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Enrolment ID", updateEEnrolmentID.Text) && ValidationHelper.ValidateOnlyIntegers("Amount paid", addEAmountPaid.Text) && ValidationHelper.ValidateOnlyIntegers("Student ID", addEStudentID.Text) && ValidationHelper.ValidateOnlyIntegers("OfferingID", addEOfferingID.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateEnrolmentDetails", "tsp_GetEnrolmentDetails", enrolmentPrimaryKey, enrolmentParameters, updateEEnrolmentID, addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null, "E", "Successfully enrolled student");
            }
        }

        private void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Student ID", updateSStudentID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveStudent", ref studentPrimaryKey, "Successfully removed student", updateSStudentID, addStudentTextBoxElements, addStudentComboBoxElementsValue, null, null);
            }
        }

        private void btnRemoveEnrolment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Enrolment ID", updateEEnrolmentID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveEnrolment", ref enrolmentPrimaryKey, "Successfully removed enrolment", updateEEnrolmentID, addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null);
            }
        }


        private void dsetEnrolments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetEnrolments, updateEEnrolmentID, ref enrolmentPrimaryKey, enrolmentParameters, "tsp_GetEnrolmentDetails", "E", addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null);
        }


        private void btnSearchAllStudents_Click(object sender, RoutedEventArgs e)
        {
            dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllStudents").DefaultView;
            dsetEnrolments.ItemsSource = null;
            databaseConnection.ClearUserInputFields(updateEEnrolmentID, addEnrolmentTextBoxElements, addEnrolmentComboBoxElementsValue, null, null);
        }

        private void btnSearchStudentName_Click(object sender, RoutedEventArgs e)
        {
            searchStudentMethods.SearchStudentName_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester);
            dsetEnrolments.ItemsSource = null;
        }

        private void btnSearchStudentID_Click(object sender, RoutedEventArgs e)
        {
            searchStudentMethods.SearchStudentIDAdmin_Click(dsetStudents, searchType, txtBoxSearchByID, searchSemester);
            dsetEnrolments.ItemsSource = null;
        }
        private void searchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchStudentMethods.SearchType_SelectionChanged(txtBoxSearchByID, txtBoxSearchByName, searchType, searchSemester);
        }

        private void dsetStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.NewDataGridSelection(dsetStudents, dsetEnrolments, 0, studentPrimaryKey, "tsp_GetStudentEnrolments");
            databaseConnection.AutoFillExistingElements(dsetStudents, updateSStudentID, ref studentPrimaryKey, studentParameters, "tsp_GetStudentDetails", "S", addStudentTextBoxElements, addStudentComboBoxElementsValue, null, null);
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
