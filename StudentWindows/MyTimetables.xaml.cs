using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Tafe_System.StudentWindows
{
    /// <summary>
    /// Interaction logic for MyTimetables.xaml
    /// </summary>
    public partial class MyTimetables : Window
    {
        private readonly MainMenu mainMenu;
        private readonly DatabaseConnection databaseConnection;
        private readonly KeyValuePair<string, SqlParameterDetails> studentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@studentid", new SqlParameterDetails(SqlDbType.Int, null));


        public MyTimetables(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();
        }

        public void NewUser(string studentID)
        {
            studentPrimaryKey.Value.value = studentID;
            dsetEnrolments.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetEnrolmentsForStudent", databaseConnection.GenerateSQLParameter(studentPrimaryKey)).DefaultView;
        }

        private void dsetEnrolments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dsetEnrolments.SelectedItem;
            if (dataRowView != null)
            {
                string selectedEnrolment = dataRowView.Row[0].ToString();
                dsetTimetableItem.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetTimetableItemsForEnrolment", new SqlParameter("@enrolmentid", selectedEnrolment)).DefaultView;
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mainMenu.Show();
        }
    }
}
