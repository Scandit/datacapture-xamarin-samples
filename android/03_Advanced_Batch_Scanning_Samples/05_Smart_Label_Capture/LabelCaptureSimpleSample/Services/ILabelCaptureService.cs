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
using Android.Content;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Label.UI.Overlay;
using Scandit.DataCapture.Label.UI.Overlay.Validation;

namespace LabelCaptureSimpleSample.Services
{
    public interface ILabelCaptureService
    {
        public void Initialize(DataCaptureContext dataCaptureContext);
        public void Enable();
        public void Disable();
        public LabelCaptureBasicOverlay BuildOverlay(Context context);
        public LabelCaptureValidationFlowOverlay BuildValidationFlowOverlay(
            Context context, Action<string> onLabelScanned);
    }
}
