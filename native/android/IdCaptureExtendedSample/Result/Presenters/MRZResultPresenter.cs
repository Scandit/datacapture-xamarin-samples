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
    public class MRZResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public MRZResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.MrzResult)
            {
                throw new ArgumentException("Unexpected null MrzResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetMrzRows(capturedId.Mrz))
                                  .ToList();
        }

        private IList<ResultEntry> GetMrzRows(MrzResult mrzResult)
        {
            var mrzRows = new[] {
                new ResultEntry(value: mrzResult.DocumentCode, title: "Document Code"),
                new ResultEntry(value: mrzResult.NamesAreTruncated ? "Yes" : "No", title: "Names are Truncated"),
                new ResultEntry(value: mrzResult.Optional, title: "Optional"),
                new ResultEntry(value: mrzResult.Optional1, title: "Optional 1"),
                new ResultEntry(value: mrzResult.CapturedMrz, title: "Captured MRZ")
            };

            return mrzRows;
        }
    }
}
