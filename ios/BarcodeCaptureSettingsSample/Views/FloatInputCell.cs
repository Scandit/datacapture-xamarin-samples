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
    public partial class FloatInputCell : UITableViewCell
    {
        public static readonly string Key = "FloatInputCell";

        public static readonly UINib Nib;

        static FloatInputCell()
        {
            Nib = UINib.FromName("FloatInputCell", NSBundle.MainBundle);
        }

        protected FloatInputCell(IntPtr handle) : base(handle) { }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.textField.TextColor = UITableViewCellExtensions.DefaultDetailTextColor;
            this.textField.Font = UITableViewCellExtensions.DefaultDetailTextFont;
            this.textField.EditingDidEnd += (obj, args) =>
            {
                this.textField.Text = NumberFormatter.Instance.FormatNFloat(this.Value);
            };
            this.textField.EditingChanged += (obj, args) =>
            {
                this.ValueChanged?.Invoke(this, new FloatInputCellChangeEventArgs(this.Value));
            };
        }

        public EventHandler<FloatInputCellChangeEventArgs> ValueChanged;

        public nfloat Value
        {
            get => string.IsNullOrEmpty(this.textField.Text) ? .0f : NumberFormatter.Instance.ParseNFloat(this.textField.Text);
            set => this.textField.Text = NumberFormatter.Instance.FormatNFloat(value);
        }

        public void StartEditing()
        {
            this.textField.BecomeFirstResponder();
        }

        public override UILabel TextLabel => this.titleLabel;
    }
}
