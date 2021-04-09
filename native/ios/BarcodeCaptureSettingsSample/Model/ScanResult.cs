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
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.Model
{
    public class ScanResult
    {
        public ScanResult(IList<Barcode> barcodes)
        {
            this.Barcodes = barcodes;
        }

        public IList<Barcode> Barcodes { get; private set; }

        public string Text
        {
            get
            {
                return this.Barcodes?.Aggregate("", (result, barcode) =>
                {
                    result += $"{barcode.Symbology.ReadableName()}: ";
                    result += barcode.Data;

                    if (!string.IsNullOrEmpty(barcode.AddOnData))
                    {
                        result += " " + barcode.AddOnData;
                    }

                    if (barcode.SymbolCount != -1)
                    {
                        result += $"\nSymbol Count: {barcode.SymbolCount}";
                    }

                    return result;
                });
            }
        }
    }
}
