using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ntra_missions
{
    public class ExcelExport
    {

        Microsoft.Office.Interop.Excel.Application excel = null;
        Microsoft.Office.Interop.Excel.Workbook wb = null;
        object missing = Type.Missing;
        Microsoft.Office.Interop.Excel.Worksheet ws = null;
        Microsoft.Office.Interop.Excel.Range rng = null;

        ExcelExport()
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
