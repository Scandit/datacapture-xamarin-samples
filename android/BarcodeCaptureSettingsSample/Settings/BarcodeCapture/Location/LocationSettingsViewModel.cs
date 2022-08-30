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

using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Area;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location
{ 
    public class LocationSettingsViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public IList<LocationType> AllowedLocationTypes => new List<LocationType>
        {
            LocationTypeNone.FromCurrentLocationSelection(this.CurrentLocationSelection),
            LocationTypeRadius.FromCurrentLocationSelection(this.CurrentLocationSelection, this.settingsManager),
            LocationTypeRectangular.FromCurrentLocationSelectionAndSettings(this.CurrentLocationSelection, this.settingsManager)
        };

        public ILocationSelection CurrentLocationSelection => this.settingsManager.LocationSelection;

        public LocationType GetCurrentLocationType()
        {
            switch (this.CurrentLocationSelection)
            {
                case RadiusLocationSelection locationSelection:
                    return LocationTypeRadius.FromCurrentLocationSelection(locationSelection, this.settingsManager); ;
                case RectangularLocationSelection locationSelection:
                    return LocationTypeRectangular.FromCurrentLocationSelectionAndSettings(locationSelection, this.settingsManager);
                default:
                    return LocationTypeNone.FromCurrentLocationSelection(this.CurrentLocationSelection);
            }
        }

        public async Task SetLocationTypeAsync(LocationType locationType)
        {
            switch (locationType)
            {
                case LocationTypeRectangular rectangular:
                    {
                        this.settingsManager.LocationSelectionRectangularWidth = rectangular.Width;
                        this.settingsManager.LocationSelectionRectangularHeight = rectangular.Height;
                        this.settingsManager.LocationSelectionRectangularHeightAspect = rectangular.HeightAspectRatio;
                        this.settingsManager.LocationSelectionRectangularWidthAspect = rectangular.WidthAspectRatio;
                        this.settingsManager.LocationSelectionRectangularSizeSpecification = rectangular.SizeSpecification;
                    }
                    break;
            }

            await this.settingsManager.SetLocationSelectionAsync(locationType.BuildLocationSelection());
        }

        public async Task SetRectangularLocationSizeSpecAsync(SizeSpecification spec)
        {
            LocationType locationType = this.GetCurrentLocationType();

            if (locationType is LocationTypeRectangular)
            {
                ((LocationTypeRectangular) locationType).SizeSpecification = spec;
                await this.SetLocationTypeAsync(locationType);
            }
        }

        public async Task SetRectangularLocationHeightAspectAsync(float heightAspect)
        {
            LocationType locationType = this.GetCurrentLocationType();

            if (locationType is LocationTypeRectangular)
            {
                ((LocationTypeRectangular)locationType).HeightAspectRatio = heightAspect;
                await this.SetLocationTypeAsync(locationType);
            }
        }

        public async Task SetRectangularLocationWidthAspectAsync(float widthAspect)
        {
            LocationType locationType = this.GetCurrentLocationType();

            if (locationType is LocationTypeRectangular)
            {
                ((LocationTypeRectangular)locationType).WidthAspectRatio = widthAspect;
                await this.SetLocationTypeAsync(locationType);
            }
        }
    }
}