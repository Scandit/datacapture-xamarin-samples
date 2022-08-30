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

namespace BarcodeCaptureSettingsSample.Settings.Views
{
    public class ViewSettingsViewModel : ViewModel
    {
        private readonly Array ViewSettings = Enum.GetValues(typeof(ViewSettingsType));

        public IList<ViewSettingsItem> GetItems()
        {
            static ViewSettingsItem selector(ViewSettingsType item)
            {
                return new ViewSettingsItem { Type = item, DisplayNameResourceId = (int)item };
            }

            return ViewSettings.OfType<ViewSettingsType>().Select(selector).ToList();
        }
    }
}