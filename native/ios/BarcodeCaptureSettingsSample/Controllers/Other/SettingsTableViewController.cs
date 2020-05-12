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
using BarcodeCaptureSettingsSample.Controllers.Settings.BarcodeCapture.Symbology;
using BarcodeCaptureSettingsSample.DataSource;
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.Views;
using Foundation;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Core.Common.Geometry;
using UIKit;

namespace BarcodeCaptureSettingsSample.Controllers.Other
{
    public abstract class SettingsTableViewController: UITableViewController, IDataSourceListener
    {
        protected IDataSource dataSource;

        protected SettingsTableViewController(IntPtr handle) : base(handle) { }

        protected SettingsTableViewController(UITableViewStyle style) : base(style) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.RegisterCells();
            this.SetupDataSource();
        }

        protected abstract void SetupDataSource();

        private void RegisterCells()
        {
            this.TableView.RegisterNibForCellReuse(BasicCell.Nib, BasicCell.Key);
            this.TableView.RegisterNibForCellReuse(SwitchCell.Nib, SwitchCell.Key);
            this.TableView.RegisterNibForCellReuse(FloatInputCell.Nib, FloatInputCell.Key);
            this.TableView.RegisterNibForCellReuse(SliderCell.Nib, SliderCell.Key);
        }

        #region UITableViewDataSource

        public override nint NumberOfSections(UITableView tableView)
        {
            return this.dataSource.Sections.Length;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.dataSource.Sections[section].Rows.Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return this.dataSource.Sections[section].Title;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var row = this.dataSource.Sections[indexPath.Section].Rows[indexPath.Row];
            var cell = tableView.DequeueReusableCell(row.ReuseIdentifier);
            row.Bind(ref cell);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var row = this.dataSource.Sections[indexPath.Section].Rows[indexPath.Row];
            if (row.ReuseIdentifier.Equals(SliderCell.Key))
            {
                return SliderCell.DesignatedHeight;
            }
            return base.GetHeightForRow(tableView, indexPath);
        }

        #endregion

        #region UITableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var row = this.dataSource.Sections[indexPath.Section].Rows[indexPath.Row];
            row.CellSelected(row, indexPath);
            tableView.DeselectRow(indexPath, true);
        }

        #endregion

        #region IDataSourceListener

        public void OnDataChange()
        {
            this.TableView.ReloadData();
        }

        public void GetFloatWithUnit(string title, FloatWithUnit currentValue, Action<FloatWithUnit> actionHandler)
        {
            this.NavigationController.PushViewController(new FloatWithUnitChooserViewController(title, currentValue, actionHandler), true);
        }

        public void PresentSymbologySettings(SymbologySettings currentSettings, Action<SymbologySettings> actionHandler)
        {
            this.NavigationController.PushViewController(new SymbologySettingsTableViewController(currentSettings, actionHandler), true); ;
        }

        public void PresentChoice<TChoice>(string title, TChoice[] choices, TChoice chosen, Action<TChoice> actionHandler) where TChoice : IEnumeration
        {
            this.NavigationController.PushViewController(new ChoiceViewController<TChoice>(title, choices, chosen, actionHandler), true);
        }

        #endregion
    }
}
