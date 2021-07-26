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

using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class ViewfinderKind : Enumeration
    {
        public static readonly ViewfinderKind None = new ViewfinderKind(0, "None", null);
        public static ViewfinderKind Rectangular = new ViewfinderKind(1, "Rectangular", RectangularViewfinder.Create());
        public static ViewfinderKind Laserline = new ViewfinderKind(2, "Laserline", LaserlineViewfinder.Create());
        public static readonly ViewfinderKind Aimer = new ViewfinderKind(3, "Aimer", AimerViewfinder.Create());

        public static ViewfinderKind UpdateRectangularStyle(RectangularViewfinderStyleType styleType)
        {
            if (styleType.Style != ((RectangularViewfinder)Rectangular.Viewfinder).Style)
            {
                Rectangular = new ViewfinderKind(1, "Rectangular", RectangularViewfinder.Create(styleType.Style));

                var viewfinder = (RectangularViewfinder)Rectangular.Viewfinder;
                viewfinder.Animation = null;
                SettingsManager.Instance.RectangularViewfinderAnimation = viewfinder.Animation;
                SettingsManager.Instance.RectangularViewfinderLineStyleType = viewfinder.LineStyle ==
                    RectangularViewfinderLineStyle.Light ? RectangularViewfinderLineStyleType.Light : RectangularViewfinderLineStyleType.Bold;
                SettingsManager.Instance.RectangularViewfinderDimming = viewfinder.Dimming;
                SettingsManager.Instance.RectangularViewfinderColor = RectangularViewfinderColor.Default;
                SettingsManager.Instance.RectangularViewfinderDisabledColor = RectangularViewfinderDisabledColor.Default;
            }

            return Rectangular;
        }

        public static ViewfinderKind UpdateRectangularStyle(RectangularViewfinderStyleType styleType, RectangularViewfinderLineStyleType lineStyleType)
        {
            if (lineStyleType.LineStyle != ((RectangularViewfinder)Rectangular.Viewfinder).LineStyle)
            {
                Rectangular = new ViewfinderKind(1, "Rectangular", RectangularViewfinder.Create(styleType.Style, lineStyleType.LineStyle));

                var viewfinder = (RectangularViewfinder)Rectangular.Viewfinder;
                viewfinder.Animation = SettingsManager.Instance.RectangularViewfinderAnimation;
                viewfinder.Dimming = SettingsManager.Instance.RectangularViewfinderDimming;
                viewfinder.Color = SettingsManager.Instance.RectangularViewfinderColor.UIColor;
                viewfinder.DisabledColor = SettingsManager.Instance.RectangularViewfinderDisabledColor.UIColor;

                if (SettingsManager.Instance.ViewfinderSizeSpecification == RectangularSizeSpecification.WidthAndHeight)
                {
                    viewfinder.SetSize(new SizeWithUnit(SettingsManager.Instance.RectangularWidth, SettingsManager.Instance.RectangularHeight));
                }
                else if (SettingsManager.Instance.ViewfinderSizeSpecification == RectangularSizeSpecification.WidthAndHeightAspect)
                {
                    viewfinder.SetWidthAndAspectRatio(SettingsManager.Instance.RectangularWidth, SettingsManager.Instance.RectangularHeightAspect);
                }
                else if (SettingsManager.Instance.ViewfinderSizeSpecification == RectangularSizeSpecification.HeightAndWidthAspect)
                {
                    viewfinder.SetHeightAndAspectRatio(SettingsManager.Instance.SpotlightHeight, SettingsManager.Instance.RectangularWidthAspect);
                }
                else if (SettingsManager.Instance.ViewfinderSizeSpecification == RectangularSizeSpecification.ShorterDimensionAndAspectRatio)
                {
                    viewfinder.SetShorterDimensionAndAspectRatio(SettingsManager.Instance.RectangularShorterDimension, SettingsManager.Instance.RectangularLongerDimensionAspect);
                }
            }

            return Rectangular;
        }

        public static ViewfinderKind UpdateLaserlineStyle(LaserlineViewfinderStyleType styleType)
        {
            if (styleType.Style != ((LaserlineViewfinder)Laserline.Viewfinder).Style)
            {
                Laserline = new ViewfinderKind(2, "Laserline", LaserlineViewfinder.Create(styleType.Style));

                SettingsManager.Instance.LaserlineViewfinderEnabledColor = LaserlineViewfinderEnabledColor.Default;
                SettingsManager.Instance.LaserlineViewfinderAnimatedEnabledColor = LaserlineViewfinderAnimatedEnabledColor.Default;
                SettingsManager.Instance.LaserlineViewfinderDisabledColor = LaserlineViewfinderDisabledColor.Default;
            }

            return Laserline;
        }

        public ViewfinderKind(int key, string value, IViewfinder viewfinder) : base(key, value)
        {
            this.Viewfinder = viewfinder;
        }

        public IViewfinder Viewfinder { get; }
    }
}
