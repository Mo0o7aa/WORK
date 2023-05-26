using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for CompanyEquipmentWindow.xaml
    /// </summary>
    public partial class CompanyEquipmentWindow : Window
    {
        Employee loggedInUser;
        SQLServer sql;
        

        public CompanyEquipmentWindow(ref Employee mLoggedInUser)
        {
            loggedInUser = mLoggedInUser;
            sql = new SQLServer();

            InitializeComponent();

            InitializeUnitTypeCombo();
        }

        private void InitializeUnitTypeCombo()
        {
            BrushConverter brushConverter = new BrushConverter();

            ComboBoxItem item1 = new ComboBoxItem();
            item1.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            item1.FontWeight = FontWeights.Bold;
            item1.Content = "Handheld Unit";

            ComboBoxItem item2 = new ComboBoxItem();
            item2.Foreground = (Brush)brushConverter.ConvertFrom("#000080");
            item2.FontWeight = FontWeights.Bold;
            item2.Content = "Vehicle Unit";

            unitTypeComboBox.Items.Add(item1);
            unitTypeComboBox.Items.Add(item2);
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if (unitNameTextBox.Text == "")
                MessageBox.Show("Unit Name is not specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            if(unitTypeComboBox.SelectedIndex == -1)
                MessageBox.Show("Unit Type is not specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //////Insert into handhelds
            if(unitTypeComboBox.SelectedIndex == 0)
            {
                int maxUnitId = 0;

                String sqlQuery = "select max(id) from NTRA.dbo.handheld";

                BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
                SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

                if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                    return;

                maxUnitId = sql.rows[0].sql_int[0] + 1;

                sqlQuery = "Insert into NTRA.dbo.handheld values(";
                sqlQuery += maxUnitId + ",'";
                sqlQuery += unitNameTextBox.Text + "', ";
                sqlQuery += loggedInUser.GetEmployeeId() + ",getdate());";

                if (!sql.InsertRows(sqlQuery))
                    return;

                this.Close();
            }
            else
            {
                int maxUnitId = 0;

                String sqlQuery = "select max(id) from NTRA.dbo.vehicles";

                BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT SQL_COLUMN_COUNT_STRUCT = new BASIC_STRUCTS.SQL_COLUMN_COUNT_STRUCT();
                SQL_COLUMN_COUNT_STRUCT.sql_int = 1;

                if (!sql.GetRows(sqlQuery, SQL_COLUMN_COUNT_STRUCT))
                    return;

                maxUnitId = sql.rows[0].sql_int[0] + 1;

                sqlQuery = "Insert into NTRA.dbo.vehicles values(";
                sqlQuery += maxUnitId + ",'";
                sqlQuery += unitNameTextBox.Text + "', ";
                sqlQuery += loggedInUser.GetEmployeeId() + ",getdate());";

                if (!sql.InsertRows(sqlQuery))
                    return;

                this.Close();
            }
        }
    }
}
