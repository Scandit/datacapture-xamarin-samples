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
using IdCaptureExtendedSample.Result.CellProviders;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class AAMVABarcodeResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

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

        private IList<ICellProvider> GetAamvaBarcodeRows(AamvaBarcodeResult aamvaBarcodeResult)
        {
            var aamvaBarcodeRows = new[] {
                new SimpleTextCellProvider(value: $"({aamvaBarcodeResult.AamvaVersion})", title: "AAMVA Version"),
                new SimpleTextCellProvider(value: $"({aamvaBarcodeResult.AamvaVersion})", title: "Jurisdiction Version"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.IIN, title: "IIN"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.IssuingJurisdiction, title: "Issuing Jurisdiction"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.IssuingJurisdictionIso, title: "Issuing Jurisdiction ISO"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.EyeColor, title: "Eye Color"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.HairColor, title: "Hair Color"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.HeightInch?.ToString(), title: "Height Inch"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.HeightCm?.ToString(), title: "Height cm"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.WeightLbs?.ToString(), title: "Weight lbs"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.WeightKg?.ToString(), title: "Weight Kg"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.PlaceOfBirth, title: "Place of Birth"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.Race, title: "Race"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.DocumentDiscriminatorNumber, title: "Document Discriminator Number"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.VehicleClass, title: "Vehicle Class"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.RestrictionsCode, title: "Restrictions Code"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.EndorsementsCode, title: "Endorsements Code"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.CardRevisionDate?.Date.ToShortDateString(), title: "Card Revision Date"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.MiddleName, title: "Middle Name"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.DriverNameSuffix, title: "Driver Name Suffix"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.DriverNamePrefix, title: "Driver Name Prefix"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.LastNameTruncation, title: "Last Name Truncation"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.FirstNameTruncation, title: "First Name Truncation"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.MiddleNameTruncation, title: "Middle Name Truncation"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.AliasFamilyName, title: "Alias Family Name"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.AliasGivenName, title: "Alias Given Name"),
                new SimpleTextCellProvider(value: aamvaBarcodeResult.AliasSuffixName, title: "Alias Suffix Name"),
                new SimpleTextCellProvider(value: string.Join("\n", aamvaBarcodeResult.BarcodeDataElements
                                                                                      .Select(e => $"{e.Key}:{e.Value}")),
                                           title: "Barcode Elements")
            };

            return aamvaBarcodeRows;
        }
    }
}
