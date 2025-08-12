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
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Views;
using LabelCaptureSimpleSample.Services;
using LabelCaptureSimpleSample.Services.Internals;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Label.UI.Overlay;
using Scandit.DataCapture.Label.UI.Overlay.Validation;

namespace LabelCaptureSimpleSample.ViewModels
{
    public class ScanViewModel
    {
        private readonly ICameraService cameraService;
        private readonly ILabelCaptureService labelCaptureService;
        private readonly Subject<string> messageSubject = new Subject<string>();

        private LabelCaptureValidationFlowOverlay? validationFlowOverlay;

        public IObservable<string> Messages { get { return this.messageSubject.AsObservable(); } }

        public DataCaptureContext DataCaptureContext { get; }

        public bool HasCameraPermission { get; set; }

        public ScanViewModel()
        {
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(MainActivity.SCANDIT_LICENSE_KEY);

            this.cameraService = new CameraService();
            this.cameraService.Initialize(this.DataCaptureContext);

            this.labelCaptureService = new LabelCaptureService();
            this.labelCaptureService.Initialize(this.DataCaptureContext);
        }

        public LabelCaptureBasicOverlay BuildOverlay(Context context)
        {
            return this.labelCaptureService.BuildOverlay(context);
        }

        public LabelCaptureValidationFlowOverlay GetValidationFlowOverlay(Context context)
        {
            if (this.validationFlowOverlay?.Parent is ViewGroup parent)
            {
                parent.RemoveView(this.validationFlowOverlay);
            }
            else
            {
                this.validationFlowOverlay =
                    this.labelCaptureService.BuildValidationFlowOverlay(
                        context,
                        onLabelScanned: label => this.messageSubject.OnNext(label));
            }

            return this.validationFlowOverlay;
        }

        public void OnDialogDismissed()
        {
            if (this.HasCameraPermission)
            {
                this.cameraService.ResumeFrameSource();
                this.labelCaptureService.Enable();
            }
        }

        public void OnPause()
        {
            this.validationFlowOverlay?.OnPause();
        }

        public void OnResume()
        {
            if (this.HasCameraPermission)
            {
                this.validationFlowOverlay?.OnResume();
            }
        }
    }
}
