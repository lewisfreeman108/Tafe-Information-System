using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for Submissions.xaml
    /// </summary>
    public partial class Submissions : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;
        private readonly BaseSearchOfferingMethods baseSearchOfferingMethods;

        private readonly WatermarkTextBox[] updateSubmissionTextBoxElements;
        private readonly ComboBox[] updateSubmissionComboBoxElementsValue;

        private KeyValuePair<string, SqlParameterDetails> submissionPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@submissionid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly SqlParameterDictionary searchSubmissionParameters = new SqlParameterDictionary();

        private readonly KeyValuePair<string, SqlParameterDetails> offeringPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@offeringid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly List<SqlParameter> searchOfferingParameters = new List<SqlParameter>();

        private readonly SqlParameterDictionary submissionResultParameters = new SqlParameterDictionary();

        private readonly KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherid", new SqlParameterDetails(SqlDbType.Int, null));


        public Submissions(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;

            InitializeComponent();

            updateSubmissionTextBoxElements = new WatermarkTextBox[] { updateSComments };
            updateSubmissionComboBoxElementsValue = new ComboBox[] { updateSResult };

            searchSubmissionParameters.Add("@timetableid", new SqlParameterDetails(SqlDbType.Int, null));
            searchSubmissionParameters.Add("@studentid", new SqlParameterDetails(SqlDbType.Int, null));

            submissionResultParameters.AddParameter("@teachermarkingid", SqlDbType.Int);
            submissionResultParameters.AddParameter("@comments", SqlDbType.VarChar, 300);
            submissionResultParameters.AddParameter("@result", SqlDbType.VarChar, 15);

            updateSubmissionTextBoxElements = new WatermarkTextBox[] { updateSSubmissionID, updateSComments };
            updateSubmissionComboBoxElementsValue = new ComboBox[] { updateSResult };

            baseSearchOfferingMethods = new BaseSearchOfferingMethods(databaseConnection);
        }

        public void SetTeacher(string teacherID)
        {
            submissionResultParameters["@teachermarkingid"].value = teacherID;
            teacherPrimaryKey.Value.value = teacherID;
        }

        public void Reset()
        {
            dsetOfferings.ItemsSource = null;
            dsetStudents.ItemsSource = null;
            dsetSubmissions.ItemsSource = null;
            dsetTimetableItem.ItemsSource = null;

            databaseConnection.ClearUserInputFields(updateSSubmissionID, updateSubmissionTextBoxElements, updateSubmissionComboBoxElementsValue, null, null);
        }


        private void dsetStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSubmissions();
        }

        private void dsetTimetableItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSubmissions();
        }

        private void GetSubmissions()
        {

            DataRowView dataRowView = (DataRowView)dsetTimetableItem.SelectedItem;

            DataRowView dataRowView2 = (DataRowView)dsetStudents.SelectedItem;

            if (dataRowView != null && dataRowView2 != null)
            {

                string selectedTimetableItem = dataRowView.Row[0].ToString();

                string selectedStudent = dataRowView2.Row[0].ToString();

                searchSubmissionParameters["@timetableid"].value = selectedTimetableItem;
                searchSubmissionParameters["@studentid"].value = selectedStudent;
                dsetSubmissions.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetSubmissions", searchSubmissionParameters).DefaultView;
            }
            else
            {
                dsetSubmissions.ItemsSource = null;
            }

        }


        private void dsetSubmissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetSubmissions, updateSSubmissionID, ref submissionPrimaryKey, submissionResultParameters, "tsp_GetSubmissionDetails", "S", updateSubmissionTextBoxElements, updateSubmissionComboBoxElementsValue, null, null);
        }

        private void btnUpdateSubmission_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Submission ID", updateSSubmissionID.Text))
            {
                submissionResultParameters.AddParameter("@submissionid", updateSSubmissionID.Text, SqlDbType.Int);
                submissionResultParameters["@result"].value = updateSResult.Text;
                submissionResultParameters["@comments"].value = updateSComments.Text;

                if (databaseConnection.ExecuteBasicQuery("tsp_UpdateSubmissionResult", submissionResultParameters))
                {
                    GetSubmissions();
                    System.Windows.MessageBox.Show("Successfully graded submission");
                    databaseConnection.ClearUserInputFields(updateSSubmissionID, updateSubmissionTextBoxElements, updateSubmissionComboBoxElementsValue, null, null);

                }
                submissionResultParameters.Remove("@submissionid");
            }
        }

        private void GetStudents()
        {
            DataRowView dataRowView = (DataRowView)dsetOfferings.SelectedItem;
            if (dataRowView != null)
            {
                string selectedCourseLocation = dataRowView.Row[0].ToString();

                offeringPrimaryKey.Value.value = selectedCourseLocation;
                dsetStudents.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetStudentsFromOffering", offeringPrimaryKey).DefaultView;
            }
            dsetSubmissions.ItemsSource = null;
        }

        private void OfferingSelectionChanged()
        {
            dsetStudents.ItemsSource = null;
            dsetSubmissions.ItemsSource = null;
            dsetTimetableItem.ItemsSource = null;
        }

        private void dsetOfferings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.NewDataGridSelection(dsetOfferings, dsetTimetableItem, 0, offeringPrimaryKey, "tsp_GetTimetableItemsForOffering");
            GetStudents();
        }

        private void btnSearchWithFilters_Click(object sender, RoutedEventArgs e)
        {
            OfferingSelectionChanged();
            baseSearchOfferingMethods.SearchOfferingsWithFilters(teacherPrimaryKey, dsetOfferings, searchOfferingParameters, searchSemester, txtBoxSearchByCourse, txtBoxSearchByLocation, txtBoxSearchYear);
        }

        private void btnSearchAllOfferings_Click(object sender, RoutedEventArgs e)
        {
            OfferingSelectionChanged();
            baseSearchOfferingMethods.SearchAllOfferings(teacherPrimaryKey, dsetOfferings);
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }

    }
}
