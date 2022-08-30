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

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes
{
    public class LocationTypeNone : LocationType
    {
        public static LocationTypeNone FromCurrentLocationSelection(ILocationSelection selection)
        {
            return new LocationTypeNone(Resource.String.none, selection == null);
        }

        public override ILocationSelection BuildLocationSelection()
        {
            return null;
        }

        private LocationTypeNone(int displayNameResourceId, bool enabled) : base(displayNameResourceId, enabled)
        { }
    }
}