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
using System.Linq;
using BarcodeCaptureSettingsSample.Base.UiColors;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeRectangular : ViewfinderType
    {
        private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
        {
            using RectangularViewfinder rectangular = RectangularViewfinder.Create();
            return new UiColor(rectangular.Color, Resource.String._default);
        });

        private static readonly Lazy<UiColor> defaultDisabledColor = new Lazy<UiColor>(() =>
        {
            using RectangularViewfinder rectangular = RectangularViewfinder.Create();
            return new UiColor(rectangular.DisabledColor, Resource.String._default);
        });

        public static class DisabledColors
        {
            private static readonly Lazy<Dictionary<RectangularViewfinderStyle, IList<UiColor>>> colors = new Lazy<Dictionary<RectangularViewfinderStyle, IList<UiColor>>>(() =>
            {
                Dictionary<RectangularViewfinderStyle, IList<UiColor>> availableColors = new Dictionary<RectangularViewfinderStyle, IList<UiColor>>();
                foreach (var style in RectangularViewfinderStyle.Values())
                {
                    using RectangularViewfinder rectangularViewfinder = RectangularViewfinder.Create(style);
                    availableColors.Add(style, new[] { new UiColor(rectangularViewfinder.Color, Resource.String._default), UiColor.Black, UiColor.White });
                }
                return availableColors;
            });
        
            public static IList<UiColor> GetAllForStyle(RectangularViewfinderStyle style)
            {
                return colors.Value[style];
            }

            public static UiColor GetDefaultForStyle(RectangularViewfinderStyle style)
            {
                return colors.Value[style].First();
            }
        }

        public static class EnabledColors
        {
            private static readonly Lazy<Dictionary<RectangularViewfinderStyle, IList<UiColor>>> colors = new Lazy<Dictionary<RectangularViewfinderStyle, IList<UiColor>>>(() =>
            {
                Dictionary<RectangularViewfinderStyle, IList<UiColor>> availableColors = new Dictionary<RectangularViewfinderStyle, IList<UiColor>>();
                foreach (var style in RectangularViewfinderStyle.Values())
                {
                    using RectangularViewfinder rectangularViewfinder = RectangularViewfinder.Create(style);
                    availableColors.Add(style, new[] { new UiColor(rectangularViewfinder.Color, Resource.String._default), UiColor.Blue, UiColor.Black });
                }
                return availableColors;
            });

            public static IList<UiColor> GetAllForStyle(RectangularViewfinderStyle style)
            {
                return colors.Value[style];
            }

            public static UiColor GetDefaultForStyle(RectangularViewfinderStyle style)
            {
                return colors.Value[style].First();
            }
        }

        public static ViewfinderTypeRectangular FromCurrentViewfinderAndSettings(IViewfinder currentViewFinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeRectangular(
                    currentViewFinder is RectangularViewfinder,
                    settingsManager.RectangularViewfinderColor,
                    settingsManager.RectangularViewfinderDisabledColor,
                    settingsManager.RectangularViewfinderSizeSpecification,
                    settingsManager.RectangularViewfinderWidth,
                    settingsManager.RectangularViewfinderHeight,
                    settingsManager.RectangularViewfinderShorterDimension,
                    settingsManager.RectangularViewfinderWidthAspect,
                    settingsManager.RectangularViewfinderHeightAspect,
                    settingsManager.RectangularViewfinderLongerDimensionAspect,
                    settingsManager.RectangularViewfinderStyle,
                    settingsManager.RectangularViewfinderLineStyle,
                    settingsManager.RectangularViewfinderDimming,
                    settingsManager.RectangularViewfinderAnimation,
                    settingsManager.RectangularViewfinderLooping
            );
        }

        public static UiColor DefaultColor { get { return defaultColor.Value; } }
        public static UiColor DefaultDisabledColor { get { return defaultDisabledColor.Value; } }

        public UiColor Color { get; set; }
        public UiColor DisabledColor { get; set; }
        public SizeSpecification SizeSpecification { get; set; }
        public FloatWithUnit Width { get; set; }
        public FloatWithUnit Height { get; set; }
        public FloatWithUnit ShorterDimension { get; set; }
        public float WidthAspect { get; set; }
        public float HeightAspect { get; set; }
        public float LongerDimensionAspect { get; set; }
        public RectangularViewfinderStyle Style { get; set; }
        public RectangularViewfinderLineStyle LineStyle { get; set; }

        public float Dimming { get; set; }
        public bool Animation { get; set; }
        public bool Looping { get; set; }

        private ViewfinderTypeRectangular(
                bool enabled,
                UiColor color,
                UiColor disabledColor,
                SizeSpecification sizeSpecification,
                FloatWithUnit width,
                FloatWithUnit height,
                FloatWithUnit shorterDimension,
                float widthAspect,
                float heightAspect,
                float longerDimensionAspect,
                RectangularViewfinderStyle style,
                RectangularViewfinderLineStyle lineStyle,
                float dimming,
                bool animation,
                bool looping) : base(Resource.String.rectangular, enabled)
        {
            this.Color = color;
            this.DisabledColor = disabledColor;
            this.SizeSpecification = sizeSpecification;
            this.Width = width;
            this.Height = height;
            this.ShorterDimension = shorterDimension;
            this.WidthAspect = widthAspect;
            this.HeightAspect = heightAspect;
            this.LongerDimensionAspect = longerDimensionAspect;
            this.Style = style;
            this.LineStyle = lineStyle;
            this.Dimming = dimming;
            this.Animation = animation;
            this.Looping = looping;
        }

        public override IViewfinder Build()
        {
            RectangularViewfinder viewfinder = RectangularViewfinder.Create(this.Style, this.LineStyle);
            viewfinder.Color = this.Color.Color;
            viewfinder.DisabledColor = this.DisabledColor.Color;
            viewfinder.Dimming = this.Dimming;

            RectangularViewfinderAnimation finalAnimation = null;
            if (this.Animation)
            {
                finalAnimation = new RectangularViewfinderAnimation(this.Looping);
            }
            viewfinder.Animation = finalAnimation;

            switch (this.SizeSpecification)
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
                case SizeSpecification.ShorterDimensionAndAspect:
                    viewfinder.SetShorterDimensionAndAspectRatio(this.ShorterDimension.Value, this.LongerDimensionAspect);
                    break;
            }

            return viewfinder;
        }

        public override void ResetDefaults()
        {
            base.ResetDefaults();

            RectangularViewfinder viewfinder = RectangularViewfinder.Create(this.Style);
            this.Color = EnabledColors.GetDefaultForStyle(this.Style);
            this.DisabledColor = DisabledColors.GetDefaultForStyle(this.Style);
            this.LineStyle = viewfinder.LineStyle;
            this.Dimming = viewfinder.Dimming;
            viewfinder.Animation = null;
            this.Animation = viewfinder.Animation != null;
            this.Looping = viewfinder.Animation?.Looping ?? false;

            SizeWithUnitAndAspect size = viewfinder.SizeWithUnitAndAspect;
            this.SizeSpecification = size.SizingMode.GetForSizingMode();
            switch (this.SizeSpecification)
            {
                case SizeSpecification.WidthAndHeight:
                    this.Width = size.WidthAndHeight.Width;
                    this.Height = size.WidthAndHeight.Height;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    this.Width = size.WidthAndAspectRatio.Size;
                    this.HeightAspect = size.WidthAndAspectRatio.Aspect;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    this.Height = size.HeightAndAspectRatio.Size;
                    this.WidthAspect = size.HeightAndAspectRatio.Aspect;
                    break;
                case SizeSpecification.ShorterDimensionAndAspect:
                    this.ShorterDimension = size.ShorterDimensionAndAspectRatio.Size;
                    this.LongerDimensionAspect = size.ShorterDimensionAndAspectRatio.Aspect;
                    break;
            }
        }
    }
}
