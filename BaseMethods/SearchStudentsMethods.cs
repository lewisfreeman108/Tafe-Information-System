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

        private readonly List<SqlParameter> searchParameters = new List<SqlParameter>();

        private readonly SqlParameter teacherID = new SqlParameter("@teacherid", SqlDbType.Int);

        //TODO: Fix the search methods
        public SearchStudentsMethods(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void SetTeacher(string teacherID)
        {
            searchParameters.Remove(this.teacherID);
            this.teacherID.Value = teacherID;
            searchParameters.Add(this.teacherID);
        }

        public void SetAdmin()
        {
            searchParameters.Remove(teacherID);
        }

        public void SearchStudentNameAdmin_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            switch (SearchStudentName_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentByName", searchParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseName", searchParameters).DefaultView;
                    break;
            }
        }

        public void SearchStudentNameTeacher_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            int courseOrStudent = SearchStudentName_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester);
            searchParameters.Add(teacherID);
            switch (courseOrStudent)
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentByNameForTeacher", searchParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseNameForTeacher", searchParameters).DefaultView;
                    break;
            }
        }


        public void SearchStudentIDAdmin_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            switch (SearchStudentID_Click(searchType, txtBoxSearchByID, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentByID", searchParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseID", searchParameters).DefaultView;
                    break;
            }
        }

        public void SearchStudentIDTeacher_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            int searchMethod = SearchStudentID_Click(searchType, txtBoxSearchByID, searchSemester);
            searchParameters.Add(teacherID);
            switch (searchMethod)
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentByIDForTeacher", searchParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseIDForTeacher", searchParameters).DefaultView;
                    break;
            }
        }

        public int SearchStudentID_Click(ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            searchParameters.Clear();

            if (searchType.SelectedIndex == 0)
            {
                if (!string.IsNullOrWhiteSpace(txtBoxSearchByID.Text))
                {
                    searchParameters.Add(new SqlParameter("@studentid", txtBoxSearchByID.Text));
                }
                else
                {
                    searchParameters.Add(new SqlParameter("@studentid", DBNull.Value));
                }
                if (ValidationHelper.ValidateOnlyIntegers("Student ID", txtBoxSearchByID.Text))
                {
                    return 1;
                }
                MessageBox.Show("Student ID must be an integer");
                return 0;
            }
            else
            {
                if (searchSemester.SelectedIndex != 0)
                {
                    searchParameters.Add(new SqlParameter("@semester", searchSemester.SelectedIndex));
                }
                else
                {
                    searchParameters.Add(new SqlParameter("@semester", DBNull.Value));
                }

                if (!string.IsNullOrWhiteSpace(txtBoxSearchByID.Text))
                {

                    searchParameters.Add(new SqlParameter("@courseid", txtBoxSearchByID.Text));

                }
                else
                {
                    searchParameters.Add(new SqlParameter("@courseid", DBNull.Value));
                }
                if (ValidationHelper.ValidateOnlyIntegers("Course ID", txtBoxSearchByID.Text))
                {
                    return 2;
                }
                MessageBox.Show("Course ID must be an integer");
                return 0;
            }
        }



        public int SearchStudentName_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            searchParameters.Clear();

            if (string.IsNullOrEmpty(txtBoxSearchByName.Text))
            {
                return 0;
            }

            if (searchType.SelectedIndex == 0)
            {
                if (!string.IsNullOrWhiteSpace(txtBoxSearchByName.Text))
                {
                    searchParameters.Add(new SqlParameter("@studentname", txtBoxSearchByName.Text));
                }
                else
                {
                    searchParameters.Add(new SqlParameter("@studentname", DBNull.Value));
                }
                if (ValidationHelper.ValidateOnlyIntegers("Student Name", txtBoxSearchByName.Text))
                {
                    return 1;
                }
                MessageBox.Show("Student ID must be an integer");
                return 0;
            }
            else
            {
                if (searchSemester.SelectedIndex != 0)
                {
                    searchParameters.Add(new SqlParameter("@semester", searchSemester.SelectedIndex));
                }
                else
                {
                    searchParameters.Add(new SqlParameter("@semester", DBNull.Value));
                }

                if (!string.IsNullOrWhiteSpace(txtBoxSearchByName.Text))
                {
                    searchParameters.Add(new SqlParameter("@coursename", txtBoxSearchByName.Text));
                }
                else
                {
                    searchParameters.Add(new SqlParameter("@coursename", DBNull.Value));
                }

                if (ValidationHelper.ValidateNoIntegers("Course Name", txtBoxSearchByName.Text))
                {
                    return 2;
                }
                MessageBox.Show("Course name must not contain integers");
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
