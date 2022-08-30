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
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes;
using Scandit.DataCapture.Core.Area;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Rectangular
{
    public class LocationRectangularViewModel : ViewModel
    {
        protected readonly SettingsManager settingsManager = SettingsManager.Instance;

        public LocationType CreateLocationType()
        {
            if (this.CurrentLocation is RectangularLocationSelection)
            {
                return LocationTypeRectangular.FromCurrentLocationSelectionAndSettings(this.CurrentLocation, this.settingsManager);
            }
            else if (this.CurrentLocation is RadiusLocationSelection)
            {
                return LocationTypeRadius.FromCurrentLocationSelection(this.CurrentLocation, this.settingsManager);
            }
            
            return LocationTypeNone.FromCurrentLocationSelection(this.CurrentLocation);
        }

        public ILocationSelection CurrentLocation => this.settingsManager.LocationSelection;

        public async Task UpdateLocationAsync()
        {
            await this.UpdateLocationAsync(this.CreateLocationType());
        }

        private async Task UpdateLocationAsync(LocationType locationType)
        {
            await this.settingsManager.SetLocationSelectionAsync(locationType.BuildLocationSelection());
        }
    }
}