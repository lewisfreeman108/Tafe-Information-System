using System.Linq;
using System.Windows;

namespace Tafe_System
{
    public class ValidationHelper
    {

        public static bool ValidateNoSpaces(string text)
        {
            return !text.Contains(" ");
        }

        public static bool ValidateOnlyIntegers(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly
            if (!text.Replace("$", "").All(c => c >= '0' && c <= '9'))
            {
                MessageBox.Show(key + "must be an integer");
                return false;
            }
            return true;
        }

        public static bool ValidateNoIntegers(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly
            if (!text.All(c => c < '0' || c > '9'))
            {
                MessageBox.Show(key + " must not have any integers");
                return false;
            }
            return true;
        }

        public static bool ValidateIsFileName(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly
            string[] temp = text.Split(".");
            foreach (string tm in temp) System.Diagnostics.Debug.Print(tm);
            if (temp.Length == 2)
            {
                if (string.Equals(temp[1], "txt") || string.Equals(temp[1], "pdf") || string.Equals(temp[1], "docx"))
                {
                    return true;
                }
            }
            MessageBox.Show(key + "must be a valid filename ending in '.pdf', '.txt', or '.docx'");
            return false;
        }

        public static bool ValidateIsEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly

            string[] temp = text.Split("@");

            if (temp.Length == 2 && temp[1].Contains("."))
            {
                return true;
            }
            MessageBox.Show("Email must be a valid email containing '@' and '.'");
            return false;
        }

        public static bool ValidateIsDate(string Key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly

            string[] temp = text.Split("-");

            if (temp.Length != 3)
            {
                MessageBox.Show("Date must be in the 'yyyy-mm-dd' format e.g. '2020-02-22'");
                return false;
            }

            foreach (string s in temp) if (!s.All(c => c >= '0' && c <= '9'))
                {
                    MessageBox.Show("Date must be in the 'yyyy-mm-dd' format e.g. '2020-02-22'");
                    return false;
                }

            int day = int.Parse(temp[2]);
            int month = int.Parse(temp[1]);
            int year = int.Parse(temp[0]);

            System.Diagnostics.Debug.WriteLine("day = " + day + ", month = " + month + ", year = " + year);


            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) if (day <= 31) return true;
            if (month == 4 || month == 6 || month == 9 || month == 11) if (day <= 30) return true;
            if (month == 2)
            {
                int leap = 0;
                if (year % 4 == 0)
                    leap += 1;
                if (day <= 28 + leap)
                    return true;
            }

            MessageBox.Show("Invalid date, must be in 'yyyy-mm-dd' format e.g. 2020-02-22");
            return false;
        }

        public static bool ValidateIsTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly
            string[] time = text.Split(":");
            if (time.Length == 2)
            {
                if (int.TryParse(time[0], out int hour) && int.TryParse(time[1], out int minute))
                {
                    if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60)
                        return true;
                }
            }
            MessageBox.Show("Invalid time, must be in hh:mm form");
            return false;
        }
        public static bool ValidateIsYear(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly

            if (ValidateOnlyIntegers(key, text))
            {
                return ValidateLength(text, 4, "Year is invalid, must be in yyyy format");
            }
            return false;
        }

        public static bool ValidateIsMobile(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly

            if (ValidateOnlyIntegers(key, text))
            {
                return ValidateLength(text, 10, "Mobile is invalid, must be in nnnnnnnnnn format");
            }
            return false;
        }

        public static bool ValidateIsPostCode(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true; //DatabaseConnection Methods handle blank strings accordingly

            if (ValidateOnlyIntegers(key, text))
            {
                return ValidateLength(text, 4, "Postcode is invalid, must be in nnnn format");
            }
            return false;
        }

        public static bool ValidateLength(string text, int length, string errorMessage)
        {
            if (!text.Length.Equals(length))
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                return true;
            }
            return false;
        }
    }
}
