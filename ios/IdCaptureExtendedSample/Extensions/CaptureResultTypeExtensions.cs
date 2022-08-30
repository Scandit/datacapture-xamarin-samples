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

using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Extensions
{
    public static class CaptureResultTypeExtensions
    {
        public static string GetName(this CapturedResultType captureResultType)
        {
            return captureResultType switch
            {
                CapturedResultType.MrzResult => "MRZ Result",
                CapturedResultType.AamvaBarcodeResult => "Aamva Barcode Result",
                CapturedResultType.UsUniformedServicesBarcodeResult => "US Uniformed Services Barcode Result",
                CapturedResultType.VizResult => "VIZ Result",
                CapturedResultType.ColombiaIdBarcodeResult => "Colombia ID Barcode Result",
                CapturedResultType.ArgentinaIdBarcodeResult => "Argentina ID Barcode Result",
                CapturedResultType.SouthAfricaDlBarcodeResult => "South Africa DL Barcode Result",
                CapturedResultType.SouthAfricaIdBarcodeResult => "South Africa Id Barcode Result",
                _ => "No Result",
            };
        }
    }
}
