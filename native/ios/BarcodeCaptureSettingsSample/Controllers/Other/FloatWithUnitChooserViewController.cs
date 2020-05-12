/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Views;
using Foundation;
using Scandit.DataCapture.Core.Common.Geometry;
using UIKit;

namespace BarcodeCaptureSettingsSample.Controllers.Other
{
    internal enum FloatWithUnitSectionKind
    {
        Value,
        Unit
    }
    
    public class FloatWithUnitChooserViewController : UITableViewController
    {
        private FloatWithUnit floatWithUnit;

        private readonly Action<FloatWithUnit> onChooseValue;

        public FloatWithUnitChooserViewController(string title, FloatWithUnit value,
            Action<FloatWithUnit> onChooseValue) : base(UITableViewStyle.Grouped)
        {
            this.floatWithUnit = value;
            this.onChooseValue = onChooseValue;
            this.Title = title;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
            this.TableView.RegisterNibForCellReuse(BasicCell.Nib, BasicCell.Key);
            this.TableView.RegisterNibForCellReuse(FloatInputCell.Nib, FloatInputCell.Key);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            this.onChooseValue(this.floatWithUnit);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Enum.GetValues(typeof(FloatWithUnitSectionKind)).Length;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            var values = Enum.GetValues(typeof(FloatWithUnitSectionKind)) as IList<FloatWithUnitSectionKind>;
            var sectionType = values[(int)section];
            return sectionType == FloatWithUnitSectionKind.Value ? 1 : Enum.GetValues(typeof(MeasureUnit)).Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var values = Enum.GetValues(typeof(FloatWithUnitSectionKind)) as IList<FloatWithUnitSectionKind>;
            var sectionType = values[indexPath.Section];
            if (sectionType == FloatWithUnitSectionKind.Value)
            {
                var cell = tableView.DequeueReusableCell(FloatInputCell.Key) as FloatInputCell;
                cell.TextLabel.Text = "Value";
                cell.Value = this.floatWithUnit.Value;
                cell.ValueChanged += (obj, args) =>
                {
                    this.floatWithUnit.Value = args.Value;
                };
                return cell;
            }
            else
            {
                var cell = tableView.DequeueReusableCell(BasicCell.Key);
                var unit = (Enum.GetValues(typeof(MeasureUnit)) as IList<MeasureUnit>)[indexPath.Row];
                cell.TextLabel.Text = unit.ToString();
                cell.DetailTextLabel.Text = string.Empty;
                cell.Accessory = unit == this.floatWithUnit.Unit ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                cell.DetailTextLabel.Text = string.Empty;
                return cell;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var values = Enum.GetValues(typeof(FloatWithUnitSectionKind)) as IList<FloatWithUnitSectionKind>;
            var sectionType = values[indexPath.Section];
            if (sectionType == FloatWithUnitSectionKind.Value)
            {
                var cell = tableView.CellAt(indexPath) as FloatInputCell;
                cell.StartEditing();
            }
            else
            {
                var unit = (Enum.GetValues(typeof(MeasureUnit)) as IList<MeasureUnit>)[indexPath.Row];
                this.floatWithUnit.Unit = unit;
                tableView.ReloadData();
            }
            tableView.DeselectRow(indexPath, true);
        }
    }
}
