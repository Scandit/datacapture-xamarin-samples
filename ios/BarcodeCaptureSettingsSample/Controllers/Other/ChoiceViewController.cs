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
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Views;
using Foundation;
using UIKit;

namespace BarcodeCaptureSettingsSample.Controllers.Other
{
    public class ChoiceViewController<TChoice> : UITableViewController where TChoice : IEnumeration
    {
        private readonly TChoice[] options;

        private readonly TChoice chosen;

        private readonly Action<TChoice> choiceHandler;

        private int ChosenIndex
        {
            get => Array.FindIndex(this.options, choice => choice.Equals(chosen));
        }

        public ChoiceViewController(string title, TChoice[] options,
            TChoice chosen, Action<TChoice> choiceHandler) : base(UITableViewStyle.Grouped)
        {
            this.options = options;
            this.chosen = chosen;
            this.choiceHandler = choiceHandler;
            this.Title = title;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.RegisterNibForCellReuse(BasicCell.Nib, BasicCell.Key);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.options.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(BasicCell.Key, indexPath);
            cell.TextLabel.Text = this.options[indexPath.Row].Name;
            cell.DetailTextLabel.Text = string.Empty;
            cell.Accessory = indexPath.Row == this.ChosenIndex ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            this.choiceHandler?.Invoke(this.options[indexPath.Row]);
            this.NavigationController.PopViewController(true);
        }
    }
}
