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

namespace BarcodeCaptureSettingsSample.Settings.Views.Logo.OffsetX
{
    public class OffsetXViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public FloatWithUnit OffsetX => this.settingsManager.AnchorXOffset;

        public void OnValueChanged(float value)
        {
            this.settingsManager.AnchorXOffset = new FloatWithUnit(value, this.settingsManager.AnchorXOffset.Unit);
        }

        public void OnMeasureChanged(MeasureUnit measureUnit)
        {
            this.settingsManager.AnchorXOffset = new FloatWithUnit(this.settingsManager.AnchorXOffset.Value, measureUnit);
        }
    }
}