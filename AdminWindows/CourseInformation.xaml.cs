using System.Collections.Generic;
using System.Data;
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
     * A class that handles the adding, updating, and deleting of rows to the entities: 
     * "Course", "UnitCluster", "Unit", and "Assessment".
     * This class also allows the admin to create bridges between the 
     * "Course" and "UnitCluster", 
     * "UnitCluster" and "Unit", 
     * and "Unit" and "Assessment" tables, creating a link between the course information
     * </Summary>
     */

    public partial class CourseInformation : Window
    {
        
        private readonly DatabaseConnection databaseConnection;
        private readonly MainMenu mainMenu;

        /*
         *Each entity requires a primary key and parameters, the primary key and parameters are custom user types.
         *The custom user types can be found in the "Helpers" folder. 
         *The custom user types were created to make it easy to add or update the database
         *they help in converting the fields the user has entered into valid sqlparameters to be used in a stored procedure
         */

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

        //This constructor initializes the custom parameters for each relevant entity and sets up the componenets
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

        //Resets the form when the admin returns to the main menu
        public void Reset()
        {
            //Resetting all tables to display no values
            dsetAssessments.ItemsSource = null;
            dsetCourses.ItemsSource = null;
            dsetUnitClusters.ItemsSource = null;
            dsetUnits.ItemsSource = null;
            //Clearing fields
            databaseConnection.ClearUserInputFields(updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements);
            databaseConnection.ClearUserInputFields(updateCLClusterID, addClusterTextBoxElements, null, null, null);
            databaseConnection.ClearUserInputFields(updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        //Searches for a course with the given name and resets the child tables
        private void btnSearchCourse_Click(object sender, RoutedEventArgs e)
        {
            courseParameters["@coursename"].value = txtBoxSearchCourse.Text;
            //Gets a valid course with the supplied @coursename if it exists
            if (ValidationHelper.ValidateNoIntegers("Course name", txtBoxSearchCourse.Text))
            {
                dsetCourses.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetCourse", new KeyValuePair<string, SqlParameterDetails>("@coursename", courseParameters["@coursename"])).DefaultView;
            }
            //resets child tables to show nothing
            dsetUnitClusters.ItemsSource = null;
            clusterTableLabel.Content = "[Select a course above or search for all]";
            dsetUnits.ItemsSource = null;
            unitTableLabel.Content = "[Select a cluster above or search for all]";
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

        }

        //Searches for all courses and resets the child tables
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

        //Searches for all clusters and resets related tables
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

        //Searches for all units and resets related tables
        private void btnSearchAllUnits_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.SelectedItem = null;
            dsetUnitClusters.SelectedItem = null;
            dsetUnits.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllUnits").DefaultView;
            dsetAssessments.ItemsSource = null;
            assessmentTableLabel.Content = "[Select a unit above or search for all]";

            unitTableLabel.Content = "All units";
        }

        //Searches for all assessments and resets related tables
        private void btnSearchAllAssessments_Click(object sender, RoutedEventArgs e)
        {
            dsetCourses.SelectedItem = null;
            dsetUnitClusters.SelectedItem = null;
            dsetUnits.SelectedItem = null;
            dsetAssessments.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllAssessments").DefaultView;

            assessmentTableLabel.Content = "All assessments";
        }

        //Adds a course to the database if all the input fields are valid
        private void btnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Course Name ", addCCourseName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCCost.Text))
            {
                databaseConnection.AddToDatabase(courseParameters, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements, "C", "Successfully added course", "tsp_AddCourse");
            }
        }

        //This method will update a row in the course entity with a valid primary key but only the columns that are specified (that the user has entered new data for)
        private void btnUpdateCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text) && ValidationHelper.ValidateNoIntegers("Course Name ", addCCourseName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateCourseDetails", "tsp_GetCourseDetails", coursePrimaryKey, courseParameters, updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements, "C", "Successfully updated course");
            }
        }

        //Adds cluster within the database if all the input fields are valid
        private void btnAddCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Cluster Name ", addCLClusterName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCLCost.Text))
            {
                databaseConnection.AddToDatabase(clusterParameters, addClusterTextBoxElements, null, null, null, "CL", "Successfully added cluster", "tsp_AddCluster");
            }
        }

        //This method will update a row in the unitcluster entity with a valid primary key but only the columns that are specified (that the user has entered new data for)
        private void btnUpdateCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Cluster ID", updateCLClusterID.Text) && ValidationHelper.ValidateNoIntegers("Cluster Name ", addCLClusterName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addCLCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateClusterDetails", "tsp_GetClusterDetails", clusterPrimaryKey, clusterParameters, updateCLClusterID, addClusterTextBoxElements, null, null, null, "CL", "Successfully updated cluster");
            }
        }

        //Adds unit within the database if all the input fields are valid
        private void btnAddUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Unit Name ", addUUnitName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addUCost.Text))
            {
                databaseConnection.AddToDatabase(unitParameters, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null, "U", "Successfully added unit", "tsp_AddUnit");
            }
        }

        //This method will update a row in the unit entity with a valid primary key but only the columns that are specified (that the user has entered new data for)
        private void btnUpdateUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Unit ID", updateUUnitID.Text) && ValidationHelper.ValidateNoIntegers("Unit Name ", addUUnitName.Text) && ValidationHelper.ValidateOnlyIntegers("Cost", addUCost.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateUnitDetails", "tsp_GetUnitDetails", unitPrimaryKey, unitParameters, updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null, "U", "Successfully updated unit");
            }
        }

        //Adds assessment within the database if all the input fields are valid
        private void btnAddAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Assessment Title", addAAssessmentTitle.Text) && ValidationHelper.ValidateIsFileName("File Name ", addAFileName.Text))
            {
                databaseConnection.AddToDatabase(assessmentParameters, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null, "A", "Successfully added assessment", "tsp_AddAssessment");
            }
        }

        //This method will update a row in the assessment entity with a valid primary key but only the columns that are specified (that the user has entered new data for)
        private void btnUpdateAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text) && ValidationHelper.ValidateNoIntegers("Assessment Title", addAAssessmentTitle.Text) && ValidationHelper.ValidateIsFileName("File Name ", addAFileName.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateAssessmentDetails", "tsp_GetAssessmentDetails", assessmentPrimaryKey, assessmentParameters, updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null, "A", "Successfully updated assessment");
            }
        }

        //Creates a link between a course and a unitcluster if it does not exist
        private void btnCreateCourseUnitClusterBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text) && ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(true, "tsp_CreateCourseUnitClusterBridge", ref coursePrimaryKey, updateCCourseID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }

        //creates a link between a unitcluster and a unit if it does not exist
        private void btnCreateUnitClusterUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(true, "tsp_CreateUnitClusterUnitBridge", ref unitPrimaryKey, updateUUnitID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }

        //creates a link between an asssessment and a unit if it does not exist
        private void btnCreateAssessmentUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(true, "tsp_CreateAssessmentUnitBridge", ref unitPrimaryKey, updateUUnitID, ref assessmentPrimaryKey, updateAAssessmentID);
            }
        }

        //removes a link between a course and a unitcluster if it exists
        private void btnRemoveCourseUnitClusterBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text) && ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(false, "tsp_RemoveCourseUnitClusterBridge", ref coursePrimaryKey, updateCCourseID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }

        //removes a link between a unitcluster and a unit if it exists
        private void btnRemoveUnitClusterUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(false, "tsp_RemoveUnitClusterUnitBridge", ref unitPrimaryKey, updateUUnitID, ref clusterPrimaryKey, updateCLClusterID);
            }
        }

        //removes a link between an assessment and a unit if it exists
        private void btnRemoveAssessmentUnitBridge_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text) && ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.CreateOrRemoveBridge(false, "tsp_RemoveAssessmentUnitBridge", ref unitPrimaryKey, updateUUnitID, ref assessmentPrimaryKey, updateAAssessmentID);
            }
        }

        //Handles the values of the selected course's child tables, showing all the unitclusters linked to the selected course
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

        //Handles the values of the selected unitcluster's related tables, showing the units linked to the selected unitcluster and resetting the course table
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

        //Handles the values of the selected units related tables, showing the assessments linked to the selected unit and resetting the course and unitcluster tables
        private void dsetUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetUnits, updateUUnitID, ref unitPrimaryKey, unitParameters, "tsp_GetUnitDetails", "U", addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            assessmentTableLabel.Content = "Assessments for 'Unit ID' = " + unitPrimaryKey.Value.value;
            databaseConnection.NewDataGridSelection(dsetUnits, dsetAssessments, 0, unitPrimaryKey, "tsp_GetAssessmentsForUnit");
            databaseConnection.ClearUserInputFields(updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        //Autofills the assessment values based on selected Assessment
        private void dsetAssessment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetAssessments, updateAAssessmentID, ref assessmentPrimaryKey, assessmentParameters, "tsp_GetAssessmentDetails", "A", addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
        }

        //Removes a row from the course table with the entered courseID if it exists
        private void btnRemoveCourse_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", updateCCourseID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveCourse", ref coursePrimaryKey, "Successfully removed course", updateCCourseID, addCourseTextBoxElements, null, addCourseComboBoxElementsIndex, addCourseCheckBoxElements);
            }
        }

        //Removes a row from the unitcluster table with the entered clusterID if it exists
        private void btnRemoveCluster_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Cluster ID", updateCLClusterID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveCluster", ref clusterPrimaryKey, "Successfully removed cluster", updateCLClusterID, addClusterTextBoxElements, null, null, null);
            }
        }

        //Removes a row from the unit table with the entered unitID if it exists
        private void btnRemoveUnit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Unit ID", updateUUnitID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveUnit", ref unitPrimaryKey, "Successfully removed unit", updateUUnitID, addUnitTextBoxElements, addUnitComboBoxElementsValue, null, null);
            }
        }

        //Removes a row from the assessment table with the entered assessmentID if it exists
        private void btnRemoveAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Assessment ID", updateAAssessmentID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveAssessment", ref assessmentPrimaryKey, "Successfully removed assessment", updateAAssessmentID, addAssessmentTextBoxElements, addAssessmentComboBoxElementsValue, null, null);
            }
        }

        //Searches for clusters that don't belong to a course
        private void btnSearchClustersNoCourse_Click(object sender, RoutedEventArgs e)
        {
            dsetUnitClusters.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetAllClustersThatHaveNoCourse").DefaultView;
            clusterTableLabel.Content = "Clusters assigned to no courses";

        }

        //Searches for units that don't belong to a cluster
        private void btnSearchUnitsNoCourse_Click(object sender, RoutedEventArgs e)
        {
            dsetUnits.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetAllUnitsThatHaveNoCluster").DefaultView;
            unitTableLabel.Content = "Units assigned to no clusters";
        }

        //Searches for assessments that don't belong to a unit
        private void btnSearchAssessmentsNoUnits_Click(object sender, RoutedEventArgs e)
        {
            dsetAssessments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetAllAssessmentsThatHaveNoUnits").DefaultView;
            assessmentTableLabel.Content = "Assessments assigned to no units";

        }

        //Returns the user to the main menu and clears the screen
        private void btn_MainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
