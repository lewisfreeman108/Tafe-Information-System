namespace Tafe_System
{
    class BaseTimetableMethods
    {
        private readonly DatabaseConnection databaseConnection;



        public BaseTimetableMethods(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        /*

        public void SearchOnCampusClasses(DataGrid dsetClass, DataGrid dsetTimetableItem, Xceed.Wpf.Toolkit.WatermarkTextBox locationTextBox, KeyValuePair<String, SqlParameterDetails> locationPrimaryKey)
        {
            locationPrimaryKey.Value.value = locationTextBox.Text;
            dsetClass.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllClassesForLocation", locationPrimaryKey).DefaultView;
            dsetTimetableItem.ItemsSource = null;
        }

        public void SearchOnlineClasses(DataGrid dsetClass, DataGrid dsetTimetableItem)
        {
            dsetClass.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllOnlineClasses").DefaultView;
            dsetTimetableItem.ItemsSource = null;
        }

        public void SearchAllClasses(DataGrid dsetClass, DataGrid dsetTimetableItem)
        {
            dsetClass.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllClasses").DefaultView;
            dsetTimetableItem.ItemsSource = null;
        }

        public void GetTimetableItems(DataGrid dsetTimetableItem, DataGrid dsetClass, KeyValuePair<String, SqlParameterDetails> courseLocationPrimaryKey)
        {
            DataRowView dataRowView = (DataRowView)dsetClass.SelectedItem;
            String selectedClass = dataRowView.Row[0].ToString();

            //classPrimaryKey.Value.value = selectedClass;
            //dsetTimetableItem.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetTimetableItems", courseLocationPrimaryKey).DefaultView;
        }*/
    }
}
