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
using BarcodeCaptureSettingsSample.Views.EventArgs;
using Foundation;
using UIKit;

namespace BarcodeCaptureSettingsSample.Views
{
    public partial class SwitchCell : UITableViewCell
    {
        public static readonly string Key = "SwitchCell";

        public static readonly UINib Nib;

        static SwitchCell()
        {
            Nib = UINib.FromName("SwitchCell", NSBundle.MainBundle);
        }

        protected SwitchCell(IntPtr handle) : base(handle) { }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
            this.switchControl.ValueChanged += (obj, args) =>
            {
                this.ValueChanged?.Invoke(this, new SwitchCellChangeEventArgs(this.On));
            };
        }

        public override UILabel TextLabel => this.titleLabel;

        public EventHandler<SwitchCellChangeEventArgs> ValueChanged;

        public bool On
        {
            get => this.switchControl.On;
            set => this.switchControl.SetState(value, true);
        }
    }
}
