using System;
using HuntLog.Models.ListModels;
using Xamarin.Forms;

namespace HuntLog.Helpers
{
    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextCellTemplate { get; set; }

        public DataTemplate DateCellTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TextCellData)
                return TextCellTemplate;

            if (item is DateCellData)
                return DateCellTemplate;

            return null;
        }
    }
}
