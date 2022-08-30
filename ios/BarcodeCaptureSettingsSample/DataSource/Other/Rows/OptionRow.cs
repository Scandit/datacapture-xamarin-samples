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
    public class OptionRow<TValue> : Row
    {
        public Func<TValue> Getter { get; }

        public Action<TValue> Setter { get; }

        public Action<Tuple<OptionRow<TValue>, NSIndexPath>> OnSelect { get; }

        protected OptionRow(string title, Func<TValue> getter,
            Action<TValue> setter, Action<Tuple<OptionRow<TValue>, NSIndexPath>> onSelect)
            : base(title)
        {
            this.Getter = getter;
            this.Setter = setter;
            this.OnSelect = onSelect;
        }

        public override string ReuseIdentifier => BasicCell.Key;

        public override string DetailText => string.Empty;

        public override UITableViewCellAccessory Accessory
        {
            get
            {
                var value = this.Getter();
                if (value is bool boolValue)
                {
                    return boolValue ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                }

                return UITableViewCellAccessory.None;
            }
        }
        
        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            var tuple = new Tuple<OptionRow<TValue>, NSIndexPath>(this, indexPath);
            this.OnSelect?.Invoke(tuple);
        }

        public static OptionRow<TValue> Create(string title, Func<TValue> getter,
            Action<Tuple<OptionRow<TValue>, NSIndexPath>> onSelect, IDataSourceListener dataSourceListener)
        {
            getter.RequireNotNull(nameof(getter));
            onSelect.RequireNotNull(nameof(onSelect));
            dataSourceListener.RequireNotNull(nameof(dataSourceListener));
            return new OptionRow<TValue>(title, getter, null, tuple =>
            {
                onSelect(tuple);
                dataSourceListener.OnDataChange();
            });
        }
    }
}
