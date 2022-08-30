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

using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes
{
    public class LocationTypeRectangular : LocationType
    {
        public SizeSpecification SizeSpecification { get; set; }
        public FloatWithUnit Width { get; set; }
        public FloatWithUnit Height { get; set; }
        public float HeightAspectRatio { get; set; }
        public float WidthAspectRatio { get; set; }

        public static LocationTypeRectangular FromCurrentLocationSelectionAndSettings(ILocationSelection selection, SettingsManager settingsManager)
        {
            return new LocationTypeRectangular(
                    Resource.String.rectangular,
                    selection is RectangularLocationSelection,
                    settingsManager.LocationSelectionRectangularSizeSpecification,
                    settingsManager.LocationSelectionRectangularWidth,
                    settingsManager.LocationSelectionRectangularHeight,
                    settingsManager.LocationSelectionRectangularHeightAspect,
                    settingsManager.LocationSelectionRectangularWidthAspect
            );
        }

        public override ILocationSelection BuildLocationSelection()
        {
            switch (this.SizeSpecification)
            {
                case SizeSpecification.WidthAndHeight:
                    return RectangularLocationSelection.Create(new SizeWithUnit(this.Width, this.Height));
                case SizeSpecification.WidthAndHeightAspect:
                    return RectangularLocationSelection.CreateWithWidthAndAspectRatio(this.Width, this.HeightAspectRatio);
                case SizeSpecification.HeightAndWidthAspect:
                    return RectangularLocationSelection.CreateWithHeightAndAspectRatio(this.Height, this.WidthAspectRatio);
                default:
                    return null;
            }
        }

        private LocationTypeRectangular(
                int displayNameResourceId,
                bool enabled,
                SizeSpecification sizeSpecification,
                FloatWithUnit width,
                FloatWithUnit height,
                float heightAspectRatio,
                float widthAspectRatio) : base (displayNameResourceId, enabled)
        {
            this.SizeSpecification = sizeSpecification;
            this.Width = width;
            this.Height = height;
            this.HeightAspectRatio = heightAspectRatio;
            this.WidthAspectRatio = widthAspectRatio;
        }
    }
}