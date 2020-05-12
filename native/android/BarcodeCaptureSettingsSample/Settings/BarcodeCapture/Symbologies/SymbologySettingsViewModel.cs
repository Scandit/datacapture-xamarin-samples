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
using System.Linq;
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies
{
    public class SymbologySettingsViewModel : ViewModel 
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public bool SwitchEnabled => this.settingsManager.EnabledSymbologies.Any();

        public async Task EnableAllSymbologyAsync(bool enabled)
        {
            await this.settingsManager.EnableAllSymbologies(enabled);
        }

        public IEnumerable<SymbologySettingsItem> GetItems()
        {
            return SymbologyDescription.All().Select(description =>
            {
                return new SymbologySettingsItem(description, this.settingsManager.IsSymbologyEnabled(description.Symbology));
            });
        }
    }
}
