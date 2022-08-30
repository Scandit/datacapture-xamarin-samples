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
using BarcodeCaptureViewsSample.Models;
using Foundation;
using UIKit;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    public class SplitViewTableController : UITableViewController
    {
        private const string cellIdentifier = "splitViewCellReuseIdentifier";
        private List<ScanResult> scanResults = new List<ScanResult>();

        public SplitViewTableController() : base(UITableViewStyle.Plain)
        {
            this.TableView.RegisterClassForCellReuse(typeof(UITableViewCell), cellIdentifier);
        }

        public void Add(ScanResult scanResult)
        {
            this.scanResults.Add(scanResult);
            this.TableView.ReloadData();
        }

        public void Clear()
        {
            this.scanResults.Clear();
            this.TableView.ReloadData();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.BackgroundColor = UIColor.White;
            this.TableView.AllowsSelection = false;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.TableView.ReloadData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override nint NumberOfSections(UITableView tableView) => 1;

        public override nint RowsInSection(UITableView tableView, nint section) => this.scanResults.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var scanResult = this.scanResults[indexPath.Row];
            UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
            cell.ContentView.BackgroundColor = UIColor.White;
            cell.TextLabel.Text = scanResult.Data;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.DetailTextLabel.Text = scanResult.Symbology;
            cell.DetailTextLabel.TextColor = UIColor.FromRGBA(0.22352941179999999f, 0.75686274509999996f, 0.80000000000000004f, 1);

            return cell;
        }
    }
}
