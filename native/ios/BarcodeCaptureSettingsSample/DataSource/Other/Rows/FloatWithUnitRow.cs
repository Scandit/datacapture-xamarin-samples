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
using Scandit.DataCapture.Core.Common.Geometry;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Other.Rows
{
    public class FloatWithUnitRow : Row
    {
        
        private readonly Func<string> detailTextGetter;

        public FloatWithUnitRow(string title, Func<string> detailTextGetter, Func<FloatWithUnit> getter, 
            Action<FloatWithUnit> setter, Action<Tuple<FloatWithUnitRow, NSIndexPath>> onSelect) : base(title)
        {
            this.detailTextGetter = detailTextGetter;
            this.Getter = getter;
            this.Setter = setter;
            this.OnSelect = onSelect;
        }

        public Action<Tuple<FloatWithUnitRow, NSIndexPath>> OnSelect { get; }

        public IDataSourceListener DataSourceListener { get; private set; }

        public Action<FloatWithUnit> Setter { get; }

        public Func<FloatWithUnit> Getter { get; }

        public override string ReuseIdentifier => BasicCell.Key;

        public override string DetailText => this.detailTextGetter?.Invoke();

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.DisclosureIndicator;
        
        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            var tuple = new Tuple<FloatWithUnitRow, NSIndexPath>(this, indexPath);
            this.OnSelect?.Invoke(tuple);
        }

        public static FloatWithUnitRow Create(
            string title, Func<FloatWithUnit> getter, Action<FloatWithUnit> setter,
            IDataSourceListener dataSourceListener)
        {
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            dataSourceListener.RequireNotNull(nameof(dataSourceListener));
            return new FloatWithUnitRow(
                title,
                () =>
                {
                    var floatWithUnit = getter();
                    return $"{NumberFormatter.Instance.FormatNFloat(floatWithUnit.Value)} ({floatWithUnit.Unit})";
                },
                getter,
                newValue =>
                {
                    setter(newValue);
                    dataSourceListener.OnDataChange();
                },
                rowIndexPathPair =>
                {
                    var row = rowIndexPathPair.Item1;
                    dataSourceListener.GetFloatWithUnit(row.Title, getter(), newValue => row.Setter(newValue));
                });
        }
    }
}
