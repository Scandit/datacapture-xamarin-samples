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
    public class SliderRow : Row
    {
        private readonly Func<nfloat> getter;

        private readonly Action<nfloat> setter;

        private readonly nfloat minimumValue;

        private readonly nfloat maximumValue;

        private readonly int decimalPlaces;

        private SliderRow(string title, Func<nfloat> getter, Action<nfloat> setter,
            nfloat minimumValue, nfloat maximumValue, int decimalPlaces = 2) : base(title)
        {
            this.getter = getter;
            this.setter = setter;
            this.minimumValue = minimumValue;
            this.maximumValue = maximumValue;
            this.decimalPlaces = decimalPlaces;
        }

        public override string ReuseIdentifier => SliderCell.Key;

        public override string DetailText => NumberFormatter.Instance.FormatNFloat(this.getter(), this.decimalPlaces);

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.None;

        public override void Bind(ref UITableViewCell cell)
        {
            base.Bind(ref cell);
            if (cell is SliderCell sliderCell)
            {
                sliderCell.MinimumValue = this.minimumValue;
                sliderCell.MaximumValue = this.maximumValue;
                sliderCell.MaximumNumberOfDecimals = this.decimalPlaces;
                sliderCell.Value = this.getter();
                sliderCell.ValueChanged += (obj, args) =>
                {
                    this.setter(args.Value);
                };
            }
        }

        public override void CellSelected(Row row, NSIndexPath indexPath) { }

        public static SliderRow Create(string title, Func<nfloat> getter, Action<nfloat> setter)
        {
            return Create(title, getter, setter, 1, 20, 1);
        }

        public static SliderRow Create(string title, Func<nfloat> getter, Action<nfloat> setter,
            nfloat minimumValue, nfloat maximumValue, int decimalPlaces = 2)
        {
            return new SliderRow(title, getter, setter, minimumValue, maximumValue, decimalPlaces);
        }
    }
}
