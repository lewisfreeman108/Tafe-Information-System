﻿using System;
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

        // private KeyValuePair<String, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherID", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly KeyValuePair<string, SqlParameterDetails> studentNameParameter = new KeyValuePair<string, SqlParameterDetails>("@studentname", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly KeyValuePair<string, SqlParameterDetails> studentIDParameter = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly SqlParameterDictionary teacherStudentParameters = new SqlParameterDictionary();

        public SearchStudentsMethods(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public SearchStudentsMethods(DatabaseConnection databaseConnection, string teacherID) : this(databaseConnection)
        {
            teacherStudentParameters.Add("@teacherID", new SqlParameterDetails(SqlDbType.Int, null));
            teacherStudentParameters.Add("@teacherID", new SqlParameterDetails(SqlDbType.Int, null));
            teacherStudentParameters["@teacherID"].value = teacherID;
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
                    teacherStudentParameters[studentNameParameter.Key].value = studentNameParameter.Value.value;
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByNameForTeacher", teacherStudentParameters).DefaultView;
                    break;
                case 2:
                    searchByCourseParameters.Add(new SqlParameter("teacherID", teacherStudentParameters["@teacherID"].value));
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseNameForTeacher", searchByCourseParameters).DefaultView;
                    break;
            }
        }


        public int SearchStudentName_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByName, ComboBox searchSemester)
        {
            searchByCourseParameters.Clear();
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
            switch (SearchStudentID_Click(dsetStudents, searchType, txtBoxSearchByID, searchSemester))
            {
                case 1:
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByID", teacherStudentParameters).DefaultView;
                    break;
                case 2:
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseID", searchByCourseParameters).DefaultView;
                    break;
            }
        }

        public void SearchStudentIDTeacher_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            switch (SearchStudentID_Click(dsetStudents, searchType, txtBoxSearchByID, searchSemester))
            {
                case 1:
                    teacherStudentParameters[studentIDParameter.Key].value = studentIDParameter.Value.value;
                    dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentByIDForTeacher", teacherStudentParameters).DefaultView;
                    break;
                case 2:
                    searchByCourseParameters.Add(new SqlParameter("teacherID", teacherStudentParameters["@teacherID"].value));
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseIDForTeacher", searchByCourseParameters).DefaultView;
                    break;
            }
        }

        public int SearchStudentID_Click(DataGrid dsetStudents, ComboBox searchType, WatermarkTextBox txtBoxSearchByID, ComboBox searchSemester)
        {
            searchByCourseParameters.Clear();
            if (searchType.SelectedIndex == 0)
            {
                if (ValidationHelper.ValidateOnlyIntegers("Student ID", txtBoxSearchByID.Text))
                {
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
                    dsetStudents.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetStudentsByCourseID", searchByCourseParameters).DefaultView;
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