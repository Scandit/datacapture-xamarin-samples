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
    public class SouthAfricaIdResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public SouthAfricaIdResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.SouthAfricaIdBarcodeResult)
            {
                throw new ArgumentException("Unexpected null SouthAfricaIdBarcodeResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetSouthAfricaIdBarcodeRows(capturedId.SouthAfricaIdBarcode))
                                  .ToList();
        }

        private IList<ResultEntry> GetSouthAfricaIdBarcodeRows(SouthAfricaIdBarcodeResult southAfricaIdBarcodeResult)
        {
            var southAfricaIdBarcodeRows = new[] {
                new ResultEntry(value: southAfricaIdBarcodeResult.CountryOfBirthIso, title: "Country of Birth ISO"),
                new ResultEntry(value: southAfricaIdBarcodeResult.CountryOfBirth, title: "Country of Birth"),
                new ResultEntry(value: southAfricaIdBarcodeResult.CitizenshipStatus, title: "Citizenship Status"),
                new ResultEntry(value: southAfricaIdBarcodeResult.PersonalIdNumber, title: "Personal ID Number")
            };

            return southAfricaIdBarcodeRows;
        }
    }
}
