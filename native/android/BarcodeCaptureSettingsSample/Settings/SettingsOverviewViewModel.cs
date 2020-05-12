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

namespace BarcodeCaptureSettingsSample.Settings
{ 
    public class SettingsOverviewViewModel : ViewModel
    {
        private readonly Array SettingsOverviews = Enum.GetValues(typeof(SettingsOverviewType));

        public IList<SettingsOverviewItem> GetItems()
        {
            static SettingsOverviewItem selector(SettingsOverviewType item) 
            {
                return new SettingsOverviewItem { Type = item, DisplayNameResourceId = (int)item };
            }

            return SettingsOverviews.OfType<SettingsOverviewType>().Select(selector).ToList();
        }
    }
}