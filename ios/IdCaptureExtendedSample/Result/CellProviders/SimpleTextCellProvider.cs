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
using UIKit;

namespace IdCaptureExtendedSample.Result.CellProviders
{
    public class SimpleTextCellProvider : ICellProvider
    {
        private const string cellIdentifier = "SimpleTextCell";

        private readonly string value;
        private readonly string title;

        public SimpleTextCellProvider(string value, string title)
        {
            this.title = title;
            this.value = value ?? string.Empty;
        }

        public nfloat Height => UITableView.AutomaticDimension;

        public UITableViewCell DequeueAndConfigureCell(UITableView tableView)
        {
            UITableViewCell cell = tableView?.DequeueReusableCell(cellIdentifier);

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
            }

            cell.TextLabel.Lines = 0;
            cell.TextLabel.Text = value;
            cell.DetailTextLabel.Text = title;

            return cell;
        }
    }
}
