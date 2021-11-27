using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tafe_System.StudentWindows
{
    /// <summary>
    /// Interaction logic for MyAssessments.xaml
    /// </summary>
    public partial class MyAssessments : Window
    {
        private string studentID;

        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;


        private readonly KeyValuePair<string, SqlParameterDetails> clusterPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@clusterid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly KeyValuePair<string, SqlParameterDetails> enrolmentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@enrolmentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly KeyValuePair<string, SqlParameterDetails> studentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly SqlParameterDictionary submissionParameters = new SqlParameterDictionary();
        private readonly KeyValuePair<string, SqlParameterDetails> submissionFileName = new KeyValuePair<string, SqlParameterDetails>("@submissionfilename", new SqlParameterDetails(SqlDbType.VarChar, 100));

        private readonly KeyValuePair<string, SqlParameterDetails> submissionResult = new KeyValuePair<string, SqlParameterDetails>("@result", new SqlParameterDetails(SqlDbType.VarChar, 15));
        
        private readonly SqlParameterDictionary resourceParameters = new SqlParameterDictionary();


        public MyAssessments(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();

            submissionParameters.AddParameter("@studentid", SqlDbType.Int);
            submissionParameters.AddParameter("@assessmentid", SqlDbType.Int);
            submissionParameters.Add(submissionFileName.Key, submissionFileName.Value);

            resourceParameters.AddParameter("@enrolmentid", SqlDbType.Int);
            resourceParameters.AddParameter("@clusterid", SqlDbType.Int);
        }

        public void NewUser(string studentID)
        {
            this.studentID = studentID;
            studentPrimaryKey.Value.value = studentID;
            submissionParameters["@studentid"].value = studentID;
            dsetEnrolments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetEnrolmentsForStudent", databaseConnection.GenerateSQLParameter(studentPrimaryKey)).DefaultView;
        }

        private void dsetEnrolments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dsetEnrolments.SelectedItem;
            if (dataRowView != null)
            {
                String selectedItem = dataRowView.Row[0].ToString();
                databaseConnection.NewDataGridSelection(dsetEnrolments, dsetUnitClusters, 0, enrolmentPrimaryKey, "tsp_GetAllCLustersForEnrolment");
                resourceParameters["@enrolmentid"].value = selectedItem;
                dsetAssessments.ItemsSource = null;
                dsetResources.ItemsSource = null;
                dsetSubmissions.ItemsSource = null;
            }
                
        }

        private void dsetUnitCluster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dsetUnitClusters.SelectedItem;
            if (dataRowView != null)
            {
                String selectedItem = dataRowView.Row[0].ToString();

                databaseConnection.NewDataGridSelection(dsetUnitClusters, dsetAssessments, 0, clusterPrimaryKey, "tsp_GetAssessmentsForUnitCluster");
                resourceParameters["@clusterid"].value = selectedItem;
                dsetResources.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetResourcesForUnitCluster", resourceParameters).DefaultView;
                dsetSubmissions.ItemsSource = null;
            }
                
        }

        private void dsetAssessment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            submissionParameters.Remove("@submissionfilename");
            DataRowView dataRowView = (DataRowView)dsetAssessments.SelectedItem;
            if (dataRowView != null)
            {
                string selectedAssessment = dataRowView.Row[0].ToString();
                updateSAssessmentID.Text = selectedAssessment;

                submissionParameters["@assessmentid"].value = selectedAssessment;

                bool enabled = string.Equals(databaseConnection.GetValueFromTable("tsp_GetSubmissionResult", submissionParameters, submissionResult), "Unsubmitted");
                updateSSubmissionFileName.IsEnabled = enabled;
                dsetSubmissions.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetSubmissionForAssessment", submissionParameters).DefaultView;
            }
            submissionParameters.Add(submissionFileName.Key, submissionFileName.Value);
        }

        private void btnDownloadResource_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Placeholder for download functionality");
        }

        private void btnSubmitAssessment_Click(object sender, RoutedEventArgs e)
        {
            submissionParameters["@submissionfilename"].value = updateSSubmissionFileName.Text;
            submissionParameters["@assessmentid"].value = updateSAssessmentID.Text;

            if (ValidationHelper.ValidateIsFileName("Submission File Name", updateSSubmissionFileName.Text))
            {
                if (databaseConnection.ExecuteBasicQuery("tsp_UpdateStudentSubmission", submissionParameters))
                {
                    System.Windows.MessageBox.Show("Successfully submitted assessment");
                }

            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mainMenu.Show();
        }
    }
}
