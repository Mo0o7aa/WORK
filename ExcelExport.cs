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

        public void ImportComplaint(string fileName, String filePath, ref Employee mLoggedInUser)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            if (filePath.Contains(".xls") || filePath.Contains(".xlsx"))
            {
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath);
                Excel.Worksheet workSheet = excelApp.ActiveSheet;

                int rowNumber = 2;
                int maxColumn = 15;

                int companyNameColumn = 0;
                int siteNameColumn = 0;
                int cityColumn = 0;
                int regionColumn = 0;
                int latColumn = 0;
                int longColumn = 0;
                int sectorColumn = 0;
                int bandColumn = 0;
                int azimuthColumn = 0;
                int rtwpColumn = 0;

                for (int i = 1; i < maxColumn; i++)
                {
                    if (Convert.ToString(workSheet.Cells[1, i].Value) == null)
                    {
                        i = maxColumn;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == "Company Name" || workSheet.Cells[1, i].Value.ToString() == "Company name" || workSheet.Cells[1, i].Value.ToString() == "company name" || workSheet.Cells[1, i].Value.ToString() == "COMPANY NAME")
                    {
                        companyNameColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"SITE" || workSheet.Cells[1, i].Value.ToString() == "Site" || workSheet.Cells[1, i].Value.ToString() == "site" || workSheet.Cells[1, i].Value.ToString() == "Site Name" || workSheet.Cells[1, i].Value.ToString() == "Site name" || workSheet.Cells[1, i].Value.ToString() == "site name" || workSheet.Cells[1, i].Value.ToString() == "Site Number" || workSheet.Cells[1, i].Value.ToString() == "Site number" || workSheet.Cells[1, i].Value.ToString() == "site number" || workSheet.Cells[1, i].Value.ToString() == "SITE_CODE" || workSheet.Cells[1, i].Value.ToString() == "SITE CODE" || workSheet.Cells[1, i].Value.ToString() == "Site Code" || workSheet.Cells[1, i].Value.ToString() == "site code" || workSheet.Cells[1, i].Value.ToString() == "Site code")
                    {
                        siteNameColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"CITY" || workSheet.Cells[1, i].Value.ToString() == "City" || workSheet.Cells[1, i].Value.ToString() == "city")
                    {
                        cityColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"REGION" || workSheet.Cells[1, i].Value.ToString() == "Region" || workSheet.Cells[1, i].Value.ToString() == "region" || workSheet.Cells[1, i].Value.ToString() == "Zone" || workSheet.Cells[1, i].Value.ToString() == "ZONE" || workSheet.Cells[1, i].Value.ToString() == "zone" || workSheet.Cells[1, i].Value.ToString() == "Area" || workSheet.Cells[1, i].Value.ToString() == "AREA" || workSheet.Cells[1, i].Value.ToString() == "area")
                    {
                        regionColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"LAT" || workSheet.Cells[1, i].Value.ToString() == "Lat" || workSheet.Cells[1, i].Value.ToString() == "lat" || workSheet.Cells[1, i].Value.ToString() == "LATITUDE" || workSheet.Cells[1, i].Value.ToString() == "Latitude" || workSheet.Cells[1, i].Value.ToString() == "latitude")
                    {
                        latColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"LONG" || workSheet.Cells[1, i].Value.ToString() == "Long" || workSheet.Cells[1, i].Value.ToString() == "LONGITUDE" || workSheet.Cells[1, i].Value.ToString() == "Longitude" || workSheet.Cells[1, i].Value.ToString() == "longitude")
                    {
                        longColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"CELL NAME" || workSheet.Cells[1, i].Value.ToString() == "Cell Name" || workSheet.Cells[1, i].Value.ToString() == "cell name" || workSheet.Cells[1, i].Value.ToString() == "Cell name" || workSheet.Cells[1, i].Value.ToString() == "Sector" || workSheet.Cells[1, i].Value.ToString() == "sector" || workSheet.Cells[1, i].Value.ToString() == "SECTOR")
                    {
                        sectorColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"Band" || workSheet.Cells[1, i].Value.ToString() == "BAND" || workSheet.Cells[1, i].Value.ToString() == "band")
                    {
                        bandColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"RTWP" || workSheet.Cells[1, i].Value.ToString() == "rtwp")
                    {
                        rtwpColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"Azimuth" || workSheet.Cells[1, i].Value.ToString() == "AZIMUTH" || workSheet.Cells[1, i].Value.ToString() == "azimuth")
                    {
                        azimuthColumn = i;
                        continue;
                    }

                }

                if (companyNameColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, company name column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (siteNameColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Site name column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (cityColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, City column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (regionColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Region column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (latColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Lat column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (longColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Long column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (sectorColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Sector or Cell Name column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (bandColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Band column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (azimuthColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Azimuth column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (rtwpColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, RTWP column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }


                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCompanies(ref companies))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCities(ref cities))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                loggedInUser = mLoggedInUser;

                bool HasValue = true;

                List<Site> sites = new List<Site>();

                Complaints complaint = new Complaints();
                complaint.sites = new List<BASIC_STRUCTS.SITE_STRUCT>();

                //if (!commonQueries.GetCompanySites(ref companySites, complaint.GetCompanySerial()))
                //{
                //    excelWorkBook.Close();
                //    return;
                //}
                //
                //for (int i = 0; i < complaint.sites.Count; i++)
                //{
                //    complaint.sites[i].site_serial = 0;
                //}

                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {
                        BASIC_STRUCTS.SITE_STRUCT tempSite = new BASIC_STRUCTS.SITE_STRUCT();
                        BASIC_STRUCTS.SECTOR_STRUCT tempSector = new BASIC_STRUCTS.SECTOR_STRUCT();

                        tempSite.sectors = new List<BASIC_STRUCTS.SECTOR_STRUCT>();
                        tempSector.sector_bands = new List<string>();

                        String tempBand = "";

                        if (workSheet.Cells[rowNumber, companyNameColumn].Value2 != null)
                        {
                            complaint.SetCompanyName(workSheet.Cells[rowNumber, companyNameColumn].Value.ToString());
                            complaint.SetCompanySerial(companies.Find(x1 => x1.value.Contains(complaint.GetCompanyName())).key);

                            if (complaint.GetCompanySerial() == 0)
                            {
                                MessageBox.Show("Spelling for company name in row '" + rowNumber + "' is not correct please check and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + companyNameColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, siteNameColumn].Value2 != null)
                        {
                            tempSite.site_number = workSheet.Cells[rowNumber, siteNameColumn].Value.ToString();
                            int tempSiteSerial = 0;
                            if(!commonQueries.GetSiteSerial(tempSite.site_number, ref tempSiteSerial))
                            {
                                excelWorkBook.Close();
                                return;
                            }

                            tempSite.site_serial = tempSiteSerial;

                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + siteNameColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, cityColumn].Value2 != null)
                        {
                            tempSite.city = workSheet.Cells[rowNumber, cityColumn].Value.ToString();
                            tempSite.city_id = cities[cities.FindIndex(x1 => x1.value == tempSite.city)].key;
                            if (tempSite.city_id == 0)
                            {
                                MessageBox.Show("City in row '" + rowNumber + "' is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }

                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + cityColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, regionColumn].Value2 != null)
                            tempSite.region = workSheet.Cells[rowNumber, regionColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + regionColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, latColumn].Value2 != null)
                            tempSite.latitude = workSheet.Cells[rowNumber, latColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + latColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, longColumn].Value2 != null)
                            tempSite.longitude = workSheet.Cells[rowNumber, longColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + longColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, sectorColumn].Value2 != null)
                        {
                            if (workSheet.Cells[rowNumber, sectorColumn].Value.ToString().Length == 1)
                                tempSector.sector_number = workSheet.Cells[rowNumber, sectorColumn].Value.ToString();
                            else
                                tempSector.sector_number = workSheet.Cells[rowNumber, sectorColumn].Value.ToString()[workSheet.Cells[rowNumber, sectorColumn].Value.ToString().Length - 1].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + sectorColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, bandColumn].Value2 != null)
                        {
                            String temp = workSheet.Cells[rowNumber, bandColumn].Value.ToString();
                            bool numeric = int.TryParse(workSheet.Cells[rowNumber, bandColumn].Value.ToString(), out int n);

                            if(numeric)
                            {
                                tempBand = workSheet.Cells[rowNumber, bandColumn].Value.ToString() + " MHz";
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + bandColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, azimuthColumn].Value2 != null)
                        {
                            String temp = workSheet.Cells[rowNumber, azimuthColumn].Value.ToString();
                            bool numeric = int.TryParse(workSheet.Cells[rowNumber, azimuthColumn].Value.ToString(), out int n);

                            if (numeric)
                            {
                                tempSector.azimuth = int.Parse(workSheet.Cells[rowNumber, azimuthColumn].Value.ToString());
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + azimuthColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, rtwpColumn].Value2 != null)
                        {
                            String temp = workSheet.Cells[rowNumber, rtwpColumn].Value.ToString();
                            bool numeric = decimal.TryParse(workSheet.Cells[rowNumber, rtwpColumn].Value.ToString(), out decimal n);

                            if (numeric)
                            {
                                tempSector.rtwp = decimal.Parse(workSheet.Cells[rowNumber, rtwpColumn].Value.ToString());
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + rtwpColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }


                        tempSector.rru = "On Tower";
                        tempSector.sector_status = "Interfered";
                        tempSector.sector_status_id = BASIC_STRUCTS.INTERFERED_SECTOR_STATUS;

                        tempSite.site_status_id = BASIC_STRUCTS.INTERFERED_SITE_STATUS;
                        tempSite.site_status = "Interfered";
                        

                        if (complaint.sites.Exists(x1 => x1.site_number == tempSite.site_number))
                        {
                            int siteIndex = complaint.sites.FindIndex(x1 => x1.site_number == tempSite.site_number);

                            if (complaint.sites[siteIndex].sectors.Exists(x1 => x1.sector_number == tempSector.sector_number))
                            {
                                int sectorIndex = complaint.sites[siteIndex].sectors.FindIndex(x1 => x1.sector_number == tempSector.sector_number);

                                if (!complaint.sites[siteIndex].sectors[sectorIndex].sector_bands.Exists(x1 => x1 == tempBand))
                                {
                                    complaint.sites[siteIndex].sectors[sectorIndex].sector_bands.Add(tempBand);
                                }
                            }
                            else
                            {
                                tempSector.sector_bands.Add(tempBand);
                                complaint.sites[siteIndex].sectors.Add(tempSector);
                            }
                        }
                        else
                        {
                            tempSector.sector_bands.Add(tempBand);
                            tempSite.sectors.Add(tempSector);
                            complaint.sites.Add(tempSite);
                        }

                        rowNumber++;
                    }
                    else
                    {
                        HasValue = false;
                    }
                }


                complaint.SetComplaintDate(DateTime.Now);
                complaint.InitializeAddedBy(loggedInUser.GetEmployeeId());
                complaint.SetComplaintStatus("Open");
                complaint.SetComplaintStatusId(BASIC_STRUCTS.OPEN_COMPLAINT_STATUS);
                complaint.SetTicketNumber("0");

                if (!complaint.IssueNewComplaint(BASIC_STRUCTS.MOBILE_OPERATOR))
                {
                    MessageBox.Show("Server connection failed please check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    excelWorkBook.Close();
                    return;
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
                int maxColumn = 12;

                int companyNameColumn = 0;
                int siteNameColumn = 0;
                int cityColumn = 0;
                int regionColumn = 0;
                int latColumn = 0;
                int longColumn = 0;

                for (int i = 1; i < maxColumn; i++)
                {
                    if (Convert.ToString(workSheet.Cells[1,i].Value) == null)
                    {
                        i = maxColumn;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == "Company Name" || workSheet.Cells[1, i].Value.ToString() == "Company name" || workSheet.Cells[1, i].Value.ToString() == "company name" || workSheet.Cells[1, i].Value.ToString() == "COMPANY NAME")
                    {
                        companyNameColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"SITE" || workSheet.Cells[1, i].Value.ToString() == "Site" || workSheet.Cells[1, i].Value.ToString() == "site" || workSheet.Cells[1, i].Value.ToString() == "Site Name" || workSheet.Cells[1, i].Value.ToString() == "Site name" || workSheet.Cells[1, i].Value.ToString() == "site name" || workSheet.Cells[1, i].Value.ToString() == "Site Number" || workSheet.Cells[1, i].Value.ToString() == "Site number" || workSheet.Cells[1, i].Value.ToString() == "site number" || workSheet.Cells[1, i].Value.ToString() == "SITE_CODE" || workSheet.Cells[1, i].Value.ToString() == "SITE CODE" || workSheet.Cells[1, i].Value.ToString() == "Site Code" || workSheet.Cells[1, i].Value.ToString() == "site code" || workSheet.Cells[1, i].Value.ToString() == "Site code")
                    {
                        siteNameColumn = i;
                        continue;
                    }
                        

                    if (workSheet.Cells[1, i].Value.ToString() == @"CITY" || workSheet.Cells[1, i].Value.ToString() == "City" || workSheet.Cells[1, i].Value.ToString() == "city")
                    {
                        cityColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"REGION" || workSheet.Cells[1, i].Value.ToString() == "Region" || workSheet.Cells[1, i].Value.ToString() == "region" || workSheet.Cells[1, i].Value.ToString() == "Zone" || workSheet.Cells[1, i].Value.ToString() == "ZONE" || workSheet.Cells[1, i].Value.ToString() == "zone" || workSheet.Cells[1, i].Value.ToString() == "Area" || workSheet.Cells[1, i].Value.ToString() == "AREA" || workSheet.Cells[1, i].Value.ToString() == "area")
                    {
                        regionColumn = i;
                        continue;
                    }
                        

                    if (workSheet.Cells[1, i].Value.ToString() == @"LAT" || workSheet.Cells[1, i].Value.ToString() == "Lat" || workSheet.Cells[1, i].Value.ToString() == "lat" || workSheet.Cells[1, i].Value.ToString() == "LATITUDE" || workSheet.Cells[1, i].Value.ToString() == "Latitude" || workSheet.Cells[1, i].Value.ToString() == "latitude")
                    {
                        latColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"LONG" || workSheet.Cells[1, i].Value.ToString() == "Long" || workSheet.Cells[1, i].Value.ToString() == "LONGITUDE" || workSheet.Cells[1, i].Value.ToString() == "Longitude" || workSheet.Cells[1, i].Value.ToString() == "longitude")
                    {
                        longColumn = i;
                        continue;
                    }
                          
                }

                if(companyNameColumn == 0)
                {
                        MessageBox.Show("Excel sheet is not in the correct format for import, company name column doesn't exist");
                        excelWorkBook.Close();
                        return;
                }

                if (siteNameColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Site name column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (cityColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, City column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (regionColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Region column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (latColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Lat column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (longColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Long column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> companies = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCompanies(ref companies))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCities(ref cities))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                loggedInUser = mLoggedInUser;

                bool HasValue = true;

                List<Site> sites = new List<Site>();
                List<String> existingSites = new List<string>();
                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {
                        Site site = new Site();
                        List<BASIC_STRUCTS.MIN_SITE_STRUCT> tempSitesList = new List<BASIC_STRUCTS.MIN_SITE_STRUCT>();
                        BASIC_STRUCTS.MIN_SITE_STRUCT tempSite = new BASIC_STRUCTS.MIN_SITE_STRUCT();

                        if (workSheet.Cells[rowNumber, companyNameColumn].Value2 != null)
                        {
                            site.SetCompanyName(workSheet.Cells[rowNumber, companyNameColumn].Value.ToString());
                            site.SetCompanySerial(companies.Find(x1 => x1.value.Contains(site.GetCompanyName())).key);

                            if(site.GetCompanySerial() == 0)
                            {
                                MessageBox.Show("Spelling for company name in row '" + rowNumber + "' is not correct please check and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + companyNameColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, siteNameColumn].Value2 != null)
                            site.SetSiteNumber(workSheet.Cells[rowNumber, siteNameColumn].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + siteNameColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, cityColumn].Value2 != null)
                        {
                            site.SetCity(workSheet.Cells[rowNumber, cityColumn].Value.ToString());
                            try
                            {
                                site.SetCityId(cities[cities.FindIndex(x1 => x1.value == site.GetCity())].key);
								if (site.GetCityId() == 0)
								{
									MessageBox.Show("City in row '" + rowNumber + "' is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
									excelWorkBook.Close();
									return;
								}
							}
                            catch
                            {
								MessageBox.Show("City '" + site.GetCity() + "' is not correct! Check spelling and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
							}
                            

                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + cityColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, regionColumn].Value2 != null)
                            site.SetRegion(workSheet.Cells[rowNumber, regionColumn].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + regionColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, latColumn].Value2 != null)
                            site.SetLat(workSheet.Cells[rowNumber, latColumn].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + latColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, longColumn].Value2 != null)
                            site.SetLong(workSheet.Cells[rowNumber, longColumn].Value.ToString());
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column " + longColumn + " cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        site.SetAddedById(loggedInUser.GetEmployeeId());
                        site.SetAddedBy(loggedInUser.GetEmployeeName());

                        if (!sites.Exists(x1 => x1.GetSiteNumber() == site.GetSiteNumber()))
                        {
                            if (commonQueries.CheckSiteNameExists(site.GetCompanySerial(), site.GetSiteNumber()))
                            {
                                sites.Add(site);
                            }
                            else
                            {
                                //MessageBox.Show("Site Number " + site.GetSiteNumber() + " already exists in the database! import failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                //excelWorkBook.Close();
                                //return;

                                existingSites.Add(site.GetSiteNumber());
                            }
                        }

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

                if (existingSites.Count > 0)
                {
                    String errorMessage = "Sites ";

                    for (int i = 0; i < existingSites.Count; i++)
                    {
                        if (!errorMessage.Contains(existingSites[i]))
                        {
                            errorMessage += existingSites[i];

                            if (i != existingSites.Count - 1)
                            {
                                errorMessage += ", ";
                            }
                        }
                    }

                    errorMessage += " already exists in the database!";

                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                excelWorkBook.Close();

            }
            else
            {
                MessageBox.Show("Please select excel file with extentions '.xls' or '.xlsx'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ImportRepeaters(string fileName, String filePath, ref Employee mLoggedInUser)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;


            if (filePath.Contains(".xls") || filePath.Contains(".xlsx"))
            {
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath);
                Excel.Worksheet workSheet = excelApp.ActiveSheet;

                int rowNumber = 2;
                int maxColumn = 12;

                int latColumn = 0;
                int longColumn = 0;
                int addressColumn = 0;
                int statusColumn = 0;
                int commentColumn = 0;
                int dateColumn = 0;
                int cityColumn = 0;
                int areaColumn = 0;

                for (int i = 1; i < maxColumn; i++)
                {
                    if (Convert.ToString(workSheet.Cells[1, i].Value) == null)
                    {
                        i = maxColumn;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == "Address")
                    {
                        addressColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"Status" || workSheet.Cells[1, i].Value.ToString() == "STATUS" || workSheet.Cells[1, i].Value.ToString() == "status")
                    {
                        statusColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"COMMENT" || workSheet.Cells[1, i].Value.ToString() == "Comment" || workSheet.Cells[1, i].Value.ToString() == "Comments" || workSheet.Cells[1, i].Value.ToString() == "comment" || workSheet.Cells[1, i].Value.ToString() == "comments")
                    {
                        commentColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"DATE" || workSheet.Cells[1, i].Value.ToString() == "Date" || workSheet.Cells[1, i].Value.ToString() == "date")
                    {
                        dateColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"LAT" || workSheet.Cells[1, i].Value.ToString() == "Lat" || workSheet.Cells[1, i].Value.ToString() == "lat" || workSheet.Cells[1, i].Value.ToString() == "LATITUDE" || workSheet.Cells[1, i].Value.ToString() == "Latitude")
                    {
                        latColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"LONG" || workSheet.Cells[1, i].Value.ToString() == "Long" || workSheet.Cells[1, i].Value.ToString() == "LONGITUDE" || workSheet.Cells[1, i].Value.ToString() == "Longitude")
                    {
                        longColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"City" || workSheet.Cells[1, i].Value.ToString() == "City" || workSheet.Cells[1, i].Value.ToString() == "city")
                    {
                        cityColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"AREA" || workSheet.Cells[1, i].Value.ToString() == "Area" || workSheet.Cells[1, i].Value.ToString() == "area" || workSheet.Cells[1, i].Value.ToString() == "REGION" || workSheet.Cells[1, i].Value.ToString() == "Region" || workSheet.Cells[1, i].Value.ToString() == "region")
                    {
                        areaColumn = i;
                        continue;
                    }

                }

                if (addressColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Address column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (statusColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Status column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (commentColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Comment column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (dateColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Date column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (latColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Lat column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (longColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Long column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (cityColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, City column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }


                if (areaColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Area or Region column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> repeatersStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetRepeatersStatus(ref repeatersStatus))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCities(ref cities))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                loggedInUser = mLoggedInUser;

                bool HasValue = true;


                List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {

                        BASIC_STRUCTS.REPEATER_STRUCT repeater = new BASIC_STRUCTS.REPEATER_STRUCT();

                        if (workSheet.Cells[rowNumber, latColumn].Value2 != null)
                        {
                            repeater.latitude = workSheet.Cells[rowNumber, latColumn].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + latColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, longColumn].Value2 != null)
                            repeater.longitude = workSheet.Cells[rowNumber, longColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + longColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, addressColumn].Value2 != null)
                        {
                            repeater.address = workSheet.Cells[rowNumber, addressColumn].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + addressColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, statusColumn].Value2 != null)
                        {
                            repeater.status = workSheet.Cells[rowNumber, statusColumn].Value.ToString();
                            repeater.status_id = repeatersStatus.Find(x1 => x1.value == repeater.status).key;

                            if (repeater.status_id == 0)
                            {
                                MessageBox.Show("Repeater status is incorrect please make sure it is either 'Pending or 'Removed' and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + statusColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, commentColumn].Value2 != null)
                            repeater.comment = workSheet.Cells[rowNumber, commentColumn].Value.ToString();
                        else
                        {
                            repeater.comment = "";
                            //MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + commentColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            //excelWorkBook.Close();
                            //return;
                        }

                        if (workSheet.Cells[rowNumber, dateColumn].Value2 != null)
                        {
                            try
                            {
                                double tempDate = double.Parse(workSheet.Cells[rowNumber, longColumn].Value.ToString());
                                repeater.date = DateTime.FromOADate(tempDate);
                            }
                            catch
                            {
                                MessageBox.Show("Repeaters date is not in correct format in row '" + rowNumber + "', Import Failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + longColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }


                        if (workSheet.Cells[rowNumber, cityColumn].Value2 != null)
                        {
                            repeater.city = workSheet.Cells[rowNumber, cityColumn].Value.ToString();
                            repeater.city_id = cities.Find(x1 => x1.value == repeater.city).key;

                            if (repeater.city_id == 0)
                            {
                                MessageBox.Show("City '" + repeater.city + "' is incorrect! please check spelling to match cities in the database and then try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + cityColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }


                        if (workSheet.Cells[rowNumber, areaColumn].Value2 != null)
                            repeater.area = workSheet.Cells[rowNumber, areaColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + areaColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        repeater.added_by_id = loggedInUser.GetEmployeeId();
                        repeater.added_by = loggedInUser.GetEmployeeName();

                        repeaters.Add(repeater);

                        rowNumber++;
                    }
                    else
                    {
                        HasValue = false;
                    }
                }



                if (!commonQueries.InsertIntoRepeaters(ref repeaters))
                {
                    MessageBox.Show("Server connection failed please check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    excelWorkBook.Close();
                    return;
                }


                excelWorkBook.Close();

                MessageBox.Show("Upload Complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("Please select excel file with extentions '.xls' or '.xlsx'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ReImportRepeaters(string fileName, String filePath, ref Employee mLoggedInUser)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = false;


            if (filePath.Contains(".xls") || filePath.Contains(".xlsx"))
            {
                Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(filePath);
                Excel.Worksheet workSheet = excelApp.ActiveSheet;

                int rowNumber = 2;
                int maxColumn = 12;

                int latColumn = 0;
                int longColumn = 0;
                int addressColumn = 0;
                int statusColumn = 0;
                int commentColumn = 0;
                int dateColumn = 0;
                int cityColumn = 0;
                int areaColumn = 0;

                for (int i = 1; i < maxColumn; i++)
                {
                    if (Convert.ToString(workSheet.Cells[1, i].Value) == null)
                    {
                        i = maxColumn;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == "Address")
                    {
                        addressColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"Status" || workSheet.Cells[1, i].Value.ToString() == "STATUS" || workSheet.Cells[1, i].Value.ToString() == "status")
                    {
                        statusColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"COMMENT" || workSheet.Cells[1, i].Value.ToString() == "Comment" || workSheet.Cells[1, i].Value.ToString() == "Comments" || workSheet.Cells[1, i].Value.ToString() == "comment" || workSheet.Cells[1, i].Value.ToString() == "comments")
                    {
                        commentColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"DATE" || workSheet.Cells[1, i].Value.ToString() == "Date" || workSheet.Cells[1, i].Value.ToString() == "date")
                    {
                        dateColumn = i;
                        continue;
                    }


                    if (workSheet.Cells[1, i].Value.ToString() == @"LAT" || workSheet.Cells[1, i].Value.ToString() == "Lat" || workSheet.Cells[1, i].Value.ToString() == "lat" || workSheet.Cells[1, i].Value.ToString() == "LATITUDE" || workSheet.Cells[1, i].Value.ToString() == "Latitude")
                    {
                        latColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"LONG" || workSheet.Cells[1, i].Value.ToString() == "Long" || workSheet.Cells[1, i].Value.ToString() == "LONGITUDE" || workSheet.Cells[1, i].Value.ToString() == "Longitude")
                    {
                        longColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"City" || workSheet.Cells[1, i].Value.ToString() == "City" || workSheet.Cells[1, i].Value.ToString() == "city")
                    {
                        cityColumn = i;
                        continue;
                    }

                    if (workSheet.Cells[1, i].Value.ToString() == @"AREA" || workSheet.Cells[1, i].Value.ToString() == "Area" || workSheet.Cells[1, i].Value.ToString() == "area" || workSheet.Cells[1, i].Value.ToString() == "REGION" || workSheet.Cells[1, i].Value.ToString() == "Region" || workSheet.Cells[1, i].Value.ToString() == "region")
                    {
                        areaColumn = i;
                        continue;
                    }

                }

                if (addressColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Address column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (statusColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Status column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (commentColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Comment column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (dateColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Date column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (latColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Lat column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (longColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Long column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                if (cityColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, City column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }


                if (areaColumn == 0)
                {
                    MessageBox.Show("Excel sheet is not in the correct format for import, Area or Region column doesn't exist");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> repeatersStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetRepeatersStatus(ref repeatersStatus))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> cities = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

                if (!commonQueries.GetCities(ref cities))
                {
                    MessageBox.Show("Server connection failed please try again later!");
                    excelWorkBook.Close();
                    return;
                }

                loggedInUser = mLoggedInUser;

                bool HasValue = true;


                List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters = new List<BASIC_STRUCTS.REPEATER_STRUCT>();

                while (HasValue)
                {
                    if (workSheet.Cells[rowNumber, 1].Value2 != null)
                    {

                        BASIC_STRUCTS.REPEATER_STRUCT repeater = new BASIC_STRUCTS.REPEATER_STRUCT();

                        if (workSheet.Cells[rowNumber, latColumn].Value2 != null)
                        {
                            repeater.latitude = workSheet.Cells[rowNumber, latColumn].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + latColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, longColumn].Value2 != null)
                            repeater.longitude = workSheet.Cells[rowNumber, longColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + longColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, addressColumn].Value2 != null)
                        {
                            repeater.address = workSheet.Cells[rowNumber, addressColumn].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + addressColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, statusColumn].Value2 != null)
                        {
                            repeater.status = workSheet.Cells[rowNumber, statusColumn].Value.ToString();
                            repeater.status_id = repeatersStatus.Find(x1 => x1.value == repeater.status).key;

                            if (repeater.status_id == 0)
                            {
                                MessageBox.Show("Repeater status is incorrect please make sure it is either 'Pending or 'Removed' and try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + statusColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        if (workSheet.Cells[rowNumber, commentColumn].Value2 != null)
                            repeater.comment = workSheet.Cells[rowNumber, commentColumn].Value.ToString();
                        else
                        {
                            repeater.comment = "";
                            //MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + commentColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            //excelWorkBook.Close();
                            //return;
                        }

                        if (workSheet.Cells[rowNumber, dateColumn].Value2 != null)
                        {
                            try
                            {
                                //double tempDate = double.Parse(workSheet.Cells[rowNumber, dateColumn].Value.ToString());
                                //repeater.date = DateTime.FromOADate(tempDate);
                                String tempDate = workSheet.Cells[rowNumber, dateColumn].Value.ToString();
                                repeater.date = DateTime.Parse(tempDate);
                            }
                            catch
                            {
                                MessageBox.Show("Repeaters date is not in correct format in row '" + rowNumber + "', Import Failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + longColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }


                        if (workSheet.Cells[rowNumber, cityColumn].Value2 != null)
                        {
                            repeater.city = workSheet.Cells[rowNumber, cityColumn].Value.ToString();
                            repeater.city_id = cities.Find(x1 => x1.value == repeater.city).key;

                            if (repeater.city_id == 0)
                            {
                                MessageBox.Show("City '" + repeater.city + "' is incorrect! please check spelling to match cities in the database and then try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                excelWorkBook.Close();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + cityColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }


                        if (workSheet.Cells[rowNumber, areaColumn].Value2 != null)
                            repeater.area = workSheet.Cells[rowNumber, areaColumn].Value.ToString();
                        else
                        {
                            MessageBox.Show("Value of cell in row '" + rowNumber + "' and column '" + areaColumn + "' cant be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }

                        repeater.added_by_id = loggedInUser.GetEmployeeId();
                        repeater.added_by = loggedInUser.GetEmployeeName();

                        repeaters.Add(repeater);

                        rowNumber++;
                    }
                    else
                    {
                        HasValue = false;
                    }
                }

                int X = 0;

                ////law check repeater false kda msh mwgod fel DB
                for(int i = 0; i < repeaters.Count; i++)
                {
                    BASIC_STRUCTS.REPEATER_STRUCT tempRepeater = repeaters[i];

                    if(!commonQueries.CheckRepeater(ref tempRepeater))
                    {
                        if (!commonQueries.InsertIntoRepeaters(ref tempRepeater))
                        {
                            MessageBox.Show("Server connection failed please check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }
                    }
                    else
                    {
                        if (!commonQueries.UpdateRepeater(ref tempRepeater))
                        {
                            MessageBox.Show("Server connection failed please check your internet connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            excelWorkBook.Close();
                            return;
                        }
                        X++;
                    }
                }


                excelWorkBook.Close();


                MessageBox.Show("Upload Complete! Total of " + X + " repeaters were updated in database", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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

        public void ExportRepeaters(ref List<BASIC_STRUCTS.REPEATER_STRUCT> repeaters)
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

            excelWorkSheet.Cells[1, 1] = "Serial";
            excelWorkSheet.Cells[1, 2] = "City";
            excelWorkSheet.Cells[1, 3] = "Area";
            excelWorkSheet.Cells[1, 4] = "Lat";
            excelWorkSheet.Cells[1, 5] = "Long";
            excelWorkSheet.Cells[1, 6] = "Address";
            excelWorkSheet.Cells[1, 7] = "Status";
            excelWorkSheet.Cells[1, 8] = "Comment";
            excelWorkSheet.Cells[1, 9] = "Date";




            int rowNumber = 2;

            for (int i = 0; i < repeaters.Count; i++)
            {
                excelWorkSheet.Cells[rowNumber, 1] = repeaters[i].repeater_serial;
                excelWorkSheet.Cells[rowNumber, 2] = repeaters[i].city;
                excelWorkSheet.Cells[rowNumber, 3] = repeaters[i].area;
                excelWorkSheet.Cells[rowNumber, 4] = repeaters[i].latitude;
                excelWorkSheet.Cells[rowNumber, 5] = repeaters[i].longitude;
                excelWorkSheet.Cells[rowNumber, 6] = repeaters[i].address;
                excelWorkSheet.Cells[rowNumber, 7] = repeaters[i].status;
                excelWorkSheet.Cells[rowNumber, 8] = repeaters[i].comment;
                excelWorkSheet.Cells[rowNumber, 9] = repeaters[i].date;

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
