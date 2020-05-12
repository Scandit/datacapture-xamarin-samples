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
using BarcodeCaptureSettingsSample.Views.EventArgs;
using Foundation;
using UIKit;

namespace BarcodeCaptureSettingsSample.Views
{
    public partial class SliderCell : UITableViewCell
    {
        public static readonly UINib Nib;

        public static readonly string Key = "SliderCell";

        internal static readonly nfloat DesignatedHeight = 83.0f;

        static SliderCell()
        {
            Nib = UINib.FromName("SliderCell", NSBundle.MainBundle);
        }

        protected SliderCell(IntPtr handle) : base(handle) { }

        public EventHandler<SliderCellChangedEventArgs> ValueChanged { get; set; }

        public int MaximumNumberOfDecimals { get; set; } = 2;

        public nfloat MinimumValue
        {
            get => this.slider.MinValue;
            set => this.slider.MinValue = (float)value;
        }

        public nfloat MaximumValue
        {
            get => this.slider.MaxValue;
            set => this.slider.MaxValue = (float)value;
        }

        public nfloat Value
        {
            get => (nfloat)Math.Round(this.slider.Value, this.MaximumNumberOfDecimals);
            set => this.slider.Value = (float)value;
        }

        public override UILabel TextLabel => this.titleLabel;

        public override UILabel DetailTextLabel => this.valueLabel;

        partial void sliderChanged(UIKit.UISlider sender)
        {
            this.DetailTextLabel.Text = NumberFormatter.Instance.FormatNFloat(this.Value, this.MaximumNumberOfDecimals);
            this.ValueChanged?.Invoke(this, new SliderCellChangedEventArgs(this.Value));
        }
    }
}
