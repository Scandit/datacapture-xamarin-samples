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
using System.Linq;
using Foundation;
using MatrixScanCountSimpleSample.Data;
using UIKit;

namespace MatrixScanCountSimpleSample
{
    public class TableSource : UITableViewSource
    {
        private readonly IList<ScanItem> multipleScanItems;
        private readonly IList<ScanItem> singleScanItems;

        public TableSource(IList<ScanItem> scanResults)
        {
            this.multipleScanItems = scanResults.Where(i => i.Quantity > 1).ToList();
            this.singleScanItems = scanResults.Where(i => i.Quantity == 1).ToList();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return this.GetItemCountForSection(section);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(ListCell.ReuseIdentifier, indexPath) as ListCell;

            if (cell == null)
            {
                return new UITableViewCell();
            }

            var item = this.GetScanListForSection(indexPath.Section)[indexPath.Row];
            cell.Configure(item, index: indexPath.Row);

            return cell;
        }

        private int GetItemCountForSection(nint section)
        {
            return this.GetScanListForSection(section).Count;
        }

        private IList<ScanItem> GetScanListForSection(nint section)
        {
            switch (section)
            {
                case 0:
                    return this.multipleScanItems;
                case 1:
                    return this.singleScanItems;
                default:
                    return new List<ScanItem>();
            }
        }
    }
}
