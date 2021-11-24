using System.Windows;
using Tafe_System.AdminWindows;
using Tafe_System.StudentWindows;
using Tafe_System.TeacherWindows;

namespace Tafe_System
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    ///
    public partial class MainMenu : Window
    {
        private readonly LogIn login;

        private TeacherInformation teacherInformation;
        private CourseInformation courseInformation;
        private Students students;
        private Locations locations;
        private Offering offering;

        private StudentResults studentResults;

        private Resources resources;
        private Submissions submissions;
        private AssessmentEvents assessmentEvents;


        private MyAssessments myAssessments;
        private MyTimetables myTimetables;

        public MainMenu(LogIn login)
        {
            InitializeComponent();
            this.login = login;
        }

        public void TeacherUser(StudentResults studentResults, Submissions submissions, AssessmentEvents assessmentEvents, Resources resources, string teacherID)
        {
            labelMainMenu.Content = "Teacher";
            btnStudentResultsTeacher.Visibility = Visibility.Visible;
            this.studentResults = studentResults;
            studentResults.setTeacher(teacherID);
            btnSubmissions.Visibility = Visibility.Visible;
            this.submissions = submissions;
            submissions.SetTeacher(teacherID);
            btnAssessmentEvents.Visibility = Visibility.Visible;
            this.assessmentEvents = assessmentEvents;
            assessmentEvents.SetTeacher(teacherID);
            btnResources.Visibility = Visibility.Visible;
            this.resources = resources;
            resources.setTeacher(teacherID);

            btnTeacherInformation.Visibility = Visibility.Hidden;
            btnCourses.Visibility = Visibility.Hidden;
            btnStudents.Visibility = Visibility.Hidden;
            btnLocations.Visibility = Visibility.Hidden;
            btnStudentResultsTeacher.Visibility = Visibility.Hidden;
            btnOfferings.Visibility = Visibility.Hidden;

            btnStudentsAssessments.Visibility = Visibility.Hidden;
            btnStudentTimeTables.Visibility = Visibility.Hidden;
        }

        public void AdminUser(CourseInformation courseInformation, Locations locations, Offering offering, StudentResults studentResults, Students students, TeacherInformation teacherInformation)
        {
            labelMainMenu.Content = "Admin";
            this.teacherInformation = teacherInformation;
            btnTeacherInformation.Visibility = Visibility.Visible;
            this.courseInformation = courseInformation;
            btnCourses.Visibility = Visibility.Visible;
            this.students = students;
            btnStudents.Visibility = Visibility.Visible;
            this.locations = locations;
            btnLocations.Visibility = Visibility.Visible;
            this.studentResults = studentResults;
            btnStudentResultsTeacher.Visibility = Visibility.Visible;
            this.offering = offering;
            btnOfferings.Visibility = Visibility.Visible;

            btnStudentResultsTeacher.Visibility = Visibility.Hidden;
            btnSubmissions.Visibility = Visibility.Hidden;
            btnAssessmentEvents.Visibility = Visibility.Hidden;
            btnResources.Visibility = Visibility.Hidden;

            btnStudentsAssessments.Visibility = Visibility.Hidden;
            btnStudentTimeTables.Visibility = Visibility.Hidden;
        }

        public void StudentUser(MyAssessments myAssessments, MyTimetables myTimetables, string userID)
        {
            labelMainMenu.Content = "Student";
            this.myAssessments = myAssessments;
            btnStudentsAssessments.Visibility = Visibility.Visible;
            myAssessments.NewUser(userID);
            this.myTimetables = myTimetables;
            btnStudentTimeTables.Visibility = Visibility.Visible;
            myTimetables.NewUser(userID);

            btnStudentResultsTeacher.Visibility = Visibility.Hidden;
            btnSubmissions.Visibility = Visibility.Hidden;
            btnAssessmentEvents.Visibility = Visibility.Hidden;
            btnResources.Visibility = Visibility.Hidden;

            btnTeacherInformation.Visibility = Visibility.Hidden;
            btnCourses.Visibility = Visibility.Hidden;
            btnStudents.Visibility = Visibility.Hidden;
            btnLocations.Visibility = Visibility.Hidden;
            btnStudentResultsTeacher.Visibility = Visibility.Hidden;
            btnOfferings.Visibility = Visibility.Hidden;
        }

        private void btnTeacherInformation_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            teacherInformation.Show();
        }

        private void btnCourses_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            courseInformation.Show();
        }

        private void btnStudents_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            students.Show();
        }

        private void btnStudentTimeTable_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            myTimetables.Show();
        }


        private void btnLocations_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            locations.Show();
        }


        private void btnStudentsAssessments_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            myAssessments.Show();
        }


        private void btnSubmissions_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            submissions.Show();
        }

        private void btnStudentResults_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            studentResults.Show();
        }


        private void btnOfferings_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            offering.Show();
        }

        private void btnAssessmentEvents_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            assessmentEvents.Show();
        }

        private void btnResources_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            resources.Show();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            login.Show();
        }
    }
}
