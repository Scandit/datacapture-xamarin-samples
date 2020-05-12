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

using AndroidX.Lifecycle;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.Views.ScanAreas
{
    public class ScanAreaSettingsViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public bool ShowGuides
        {
            get { return this.settingsManager.ShouldShowScanAreaGuides; }
            set { this.settingsManager.ShouldShowScanAreaGuides = value; }
        }

        public FloatWithUnit TopMargin
        {
            get { return this.settingsManager.ScanAreaMargins.Top; }
            set { this.settingsManager.SetScanAreaTopMargin(value); }
        }

        public FloatWithUnit RightMargin
        {
            get { return this.settingsManager.ScanAreaMargins.Right; }
            set { this.settingsManager.SetScanAreaRightMargin(value); }
        }

        public FloatWithUnit BottomMargin
        {
            get { return this.settingsManager.ScanAreaMargins.Bottom; }
            set { this.settingsManager.SetScanAreaBottomMargin(value); }
        }

        public FloatWithUnit LeftMargin
        {
            get { return this.settingsManager.ScanAreaMargins.Left; }
            set { this.settingsManager.SetScanAreaLeftMargin(value); }
        }
    }
}