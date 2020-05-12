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
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeLaserline : ViewfinderType
    {
        public static class EnabledColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.Red, UiColor.White, Default });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() => 
            {
                using LaserlineViewfinder laserline = LaserlineViewfinder.Create();
                return new UiColor(laserline.EnabledColor, Resource.String._default);
            });
            
            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static class DisabledColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { UiColor.Blue, UiColor.Red, Default });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using LaserlineViewfinder laserline = LaserlineViewfinder.Create();
                return new UiColor(laserline.DisabledColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public UiColor EnabledColor { get; set; }
        public UiColor DisabledColor { get; set; }
        public FloatWithUnit Width { get; set; }

        public static ViewfinderTypeLaserline FromCurrentViewfinderAndSettings(
                IViewfinder currentViewfinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeLaserline(
                    currentViewfinder is LaserlineViewfinder,
                    settingsManager.LaserlineViewfinderWidth,
                    settingsManager.LaserlineViewfinderEnabledColor,
                    settingsManager.LaserlineViewfinderDisabledColor);
        }

        private ViewfinderTypeLaserline(
                bool enabled,
                FloatWithUnit width,
                UiColor enabledColor,
                UiColor disabledColor) : base(Resource.String.laserline, enabled)
        {
            this.Width = width;
            this.EnabledColor = enabledColor;
            this.DisabledColor = disabledColor;
        }

        public override IViewfinder Build()
        {
            LaserlineViewfinder viewfinder = LaserlineViewfinder.Create();
            viewfinder.Width = this.Width;
            viewfinder.EnabledColor = this.EnabledColor.Color;
            viewfinder.DisabledColor = this.DisabledColor.Color;
            return viewfinder;
        }
    }
}