using System;
using HuntLog.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ExtendedViewCellRenderer))]
namespace HuntLog.iOS.Renderers
{
    public class ExtendedViewCellRenderer : ViewCellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            if(cell != null)
            {

                cell.BackgroundColor = UIColor.Clear;

                switch (item.StyleId)
                {

                    case "checkmark":
                        cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
                        break;
                    case "detail-button":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
                        break;
                    case "detail-disclosure-button":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
                        break;
                    case "disclosure":
                        cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
                        break;
                    case "none":
                    default:
                        cell.Accessory = UIKit.UITableViewCellAccessory.None;
                        break;
                }
            }
            
            return cell;
        }

    }
}

