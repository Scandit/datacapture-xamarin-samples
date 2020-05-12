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

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder
{
    public class ViewfinderSettingsViewModel : ViewfinderTypeViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public IList<ViewfinderType> ViewfinderTypes => new List<ViewfinderType>
        {
            ViewfinderTypeNone.FromCurrentViewFinder(this.settingsManager.CurrentViewfinder),
            ViewfinderTypeRectangular.FromCurrentViewfinderAndSettings(this.settingsManager.CurrentViewfinder, this.settingsManager),
            ViewfinderTypeLaserline.FromCurrentViewfinderAndSettings(this.settingsManager.CurrentViewfinder, this.settingsManager)
        };

        public void SetViewfinderType(ViewfinderType viewfinderType)
        {
            if (viewfinderType is ViewfinderTypeRectangular rectangular)
            {
                this.settingsManager.RectangularViewfinderColor = (rectangular.Color);
                this.settingsManager.RectangularViewfinderSizeSpecification = (rectangular.SizeSpecification);
                this.settingsManager.RectangularViewfinderWidth = rectangular.Width;
                this.settingsManager.RectangularViewfinderWidthAspect = (rectangular.WidthAspect);
                this.settingsManager.RectangularViewfinderHeight = (rectangular.Height);
                this.settingsManager.RectangularViewfinderHeightAspect = (rectangular.HeightAspect);
            }
            else if (viewfinderType is ViewfinderTypeLaserline laserline)
            {
                this.settingsManager.LaserlineViewfinderWidth = (laserline.Width);
                this.settingsManager.LaserlineViewfinderEnabledColor = (laserline.EnabledColor);
                this.settingsManager.LaserlineViewfinderDisabledColor = (laserline.DisabledColor);
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

        public void SetLaserlineViewfinderEnabledColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeLaserline)
            {
                ((ViewfinderTypeLaserline)currentViewfinder).EnabledColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
            this.settingsManager.LaserlineViewfinderEnabledColor = color;
        }

        public void SetLaserlineViewfinderDisabledColor(UiColor color)
        {
            ViewfinderType currentViewfinder = this.GetCurrentViewfinderType();
            if (currentViewfinder is ViewfinderTypeLaserline)
            {
                ((ViewfinderTypeLaserline)currentViewfinder).DisabledColor = color;
                this.SetViewfinderType(currentViewfinder);
            }
            this.settingsManager.LaserlineViewfinderDisabledColor = color;
        }
    }
}
