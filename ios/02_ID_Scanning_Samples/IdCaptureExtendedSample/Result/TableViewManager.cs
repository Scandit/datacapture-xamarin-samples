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
using Foundation;
using IdCaptureExtendedSample.Result.CellProviders;
using IdCaptureExtendedSample.Result.Presenters;
using UIKit;

namespace IdCaptureExtendedSample.Result
{
    public class TableViewManager : UITableViewDataSource, IUITableViewDelegate
    {
        private readonly IResultPresenter presenter;

        public TableViewManager(IResultPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ICellProvider row = this.presenter.Rows[indexPath.Row];
            return row.DequeueAndConfigureCell(tableView);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return this.presenter.Rows.Count;
        }
    }
}
