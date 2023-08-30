using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;
using Label = System.Windows.Controls.Label;
using Style = System.Windows.Style;
using System.IO;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using CheckBox = System.Windows.Controls.CheckBox;
using Button = System.Windows.Controls.Button;
using System.Security.Cryptography;

namespace ntra_missions
{
    public class ExcelExport
    {

        Microsoft.Office.Interop.Excel.Application excel = null;
        Microsoft.Office.Interop.Excel.Workbook wb = null;
        object missing = Type.Missing;
        Microsoft.Office.Interop.Excel.Worksheet ws = null;
        Microsoft.Office.Interop.Excel.Range rng = null;

        List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> emfStatus;

        CommonQueries commonQueries;

        Popup popUp = new Popup();

        Employee loggedInUser = new Employee();

        public ExcelExport()
        {
            commonQueries = new CommonQueries();

            emfStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            if (!commonQueries.GetEMFPointStatus(ref emfStatus))
                return;


            popUp.PopupAnimation = PopupAnimation.Fade;
            popUp.Placement = PlacementMode.Center;
            popUp.AllowsTransparency = true;
        }

        public void ImportEMF(string fileName, String filePath)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;
            if (filePath.Contains(".xls") || filePath.Contains(".xlsx"))
            {
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath);
                Excel.Worksheet workSheet = excelApp.ActiveSheet;

                int rowNumber = 2;
                int maxColumn = 11;

                if (workSheet.Cells[1, 1].Value.ToString() != "Area")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import");
                    excelWorkBook.Close();
                    return;
                }

                //if (workSheet.Cells[rowNumber, 1].Value.ToString() != @"Gov. / District")
                //{
                //    MessageBox.Show("Excel sheet is not in the correct format for import");
                //    excelWorkBook.Close();
                //    return;
                //}


                //BrushConverter brushConverter = new BrushConverter();
                //System.Windows.Controls.Border border = new System.Windows.Controls.Border() { BorderThickness = new System.Windows.Thickness(3), BorderBrush = (Brush)brushConverter.ConvertFrom("#000080"),Height = 100, Width = 400};
                //
                //ScrollViewer scrollViewer = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                //StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                //
                //Label headerLabel = new Label() { HorizontalAlignment = HorizontalAlignment.Stretch};
                //headerLabel.Content = "Please match your excel column headers with the excel form header";
                //
                //stackPanel.Children.Add(headerLabel);
                //
                //Label excelFormHeaders = new Label() { HorizontalAlignment = HorizontalAlignment.Stretch };
                //excelFormHeaders.Content = "Area-District-Name-Lattitude-Longitude-Date-AveragePower-MaxPower-ActualLat-ActualLong";
                //
                //stackPanel.Children.Add(excelFormHeaders);
                //
                //StackPanel selectedHeaders = new StackPanel() { Orientation = Orientation.Horizontal };
                //
                //stackPanel.Children.Add(selectedHeaders);
                //
                //StackPanel excelHeaders = new StackPanel() { Orientation = Orientation.Horizontal };
                //
                //for(int i = 0; i < columnHeadersList.Count; i++)
                //{
                //    CheckBox currentCheckBox = new CheckBox() { Margin = new Thickness(4)};
                //    currentCheckBox.Content = columnHeadersList[i].ToString();
                //    currentCheckBox.Checked += OnCheckHeadersCheckBox;
                //    currentCheckBox.Unchecked += OnUnCheckHeadersCheckBox;
                //}
                //
                //Button finishButton = new Button();
                //finishButton.Background = (Brush)brushConverter.ConvertFrom("#000080");
                //finishButton.Foreground = Brushes.White;
                //finishButton.FontWeight = FontWeights.Bold;
                //finishButton.FontSize = 12;
                //finishButton.Click += OnClickFinishButton;
                //
                //popUp.IsOpen = true;

                bool HasValue = true;

