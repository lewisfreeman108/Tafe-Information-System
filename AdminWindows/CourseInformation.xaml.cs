using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for CourseInformation.xaml
    /// </summary>
    public partial class CourseInformation : Window
    {
        private readonly DatabaseConnection databaseConnection;
        private readonly MainMenu mainMenu;

        private KeyValuePair<string, SqlParameterDetails> coursePrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@courseid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary courseParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addCourseTextBoxElements;
        private readonly ComboBox[] addCourseComboBoxElementsIndex;
        private readonly CheckBox[] addCourseCheckBoxElements;

        private KeyValuePair<string, SqlParameterDetails> clusterPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@clusterid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary clusterParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addClusterTextBoxElements;

        private KeyValuePair<string, SqlParameterDetails> unitPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@unitid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary unitParameters = new SqlParameterDictionary();


        private readonly WatermarkTextBox[] addUnitTextBoxElements;
        private readonly ComboBox[] addUnitComboBoxElementsValue;

        private KeyValuePair<string, SqlParameterDetails> assessmentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@assessmentid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary assessmentParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addAssessmentTextBoxElements;
        private readonly ComboBox[] addAssessmentComboBoxElementsValue;

        public CourseInformation(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.databaseConnection = databaseConnection;
            this.mainMenu = mainMenu;
            InitializeComponent();


            courseParameters.AddParameter("@coursename", SqlDbType.VarChar, 100);
            courseParameters.AddParameter("@description", SqlDbType.VarChar, 300);
            courseParameters.AddParameter("@cost", SqlDbType.Int);
            courseParameters.AddParameter("@parttime", SqlDbType.Bit);
            courseParameters.AddParameter("@fulltime", SqlDbType.Bit);
            courseParameters.AddParameter("@online", SqlDbType.Bit);
            courseParameters.AddParameter("@aqflevel", SqlDbType.SmallInt);
            courseParameters.AddParameter("@semester1", SqlDbType.Bit);
            courseParameters.AddParameter("@semester2", SqlDbType.Bit);

            addCourseTextBoxElements = new WatermarkTextBox[] { addCCourseName, addCCost, addCDescription };
            addCourseComboBoxElementsIndex = new ComboBox[] { addCAQFLevel };
            addCourseCheckBoxElements = new CheckBox[] { addCPartTime, addCFullTime, addCOnline, addCSemester1, addCSemester2 };

            clusterParameters.AddParameter("@clustername", SqlDbType.VarChar, 50);
            clusterParameters.AddParameter("@description", SqlDbType.VarChar, 300);
            clusterParameters.AddParameter("@cost", SqlDbType.Int);

            addClusterTextBoxElements = new WatermarkTextBox[] { addCLClusterName, addCLCost, addCLDescription };

            unitParameters.AddParameter("@unitname", SqlDbType.VarChar, 100);
            unitParameters.AddParameter("@type", SqlDbType.VarChar, 20);
            unitParameters.AddParameter("@description", SqlDbType.VarChar, 300);
            unitParameters.AddParameter("@cost", SqlDbType.Int);

            addUnitTextBoxElements = new WatermarkTextBox[] { addUUnitName, addUCost, addUDescription };
            addUnitComboBoxElementsValue = new ComboBox[] { addUType };

            assessmentParameters.AddParameter("@assessmenttitle", SqlDbType.VarChar, 150);
            assessmentParameters.AddParameter("@filename", SqlDbType.VarChar, 255);
            assessmentParameters.AddParameter("@type", SqlDbType.VarChar, 20);
            assessmentParameters.AddParameter("@description", SqlDbType.VarChar, 300);

            addAssessmentTextBoxElements = new WatermarkTextBox[] { addAAssessmentTitle, addAFileName, addADescription };
            addAssessmentComboBoxElementsValue = new ComboBox[] { addAType };




        }

        public void Reset()
        {
            dsetAssessments.ItemsSource = null;
            dsetCourses.ItemsSource = null;
            dsetUnitClusters.ItemsSource = null;
            dsetUnits.ItemsSource = null;
            databaseConnection.ClearUserInputFields(updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements);
            databaseConnection.ClearUserInputFields(updateCLClusterID, addClusterTextBoxElements, null, null, null);
            databaseConnection.ClearUserInputFields(updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }
        private void btnSearchCourse_Click(object sender, RoutedEventArgs e)
        {
            courseParameters["@coursename"].value = txtBoxSearchCourse.Text;
            if (ValidationHelper.ValidateNoIntegers("Course name", txtBoxSearchCourse.Text))
            {
                dsetCourses.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetCourse", new KeyValuePair<string, SqlParameterDetails>("@coursename", courseParameters["@coursename"])).DefaultView;
            }

            dsetUnitClusters.ItemsSource = null;
            clusterTableLabel.Content = "[Select a course above or search for all]";
            dsetUnits.ItemsSource = null;
            unitTableLabel.Content = "[Select a cluster above or search for all]";
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

        }

        private void btnSearchAllCourses_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllCourses").DefaultView;
            dsetUnitClusters.ItemsSource = null;
            clusterTableLabel.Content = "[Select a course above or search for all]";
            dsetUnits.ItemsSource = null;
            unitTableLabel.Content = "[Select a cluster above or search for all]";
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";
        }

        private void btnSearchAllClusters_Click(object sender, RoutedEventArgs e)
        {

            dsetCourses.SelectedItem = null;
            dsetUnitClusters.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllClusters").DefaultView;
            dsetUnits.ItemsSource = null;
            unitTableLabel.Content = "[Select a cluster above or search for all]";
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

            clusterTableLabel.Content = "All clusters";
        }

        private void btnSearchAllUnits_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.SelectedItem = null;
            dsetUnitClusters.SelectedItem = null;
            dsetUnits.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllUnits").DefaultView;
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

            unitTableLabel.Content = "All units";
        }

        private void btnSearchAllAssessments_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.SelectedItem = null;
            dsetUnitClusters.SelectedItem = null;
            dsetUnits.SelectedItem = null;
            dsetAssessments.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllAssessments").DefaultView;

            assessmentTableLabel.Content = "All assessments";
        }

        private void btnSearchAllResources_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.SelectedItem = null;
            dsetUnitClusters.SelectedItem = null;
            dsetUnits.SelectedItem = null;

            assessmentTableLabel.Content = "All assessments";
        }

        private void btnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Course Name ", addCCourseName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCCost.Text))
            {
                databaseConnection.AddToDatabase(courseParameters, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements, "C", "Successfully added course", "tsp_AddCourse");
            }
        }

        private void btnUpdateCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text) && ValidationHelper.ValidateNoIntegers("Course Name ", addCCourseName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateCourseDetails", "tsp_GetCourseDetails", coursePrimaryKey, courseParameters, updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements, "C", "Successfully updated course");
            }
        }

        private void btnAddCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Cluster Name ", addCLClusterName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCLCost.Text))
            {
                databaseConnection.AddToDatabase(clusterParameters, addClusterTextBoxElements, null, null, null, "CL", "Successfully added cluster", "tsp_AddCluster");
            }
        }

        private void btnUpdateCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Cluster ID", updateCLClusterID.Text) && ValidationHelper.ValidateNoIntegers("Cluster Name ", addCLClusterName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCLCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateClusterDetails", "tsp_GetClusterDetails", clusterPrimaryKey, clusterParameters, updateCLClusterID, addClusterTextBoxElements, null, null, null, "CL", "Successfully updated cluster");
            }
        }

        private void btnAddUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Unit Name ", addUUnitName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addUCost.Text))
            {
                databaseConnection.AddToDatabase(unitParameters, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null, "U", "Successfully added unit", "tsp_AddUnit");
            }
        }

        private void btnUpdateUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Unit ID", updateUUnitID.Text) && ValidationHelper.ValidateNoIntegers("Unit Name ", addUUnitName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addUCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateUnitDetails", "tsp_GetUnitDetails", unitPrimaryKey, unitParameters, updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null, "U", "Successfully updated unit");
            }
        }

        private void btnAddAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Assessment Title", addAAssessmentTitle.Text) && ValidationHelper.ValidateIsFileName("File Name ", addAFileName.Text))
            {
                databaseConnection.AddToDatabase(assessmentParameters, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null, "A", "Successfully added assessment", "tsp_AddAssessment");
            }
        }

        private void btnUpdateAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text) && ValidationHelper.ValidateNoIntegers("Assessment Title", addAAssessmentTitle.Text) && ValidationHelper.ValidateIsFileName("File Name ", addAFileName.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateAssessmentDetails", "tsp_GetAssessmentDetails", assessmentPrimaryKey, assessmentParameters, updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null, "A", "Successfully updated assessment");
            }
        }

        private void btnCreateCourseUnitClusterBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text) && ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text))
            {
                databaseConnection.CreateBridge("tsp_CreateCourseUnitClusterBridge", ref coursePrimaryKey, updateCCourseID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }


        private void btnCreateUnitClusterUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateBridge("tsp_CreateUnitClusterUnitBridge", ref unitPrimaryKey, updateUUnitID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }

        private void btnCreateAssessmentUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateBridge("tsp_CreateAssessmentUnitBridge", ref unitPrimaryKey, updateUUnitID, ref assessmentPrimaryKey, updateAAssessmentID);
            }
        }



        private void dsetCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetCourses, updateCCourseID, ref coursePrimaryKey, courseParameters, "tsp_GetCourseDetails", "C", addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements);
            clusterTableLabel.Content = "Clusters for 'Course ID' = " + coursePrimaryKey.Value.value;
            databaseConnection.NewDataGridSelection(dsetCourses, dsetUnitClusters, 0, coursePrimaryKey, "tsp_GetUnitClustersForCourse");
            dsetUnits.ItemsSource = null;
            unitTableLabel.Content = "[Select a cluster above or search for all]";
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";
            databaseConnection.ClearUserInputFields(updateCLClusterID, addClusterTextBoxElements, null, null, null);
            databaseConnection.ClearUserInputFields(updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        private void dsetUnitCluster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetUnitClusters, updateCLClusterID, ref clusterPrimaryKey, clusterParameters, "tsp_GetClusterDetails", "CL", addClusterTextBoxElements, null, null, null);
            unitTableLabel.Content = "Units for 'Cluster ID' = " + clusterPrimaryKey.Value.value;
            databaseConnection.NewDataGridSelection(dsetUnitClusters, dsetUnits, 0, clusterPrimaryKey, "tsp_GetUnitsForUnitCluster");
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

            databaseConnection.ClearUserInputFields(updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        private void dsetUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetUnits, updateUUnitID, ref unitPrimaryKey, unitParameters, "tsp_GetUnitDetails", "U", addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            assessmentTableLabel.Content = "Assessments for 'Unit ID' = " + unitPrimaryKey.Value.value;
            databaseConnection.NewDataGridSelection(dsetUnits, dsetAssessments, 0, unitPrimaryKey, "tsp_GetAssessmentsForUnit");
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        private void dsetAssessment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetAssessments, updateAAssessmentID, ref assessmentPrimaryKey, assessmentParameters, "tsp_GetAssessmentDetails", "A", addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }




        private void btnRemoveCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveCourse", ref coursePrimaryKey, "Successfully removed course", updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements);
            }
        }

        private void btnRemoveCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveCluster", ref clusterPrimaryKey, "Successfully removed cluster", updateCLClusterID, addClusterTextBoxElements, null, null, null);
            }
        }

        private void btnRemoveUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveUnit", ref coursePrimaryKey, "Successfully removed unit", updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            }
        }


        private void btnRemoveAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveAssessment", ref assessmentPrimaryKey, "Successfully removed assessment", updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
            }
        }

        private void btnSearchClustersNoCourse_Click(object sender, RoutedEventArgs e)
        {
            dsetUnitClusters.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetAllClustersThatHaveNoCourse").DefaultView;
            clusterTableLabel.Content = "Clusters assigned to no courses";

        }

        private void btnSearchUnitsNoCourse_Click(object sender, RoutedEventArgs e)
        {
            dsetUnitClusters.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetAllUnitsThatHaveNoCourse").DefaultView;
            unitTableLabel.Content = "Units assigned to no courses";
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
