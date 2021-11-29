using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Tafe_System.AdminWindows
{
    /// <summary>
    /// Interaction logic for AssessmentSubmissions.xaml
    /// </summary>
    public partial class AssessmentEvents : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;

        private readonly KeyValuePair<string, SqlParameterDetails> offeringPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@offeringid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary assessmentEventParameters = new SqlParameterDictionary();

        public AssessmentEvents(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();
            assessmentEventParameters.AddParameter("@assessmentid", SqlDbType.Int);
            assessmentEventParameters.AddParameter("@duedate", SqlDbType.VarChar, 10);
            assessmentEventParameters.AddParameter("@teachermarkingid", SqlDbType.Int);
            assessmentEventParameters.AddParameter("@offeringid", SqlDbType.Int);
        }

        public void SetTeacher(string teacherID)
        {
            assessmentEventParameters["@teachermarkingid"].value = teacherID;
        }

        private void Reset()
        {
            dsetAssessments.ItemsSource = null;
            Reset2();
        }

        private void Reset2()
        {
            dsetAssessments.SelectedItem = null;
            createAEAssessmentID.Text = null;
            createAEDueDate.Text = null;
        }

        private void SetAssessmentEventParameters()
        {
            assessmentEventParameters["@assessmentid"].value = createAEAssessmentID.Text;
            assessmentEventParameters["@duedate"].value = createAEDueDate.Text;
        }


        private void btnSearchCourse_Click(object sender, RoutedEventArgs e)
        {
            string  id = string.IsNullOrEmpty(txtBoxSearchAssessments.Text) ? "-1" : txtBoxSearchAssessments.Text; // ID can't be blank or it causes sql error
            offeringPrimaryKey.Value.value = id;
            assessmentEventParameters["@offeringid"].value = id;
            dsetAssessments.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAssessmentsForOffering", offeringPrimaryKey).DefaultView;
        }

        private void dsetAssessment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dsetAssessments.SelectedItem;
            if (dataRowView != null)
            {
                createAEAssessmentID.Text = (dataRowView.Row[0]).ToString();
            }
        }

        private void btnUpdateAssessmentEvent_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateIsDate("Due date", createAEDueDate.Text))
            {
                SetAssessmentEventParameters();
                if (databaseConnection.ExecuteBasicQuery("tsp_UpdateAssessmentEvent", assessmentEventParameters, out _))
                {
                    Reset2();
                    MessageBox.Show("Successfully updated assessment event");
                }
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            this.Hide();
            mainMenu.Show();
        }
    }
}
