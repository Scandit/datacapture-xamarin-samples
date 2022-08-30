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
    public class SwitchRow : Row
    {
        private SwitchRow(string title, Func<bool> getter, Action<bool> setter) : base(title)
        {
            this.Getter = getter;
            this.Setter = setter;
        }

        public Func<bool> Getter { get; }
        
        public Action<bool> Setter { get; }

        public override string ReuseIdentifier => SwitchCell.Key;
        
        public override string DetailText => string.Empty;
        
        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.None;
        
        public override void CellSelected(Row row, NSIndexPath indexPath) { }

        public override void Bind(ref UITableViewCell cell)
        {
            if (cell is SwitchCell switchCell)
            {
                switchCell.TextLabel.Text = this.Title;
                switchCell.On = this.Getter();
                switchCell.ValueChanged += (sender, args) => this.Setter(args.Value);
                cell = switchCell;
            }
        }

        public static SwitchRow Create(string title, Func<bool> getter, Action<bool> setter)
        {
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            return new SwitchRow(title, getter, setter);
        }
    }
}
