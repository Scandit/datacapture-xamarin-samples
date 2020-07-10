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
    public class ChoiceRow<TEnumeration> : Row where TEnumeration : Enumeration
    {
        private ChoiceRow(string title, Func<TEnumeration> getter, 
            Action<TEnumeration> setter, Action<Tuple<ChoiceRow<TEnumeration>, NSIndexPath>> onSelect) : base(title)
        {
            this.Getter = getter;
            this.Setter = setter;
            this.OnSelect = onSelect;
        }

        public override string ReuseIdentifier => BasicCell.Key;

        public override string DetailText => this.Getter().GetValueString();

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.DisclosureIndicator;
        
        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            var tuple = new Tuple<ChoiceRow<TEnumeration>, NSIndexPath>(this, indexPath);
            this.OnSelect?.Invoke(tuple);
        }

        public Func<TEnumeration> Getter { get; }
        
        public Action<TEnumeration> Setter { get; }
        
        public Action<Tuple<ChoiceRow<TEnumeration>, NSIndexPath>> OnSelect { get; }

        public static ChoiceRow<TEnumeration> Create(string title, TEnumeration[] options,
            Func<TEnumeration> getter, Action<TEnumeration> setter, IDataSourceListener dataSourceListener)
        {
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            dataSourceListener.RequireNotNull(nameof(dataSourceListener));
            return new ChoiceRow<TEnumeration>(title, getter, value =>
            {
                setter(value);
                dataSourceListener.OnDataChange();
            }, tuple =>
            {
                var row = tuple.Item1;
                dataSourceListener.PresentChoice(
                    row.Title,
                    options,
                    getter(),
                    value => row.Setter(value)
                );
            });
        }
    }
}
