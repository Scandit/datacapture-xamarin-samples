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
using ListBuildingSample.Models;
using UIKit;

namespace ListBuildingSample.Views
{
    public class TableSource : UITableViewSource
    {
        private readonly IEnumerable<ListItem> items;

        public TableSource(IEnumerable<ListItem> items)
        {
            this.items = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return this.items.Count();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ListItem item = this.items.ElementAt(indexPath.Row);
            ItemTableViewCell cell = tableView.DequeueReusableCell(ItemTableViewCell.Key, indexPath) as ItemTableViewCell;
            cell.Configure(item);
            return cell;
        }
    }
}
