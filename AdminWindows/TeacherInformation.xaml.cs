using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /* <VersionInfo>
     * Created 30/10/2021, Lewis Freeman
     * Last Updated 29/11/2021, Lewis Freeman
     * </VersionInfo>
     * 
     * <Summary> 
     * A class that handles the adding, updating, and deleting of rows to the teacher entity.
     * </Summary>
     */
    public partial class TeacherInformation : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;

        /*
         *Each entity requires a primary key and parameters, the primary key and parameters are custom user types.
         *The custom user types can be found in the "Helpers" folder. 
         *The custom user types were created to make it easy to add or update the database
         *they help in converting the fields the user has entered into valid sqlparameters to be used in a stored procedure
         */

        private readonly WatermarkTextBox[] addTeacherTextBoxElements;
        private readonly ComboBox[] addTeacherComboBoxElementsValue;
        private readonly CheckBox[] addTeacherCheckBoxElements;

        private KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary teacherParameters = new SqlParameterDictionary();

        private readonly List<SqlParameter> searchTeacherFilters = new List<SqlParameter>();

        //This constructor initializes the custom parameters for each relevant entity and sets up the componenets
        public TeacherInformation(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            InitializeComponent();
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;

            teacherParameters.Add("@admin", new SqlParameterDetails(SqlDbType.Bit, null));
            teacherParameters.Add("@password", new SqlParameterDetails(SqlDbType.VarChar, 255));
            teacherParameters.Add("@salt", new SqlParameterDetails(SqlDbType.VarChar, 255));
            teacherParameters.Add("@firstname", new SqlParameterDetails(SqlDbType.VarChar, 50));
            teacherParameters.Add("@surname", new SqlParameterDetails(SqlDbType.VarChar, 50));
            teacherParameters.Add("@email", new SqlParameterDetails(SqlDbType.VarChar, 100));
            teacherParameters.Add("@mobile", new SqlParameterDetails(SqlDbType.Char, 10));
            teacherParameters.Add("@streetaddress", new SqlParameterDetails(SqlDbType.VarChar, 100));
            teacherParameters.Add("@suburb", new SqlParameterDetails(SqlDbType.VarChar, 40));
            teacherParameters.Add("@postcode", new SqlParameterDetails(SqlDbType.Char, 4));
            teacherParameters.Add("@state", new SqlParameterDetails(SqlDbType.VarChar, 10));
            teacherParameters.Add("@employmenttype", new SqlParameterDetails(SqlDbType.VarChar, 15));
            teacherParameters.Add("@locationname", new SqlParameterDetails(SqlDbType.VarChar, 80));

            addTeacherTextBoxElements = new WatermarkTextBox[] { addTFirstName, addTSurname, addTEmail, addTMobile, addTStreetAddress, addTSuburb, addTPostcode, addTLocationName };
            addTeacherComboBoxElementsValue = new ComboBox[] { addTEmploymentType, addTState };
            addTeacherCheckBoxElements = new CheckBox[] { addTAdmin };
        }

        //Resets the form for next time it is opned
        public void Reset()
        {
            dsetAllTeachers.ItemsSource = null;
            dsetPastCoursesForTeacher.ItemsSource = null;
            dsetCoursesAndLocationsForTeacher.ItemsSource = null;

            databaseConnection.ClearUserInputFields(updateTTeacherID, addTeacherTextBoxElements, addTeacherComboBoxElementsValue, null, addTeacherCheckBoxElements);
        }


        private void btnSearchTeachers_Click(object sender, RoutedEventArgs e)
        {
            SearchTeachersTeaching();
        }

        //adds a variety of filters to the search query, the search query allows nulls as it makes use of coalesce, this is how a variety of filters can be used in a simple way
        private void SearchAllTeacherFilters()
        {
            searchTeacherFilters.Clear();

            if (!string.IsNullOrWhiteSpace(txtBoxSearchFirstName.Text))
            {
                searchTeacherFilters.Add(new SqlParameter("@firstname", txtBoxSearchFirstName.Text));
            }
            else
            {
                searchTeacherFilters.Add(new SqlParameter("@firstname", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchSurname.Text))
            {
                searchTeacherFilters.Add(new SqlParameter("@surname", txtBoxSearchSurname.Text));
            }
            else
            {
                searchTeacherFilters.Add(new SqlParameter("@surname", DBNull.Value));
            }

            if (cmbBoxEmploymentType.SelectedIndex != 0)
            {
                searchTeacherFilters.Add(new SqlParameter("@employmenttype", cmbBoxEmploymentType.SelectedIndex == 1 ? "Part Time" : "Full Time"));
            }
            else
            {
                searchTeacherFilters.Add(new SqlParameter("@employmenttype", DBNull.Value));
            }
        }

        //This method will return all teachers that have related timetableitems. (Their class)
        private void SearchTeachersTeaching()
        {
            SearchAllTeacherFilters();

            if (cmbBoxSemester.SelectedIndex != 0)
            {
                searchTeacherFilters.Add(new SqlParameter("@semester", cmbBoxSemester.SelectedIndex));
            }
            else
            {
                searchTeacherFilters.Add(new SqlParameter("@semester", DBNull.Value));
            }

            if (cmbBoxLocationSearchType.SelectedIndex != 0)
            {
                if (!string.IsNullOrWhiteSpace(txtBoxSearchLocation.Text))
                {
                    searchTeacherFilters.Add(new SqlParameter("@locationname", txtBoxSearchLocation.Text));
                }
                else
                {
                    searchTeacherFilters.Add(new SqlParameter("@locationname", DBNull.Value));
                }
            }


            switch (cmbBoxLocationSearchType.SelectedIndex)
            {
                case 0:
                    dsetAllTeachers.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_SearchTeachersWithNoLocationFilter", searchTeacherFilters).DefaultView;
                    break;
                case 1:
                    dsetAllTeachers.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_SearchTeachersWithBasedLocationFilter", searchTeacherFilters).DefaultView;
                    break;
                case 2:
                    dsetAllTeachers.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_SearchTeachersWithTeachingAtLocationFilter", searchTeacherFilters).DefaultView;
                    break;
            }
        }

        //This method will add a teacher to the database if the input variables are validated
        private void btnAddTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (databaseConnection.AddUserToDatabase(teacherParameters, addTPassword.Text, addTeacherTextBoxElements, addTeacherComboBoxElementsValue, addTeacherCheckBoxElements, "T", "Successfully added teacher", "tsp_AddTeacher"))
            {
                addTPassword.Text = "";
            }
        }

        //This method will update a row in the teacher entity with a valid primary key but only the columns that are specified (that the user has entered new data for)
        private void btnUpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (databaseConnection.UpdateUserInDatabase(teacherPrimaryKey, teacherParameters, addTPassword.Text, "tsp_GetTeacherSalt", updateTTeacherID, addTeacherTextBoxElements, addTeacherComboBoxElementsValue, "T", "tsp_GetTeacherDetails", "tsp_UpdateTeacherDetails", "Successfully updated teacher"))
            {
                addTPassword.Text = "";
            }
        }

        //This method will show the past courses of a teacher and the past course and location combinations for a teacher, as well as autofill the update form
        private void dsetAllTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetAllTeachers, updateTTeacherID, ref teacherPrimaryKey, teacherParameters, "tsp_GetTeacherDetails", "T", addTeacherTextBoxElements, addTeacherComboBoxElementsValue, null, addTeacherCheckBoxElements);
            databaseConnection.NewDataGridSelection(dsetAllTeachers, dsetPastCoursesForTeacher, 0, teacherPrimaryKey, "tsp_ShowAllPreviousCoursesForTeacher");
            databaseConnection.NewDataGridSelection(dsetAllTeachers, dsetCoursesAndLocationsForTeacher, 0, teacherPrimaryKey, "tsp_DisplayCoursesAndLocationsForTeacher");
        }

        //This method will remove a row from the teacher entity of it exists
        private void btnRemoveTeacher_Click(object sender, RoutedEventArgs e)
        {
            databaseConnection.RemoveRow("tsp_RemoveTeacher", ref teacherPrimaryKey, "Successfully removed teacher", updateTTeacherID, addTeacherTextBoxElements, addTeacherComboBoxElementsValue, null, addTeacherCheckBoxElements);
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }

        //This method will return all teachers
        private void btnSearchAllTeachers_Click(object sender, RoutedEventArgs e)
        {
            SearchAllTeacherFilters();
            dsetAllTeachers.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_SearchAllTeachers", searchTeacherFilters).DefaultView;
        }

        //This method handles the visibility of the search location textbox based on the selected location search type
        private void cmbBoxLocationSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBoxLocationSearchType.SelectedIndex == 0) txtBoxSearchLocation.Text = "";
            txtBoxSearchLocation.Visibility = cmbBoxLocationSearchType.SelectedIndex == 0 ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
