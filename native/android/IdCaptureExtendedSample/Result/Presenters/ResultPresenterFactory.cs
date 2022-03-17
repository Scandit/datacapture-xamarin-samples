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
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class ResultPresenterFactory
    {
        private readonly Dictionary<CapturedResultType, Func<CapturedId, IResultPresenter>> mappings = new Dictionary<CapturedResultType, Func<CapturedId, IResultPresenter>>()
        {
            { CapturedResultType.AamvaBarcodeResult, (CapturedId id) => new AAMVABarcodeResultPresenter(id) },
            { CapturedResultType.ArgentinaIdBarcodeResult, (CapturedId id) => new ArgentinaIdResultPresenter(id) },
            { CapturedResultType.ColombiaIdBarcodeResult, (CapturedId id) => new ColombiaIdBarcodeResultPresenter(id) },
            { CapturedResultType.MrzResult, (CapturedId id) => new MRZResultPresenter(id) },
            { CapturedResultType.SouthAfricaDlBarcodeResult, (CapturedId id) => new SouthAfricaDlResultPresenter(id) },
            { CapturedResultType.SouthAfricaIdBarcodeResult, (CapturedId id) => new SouthAfricaIdResultPresenter(id) },
            { CapturedResultType.UsUniformedServicesBarcodeResult, (CapturedId id) => new UsUniformedServicesResultPresenter(id) },
            { CapturedResultType.VizResult, (CapturedId id) => new VizResultPresenter(id) },
        };

        public IResultPresenter Create(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            return this.mappings[capturedId.CapturedResultType].Invoke(capturedId);
        }
    }
}
