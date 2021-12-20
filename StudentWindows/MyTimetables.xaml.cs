using System.Collections.Generic;
using System.Data;
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
        private readonly KeyValuePair<string, SqlParameterDetails> enrolmentPrimaryKey = new KeyValuePair<string, SqlParameterDetails>("@enrolmentid", new SqlParameterDetails(SqlDbType.Int, null));


        public MyTimetables(DatabaseConnection databaseConnection, MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
            this.databaseConnection = databaseConnection;
            InitializeComponent();
        }

        public void NewUser(string studentID)
        {
            studentPrimaryKey.Value.value = studentID;
            dsetEnrolments.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetEnrolmentsForStudent", studentPrimaryKey).DefaultView;
        }

        private void dsetEnrolments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            databaseConnection.NewDataGridSelection(dsetEnrolments, dsetTimetableItem, 0, enrolmentPrimaryKey, "tsp_GetTimetableItemsForEnrolment");
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mainMenu.Show();
        }
    }
}
