using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Course
{
    class ReportBuilder
    {
        static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.FinalReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public static void ProductQuantityReport(DataTable data)
        {
            var excelApp = new Excel.Application();
            Excel.Workbooks workbooks = excelApp.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Type.Missing);
            Excel.Sheets worksheets = workbook.Worksheets;
            Excel.Worksheet worksheet = worksheets.Add();
            worksheet.Cells[1, "A"] = "Остатки товаров";
            worksheet.Cells[2, "A"] = "Наименование";
            worksheet.Cells[2, "B"] = "Количество";
            var table = data;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                worksheet.Cells[i + 3, "A"] = table.Rows[i].ItemArray[table.Columns["ProductName"].Ordinal].ToString();
                worksheet.Cells[i + 3, "B"] = table.Rows[i].ItemArray[table.Columns["ProductQuantity"].Ordinal].ToString();
            }
            excelApp.Visible = true;
            ReleaseObject(worksheet);
            ReleaseObject(worksheets);
            ReleaseObject(workbook);
            ReleaseObject(workbooks);
            excelApp.Quit();
            ReleaseObject(excelApp);
        }
        public static void ProductRevenueReport(DataTable data, DateTime dateFrom, DateTime dateTo)
        {
            var excelApp = new Excel.Application();
            Excel.Workbooks workbooks = excelApp.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Type.Missing);
            Excel.Sheets worksheets = workbook.Worksheets;
            Excel.Worksheet worksheet = worksheets.Add();
            worksheet.Cells[1, "A"] = $"Выручка товаров за период {dateFrom.Date.ToString("dd.MM.yyyy")}-{dateTo.Date.ToString("dd.MM.yyyy")}";
            worksheet.Cells[2, "A"] = "Наименование";
            worksheet.Cells[2, "B"] = "Сумма";
            var table = data;
            var totalRevenue = 0;
            var row = 3;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var id = table.Rows[i].ItemArray[table.Columns["ProductId"].Ordinal].ToString();
                string revenue;
                try
                {
                    revenue = Connection.GetProductRevenue(id, dateFrom, dateTo);
                    if (revenue == "0")
                    {
                        continue;
                    }
                    totalRevenue += Convert.ToInt32(revenue);
                }
                catch (Exception ex)
                {
                    ReleaseObject(worksheet);
                    ReleaseObject(worksheets);
                    ReleaseObject(workbook);
                    ReleaseObject(workbooks);
                    excelApp.Quit();
                    ReleaseObject(excelApp);
                    throw ex;
                }
                worksheet.Cells[row, "A"] = table.Rows[i].ItemArray[table.Columns["ProductName"].Ordinal].ToString();
                worksheet.Cells[row, "B"] = revenue;
                row++;
            }
            worksheet.Cells[row, "A"] = "ИТОГО:";
            worksheet.Cells[row, "B"] = totalRevenue.ToString();
            worksheet.Columns.AutoFit();
            excelApp.Visible = true;
            ReleaseObject(worksheet);
            ReleaseObject(worksheets);
            ReleaseObject(workbook);
            ReleaseObject(workbooks);
            excelApp.Quit();
            ReleaseObject(excelApp);
        }
        public static void OrderItemsReport(string orderId, DateTime orderDate, string orderWorker)
        {
            var excelApp = new Excel.Application();
            Excel.Workbooks workbooks = excelApp.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Type.Missing);
            Excel.Sheets worksheets = workbook.Worksheets;
            Excel.Worksheet worksheet = worksheets.Add();
            worksheet.Cells[1, "A"] = "ИП \"Пекарни Круглова\"";
            worksheet.Cells[2, "A"] = "019193, г. Городец, ул. Ленина 52";
            worksheet.Cells[3, "A"] = "РН ККТ 0893512481100103";
            worksheet.Cells[3, "B"] = "ФН 1872827102002873";
            worksheet.Cells[4, "A"] = "ККТ 1231541792055631";
            worksheet.Cells[4, "B"] = "ИНН 1023715210";
            worksheet.Cells[5, "A"] = "КАССОВЫЙ ЧЕК/ПРИХОД";
            worksheet.Cells[5, "B"] = orderId;
            worksheet.Cells[6, "A"] = "КАССИР";
            worksheet.Cells[6, "B"] = orderWorker;
            worksheet.Cells[7, "B"] = orderDate.ToString("dd.MM.yyyy");
            DataTable table;
            try
            {
                table = Connection.GetOrderItemsForReport(orderId);
            }
            catch (Exception ex)
            {
                ReleaseObject(worksheet);
                ReleaseObject(worksheets);
                ReleaseObject(workbook);
                ReleaseObject(workbooks);
                excelApp.Quit();
                ReleaseObject(excelApp);
                throw ex;
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var quantity = table.Rows[i].ItemArray[table.Columns["OrderItemQuantity"].Ordinal].ToString();
                var cost = table.Rows[i].ItemArray[table.Columns["OrderItemCost"].Ordinal].ToString();
                worksheet.Cells[i * 2 + 8, "A"] = table.Rows[i].ItemArray[table.Columns["ProductName"].Ordinal].ToString();
                worksheet.Cells[i * 2 + 8, "A"].WrapText = true;
                worksheet.Cells[i * 2 + 8, "B"] = $"{quantity} * {cost}р";
                worksheet.Cells[i * 2 + 9, "B"] = table.Rows[i].ItemArray[table.Columns["TotalCost"].Ordinal].ToString() + "р";
            }
            var row = table.Rows.Count * 2 + 8;
            worksheet.Cells[row, "A"] = "ИТОГО:";
            try
            {
                worksheet.Cells[row, "B"] = Connection.GetOrderTotalCost(orderId) + "р";
            }
            catch (Exception ex)
            {
                ReleaseObject(worksheet);
                ReleaseObject(worksheets);
                ReleaseObject(workbook);
                ReleaseObject(workbooks);
                excelApp.Quit();
                ReleaseObject(excelApp);
                throw ex;
            }
            row++;
            worksheet.Cells[row, "A"] = "Сайт ФНС:";
            worksheet.Cells[row, "B"] = "www.nalog.ru";
            ((Excel.Range)worksheet.Columns["A"]).ColumnWidth = 24;
            ((Excel.Range)worksheet.Columns["B"]).ColumnWidth = 22;
            worksheet.Rows.AutoFit();
            excelApp.Visible = true;
            ReleaseObject(worksheet);
            ReleaseObject(worksheets);
            ReleaseObject(workbook);
            ReleaseObject(workbooks);
            excelApp.Quit();
            ReleaseObject(excelApp);
        }
    }
}
