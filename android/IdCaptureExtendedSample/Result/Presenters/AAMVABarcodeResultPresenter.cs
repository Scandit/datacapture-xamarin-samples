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
using IdCaptureExtendedSample.Extensions;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class AAMVABarcodeResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public AAMVABarcodeResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.AamvaBarcodeResult)
            { 
                throw new ArgumentException("Unexpected null AAMVABarcodeResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetAamvaBarcodeRows(capturedId.AamvaBarcode))
                                  .ToList();
        }

        private IList<ResultEntry> GetAamvaBarcodeRows(AamvaBarcodeResult aamvaBarcodeResult)
        {
            var aamvaBarcodeRows = new[] {
                new ResultEntry(value: $"({aamvaBarcodeResult.AamvaVersion})", title: "AAMVA Version"),
                new ResultEntry(value: $"({aamvaBarcodeResult.AamvaVersion})", title: "Jurisdiction Version"),
                new ResultEntry(value: aamvaBarcodeResult.IIN, title: "IIN"),
                new ResultEntry(value: aamvaBarcodeResult.IssuingJurisdiction, title: "Issuing Jurisdiction"),
                new ResultEntry(value: aamvaBarcodeResult.IssuingJurisdictionIso, title: "Issuing Jurisdiction ISO"),
                new ResultEntry(value: aamvaBarcodeResult.EyeColor, title: "Eye Color"),
                new ResultEntry(value: aamvaBarcodeResult.HairColor, title: "Hair Color"),
                new ResultEntry(value: aamvaBarcodeResult.HeightInch?.ToString(), title: "Height Inch"),
                new ResultEntry(value: aamvaBarcodeResult.HeightCm?.ToString(), title: "Height cm"),
                new ResultEntry(value: aamvaBarcodeResult.WeightLbs?.ToString(), title: "Weight lbs"),
                new ResultEntry(value: aamvaBarcodeResult.WeightKg?.ToString(), title: "Weight Kg"),
                new ResultEntry(value: aamvaBarcodeResult.PlaceOfBirth, title: "Place of Birth"),
                new ResultEntry(value: aamvaBarcodeResult.Race, title: "Race"),
                new ResultEntry(value: aamvaBarcodeResult.DocumentDiscriminatorNumber, title: "Document Discriminator Number"),
                new ResultEntry(value: aamvaBarcodeResult.VehicleClass, title: "Vehicle Class"),
                new ResultEntry(value: aamvaBarcodeResult.RestrictionsCode, title: "Restrictions Code"),
                new ResultEntry(value: aamvaBarcodeResult.EndorsementsCode, title: "Endorsements Code"),
                new ResultEntry(value: aamvaBarcodeResult.CardRevisionDate?.Date.ToShortDateString(), title: "Card Revision Date"),
                new ResultEntry(value: aamvaBarcodeResult.MiddleName, title: "Middle Name"),
                new ResultEntry(value: aamvaBarcodeResult.DriverNameSuffix, title: "Driver Name Suffix"),
                new ResultEntry(value: aamvaBarcodeResult.DriverNamePrefix, title: "Driver Name Prefix"),
                new ResultEntry(value: aamvaBarcodeResult.LastNameTruncation, title: "Last Name Truncation"),
                new ResultEntry(value: aamvaBarcodeResult.FirstNameTruncation, title: "First Name Truncation"),
                new ResultEntry(value: aamvaBarcodeResult.MiddleNameTruncation, title: "Middle Name Truncation"),
                new ResultEntry(value: aamvaBarcodeResult.AliasFamilyName, title: "Alias Family Name"),
                new ResultEntry(value: aamvaBarcodeResult.AliasGivenName, title: "Alias Given Name"),
                new ResultEntry(value: aamvaBarcodeResult.AliasSuffixName, title: "Alias Suffix Name"),
                new ResultEntry(value: string.Join("\n", aamvaBarcodeResult.BarcodeDataElements
                                                                           .Select(e => $"{e.Key}:{e.Value}")),
                                title: "Barcode Elements")
            };

            return aamvaBarcodeRows;
        }
    }
}
