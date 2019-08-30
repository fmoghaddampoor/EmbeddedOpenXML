using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
   public partial class Form1 : Form
   {
      public Form1()
      {
         InitializeComponent();
      }

      private void Button1_Click(object sender, EventArgs e)
      {
         // Open the document for editing.
         using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument spreadsheetDocument =
             DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(@"C:\Users\fmogh\OneDrive\Desktop\Excel\Farzad.xlsx", false))
         {
            DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
            DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.SheetData>().First();
            string text;
            foreach (DocumentFormat.OpenXml.Spreadsheet.Row r in sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>())
            {
               foreach (DocumentFormat.OpenXml.Spreadsheet.Cell c in r.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
               {
                  text = c.CellValue.Text;
                  Console.Write(text + " ");
               }
            }
         }
      }
   }
}
