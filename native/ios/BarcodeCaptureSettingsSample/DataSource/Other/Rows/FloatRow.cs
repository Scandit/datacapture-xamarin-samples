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
    public class FloatRow : Row
    {
        private Action onSelect;

        private FloatRow(string title, Func<string> detailTextGetter, Func<nfloat> getter, Action<nfloat> setter) : base(title)
        {
            this.DetailTextGetter = detailTextGetter;
            this.Getter = getter;
            this.Setter = setter;
        }

        public override string ReuseIdentifier => FloatInputCell.Key;

        public override string DetailText => string.Empty;

        public override UITableViewCellAccessory Accessory => UITableViewCellAccessory.None;

        public Func<string> DetailTextGetter { get; }

        public Func<nfloat> Getter { get; }

        public Action<nfloat> Setter { get; }

        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            this.onSelect?.Invoke();
        }

        public override void Bind(ref UITableViewCell cell)
        {
            if (cell is FloatInputCell floatCell)
            {
                floatCell.TextLabel.Text = this.Title;
                floatCell.Value = this.Getter();
                this.onSelect = () => floatCell.StartEditing();
                floatCell.ValueChanged += (obj, args) =>
                {
                    this.Setter(args.Value);
                };
                cell = floatCell;
            }
        }

        public static FloatRow Create(string title,
                                      Func<string> detailTextGetter,
                                      Func<nfloat> getter,
                                      Action<nfloat> setter)
        {
            detailTextGetter.RequireNotNull(nameof(detailTextGetter));
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            return new FloatRow(title, detailTextGetter, getter, setter);
        }
    }
}
