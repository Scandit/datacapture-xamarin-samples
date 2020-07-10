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
using Scandit.DataCapture.Barcode.Capture;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Other.Rows
{
    public class SymbologyRow : Row
    {
        private SymbologyRow(string title, Func<SymbologySettings> getter,
            Action<SymbologySettings> setter, Action<Tuple<SymbologyRow,NSIndexPath>> onSelect)
            : base(title)
        {
            this.Getter = getter;
            this.Setter = setter;
            this.OnSelect = onSelect;
        }

        public Func<SymbologySettings> Getter { get; }

        public Action<SymbologySettings> Setter { get; }

        public Action<Tuple<SymbologyRow, NSIndexPath>> OnSelect;

        public override string ReuseIdentifier => BasicCell.Key;

        public override string DetailText => this.Getter().Enabled ? "On" : "Off";

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.DisclosureIndicator;

        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            this.OnSelect(new Tuple<SymbologyRow, NSIndexPath>(this, indexPath));
        }

        public static SymbologyRow Create(Func<SymbologySettings> getter,
            Action<SymbologySettings> setter, IDataSourceListener dataSourceListener)
        {
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            return new SymbologyRow(
                getter().Symbology.ReadableName(),
                getter,
                settings =>
                {
                    setter(settings);
                    dataSourceListener.OnDataChange();
                },
                tuple =>
                {
                    var row = tuple.Item1;
                    dataSourceListener.PresentSymbologySettings(
                        row.Getter(),
                        settings => row.Setter(settings)
                    );
                }
            );
        }
    }
}
