using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System.AdminWindows
{
    public class SearchStudentsMethods
    {
        private readonly DatabaseConnection databaseConnection = new DatabaseConnection();

        private readonly List<SqlParameter> searchByCourseParameters = new List<SqlParameter>();

        private readonly KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly KeyValuePair<string, SqlParameterDetails> studentNameParameter = new KeyValuePair<string, SqlParameterDetails>("@studentname", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly KeyValuePair<string, SqlParameterDetails> studentIDParameter = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly KeyValuePair<string, SqlParameterDetails> courseNameParameter = new KeyValuePair<string, SqlParameterDetails>("@coursename", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly KeyValuePair<string, SqlParameterDetails> courseIDParameter = new KeyValuePair<string, SqlParameterDetails>("@courseid", new SqlParameterDetails(SqlDbType.Int, null));


        private readonly SqlParameterDictionary searchParameters = new SqlParameterDictionary();

        //TODO: Fix the search methods
        public SearchStudentsMethods(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
            searchParameters.Add("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
        }

        public void SetTeacher(string teacherID)
        {
            searchParameters.Remove("@teacherid");
            searchParameters.Add("@teacherid", teacherPrimaryKey.Value);
            searchParameters["@teacherid"].value = teacherID;
        }

        public void SetAdmin()
        {
            searchParameters.Remove("@teacherid");
        }

        public void SearchStudentNameAdmin_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            switch (SearchStudentName_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByName", studentNameParameter).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseName", searchByCourseParameters).DefaultView;
                    break;
            }
        }

        public void SearchStudentNameTeacher_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            switch (SearchStudentName_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester))
            {
                case 1:
                    searchParameters.Add("@studentname", studentNameParameter.Value);
                    searchParameters[studentNameParameter.Key].value = txtBoxSearchByName.Text;
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByNameForTeacher", searchParameters).DefaultView;
                    searchParameters.Remove("@studentname");
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseNameForTeacher", searchByCourseParameters).DefaultView;
                    break;
            }
        }


        public int SearchStudentName_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            searchByCourseParameters.Clear();
            searchByCourseParameters.Add(new SqlParameter("@teacherid", searchParameters["@teacherid"].value));
            if (searchType.SelectedIndex == 0)
            {
                if (ValidationHelper.ValidateNoIntegers("Student name", txtBoxSearchByName.Text))
                {
                    studentNameParameter.Value.value = txtBoxSearchByName.Text;
                    return 1;
                }
                MessageBox.Show("Student name must not contain integers");
                return 0;
            }
            else
            {
                if (searchSemester.SelectedIndex != 0)
                {
                    searchByCourseParameters.Add(new SqlParameter("@semester", searchSemester.SelectedIndex));
                }
                else
                {
                    searchByCourseParameters.Add(new SqlParameter("@semester", DBNull.Value));
                }

                if (!string.IsNullOrWhiteSpace(txtBoxSearchByName.Text))
                {
                    searchByCourseParameters.Add(new SqlParameter("@coursename", txtBoxSearchByName.Text));
                }
                else
                {
                    searchByCourseParameters.Add(new SqlParameter("@coursename", DBNull.Value));
                }

                if (ValidationHelper.ValidateNoIntegers("Course Name", txtBoxSearchByName.Text))
                {
                    return 2;
                }
                MessageBox.Show("Course name must not contain integers");
                return 0;
            }
        }

        public void SearchStudentIDAdmin_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            switch (SearchStudentID_Click(searchType, txtBoxSearchByID, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByID", searchParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseID", searchByCourseParameters).DefaultView;
                    break;
            }
        }

        public void SearchStudentIDTeacher_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {

            switch (SearchStudentID_Click(searchType, txtBoxSearchByID, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByIDForTeacher", searchParameters).DefaultView;
                    searchParameters.Remove("@studentid");
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseIDForTeacher", searchByCourseParameters).DefaultView;
                    break;
            }
        }

        public int SearchStudentID_Click(ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            searchParameters.Clear();
            searchParameters.Add("@teacherid", teacherPrimaryKey.Value);

            if (searchType.SelectedIndex == 0)
            {
                if (ValidationHelper.ValidateOnlyIntegers("Student ID", txtBoxSearchByID.Text))
                {
                    searchParameters["@studentid"].value = txtBoxSearchByID.Text;
                    studentIDParameter.Value.value = txtBoxSearchByID.Text;
                    return 1;
                }
                MessageBox.Show("Student ID must be an integer");
                return 0;
            }
            else
            {
                if (searchSemester.SelectedIndex != 0)
                {
                    searchByCourseParameters.Add(new SqlParameter("@semester", searchSemester.SelectedIndex));
                }
                else
                {
                    searchByCourseParameters.Add(new SqlParameter("@semester", DBNull.Value));
                }

                if (!string.IsNullOrWhiteSpace(txtBoxSearchByID.Text))
                {

                    searchByCourseParameters.Add(new SqlParameter("@courseid", txtBoxSearchByID.Text));

                }
                else
                {
                    searchByCourseParameters.Add(new SqlParameter("@courseid", DBNull.Value));
                }
                if (ValidationHelper.ValidateOnlyIntegers("Course ID", txtBoxSearchByID.Text))
                {
                    searchParameters.Add("@courseid", courseIDParameter.Value);
                    searchParameters["@courseid"].value = txtBoxSearchByID.Text;
                    return 2;
                }
                MessageBox.Show("Course ID must be an integer");
                return 0;
            }
        }

        public void SearchType_SelectionChanged(WatermarkTextBox txtBoxSearchByID, WatermarkTextBox txtBoxSearchByName, ComboBox searchType, ComboBox searchSemester)
        {
            txtBoxSearchByID.Watermark = searchType.SelectedIndex == 0 ? "Search for a student by their ID" : "Search for students by course ID";
            txtBoxSearchByName.Watermark = searchType.SelectedIndex == 0 ? "Search for a student by their name" : "Search for students by course name";

            if (searchSemester != null) searchSemester.IsEnabled = searchType.SelectedIndex == 1;
        }
    }
}
