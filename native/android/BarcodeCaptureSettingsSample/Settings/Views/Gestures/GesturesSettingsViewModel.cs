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

namespace BarcodeCaptureSettingsSample.Settings.Views.Gestures
{
    public class GesturesSettingsViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public bool TapToFocusEnabled
        {
            get { return settingsManager.TapToFocusEnabled; }
            set { settingsManager.TapToFocusEnabled = value; }
        }

        public bool SwipeToZoomEnabled
        {
            get { return settingsManager.SwipeToZoomEnalbed; }
            set { settingsManager.SwipeToZoomEnalbed = value; }
        }
    }
}
