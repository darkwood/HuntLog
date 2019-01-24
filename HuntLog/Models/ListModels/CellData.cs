using System;
namespace HuntLog.Models.ListModels
{
    public class CellData
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
    public class TextCellData : CellData { }
    public class DateCellData : CellData { }
}
