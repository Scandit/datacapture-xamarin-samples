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

using AndroidX.Lifecycle;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public ViewfinderType GetCurrentViewfinderType()
        {
            IViewfinder viewfinder = this.settingsManager.CurrentViewfinder;

            return viewfinder switch
            {
                RectangularViewfinder _ => ViewfinderTypeRectangular.FromCurrentViewfinderAndSettings(viewfinder, this.settingsManager),
                LaserlineViewfinder _ => ViewfinderTypeLaserline.FromCurrentViewfinderAndSettings(viewfinder, this.settingsManager),
                _ => ViewfinderTypeNone.FromCurrentViewFinder(viewfinder)
            };
        }

        public void UpdateViewfinder()
        {
            this.UpdateViewfinder(this.GetCurrentViewfinderType());
        }

        public void UpdateViewfinder(ViewfinderType viewfinderType)
        {
            this.settingsManager.CurrentViewfinder = viewfinderType.Build();
        }
    }
}
