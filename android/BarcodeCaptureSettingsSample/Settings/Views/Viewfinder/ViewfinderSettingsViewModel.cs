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
using BarcodeCaptureSettingsSample.Base.UiColors;
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder
{
    public class ViewfinderSettingsViewModel : ViewfinderTypeViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public IList<ViewfinderType> ViewfinderTypes => new List<ViewfinderType>
        {
            ViewfinderTypeNone.FromCurrentViewFinder(this.settingsManager.CurrentViewfinder),
            ViewfinderTypeRectangular.FromCurrentViewfinderAndSettings(this.settingsManager.CurrentViewfinder, this.settingsManager),
            ViewfinderTypeLaserline.FromCurrentViewfinderAndSettings(this.settingsManager.CurrentViewfinder, this.settingsManager),
            ViewfinderTypeAimer.FromCurrentViewfinderAndSettings(this.settingsManager.CurrentViewfinder, this.settingsManager)
        };

        public void SetViewfinderType(ViewfinderType viewfinderType)
        {
            if (viewfinderType is ViewfinderTypeRectangular rectangular)
            {
                this.settingsManager.RectangularViewfinderColor = rectangular.Color;
                this.settingsManager.RectangularViewfinderDisabledColor = rectangular.DisabledColor;
                this.settingsManager.RectangularViewfinderSizeSpecification = rectangular.SizeSpecification;
                this.settingsManager.RectangularViewfinderWidth = rectangular.Width;
                this.settingsManager.RectangularViewfinderWidthAspect = rectangular.WidthAspect;
                this.settingsManager.RectangularViewfinderHeight = rectangular.Height;
                this.settingsManager.RectangularViewfinderHeightAspect = rectangular.HeightAspect;
                this.settingsManager.RectangularViewfinderShorterDimension = rectangular.ShorterDimension;
                this.settingsManager.RectangularViewfinderLongerDimensionAspect = rectangular.LongerDimensionAspect;
                this.settingsManager.RectangularViewfinderDimming = rectangular.Dimming;
                this.settingsManager.RectangularViewfinderStyle = rectangular.Style;
                this.settingsManager.RectangularViewfinderLineStyle = rectangular.LineStyle;
                this.settingsManager.RectangularViewfinderAnimation = rectangular.Animation;
                this.settingsManager.RectangularViewfinderLooping = rectangular.Looping;
            }
            else if (viewfinderType is ViewfinderTypeLaserline laserline)
            {
                this.settingsManager.LaserlineViewfinderStyle = laserline.Style;
                this.settingsManager.LaserlineViewfinderWidth = laserline.Width;
                this.settingsManager.LaserlineViewfinderEnabledColor = laserline.EnabledColor;
                this.settingsManager.LaserlineViewfinderDisabledColor = laserline.DisabledColor;
            }
            else if (viewfinderType is ViewfinderTypeAimer aimer)
            {
                this.settingsManager.AimerViewfinderDotColor = aimer.DotColor;
                this.settingsManager.AimerViewfinderFrameColor = aimer.FrameColor;
            }

            this.UpdateViewfinder(viewfinderType);
        }

        public void SetRectangularViewfinderColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).Color = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderDisabledColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).DisabledColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderSizeSpec(SizeSpecification spec)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).SizeSpecification = spec;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderHeightAspect(float aspect)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).HeightAspect = aspect;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderWidthAspect(float aspect)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).WidthAspect = aspect;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderShorterDimension(float shorterDimensionFraction)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).ShorterDimension = new FloatWithUnit(shorterDimensionFraction, MeasureUnit.Fraction);
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderLongerDimensionAspect(float longerDimensionAspect)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).LongerDimensionAspect = longerDimensionAspect;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderStyle(RectangularViewfinderStyle style)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).Style = style;
                currentViewfinder.ResetDefaults();
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularViewfinderLineStyle(RectangularViewfinderLineStyle style)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).LineStyle = style;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetRectangularDimming(float dimming)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).Dimming = dimming;
                this.SetViewfinderType(currentViewfinder);
            }
            settingsManager.RectangularViewfinderDimming = dimming;
        }

        public void SetRectangularAnimationEnabled(bool enabled)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).Animation = enabled;
                this.SetViewfinderType(currentViewfinder);
            }
            settingsManager.RectangularViewfinderAnimation = enabled;
        }

        public void SetRectangularLoopingEnabled(bool enabled)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeRectangular)
            {
                ((ViewfinderTypeRectangular)currentViewfinder).Looping = enabled;
                this.SetViewfinderType(currentViewfinder);
            }
            settingsManager.RectangularViewfinderLooping = enabled;
        }

        public void SetLaserlineViewfinderStyle(LaserlineViewfinderStyle style)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeLaserline)
            {
                ((ViewfinderTypeLaserline)currentViewfinder).Style = style;
                currentViewfinder.ResetDefaults();
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetLaserlineViewfinderEnabledColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeLaserline)
            {
                ((ViewfinderTypeLaserline)currentViewfinder).EnabledColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetLaserlineViewfinderDisabledColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeLaserline)
            {
                ((ViewfinderTypeLaserline)currentViewfinder).DisabledColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetAimerViewfinderFrameColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeAimer)
            {
                ((ViewfinderTypeAimer)currentViewfinder).FrameColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }

        public void SetAimerViewfinderDotColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeAimer)
            {
                ((ViewfinderTypeAimer)currentViewfinder).DotColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
        }
    }
}
