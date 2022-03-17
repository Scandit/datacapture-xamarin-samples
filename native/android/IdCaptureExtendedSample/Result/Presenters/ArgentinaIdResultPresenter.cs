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
    public class ArgentinaIdResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public ArgentinaIdResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.ArgentinaIdBarcodeResult)
            {
                throw new ArgumentException("Unexpected null ArgentinaIdBarcodeResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetArgentinaIdBarcodeRows(capturedId.ArgentinaIdBarcode))
                                  .ToList();
        }

        private IList<ResultEntry> GetArgentinaIdBarcodeRows(ArgentinaIdBarcodeResult argentinaIdBarcodeResult)
        {
            var argentinaIdRows = new[] {
                new ResultEntry(value: argentinaIdBarcodeResult.DocumentCopy, title: "Document Copy"),
                new ResultEntry(value: argentinaIdBarcodeResult.PersonalIdNumber, title: "Personal ID Number")
            };

            return argentinaIdRows;
        }
    }
}
