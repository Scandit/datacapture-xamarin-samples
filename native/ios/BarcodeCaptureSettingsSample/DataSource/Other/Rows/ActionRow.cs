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
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Views;
using Foundation;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Other.Rows
{
    public class ActionRow : Row
    {
        private ActionRow(string title, Action<Tuple<ActionRow, NSIndexPath>> onSelect) : base(title)
        {
            this.OnSelect = onSelect;
        }

        public Action<Tuple<ActionRow, NSIndexPath>> OnSelect { get; }

        public override string ReuseIdentifier => BasicCell.Key;

        public override string DetailText => string.Empty;

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.None;

        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            this.OnSelect(new Tuple<ActionRow, NSIndexPath>(this, indexPath));
        }

        public static ActionRow Create(string title, Action<Tuple<ActionRow,NSIndexPath>> onSelect)
        {
            onSelect.RequireNotNull(nameof(onSelect));
            return new ActionRow(title, onSelect);
        }
    }
}
