using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for Locations.xaml
    /// </summary>
    public partial class Locations : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;

        private KeyValuePair<string, SqlParameterDetails> locationPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@locationname", new SqlParameterDetails(SqlDbType.VarChar, 100));
        private readonly SqlParameterDictionary locationParameters = new SqlParameterDictionary();

        private readonly WatermarkTextBox[] addLocationTextBoxElements;
        private readonly WatermarkTextBox[] updateLocationTextBoxElements;
        private readonly ComboBox[] addLocationComboBoxElementsValue;


        public Locations(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();

            locationParameters.AddParameter("@streetaddress", SqlDbType.VarChar, 50);
            locationParameters.AddParameter("@suburb", SqlDbType.VarChar, 30);
            locationParameters.AddParameter("@postcode", SqlDbType.Char, 10);
            locationParameters.AddParameter("@state", SqlDbType.VarChar, 10);

            addLocationTextBoxElements = new WatermarkTextBox[] { addLStreetAddress, addLSuburb, addLPostCode };
            updateLocationTextBoxElements = new WatermarkTextBox[] { addLStreetAddress, addLSuburb, addLPostCode };
            addLocationComboBoxElementsValue = new ComboBox[] { addLState };
        }

        public void Reset()
        {
            dsetLocations.ItemsSource = null;
            databaseConnection.ClearUserInputFields(addLLocationName, addLocationTextBoxElements, addLocationComboBoxElementsValue, null, null);
        }

        private void btnSearchLocations_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Location name", txtBoxSearchLocation.Text))
            {
                locationPrimaryKey.Value.value = txtBoxSearchLocation.Text;
                dsetLocations.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetLocation", locationPrimaryKey).DefaultView;
            }
        }

        private void btnSearchAllLocations_Click(object sender, RoutedEventArgs e)
        {
            dsetLocations.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllLocations").DefaultView;

        }

        private void btnAddLocation_Click(object sender, RoutedEventArgs e)
        {
            locationParameters.AddParameter("@locationname", SqlDbType.VarChar, 100);
            locationParameters["@locationname"].value = addLLocationName.Text;

            if (ValidationHelper.ValidateNoIntegers("Location Name ", addLLocationName.Text) && ValidationHelper.ValidateOnlyIntegers("Postcode", addLPostCode.Text))
            {
                databaseConnection.AddToDatabase(locationParameters, addLocationTextBoxElements, addLocationComboBoxElementsValue, null, null, "L", "Successfully added location", "tsp_AddLocation");
            }
            locationParameters.Remove("@locationname");
        }

        private void btnUpdateLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateNoIntegers("Location Name", addLLocationName.Text) && ValidationHelper.ValidateOnlyIntegers("Postcode", addLPostCode.Text))
            {
                databaseConnection.UpdateDatabase("tsp_UpdateLocationDetails", "tsp_GetLocationDetails", locationPrimaryKey, locationParameters, addLLocationName, updateLocationTextBoxElements, addLocationComboBoxElementsValue, null, null, "L", "Successfully updated location");
            }
        }

        private void dsetLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.AutoFillExistingElements(dsetLocations, addLLocationName, ref locationPrimaryKey, locationParameters, "tsp_GetLocationDetails", "L", updateLocationTextBoxElements, addLocationComboBoxElementsValue, null, null);
        }

        private void btnRemoveLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateOnlyIntegers("Location ID", addLLocationName.Text))
            {
                databaseConnection.RemoveRow("tsp_RemoveLocation", ref locationPrimaryKey, "Successfully removed location", addLLocationName, updateLocationTextBoxElements, addLocationComboBoxElementsValue, null, null);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Reset();
            mainMenu.Show();
        }
    }
}
