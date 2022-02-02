using System;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Egate_Ecommerce
{
    public static class NPOIExt
    {
        public static object GetCellValue(this ICell cell)
        {
            if (cell == null) return null;
            switch (cell.CellType)
            {
                case CellType.String: return cell.StringCellValue;
                case CellType.Numeric:
                    if (HSSFDateUtil.IsCellDateFormatted(cell))
                    {
                        try
                        {
                            return cell.DateCellValue;
                        }
                        catch (NullReferenceException)
                        {
                            return DateTime.FromOADate(cell.NumericCellValue);
                        }
                    }
                    else 
                        return cell.NumericCellValue;
                case CellType.Boolean: return cell.BooleanCellValue;
                //case CellType.Formula: return cell.StringCellValue;
                default: return null;
            }
        }

        public static void SetCellObjectValue(this ICell cell, object value)
        {
            if (value == null)
            {
                cell.SetCellType(CellType.Blank);
                return;
            }
            Type type = value.GetType();
            if (Nullable.GetUnderlyingType(type) != null)
                type = Nullable.GetUnderlyingType(type);
            if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (type == typeof(DateTime))
            {
                cell.SetCellValue(Convert.ToDateTime(value));
            }
            else
            {
                cell.SetCellValue(Convert.ToString(value));
            }
        }

        public static ICell CreateCell(this IRow row, int column, Type type)
        {
            ICell cell = row.CreateCell(column);
            if (type == typeof(DateTime))
            {
                ICellStyle cellStyle = row.Sheet.Workbook.CreateCellStyle();
                ICreationHelper creationHelper = row.Sheet.Workbook.GetCreationHelper();
                cellStyle.DataFormat = creationHelper.CreateDataFormat().GetFormat("MM/dd/yyyy");
                cell.CellStyle = cellStyle;
            }
            return cell;
        }
    }
}
