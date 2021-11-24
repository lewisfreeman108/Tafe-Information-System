using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System.TeacherWindows
{
    /// <summary>
    /// Interaction logic for Resources.xaml
    /// </summary>
    public partial class Resources : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;
        readonly WatermarkTextBox[] addResourceTextBoxElements;
        private KeyValuePair<string, SqlParameterDetails> resourcePrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@resourceid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary resourceParameters = new SqlParameterDictionary();

        private readonly KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@teacherid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary searchResourceParameters = new SqlParameterDictionary();

        public Resources(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();

            resourceParameters.AddParameter("@resourcetitle", SqlDbType.VarChar, 150);
            resourceParameters.AddParameter("@resourcefilename", SqlDbType.VarChar, 150);
            resourceParameters.AddParameter("@authorid", SqlDbType.Int);
            addResourceTextBoxElements = new WatermarkTextBox[] { addRResourceFileName, addRResourceTitle, addRCourseUnitClusterID };
        }

        public void setTeacher(string teacherID)
        {
            searchResourceParameters["@teacherid"].value = teacherID;
            teacherPrimaryKey.Value.value = teacherID;
        }

        private void dsetResources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetResources, updateRResourceID, ref resourcePrimaryKey, resourceParameters, "tsp_GetResourceDetails", "R", addResourceTextBoxElements, null, null, null);
        }

        private void btnAddResource_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateIsFileName("Resource ", addRResourceFileName.Text) && ValidationHelper.ValidateOnlyIntegers("CourseUnitCluster ID", addRCourseUnitClusterID.Text))
            {
                databaseConnection.AddToDatabase(resourceParameters, addResourceTextBoxElements, null, null, null, "R", "Successfully added resource", "tsp_AddResource");
            }
        }

        private void btnUpdateResource_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateIsFileName("Resource ", addRResourceFileName.Text) && ValidationHelper.ValidateOnlyIntegers("CourseUnitCluster ID", addRCourseUnitClusterID.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateResourceDetails", "tsp_GetResourceDetails", resourcePrimaryKey, resourceParameters, updateRResourceID, addResourceTextBoxElements, null, null, null, "R", "Successfully updated resource");
            }
        }

        private void btnRemoveResource_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Resource ID", updateRResourceID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveResource", ref resourcePrimaryKey, "Successfully removed resource", updateRResourceID, addResourceTextBoxElements, null, null, null);
            }
        }

        public void Reset()
        {
            dsetResources.ItemsSource = null;
            databaseConnection.ClearUserInputFields(updateRResourceID, addResourceTextBoxElements, null, null, null);
        }

        private void btnSearchResources_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtboxSearchResources.Text))
            {
                databaseConnection.GetTableFromDatabase("tsp_GetAllTeacherResources", teacherPrimaryKey);
            }
            else
            {
                searchResourceParameters["@courseunitclusterid"].value = txtboxSearchResources.Text;
                databaseConnection.GetTableFromDatabase("tsp_GetTeacherResourcesForCourseUnitCluster", searchResourceParameters);
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
