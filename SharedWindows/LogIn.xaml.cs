using System.Windows;
using Tafe_System.AdminWindows;
using Tafe_System.StudentWindows;
using Tafe_System.TeacherWindows;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        private readonly MainMenu mainMenu;

        private readonly DatabaseConnection databaseConnection = new DatabaseConnection();
        private readonly TeacherInformation teacherInformation;
        private readonly CourseInformation courseInformation;
        private readonly Students students;
        private readonly Locations locations;
        private readonly Submissions submissions;
        private readonly StudentResults studentResults;
        private readonly Offering offering;
        private readonly AssessmentEvents assessmentEvents;

        private readonly MyAssessments myAssessments;
        private readonly MyTimetables myTimetables;
        private readonly Resources resources;

        public LogIn()
        {
            mainMenu = new MainMenu(this);
            InitializeComponent();
            teacherInformation = new TeacherInformation(databaseConnection, mainMenu);
            courseInformation = new CourseInformation(databaseConnection, mainMenu);
            students = new Students(databaseConnection, mainMenu);
            locations = new Locations(databaseConnection, mainMenu);
            offering = new Offering(databaseConnection, mainMenu);

            studentResults = new StudentResults(databaseConnection, mainMenu);

            resources = new Resources(databaseConnection, mainMenu);
            assessmentEvents = new AssessmentEvents(databaseConnection, mainMenu);
            submissions = new Submissions(databaseConnection, mainMenu);

            myAssessments = new MyAssessments(databaseConnection, mainMenu);
            myTimetables = new MyTimetables(databaseConnection, mainMenu);

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationHelper.ValidateIsEmail(txtUserEmail.Text))
            {
                string userID = databaseConnection.GetStudentOrTeacherIDFromUserEmail(txtUserEmail.Text);
                System.Diagnostics.Debug.WriteLine("userID is " + userID);
                int loginAttemptResult = databaseConnection.LogUserIn(userID, txtPassword.Password);


                switch (loginAttemptResult)
                {
                    default:
                        _ = MessageBox.Show("UserID or Password is incorrect");
                        break;
                    case 1:
                        mainMenu.StudentUser(myAssessments, myTimetables, userID);
                        break;
                    case 2:
                        mainMenu.TeacherUser(studentResults, submissions, assessmentEvents, resources, userID);
                        break;
                    case 3:
                        mainMenu.AdminUser(courseInformation, locations, offering, studentResults, students, teacherInformation);
                        break;
                }
                if (loginAttemptResult != 0)
                {
                    mainMenu.Show();
                    txtPassword.Clear();
                    txtUserEmail.Clear();
                    this.Hide();
                }
            }
        }

    }
}
