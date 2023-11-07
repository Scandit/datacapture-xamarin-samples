﻿/*
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
    public class ArgentinaIdResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

        public ArgentinaIdResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (!capturedId.CapturedResultTypes.HasFlag(CapturedResultType.ArgentinaIdBarcodeResult))
            {
                throw new ArgumentException("Unexpected null ArgentinaIdBarcodeResult");
            }

            this.Rows = this.GetArgentinaIdBarcodeRows(capturedId.ArgentinaIdBarcode).ToList();
        }

        private IList<ICellProvider> GetArgentinaIdBarcodeRows(ArgentinaIdBarcodeResult argentinaIdBarcodeResult)
        {
            var argentinaIdRows = new[] {
                new SimpleTextCellProvider(value: argentinaIdBarcodeResult.DocumentCopy, title: "Document Copy"),
                new SimpleTextCellProvider(value: argentinaIdBarcodeResult.PersonalIdNumber, title: "Personal ID Number")
            };

            return argentinaIdRows;
        }
    }
}
