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

using IdCaptureExtendedSample.Result.Presenters;
using Scandit.DataCapture.ID.Data;
using UIKit;

namespace IdCaptureExtendedSample.Result
{
    public partial class ResultViewController : UIViewController
    {
        private readonly ResultPresenterFactory factory = new ResultPresenterFactory();
        private readonly TableViewManager tableViewManager;

        private UITableView tableView;

        public CapturedId CapturedId { get; set; }

        public ResultViewController(CapturedId capturedId)
        {
            IResultPresenter presenter = this.factory.Create(capturedId);
            this.tableViewManager = new TableViewManager(presenter);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.tableView = new UITableView(this.View.Bounds, UITableViewStyle.Plain);

            this.Title = "Scan Result";
            this.tableView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.View.AddSubview(this.tableView);

            this.tableView.DataSource = this.tableViewManager;
            this.tableView.Delegate = this.tableViewManager;
        }
    }
}
