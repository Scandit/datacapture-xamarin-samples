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
    public class ViewfinderTypeSpotlight : ViewfinderType
    {
        public static class BackgroundColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.CustomGreen, UiColor.CustomBlue, Default });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using SpotlightViewfinder spotlight = SpotlightViewfinder.Create();
                return new UiColor(spotlight.BackgroundColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static class EnabledColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.Blue, UiColor.Black, Default });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using SpotlightViewfinder spotlight = SpotlightViewfinder.Create();
                return new UiColor(spotlight.EnabledBorderColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static class DisabledColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.Red, UiColor.Black, Default });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using SpotlightViewfinder spotlight = SpotlightViewfinder.Create();
                return new UiColor(spotlight.DisabledBorderColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static ViewfinderTypeSpotlight FromCurrentViewfinderAndSettings(IViewfinder currentViewFinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeSpotlight(
                    currentViewFinder is SpotlightViewfinder,
                    settingsManager.SpotlightViewfinderBackgroundColor,
                    settingsManager.SpotlightViewfinderEnabledColor,
                    settingsManager.SpotlightViewfinderDisabledColor,
                    settingsManager.SpotlightViewfinderSizeSpecification,
                    settingsManager.SpotlightViewfinderWidth,
                    settingsManager.SpotlightViewfinderHeight,
                    settingsManager.SpotlightViewfinderWidthAspect,
                    settingsManager.SpotlightViewfinderHeightAspect
            );
        }

        public UiColor BackgroundColor { get; set; }
        public UiColor EnabledColor { get; set; }
        public UiColor DisabledColor { get; set; }

        public SizeSpecification SizeSpecification { get; set; }
        public FloatWithUnit Width { get; set; }
        public FloatWithUnit Height { get; set; }
        public float WidthAspect { get; set; }
        public float HeightAspect { get; set; }

        private ViewfinderTypeSpotlight(
                bool enabled,
                UiColor backgroundColor,
                UiColor enabledColor,
                UiColor disabledColor,
                SizeSpecification sizeSpecification,
                FloatWithUnit width,
                FloatWithUnit height,
                float widthAspect,
                float heightAspect) : base(Resource.String.spotlight, enabled)
        {
            this.BackgroundColor = backgroundColor;
            this.EnabledColor = enabledColor;
            this.DisabledColor = disabledColor;
            this.SizeSpecification = sizeSpecification;
            this.Width = width;
            this.Height = height;
            this.WidthAspect = widthAspect;
            this.HeightAspect = heightAspect;
        }

        public override IViewfinder Build()
        {
            SpotlightViewfinder viewfinder = SpotlightViewfinder.Create();
            viewfinder.BackgroundColor = this.BackgroundColor.Color;
            viewfinder.EnabledBorderColor = this.EnabledColor.Color;
            viewfinder.DisabledBorderColor = this.DisabledColor.Color;

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