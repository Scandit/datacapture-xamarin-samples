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
using AndroidX.Lifecycle;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Radius
{
    public class LocationRadiusMeasureUnitViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public FloatWithUnit GetCurrentRadius()
        {
            ILocationSelection locationSelection = this.settingsManager.LocationSelection;
            
            if (locationSelection is RadiusLocationSelection)
            {
                return ((RadiusLocationSelection) locationSelection).Radius;
            }
            else
            {
                return new FloatWithUnit(0f, MeasureUnit.Dip);
            }
        }

        public async Task UpdateRadiusValueAsync(float value)
        {
            await this.settingsManager.SetLocationSelectionRadiusValueAsync(value);
        }

        public async Task UpdateRadiusMeasureUnitAsync(MeasureUnit measureUnit)
        {
            await this.settingsManager.SetLocationSelectionRadiusMeasureUnitAsync(measureUnit);
        }
    }
}