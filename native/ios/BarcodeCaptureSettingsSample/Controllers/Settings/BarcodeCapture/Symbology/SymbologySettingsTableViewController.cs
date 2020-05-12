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
using BarcodeCaptureSettingsSample.Controllers.Other;
using BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture.Symbology;
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Barcode.Capture;
using UIKit;

namespace BarcodeCaptureSettingsSample.Controllers.Settings.BarcodeCapture.Symbology
{
    public class SymbologySettingsTableViewController : SettingsTableViewController
    {
        protected SymbologySettingsTableViewController(IntPtr handle) : base(handle) { }

        public SymbologySettingsTableViewController(SymbologySettings symbologySettings, Action<SymbologySettings> onChange) : base(UITableViewStyle.Grouped)
        {
            this.SymbologySettings = symbologySettings;
            this.OnChange = onChange;
            this.Title = symbologySettings.Symbology.ReadableName();
        }

        public SymbologySettings SymbologySettings { get; }

        public Action<SymbologySettings> OnChange { get; }

        protected override void SetupDataSource() => this.dataSource = new SymbologySettingsDataSource(this, this.SymbologySettings);

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            this.OnChange(this.SymbologySettings);
        }
    }
}
