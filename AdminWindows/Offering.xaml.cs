using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for Offering.xaml
    /// </summary>
    public partial class Offering : Window
    {

        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;
        private readonly BaseSearchOfferingMethods baseSearchOfferingMethods;

        private readonly KeyValuePair<string, SqlParameterDetails> coursePrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@courseid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly KeyValuePair<string, SqlParameterDetails> courseOnlineKey = new KeyValuePair<string, SqlParameterDetails>("@online", new SqlParameterDetails(SqlDbType.Bit, null));
        private readonly KeyValuePair<string, SqlParameterDetails> courseFullTimeKey = new KeyValuePair<string, SqlParameterDetails>("@fulltime", new SqlParameterDetails(SqlDbType.Bit, null));
        private readonly KeyValuePair<string, SqlParameterDetails> coursePartTimeKey = new KeyValuePair<string, SqlParameterDetails>("@parttime", new SqlParameterDetails(SqlDbType.Bit, null));
        private readonly KeyValuePair<string, SqlParameterDetails> locationPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@locationname", new SqlParameterDetails(SqlDbType.VarChar, 80));

        private readonly SqlParameterDictionary offeringParameters = new SqlParameterDictionary();
        private KeyValuePair<string, SqlParameterDetails> offeringPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@offeringid", new SqlParameterDetails(SqlDbType.Int, null));

        private readonly WatermarkTextBox[] addOfferingTextBoxElements;
        private readonly ComboBox[] addOfferingComboBoxElementsValue;
        private readonly ComboBox[] addOfferingComboBoxElementsIndex;

        private readonly List<SqlParameter> searchOfferingFilter = new List<SqlParameter>();

        private KeyValuePair<string, SqlParameterDetails> timetableitemPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@timetableitemid", new SqlParameterDetails(SqlDbType.Int, null));
        private readonly SqlParameterDictionary timetableParameters = new SqlParameterDictionary();
        private readonly WatermarkTextBox[] addTimetableItemTextboxElements;
        private readonly ComboBox[] addTimetableItemComboBoxElementsValue;

        private readonly KeyValuePair<string, SqlParameterDetails> courseAlternateKey = new KeyValuePair<string, SqlParameterDetails>("@coursename", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly KeyValuePair<string, SqlParameterDetails> clusterAlternateKey = new KeyValuePair<string, SqlParameterDetails>("@clustername", new SqlParameterDetails(SqlDbType.VarChar, 100));


        public Offering(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            InitializeComponent();

            offeringParameters.Add("@courseid", new SqlParameterDetails(SqlDbType.Int, null));
            offeringParameters.Add("@locationname", new SqlParameterDetails(SqlDbType.VarChar, 100));
            offeringParameters.Add("@semester", new SqlParameterDetails(SqlDbType.SmallInt, null));
            offeringParameters.Add("@offeringtype", new SqlParameterDetails(SqlDbType.VarChar, 15));
            offeringParameters.AddParameter("@startdate", SqlDbType.Char, 10);
            offeringParameters.AddParameter("@enddate", SqlDbType.Char, 10);

            addOfferingTextBoxElements = new WatermarkTextBox[] { addOCourseID, addOLocationName, addOStartDate, addOEndDate };
            addOfferingComboBoxElementsIndex = new ComboBox[] { addOSemester };
            addOfferingComboBoxElementsValue = new ComboBox[] { addOOfferingType };

            timetableParameters.AddParameter("@offeringid", SqlDbType.Int);
            timetableParameters.AddParameter("@courseunitclusterid", SqlDbType.Int);
            timetableParameters.AddParameter("@teacherid", SqlDbType.Int);
            timetableParameters.AddParameter("@starttime", SqlDbType.VarChar, 7);
            timetableParameters.AddParameter("@endtime", SqlDbType.VarChar, 7);
            timetableParameters.AddParameter("@building", SqlDbType.VarChar, 10);
            timetableParameters.AddParameter("@room", SqlDbType.VarChar, 10);
            timetableParameters.AddParameter("@dayrunning", SqlDbType.VarChar, 10);

            addTimetableItemTextboxElements = new WatermarkTextBox[] { addTCourseUnitClusterID, addTTeacherID, addTBuilding, addTRoom, addTStartTime, addTEndTime };
            addTimetableItemComboBoxElementsValue = new ComboBox[] { addTDayRunning };

            this.databaseConnection = databaseConnection;

            baseSearchOfferingMethods = new BaseSearchOfferingMethods(databaseConnection);
        }

        public void Reset()
        {
            dsetOfferings.ItemsSource = null;
            dsetTimetableItem.ItemsSource = null;

            databaseConnection.ClearUserInputFields(updateOOfferingID, addOfferingTextBoxElements, addOfferingComboBoxElementsValue, addOfferingComboBoxElementsIndex, null);
            databaseConnection.ClearUserInputFields(updateTTimetableID, addTimetableItemTextboxElements, addTimetableItemComboBoxElementsValue, null, null);
        }

        private void addEEnrolmenttype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addOLocationName != null)
            {
                if (addOOfferingType.SelectedIndex == 2)
                {
                    addOLocationName.Text = "NULLVALUE";
                    addOLocationName.IsEnabled = false;
                    addOLocationName.Visibility = Visibility.Hidden;
                }
                else
                {
                    addOLocationName.Text = "";
                    addOLocationName.IsEnabled = true;
                    addOLocationName.Visibility = Visibility.Visible;
                }

            }
        }

        private void btnAddOffering_Click(object sender, RoutedEventArgs e)
        {


            if (ValidationHelper.ValidateOnlyIntegers("Course ID", addOCourseID.Text) && ValidationHelper.ValidateNoIntegers("Location Name", addOLocationName.Text) && ValidationHelper.ValidateIsDate("Start Date", addOStartDate.Text) && ValidationHelper.ValidateIsDate("End Date", addOEndDate.Text))
            {
                bool offeringTypeValidated = false;
                switch (addOOfferingType.SelectedIndex)
                {
                    case 0:
                        coursePrimaryKey.Value.value = addOCourseID.Text;
                        if (databaseConnection.GetValueFromTable("tsp_GetCourseOffersFullTime", coursePrimaryKey, courseFullTimeKey) == "true")
                        {
                            System.Windows.MessageBox.Show("Course cannot be offered as full time");
                            return;
                        }
                        break;
                    case 1:
                        coursePrimaryKey.Value.value = addOCourseID.Text;
                        if (offeringTypeValidated = databaseConnection.GetValueFromTable("tsp_GetCourseOffersPartTime", coursePrimaryKey, coursePartTimeKey) == "true")
                        {
                            System.Windows.MessageBox.Show("Course cannot be offered as part time");
                            return;
                        }
                        break;
                    case 2:
                        coursePrimaryKey.Value.value = addOCourseID.Text;
                        if (offeringTypeValidated = databaseConnection.GetValueFromTable("tsp_GetCourseOffersOnline", coursePrimaryKey, courseOnlineKey) == "true")
                        {
                            System.Windows.MessageBox.Show("Course cannot be offered as online");
                            return;
                        }
                        break;
                }
                databaseConnection.AddToDatabase(offeringParameters, addOfferingTextBoxElements, addOfferingComboBoxElementsValue, addOfferingComboBoxElementsIndex, null, "O", "Successfully added Offering", "tsp_AddOffering");
            }
        }

        private void btnUpdateOffering_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Course ID", addOCourseID.Text) && ValidationHelper.ValidateNoIntegers("Location Name", addOLocationName.Text) && ValidationHelper.ValidateIsDate("Start Date", addOStartDate.Text) && ValidationHelper.ValidateIsDate("End Date", addOEndDate.Text))
            {
                coursePrimaryKey.Value.value = addOCourseID.Text;
                locationPrimaryKey.Value.value = addOLocationName.Text;
                databaseConnection.UpdateDatabase("tsp_UpdateOfferingDetails", "tsp_GetOfferingDetails", offeringPrimaryKey, offeringParameters, updateOOfferingID, addOfferingTextBoxElements, addOfferingComboBoxElementsValue, addOfferingComboBoxElementsIndex, null, "O", "Successfully updated Offering");
            }
        }

        private void btnRemoveOffering_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Offering ID", updateOOfferingID.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveOffering", ref offeringPrimaryKey, "Successfully removed offering", updateOOfferingID, addOfferingTextBoxElements, addOfferingComboBoxElementsValue, addOfferingComboBoxElementsIndex, null);
            }

        }

        private void btnSearchAllOfferings_Click(object sender, RoutedEventArgs e)
        {
            baseSearchOfferingMethods.SearchAllOfferings(dsetOfferings);
        }

        private void btnSearchWithFilters_Click(object sender, RoutedEventArgs e)
        {
            baseSearchOfferingMethods.SearchOfferingsWithFilters(dsetOfferings, searchOfferingFilter, searchSemester, txtBoxSearchByCourse, txtBoxSearchByLocation, txtBoxSearchYear);
        }

        private void dsetOfferings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dsetOfferings.SelectedItem;

            if (dataRowView != null)
            {
                timetableParameters["@offeringid"].value = dataRowView.Row[0].ToString();
                if (string.Equals(dataRowView.Row[1].ToString(), "Online"))
                {
                    addTBuilding.Text = "NULLVALUE";
                    addTBuilding.Visibility = Visibility.Hidden;
                    addTRoom.Text = "NULLVALUE";
                    addTRoom.Visibility = Visibility.Hidden;
                }
                else
                {
                    addTBuilding.Text = "";
                    addTBuilding.Visibility = Visibility.Visible;
                    addTRoom.Text = "";
                    addTRoom.Visibility = Visibility.Visible;
                }
            }

            databaseConnection.AutoFillExistingElements(dsetOfferings, updateOOfferingID, ref offeringPrimaryKey, offeringParameters, "tsp_GetOfferingDetails", "O", addOfferingTextBoxElements, addOfferingComboBoxElementsValue, addOfferingComboBoxElementsIndex, null);
            databaseConnection.NewDataGridSelection(dsetOfferings, dsetTimetableItem, 0, offeringPrimaryKey, "tsp_GetTimetableItemsForOffering");
        }

        private void btnAddTimetableItem_Click(object sender, RoutedEventArgs e)
        {
            databaseConnection.AddToDatabase(timetableParameters, addTimetableItemTextboxElements, addTimetableItemComboBoxElementsValue, null, null, "T", "Successfully added timetable item", "tsp_AddTimetableItem");
        }

        private void btnUpdateTimetableItem_Click(object sender, RoutedEventArgs e)
        {
            databaseConnection.UpdateDatabase("tsp_UpdateTimetableItemDetails", "tsp_GetTimetableItemDetails", timetableitemPrimaryKey, timetableParameters, updateTTimetableID, addTimetableItemTextboxElements, addTimetableItemComboBoxElementsValue, null, null, "T", "Successfully updated timetable item");
        }

        private void btnRemoveTimetableItem_Click(object sender, RoutedEventArgs e)
        {
            databaseConnection.RemoveRow("tsp_RemoveTimetableItem", ref timetableitemPrimaryKey, "Successfully removed timetable item", updateTTimetableID, addTimetableItemTextboxElements, addTimetableItemComboBoxElementsValue, null, null);
        }

        private void btnFindBClassInformation_Click(object sender, RoutedEventArgs e)
        {
            string courseName = findBCourseName.Text;
            string clusterName = findBClusterName.Text;

            clusterAlternateKey.Value.value = clusterName;
            courseAlternateKey.Value.value = courseName;

            if (!(string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(clusterName)))
                FindBCourseUnitClusterID.Text = "CourseUnitClusterID:\n" + databaseConnection.FindBridge("tsp_FindCourseUnitClusterID", courseAlternateKey, clusterAlternateKey);
        }

        private void dsetTimetableItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetTimetableItem, updateTTimetableID, ref timetableitemPrimaryKey, timetableParameters, "tsp_GetTimetableItemDetails", "T", addTimetableItemTextboxElements, addTimetableItemComboBoxElementsValue, null, null);
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
