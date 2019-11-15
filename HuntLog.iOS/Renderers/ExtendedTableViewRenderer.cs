using System;
using HuntLog.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedTableView), typeof(HuntLog.iOS.Renderers.ExtendedTableViewRenderer))]
namespace HuntLog.iOS.Renderers
{
    public class ExtendedTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var tableView = Control as UITableView;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (ExtendedTableView)Element;
            if(Control != null)
            {
                Control.Source = new ExtendedUnEvenTableViewModelRenderer(view);
            }

        }

        public class ExtendedUnEvenTableViewModelRenderer : UnEvenTableViewModelRenderer
        {
            public ExtendedUnEvenTableViewModelRenderer(TableView model) : base(model)
            {
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if(section == 0)
                {
                    return 0.0f;
                }
                return base.GetHeightForHeader(tableView, section);
            }

            public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                var cell = base.GetCell(tableView, indexPath);
                return cell;
            }
        }
    }
}