                List<EMF> emfPoints = new List<EMF>();

                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {
                        EMF emf = new EMF();
                        List<BASIC_STRUCTS.EMF_BAND_STRUCT> pointBands = new List<BASIC_STRUCTS.EMF_BAND_STRUCT>();
                        BASIC_STRUCTS.EMF_BAND_STRUCT pointBand = new BASIC_STRUCTS.EMF_BAND_STRUCT();

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetArea(workSheet.Cells[rowNumber, 1].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '1' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetDistrict(workSheet.Cells[rowNumber, 2].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '2' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetName(workSheet.Cells[rowNumber, 3].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '3' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetLat(workSheet.Cells[rowNumber, 4].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '4' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetLong(workSheet.Cells[rowNumber, 5].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '5' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                        {
                            try
                            {
                                emf.SetReadingDate(DateTime.Parse(workSheet.Cells[rowNumber, 6].Value.ToString()));
                            }
                            catch
                            {
                                MessageBox.Show("Date is not in the correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '6' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            pointBand.average_power_density = ((Double)workSheet.Cells[rowNumber, 7].Value);
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '7' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            pointBand.max_power_density = ((Double)workSheet.Cells[rowNumber, 8].Value);
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column 8' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetActualLat(workSheet.Cells[rowNumber, 9].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '9' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                            emf.SetActualLong(workSheet.Cells[rowNumber, 10].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '10' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (emf.GetActualLat() != "")
                        {
                            emf.SetStatus("Closed");
                            emf.SetStatusId(2);
                        }
                        else
                        {
                            emf.SetStatus("Pending");
                            emf.SetStatusId(1);
                        }

                        if (pointBand.max_power_density != 0)
                        {
                            pointBand.band = 1;
                            pointBands.Add(pointBand);
                            emf.SetPointBands(ref pointBands);
                        }

                        emfPoints.Add(emf);

                        rowNumber++;
                    }
                    else
                    {
                        HasValue = false;
                    }
                }


                for (int i = 0; i < emfPoints.Count; i++)
                {
                    EMF currentPoint = emfPoints[i];

                    if (!currentPoint.GetNewSerial())
                    {
                        excelWorkBook.Close();
                        return;
                    }

                    if (!currentPoint.InsertIntoEMFPoints())
                    {
                        excelWorkBook.Close();
                        return;
                    }

                    if (!currentPoint.InsertIntoEMFPointBands())
                    {
                        excelWorkBook.Close();
                        return;
                    }
                }

                excelWorkBook.Close();

            }
            else
            {
                MessageBox.Show("Please select excel file with extentions '.xls' or '.xlsx'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ImportCompanySites(string fileName, String filePath, ref Employee mLoggedInUser)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            if (filePath.Contains(".xls") || filePath.Contains(".xlsx"))
            {
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath);
                Excel.Worksheet workSheet = excelApp.ActiveSheet;

                int rowNumber = 2;
                int maxColumn = 8;

                if (workSheet.Cells[1, 1].Value.ToString() != "Company Name")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that first column header  Company Name");
                    excelWorkBook.Close();
                    return;
                }

                if (workSheet.Cells[1, 2].Value.ToString() != @"SITE" && workSheet.Cells[1, 2].Value.ToString() != "Site" && workSheet.Cells[1, 2].Value.ToString() != "Site Name" && workSheet.Cells[1, 2].Value.ToString() != "Site Number")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that second column header is Site");
                    excelWorkBook.Close();
                    return;
                }

                if (workSheet.Cells[1, 4].Value.ToString() != @"CITY" && workSheet.Cells[1, 4].Value.ToString() != "City")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that Forth column header is City");
                    excelWorkBook.Close();
                    return;
                }

                if (workSheet.Cells[1, 5].Value.ToString() != @"REGION" && workSheet.Cells[1, 5].Value.ToString() != "Region" && workSheet.Cells[1, 5].Value.ToString() != "Zone" && workSheet.Cells[1, 5].Value.ToString() != "ZONE")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that Fifth column header is Region");
                    excelWorkBook.Close();
                    return;
                }

                if (workSheet.Cells[1, 7].Value.ToString() != @"LAT" && workSheet.Cells[1, 7].Value.ToString() != "Lat" && workSheet.Cells[1, 7].Value.ToString() != "lat" && workSheet.Cells[1, 7].Value.ToString() != "LATITUDE" && workSheet.Cells[1, 7].Value.ToString() != "Latitude")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that seventh column header is Lat");
                    excelWorkBook.Close();
                    return;
                }

                if (workSheet.Cells[1, 8].Value.ToString() != @"LONG" && workSheet.Cells[1, 8].Value.ToString() != "Long" && workSheet.Cells[1, 8].Value.ToString() != "LONGITUDE" && workSheet.Cells[1, 8].Value.ToString() != "Longitude")
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Please make sure that Eighth column header is Long");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if(!commonQueries.GetCompanies(ref companies))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCities(ref cities))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    return;
                }

                loggedInUser = mLoggedInUser;

                //BrushConverter brushConverter = new BrushConverter();
                //System.Windows.Controls.Border border = new System.Windows.Controls.Border() { BorderThickness = new System.Windows.Thickness(3), BorderBrush = (Brush)brushConverter.ConvertFrom("#000080"),Height = 100, Width = 400};
                //
                //ScrollViewer scrollViewer = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                //StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                //
                //Label headerLabel = new Label() { HorizontalAlignment = HorizontalAlignment.Stretch};
                //headerLabel.Content = "Please match your excel column headers with the excel form header";
                //
                //stackPanel.Children.Add(headerLabel);
                //
                //Label excelFormHeaders = new Label() { HorizontalAlignment = HorizontalAlignment.Stretch };
                //excelFormHeaders.Content = "Area-District-Name-Lattitude-Longitude-Date-AveragePower-MaxPower-ActualLat-ActualLong";
                //
                //stackPanel.Children.Add(excelFormHeaders);
                //
                //StackPanel selectedHeaders = new StackPanel() { Orientation = Orientation.Horizontal };
                //
                //stackPanel.Children.Add(selectedHeaders);
                //
                //StackPanel excelHeaders = new StackPanel() { Orientation = Orientation.Horizontal };
                //
                //for(int i = 0; i < columnHeadersList.Count; i++)
                //{
                //    CheckBox currentCheckBox = new CheckBox() { Margin = new Thickness(4)};
                //    currentCheckBox.Content = columnHeadersList[i].ToString();
                //    currentCheckBox.Checked += OnCheckHeadersCheckBox;
                //    currentCheckBox.Unchecked += OnUnCheckHeadersCheckBox;
                //}
                //
                //Button finishButton = new Button();
                //finishButton.Background = (Brush)brushConverter.ConvertFrom("#000080");
                //finishButton.Foreground = Brushes.White;
                //finishButton.FontWeight = FontWeights.Bold;
                //finishButton.FontSize = 12;
                //finishButton.Click += OnClickFinishButton;
                //
                //popUp.IsOpen = true;

                bool HasValue = true;

                List<Site> sites = new List<Site>();

                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {
                        Site site = new Site();
                        List<BASIC_STRUCTS.MIN_SITE_STRUCT> tempSitesList = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
                        BASIC_STRUCTS.MIN_SITE_STRUCT tempSite = new BASIC_STRUCTS.MIN_SITE_STRUCT();

                        if (workSheet.Cells[rowNumber, 1].Value2 != null)
                        {
                            site.SetCompanyName(workSheet.Cells[rowNumber, 1].Value.ToString());
                            site.SetCompanySerial(companies.Find(x1 => x1.value.Contains(site.GetCompanyName())).key);
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '1' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 2].Value2 != null)
                            site.SetSiteNumber(workSheet.Cells[rowNumber, 2].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '2' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 4].Value2 != null)
                        {
                            site.SetCity(workSheet.Cells[rowNumber, 4].Value.ToString());
                            site.SetCityId(cities[cities.FindIndex(x1 => x1.value == site.GetCity())].key);
                            if(site.GetCityId() == 0)
                            {
                                MessageBox.Show("City in row '" + rowNumber + "' is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }

                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '4' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 5].Value2 != null)
                            site.SetRegion(workSheet.Cells[rowNumber, 5].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '5' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 7].Value2 != null)
                            site.SetLat(workSheet.Cells[rowNumber, 7].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '7' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, 8].Value2 != null)
                            site.SetLong(workSheet.Cells[rowNumber, 8].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '8' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        site.SetAddedById(loggedInUser.GetEmployeeId());
                        site.SetAddedBy(loggedInUser.GetEmployeeName());

                        if(!sites.Exists(x1 => x1.GetSiteNumber() == site.GetSiteNumber()))
                            sites.Add(site);

                        rowNumber++;
                    }
                    else
                    {
                        HasValue = false;
                    }
                }


                for (int i = 0; i < sites.Count; i++)
                {
                    Site currentSite = sites[i];


                    if (!currentSite.IssueNewSite())
                    {
                        MessageBox.Show("Server connection failed please check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        excelWorkBook.Close();
                        return;
                    }

                }

                excelWorkBook.Close();

            }
            else
            {
                MessageBox.Show("Please select excel file with extentions '.xls' or '.xlsx'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void ExportCompanySites(ref List<BASIC_STRUCTS.MIN_SITE_STRUCT> sites)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbooks excelWorkBooks = null;
            Excel.Workbook excelWorkBook = null;
            Excel.Worksheet excelWorkSheet = null;

            excelWorkBooks = excelApp.Workbooks;
            excelApp.Workbooks.Add();
            excelWorkBook = excelWorkBooks[1];
            excelWorkSheet = excelWorkBook.Worksheets[1];

            object misValue = System.Reflection.Missing.Value;


            Microsoft.Office.Interop.Excel.Range columnsNameRange;

            excelWorkSheet.Cells[1, 1] = "Company Name";
            excelWorkSheet.Cells[1, 2] = "Site Number";
            excelWorkSheet.Cells[1, 3] = "Latitude";
            excelWorkSheet.Cells[1, 4] = "Longitude";
            excelWorkSheet.Cells[1, 5] = "City";
            excelWorkSheet.Cells[1, 6] = "Region";

            


            int rowNumber = 2;

            for(int i = 0; i < sites.Count; i++)
            {
                excelWorkSheet.Cells[rowNumber, 1] = sites[i].company_name;
                excelWorkSheet.Cells[rowNumber, 2] = sites[i].site_number;
                excelWorkSheet.Cells[rowNumber, 3] = sites[i].latitude;
                excelWorkSheet.Cells[rowNumber, 4] = sites[i].longitude;
                excelWorkSheet.Cells[rowNumber, 5] = sites[i].city;
                excelWorkSheet.Cells[rowNumber, 6] = sites[i].region;

                rowNumber++;
            }

            excelWorkSheet.Columns.AutoFit();
            //excelWorkSheet.Columns.HorizontalAlignment = HorizontalAlignment.Center;
            //columnsNameRange = excelWorkSheet.get_Range("A1", misValue).get_Resize(1, 6);
            //columnsNameRange.Columns.AutoFit();
            excelApp.Visible = true;

        }



        public void ReadEMFExcel(int rowNumber, Excel.Worksheet workSheet, Excel.Workbook excelWorkBook)
        {
            
        }

        private void OnClickFinishButton(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            StackPanel parentStackPanel = (StackPanel)currentButton.Parent;
            StackPanel selectedHeadersStackPanel = (StackPanel)parentStackPanel.Children[1];
            StackPanel excelSheetHeaders = (StackPanel)parentStackPanel.Children[2];

            for(int i = 0; i < excelSheetHeaders.Children.Count; i++)
            {
                CheckBox currentCheckBox = (CheckBox)excelSheetHeaders.Children[i];
                if(currentCheckBox.IsChecked == false)
                {
                    MessageBox.Show(currentCheckBox.Content.ToString() + " column header is not selected, Please select all your excel sheet headers", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }


        }

        private void OnUnCheckHeadersCheckBox(object sender, RoutedEventArgs e)
        {

        }

        private void OnCheckHeadersCheckBox(object sender, RoutedEventArgs e)
        {

        }

        public void ExportDataGrid(ref DataGrid dataGrid)
        {

            excel = new Microsoft.Office.Interop.Excel.Application();
            wb = excel.Workbooks.Add();
            ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;
            ws.Columns.AutoFit();
            ws.Columns.EntireColumn.ColumnWidth = 25;


            //for (int Idx = 0; Idx < dataGrid.Columns.Count; Idx++)
            //{

            //    ws.Range["A1"].Offset[0, Idx].Value = dataGrid.Columns[Idx].GetValue();
            //}

            //// Data Rows
            //for (int Idx = 0; Idx < dataGrid.Rows.Count; Idx++)
            //{
            //    ws.Range["A2"].Offset[Idx].Resize[1, dataGrid.Columns.Count].Value = dataGrid.Rows[Idx].GetValue();
            //}

            for(int i = 0; i < dataGrid.Items.Count; i++)
            {
                var temp = dataGrid.Items[i].ToString();
            }

            excel.Visible = true;
            wb.Activate();

        }
    }
}
