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
    public class ColombiaIdBarcodeResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

        public ColombiaIdBarcodeResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (!capturedId.CapturedResultTypes.HasFlag(CapturedResultType.ColombiaIdBarcodeResult))
            {
                throw new ArgumentException("Unexpected null ColombiaIdBarcodeResult");
            }

            this.Rows = this.GetColombiaIdBarcodeRows(capturedId.ColombiaIdBarcode).ToList();
        }

        private IList<ICellProvider> GetColombiaIdBarcodeRows(ColombiaIdBarcodeResult colombiaIdBarcodeResult)
        {
            var colombiaIdRows = new[] {
                new SimpleTextCellProvider(value: colombiaIdBarcodeResult.BloodType, title: "Blood Type")
            };

            return colombiaIdRows;
        }
    }
}
