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

using System;
using System.Collections.Generic;
using BarcodeCaptureSettingsSample.Base.UiColors;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeRectangular : ViewfinderType
    {
        private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.Blue, UiColor.Black, DefaultColor });
        private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
        {
            using RectangularViewfinder rectangular = RectangularViewfinder.Create();
            return new UiColor(rectangular.Color, Resource.String._default);
        });

        public static ViewfinderTypeRectangular FromCurrentViewfinderAndSettings(IViewfinder currentViewFinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeRectangular(
                    currentViewFinder is RectangularViewfinder,
                    settingsManager.RectangularViewfinderColor,
                    settingsManager.RectangularViewfinderSizeSpecification,
                    settingsManager.RectangularViewfinderWidth,
                    settingsManager.RectangularViewfinderHeight,
                    settingsManager.RectangularViewfinderWidthAspect,
                    settingsManager.RectangularViewfinderHeightAspect
            );
        }

        public static IList<UiColor> Colors { get { return colors.Value; } }

        public static UiColor DefaultColor { get { return defaultColor.Value; } }

        public UiColor Color { get; set; }
        public SizeSpecification SizeSpecification { get; set; }
        public FloatWithUnit Width { get; set; }
        public FloatWithUnit Height { get; set; }
        public float WidthAspect { get; set; }
        public float HeightAspect { get; set; }

        private ViewfinderTypeRectangular(
                bool enabled,
                UiColor color,
                SizeSpecification sizeSpecification,
                FloatWithUnit width,
                FloatWithUnit height,
                float widthAspect,
                float heightAspect) : base(Resource.String.rectangular, enabled)
        {
            this.Color = color;
            this.SizeSpecification = sizeSpecification;
            this.Width = width;
            this.Height = height;
            this.WidthAspect = widthAspect;
            this.HeightAspect = heightAspect;
        }

        public override IViewfinder Build()
        {
            RectangularViewfinder viewfinder = RectangularViewfinder.Create();
            viewfinder.Color = this.Color.Color;

            switch (SizeSpecification)
            {
                case SizeSpecification.WidthAndHeight:
                    viewfinder.SetSize(new SizeWithUnit(Width, Height));
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    viewfinder.SetWidthAndAspectRatio(Width, HeightAspect);
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    viewfinder.SetHeightAndAspectRatio(Height, WidthAspect);
                    break;
            }

            return viewfinder;
        }
    }
}
