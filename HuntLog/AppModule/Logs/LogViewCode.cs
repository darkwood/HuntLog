using System;
using System.Collections.Generic;
using System.Linq;
using HuntLog.Cells;
using HuntLog.Controls;
using HuntLog.InputViews;
using Xamarin.Forms;

namespace HuntLog.AppModule.Logs
{
    public class LogViewCode : ContentPage
    {
        private readonly LogViewModel _viewModel;

        public bool IsLoaded { get; private set; }

        public LogViewCode(LogViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeToolbarItems();
            var spin = new ActivityIndicator { IsRunning = true };
            Content = spin;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
            InitializeContent();
        }

        private void InitializeContent()
        {
            var tableView = new ExtendedTableView { HasUnevenRows = true, Intent = TableIntent.Form };

            var section1 = new TableSection();
            var section2 = new TableSection("Egendefinerte felter");
            var section3 = new TableSection();
            section1.Add(CreateImageHeaderCell("150", "ImageSource", "ImageAction"));
            section1.Add(CreateMapCell("Posisjon", "Position", "MapAction"));
            section1.Add(CreatePicker("Art", "SpeciesPickers", PickerMode.Single, "Velg dine arter i oppsettfanen"));
            section1.Add(CreatePicker("Antall sett", "ObservedPickers", PickerMode.Numeric));
            section1.Add(CreatePicker("Antall skudd", "ShotsPickers", PickerMode.Numeric));
            section1.Add(CreatePicker("Antall treff", "HitsPickers", PickerMode.Numeric));
            section1.Add(CreatePicker("Jeger", "HuntersPickers", PickerMode.Single));
            section1.Add(CreatePicker("Hunder", "DogsPickers", PickerMode.Single));
            section1.Add(CreateTextCell("Dato", "Date", "TimeCommand"));

            tableView.Root.Add(section1);

            if (_viewModel.CustomFields.Any(c => c.Selected))
            {
                foreach(var field in _viewModel.CustomFields.Where(c => c.Selected))
                {
                    section2.Add(CreateTextCell(field.Name, field.ID));
                }

                tableView.Root.Add(section2);
            }

            if (!_viewModel.IsNew)
            {
                section3.Add(CreateDeleteCell("Slett", nameof(_viewModel.DeleteCommand)));
                tableView.Root.Add(section3);
            }

            Content = tableView;
        }

        //TOD: Create CellFactory and move methods
        private Cell CreateMapCell(string text, string positionBinding, string commandBinding)
        {
            var mapCell = new MapCell();
            mapCell.Text = text;
            mapCell.SetBinding(MapCell.PositionProperty, positionBinding);
            mapCell.SetBinding(MapCell.CellActionProperty, commandBinding);
            return mapCell;
        }

        private Cell CreateDeleteCell(string text, string commandBinding)
        {
            var cell = new ViewCell();
            var button = new Button
            {
                Text = text,
                Margin = 15,
                StyleClass = new List<string> { "DangerButton" }
            };
            button.SetBinding(Button.CommandProperty, commandBinding);
            cell.View = button;
            return cell;
        }

        private static ImageHeaderCell CreateImageHeaderCell(string height, string sourceBinding, string actionBinding)
        {
            var cell = new ImageHeaderCell();
            cell.HeightRequest = height;
            cell.SetBinding(ImageHeaderCell.SourceProperty, sourceBinding);
            cell.SetBinding(BaseCell.CellActionProperty, actionBinding);
            return cell;
        }

        private static PickerCell CreatePicker(string text, string itemsBinding, PickerMode mode, string subtext = "", string commandBinding = null)
        {
            var cell = new PickerCell();
            cell.Text = text;
            cell.SetBinding(PickerCell.PickerItemsProperty, itemsBinding);
            cell.Mode = mode;
            cell.SubText = subtext;
            if(commandBinding != null)
            {
                cell.SetBinding(PickerCell.EmptyCommandProperty, commandBinding);
            }

            return cell;
        }

        private static ExtendedTextCell CreateTextCell(string text, string text2Binding, string commandBinding = null)
        {
            var cell = new ExtendedTextCell();
            cell.Text = text;
            cell.SetBinding(ExtendedTextCell.Text2Property, text2Binding);
            if(commandBinding != null)
            {
                cell.SetBinding(ExtendedTextCell.CommandProperty, commandBinding);
            }

            return cell;
        }

        private void InitializeToolbarItems()
        {
            var cancel = new ToolbarItem { Priority = 0, Text = "Avbryt" };
            cancel.SetBinding(MenuItem.CommandProperty, "CancelCommand");

            var save = new ToolbarItem { Priority = 1, Text = "Lagre" };
            save.SetBinding(MenuItem.CommandProperty, "SaveCommand");

            ToolbarItems.Add(cancel);
            ToolbarItems.Add(save);
        }

    }
}
