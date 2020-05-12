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
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Additions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies.SpecificSymbology
{
    public class SpecificSymbologyViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;
        private readonly Symbology symbology;
        private readonly SymbologyDescription symbologyDescription;

        public SpecificSymbologyViewModel(string symbologyIdentifier)
        {
            symbologyIdentifier.RequireNotNullOrWhiteSpace(nameof(symbologyIdentifier));

            this.symbologyDescription = SymbologyDescription.ForIdentifier(symbologyIdentifier);
            this.symbology = this.symbologyDescription.Symbology;
        }

        public string SymbologyReadableName => this.symbologyDescription.ReadableName;

        public bool ColorInvertedSettingsAvailable => this.symbologyDescription.ColorInvertible;

        public bool CurrentSymbologyEnabled => this.settingsManager.IsSymbologyEnabled(this.symbology);

        public async Task SetCurrentSymbologyEnabledAsync(bool enabled)
        {
            await this.settingsManager.EnableSymbologyAsync(this.symbology, enabled, updateBarcodeCaptureSettings: true);
        }

        public bool CurrentSymbologyColorInverted => this.settingsManager.IsColorInverted(this.symbology);

        public async Task SetCurrentSymbologyColorInvertedAsync(bool enabled)
        {
            await this.settingsManager.SetColorInvertedAsync(this.symbology, enabled);
        }

        public bool IsRangeSettingsAvailable()
        {
            Range range = this.symbologyDescription.ActiveSymbolCountRange;
            return range.Minimum != range.Maximum;
        }

        public Range SymbolCountRange => this.symbologyDescription.ActiveSymbolCountRange;

        public int CurrentMinActiveSymbolCount => this.settingsManager.GetMinSymbolCount(this.symbology);

        public async Task SetCurrentMinActiveSymbolCountAsync(int value)
        {
            await this.settingsManager.SetMinSymbolCountAsync(this.symbology, (short)value);
        }

        public int CurrentMaxActiveSymbolCount => this.settingsManager.GetMaxSymbolCount(this.symbology);

        public async Task SetCurrentMaxActiveSymbolCountAsync(int value)
        {
            await this.settingsManager.SetMaxSymbolCountAsync(this.symbology, (short)value);
        }

        public bool ExtensionsAvailable => this.symbologyDescription.SupportedExtensions.Any();

        public IList<SymbologyItem> GetItems()
        {
            var supportedExtensions = this.symbologyDescription.SupportedExtensions;
            List<SymbologyItem> extensionsAndEnabledState = new List<SymbologyItem>(capacity: supportedExtensions.Count);
            extensionsAndEnabledState.AddRange(supportedExtensions.Select(extension => new SymbologyItem
            {
                Name = extension,
                Enabled = settingsManager.IsExtensionEnabled(symbology, extension)
            }));

            return extensionsAndEnabledState;
        }

        public async Task ToggleExtensionAsync(string extension)
        {
            extension.RequireNotNullOrWhiteSpace(nameof(extension));
            await this.settingsManager.SetExtensionEnabledAsync(
                this.symbology, 
                extension, 
                !this.settingsManager.IsExtensionEnabled(this.symbology, extension));
        }
    }
}
