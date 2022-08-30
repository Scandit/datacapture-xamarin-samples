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
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeLaserline : ViewfinderType
    {
        private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
        {
            using LaserlineViewfinder laserline = LaserlineViewfinder.Create();
            return new UiColor(laserline.EnabledColor, Resource.String._default);
        });

        private static readonly Lazy<UiColor> defaultDisabledColor = new Lazy<UiColor>(() =>
        {
            using LaserlineViewfinder laserline = LaserlineViewfinder.Create();
            return new UiColor(laserline.DisabledColor, Resource.String._default);
        });

        public static class EnabledColors
        {
            private static readonly Lazy<Dictionary<LaserlineViewfinderStyle, IList<UiColor>>> colors = new Lazy<Dictionary<LaserlineViewfinderStyle, IList<UiColor>>>(() =>
            {
                Dictionary<LaserlineViewfinderStyle, IList<UiColor>> availableColors = new Dictionary<LaserlineViewfinderStyle, IList<UiColor>>();
                foreach (var style in LaserlineViewfinderStyle.Values())
                {
                    using LaserlineViewfinder laserlineViewfinder = LaserlineViewfinder.Create(style);

                    if (style == LaserlineViewfinderStyle.Legacy)
                    {
                        availableColors.Add(style, new[] { new UiColor(laserlineViewfinder.EnabledColor, Resource.String._default), UiColor.Red, UiColor.White });
                    }
                    else if (style == LaserlineViewfinderStyle.Animated)
                    {
                        availableColors.Add(style, new[] { new UiColor(laserlineViewfinder.EnabledColor, Resource.String._default), UiColor.Blue, UiColor.Red });
                    }
                }
                return availableColors;
            });

            public static IList<UiColor> GetAllForStyle(LaserlineViewfinderStyle style)
            {
                return colors.Value[style];
            }

            public static UiColor GetDefaultForStyle(LaserlineViewfinderStyle style)
            {
                return colors.Value[style].First();
            }
        }

        public static class DisabledColors
        {
            private static readonly Lazy<Dictionary<LaserlineViewfinderStyle, IList<UiColor>>> colors = new Lazy<Dictionary<LaserlineViewfinderStyle, IList<UiColor>>>(() =>
            {
                Dictionary<LaserlineViewfinderStyle, IList<UiColor>> availableColors = new Dictionary<LaserlineViewfinderStyle, IList<UiColor>>();
                foreach (var style in LaserlineViewfinderStyle.Values())
                {
                    using LaserlineViewfinder laserlineViewfinder = LaserlineViewfinder.Create(style);
                    availableColors.Add(style, new[] { new UiColor(laserlineViewfinder.DisabledColor, Resource.String._default), UiColor.Blue, UiColor.Red });
                }
                return availableColors;
            });

            public static IList<UiColor> GetAllForStyle(LaserlineViewfinderStyle style)
            {
                return colors.Value[style];
            }

            public static UiColor GetDefaultForStyle(LaserlineViewfinderStyle style)
            {
                return colors.Value[style].First();
            }
        }

        public static UiColor DefaultEnabledColor { get { return defaultColor.Value; } }
        public static UiColor DefaultDisabledColor { get { return defaultDisabledColor.Value; } }

        public UiColor EnabledColor { get; set; }
        public UiColor DisabledColor { get; set; }
        public FloatWithUnit Width { get; set; }
        public LaserlineViewfinderStyle Style { get; set; }

        public static ViewfinderTypeLaserline FromCurrentViewfinderAndSettings(
                IViewfinder currentViewfinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeLaserline(
                    currentViewfinder is LaserlineViewfinder,
                    settingsManager.LaserlineViewfinderWidth,
                    settingsManager.LaserlineViewfinderEnabledColor,
                    settingsManager.LaserlineViewfinderDisabledColor,
                    settingsManager.LaserlineViewfinderStyle);
        }

        private ViewfinderTypeLaserline(
                bool enabled,
                FloatWithUnit width,
                UiColor enabledColor,
                UiColor disabledColor,
                LaserlineViewfinderStyle style) : base(Resource.String.laserline, enabled)
        {
            this.Width = width;
            this.EnabledColor = enabledColor;
            this.DisabledColor = disabledColor;
            this.Style = style;
        }

        public override IViewfinder Build()
        {
            LaserlineViewfinder viewfinder = LaserlineViewfinder.Create(this.Style);
            viewfinder.Width = this.Width;
            viewfinder.EnabledColor = this.EnabledColor.Color;
            viewfinder.DisabledColor = this.DisabledColor.Color;
            return viewfinder;
        }

        public override void ResetDefaults()
        {
            base.ResetDefaults();

            LaserlineViewfinder viewfinder = LaserlineViewfinder.Create(this.Style);
            this.Width = viewfinder.Width;
            this.EnabledColor = EnabledColors.GetDefaultForStyle(this.Style);
            this.DisabledColor = DisabledColors.GetDefaultForStyle(this.Style);
        }
    }
}