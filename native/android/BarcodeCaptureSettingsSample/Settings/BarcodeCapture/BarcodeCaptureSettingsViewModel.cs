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
using AndroidX.Lifecycle;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture
{ 
    public class BarcodeCaptureSettingsViewModel : ViewModel
    {
        private readonly Array BarcodeCaptureSettings = Enum.GetValues(typeof(BarcodeCaptureSettingsType));

        public IList<BarcodeCaptureSettingsItem> GetItems()
        {
            static BarcodeCaptureSettingsItem selector(BarcodeCaptureSettingsType item)
            {
                return new BarcodeCaptureSettingsItem { Type = item, DisplayNameResourceId = (int)item };
            }

            return BarcodeCaptureSettings.OfType<BarcodeCaptureSettingsType>().Select(selector).ToList();
        }
    }
}