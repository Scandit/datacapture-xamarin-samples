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

using System.Threading.Tasks;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Rectangular
{ 
    public class LocationRectangularWidthMeasureUnitViewModel : LocationRectangularViewModel
    {
        public FloatWithUnit CurrentWidth => this.settingsManager.LocationSelectionRectangularWidth;

        public async Task UpdateWidthValueAsync(float newValue)
        {
            this.settingsManager.LocationSelectionRectangularWidth = new FloatWithUnit(newValue, this.CurrentWidth.Unit);
            await this.UpdateLocationAsync();
        }

        public async Task UpdateWidthMeasureUnitAsync(MeasureUnit measureUnit)
        {
            this.settingsManager.LocationSelectionRectangularWidth = new FloatWithUnit(this.CurrentWidth.Value, measureUnit);
            await this.UpdateLocationAsync();
        }
    }
}