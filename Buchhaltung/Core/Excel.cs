using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buchhaltung.Core
{
    class Excel
    {
        public string path { get; set; }
        Workbook wb { get; set; }
        public Excel(string path, string[] Sheets)
        {
            this.path = path;
            Application excel = new Application();
            this.wb = excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet); ;
            for (int i = 1; i < Sheets.Length; i++)
            {
                wb.Worksheets.Add(After: wb.Worksheets[i]);
            }
            for (int i = 0; i < Sheets.Length; i++)
            {
                wb.Worksheets[i + 1].Name = Sheets[i];
            }
        }
        public void writeArray(int sheetNumber, string range, string[] array)
        {
            Worksheet ws;
            ws = wb.Worksheets[sheetNumber];
            Microsoft.Office.Interop.Excel.Range cellRange = ws.Range[range];
            cellRange.set_Value(XlRangeValueDataType.xlRangeValueDefault, array);
        }
        public void changeColumnWidth(int sheetNumber,string range,int width)
        {
            Worksheet ws;
            ws = wb.Worksheets[sheetNumber];
            Microsoft.Office.Interop.Excel.Range cellRange = ws.Range[range];
            cellRange.ColumnWidth = width;
        }
        public void save()
        {
            wb.SaveAs(path);
            wb.Close();
        }


    }
}
