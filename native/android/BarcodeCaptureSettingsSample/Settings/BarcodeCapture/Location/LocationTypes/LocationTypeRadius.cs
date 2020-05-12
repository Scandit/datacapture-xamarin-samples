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

using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes
{
    public class LocationTypeRadius : LocationType
    {
        public MeasureUnit MeasureUnit { get; set; }

        public float Radius { get; set; }

        public static LocationTypeRadius FromCurrentLocationSelection(ILocationSelection selection, SettingsManager settingsManager)
        {
            return new LocationTypeRadius(Resource.String.radius, 
                                          selection is RadiusLocationSelection, 
                                          settingsManager.LocationSelectionRadiusValue, 
                                          settingsManager.LocationSelectionRadiusMeasureUnit);
        }

        public override ILocationSelection BuildLocationSelection()
        {
            return RadiusLocationSelection.Create(new FloatWithUnit(this.Radius, this.MeasureUnit));
        }

        private LocationTypeRadius(int displayNameResourceId, bool enabled, float radius, MeasureUnit measureUnit)
            : base(displayNameResourceId, enabled)
        {
            this.Radius = radius;
            this.MeasureUnit = measureUnit;
        }
    }
}