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

using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderRectangleWidthViewModel : ViewfinderTypeViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public void ValueChanged(float newValue)
        {
            MeasureUnit currentMeasureUnit = this.settingsManager.RectangularViewfinderWidth.Unit;
            this.settingsManager.RectangularViewfinderWidth = new FloatWithUnit(newValue, currentMeasureUnit);
            this.UpdateViewfinder();
        }

        public void MeasureChanged(MeasureUnit measureUnit)
        {
            float currentValue = this.settingsManager.RectangularViewfinderWidth.Value;
            this.settingsManager.RectangularViewfinderWidth = new FloatWithUnit(currentValue, measureUnit);
            this.UpdateViewfinder();
        }

        public FloatWithUnit CurrentWidth => this.settingsManager.RectangularViewfinderWidth;
    }
}
