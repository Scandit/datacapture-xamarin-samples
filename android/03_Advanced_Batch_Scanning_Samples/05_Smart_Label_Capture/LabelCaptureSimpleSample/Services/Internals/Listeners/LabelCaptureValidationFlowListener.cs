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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using LabelCaptureSimpleSample.Extensions;
using Scandit.DataCapture.Label.Data;
using Scandit.DataCapture.Label.UI.Overlay.Validation;

namespace LabelCaptureSimpleSample.Services.Internals.Listeners
{
    internal class LabelCaptureValidationFlowListener : Java.Lang.Object, ILabelCaptureValidationFlowListener
    {
        private readonly Action<string> onLabelScanned;

        public LabelCaptureValidationFlowListener(Action<string> onLabelScanned)
        {
            this.onLabelScanned = onLabelScanned;
        }

        public void OnValidationFlowLabelCaptured(IList<LabelField> fields)
        {
            var result = fields.ToScannedResult();

            if (result.Fields.Any())
            {
                onLabelScanned(result.ToString());
            }
        }
    }
}
