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
using Foundation;
using UIKit;

namespace MatrixScanSimpleSample
{
    public partial class ResultsViewController : UIViewController, IUITableViewDataSource
    {
        private const string CellIdentifier = "TableCell";

        public ResultsViewController(IntPtr handle) : base(handle)
        {
        }

        public List<ScanResult> Items { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.tableView.DataSource = this;
            this.Add(this.tableView);
            this.Add(this.scanAgainButton);
        }

        #region IUITableViewDataSource

        public nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = this.tableView.DequeueReusableCell(CellIdentifier);

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
            }

            if ((this.Items != null) && (this.Items.Count > indexPath.Row))
            {
                var scanResult = this.Items[indexPath.Row];
                cell.TextLabel.Text = scanResult.Data;
                cell.DetailTextLabel.Text = scanResult.Symbology;
            }

            return cell;
        }

        public nint RowsInSection(UITableView tableView, nint section) => this.Items != null ? this.Items.Count : 0;

        #endregion

        partial void ScanAgainButton_TouchUpInside(UIButton sender)
        {
            this.DismissViewController(true, null);
        }
    }
}

