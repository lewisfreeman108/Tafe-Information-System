using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Tafe_System
{
    public class BaseSearchOfferingMethods
    {
        private readonly DatabaseConnection databaseConnection;
        public BaseSearchOfferingMethods(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void SearchAllOfferings(DataGrid dsetOfferings)
        {
            dsetOfferings.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllOfferings").DefaultView;
        }
        public void SearchAllOfferings(KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey, DataGrid dsetOfferings)
        {
            dsetOfferings.ItemsSource = databaseConnection.GetTableFromDatabase("tsp_GetAllOfferings", teacherPrimaryKey).DefaultView;
        }

        private bool SearchOfferingsFiltered(List<SqlParameter> searchOfferingFilter, ComboBox searchSemester, WatermarkTextBox txtBoxSearchByCourse, WatermarkTextBox txtBoxSearchByLocation, WatermarkTextBox txtBoxSearchYear)
        {
            searchOfferingFilter.Clear();

            if (searchSemester.SelectedIndex != 0)
            {
                searchOfferingFilter.Add(new SqlParameter("@semester", searchSemester.SelectedIndex));
            }
            else
            {
                searchOfferingFilter.Add(new SqlParameter("@semester", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchByCourse.Text))
            {
                searchOfferingFilter.Add(new SqlParameter("@courseid", txtBoxSearchByCourse.Text));
            }
            else
            {
                searchOfferingFilter.Add(new SqlParameter("@courseid", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchByLocation.Text))
            {
                searchOfferingFilter.Add(new SqlParameter("@locationname", txtBoxSearchByLocation.Text));
            }
            else
            {
                searchOfferingFilter.Add(new SqlParameter("@locationname", DBNull.Value));
            }

            if (!string.IsNullOrWhiteSpace(txtBoxSearchYear.Text))
            {
                searchOfferingFilter.Add(new SqlParameter("@year", txtBoxSearchYear.Text));
            }
            else
            {
                searchOfferingFilter.Add(new SqlParameter("@year", DBNull.Value));
            }

            return ValidationHelper.ValidateOnlyIntegers("Year", txtBoxSearchYear.Text) && ValidationHelper.ValidateNoIntegers("Location Name", txtBoxSearchByLocation.Text) && ValidationHelper.ValidateOnlyIntegers("Course ID", txtBoxSearchByCourse.Text);

        }

        public void SearchOfferingsWithFilters(DataGrid dsetOfferings, List<SqlParameter> searchOfferingFilter, ComboBox searchSemester, WatermarkTextBox txtBoxSearchByCourse, WatermarkTextBox txtBoxSearchByLocation, WatermarkTextBox txtBoxSearchYear)
        {
            if (SearchOfferingsFiltered(searchOfferingFilter, searchSemester, txtBoxSearchByCourse, txtBoxSearchByLocation, txtBoxSearchYear))
            {
                dsetOfferings.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetOfferings", searchOfferingFilter).DefaultView;
            }
        }

        public void SearchOfferingsWithFilters(KeyValuePair<string, SqlParameterDetails> teacherPrimaryKey, DataGrid dsetOfferings, List<SqlParameter> searchOfferingFilter, ComboBox searchSemester, WatermarkTextBox txtBoxSearchByCourse, WatermarkTextBox txtBoxSearchByLocation, WatermarkTextBox txtBoxSearchYear)
        {
            if (SearchOfferingsFiltered(searchOfferingFilter, searchSemester, txtBoxSearchByCourse, txtBoxSearchByLocation, txtBoxSearchYear))
            {
                searchOfferingFilter.Add(databaseConnection.GenerateSQLParameter(teacherPrimaryKey));
                dsetOfferings.ItemsSource = databaseConnection.GetAppropriateDataTableFromStoredProcedure("tsp_GetOfferings", searchOfferingFilter).DefaultView;
            }
        }
    }
}
