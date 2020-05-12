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

namespace BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Right
{
    public class ScanAreaRightMarginViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public FloatWithUnit Margin
        {
            get { return this.settingsManager.ScanAreaMargins.Right; }
        }

        public void SetMarginValue(float rightMargin)
        {
            this.settingsManager.SetScanAreaRightMargin(new FloatWithUnit(rightMargin, this.Margin.Unit));
        }

        public void SetMarginUnit(MeasureUnit rightMarginUnit)
        {
            this.settingsManager.SetScanAreaRightMargin(new FloatWithUnit(this.Margin.Value, rightMarginUnit));
        }
    }
}
