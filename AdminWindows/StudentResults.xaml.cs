using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Tafe_System.AdminWindows;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for StudentResults.xaml
    /// </summary>
    public partial class StudentResults : Window
    {
        private readonly DatabaseConnection databaseConnection;
        private readonly MainMenu mainMenu;

        private readonly SearchStudentsMethods searchStudentMethods;
        private readonly KeyValuePair<string, SqlParameterDetails> studentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherid", new SqlParameterDetails(SqlDbType.Int, null));

        public StudentResults(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.databaseConnection = databaseConnection;
            this.mainMenu = mainMenu;
            teacherPrimaryKey.Value.value = "-1";
            searchStudentMethods = new SearchStudentsMethods(databaseConnection);
            InitializeComponent();
            searchSemester.IsEnabled = false;
        }

        public void setTeacher(string teacherID)
        {
            teacherPrimaryKey.Value.value = teacherID;
            searchStudentMethods.SetTeacher(teacherID);
        }

        public void Reset()
        {
            dsetStudents.ItemsSource = null;
            teacherPrimaryKey.Value.value = "-1";
            ResetResults();
        }

        private void ResetResults()
        {
            dsetUnitResults.ItemsSource = null;
            dsetClusterResults.ItemsSource = null;
            dsetUnitResults.ItemsSource = null;
            dsetAssessmentResults.ItemsSource = null;
            dsetCourseResults.ItemsSource = null;
        }
        private void btnSearchAllStudents_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(teacherPrimaryKey.Value.value, "-1"))
            {
                dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllStudents").DefaultView;
            }
            else
            {
                dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllStudentsForTeacher", teacherPrimaryKey).DefaultView;
            }
            ResetResults();
        }

        private void btnSearchStudentName_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(teacherPrimaryKey.Value.value, "-1"))
            {
                searchStudentMethods.SearchStudentNameAdmin_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester);
            }
            else
            {
                searchStudentMethods.SearchStudentNameTeacher_Click(dsetStudents, searchType, txtBoxSearchByName, searchSemester);
            }

            ResetResults();

        }

        private void btnSearchStudentID_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(teacherPrimaryKey.Value.value, "-1"))
            {
                searchStudentMethods.SearchStudentIDAdmin_Click(dsetStudents, searchType, txtBoxSearchByID, searchSemester);
            }
            else
            {
                searchStudentMethods.SearchStudentIDTeacher_Click(dsetStudents, searchType, txtBoxSearchByID, searchSemester);
            }
            ResetResults();
        }

        private void searchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchStudentMethods.SearchType_SelectionChanged(txtBoxSearchByID, txtBoxSearchByName, searchType, searchSemester);
        }


        private void dsetStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            databaseConnection.NewDataGridSelection(dsetStudents, dsetAssessmentResults, 0, studentPrimaryKey, "tsp_GetAssessmentResults");
            databaseConnection.NewDataGridSelection(dsetStudents, dsetUnitResults, 0, studentPrimaryKey, "tsp_GetUnitResults");
            databaseConnection.NewDataGridSelection(dsetStudents, dsetClusterResults, 0, studentPrimaryKey, "tsp_GetUnitClusterResults");
            databaseConnection.NewDataGridSelection(dsetStudents, dsetCourseResults, 0, studentPrimaryKey, "tsp_GetCourseResults");

        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
