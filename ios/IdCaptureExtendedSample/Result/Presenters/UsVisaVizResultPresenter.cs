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
using IdCaptureExtendedSample.Result.CellProviders;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class UsVisaVizResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

        public UsVisaVizResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (!capturedId.CapturedResultTypes.HasFlag(CapturedResultType.UsVisaVizResult))
            {
                throw new ArgumentException("Unexpected null UsVisaVizResult");
            }

            this.Rows = this.GetRows(capturedId).ToList();
        }

        private IList<ICellProvider> GetRows(CapturedId capturedId)
        {
            var usVisaVizResult = capturedId.UsVisaViz;

            ICellProvider[] rows = new ICellProvider[] {
                new SimpleTextCellProvider(value: usVisaVizResult.PassportNumber, title: "Passport number"),
                new SimpleTextCellProvider(value: usVisaVizResult.VisaNumber, title: "Visa number"),
            };

            return rows;
        }
    }
}
