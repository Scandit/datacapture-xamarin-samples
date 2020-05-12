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

using Android.OS;
using Android.Views;
using AndroidX.Lifecycle;
using Java.Util.Concurrent.Atomic;
using MatrixScanBubblesSample.Models;
using MatrixScanBubblesSample.Scan.Bubbles.Data;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.Data;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Style;

namespace MatrixScanBubblesSample.Scan
{
    public class ScanViewModel : ViewModel,
                                 IBarcodeTrackingListener,
                                 IBarcodeTrackingAdvancedOverlayListener
    {
        private readonly DataCaptureManager dataCaptureManager = DataCaptureManager.Instance;

        private readonly BubbleDataProvider bubbleDataProvider = new BubbleDataProvider();
        private readonly Handler mainHandler = new Handler(Looper.MainLooper);
        private readonly AtomicBoolean frozen = new AtomicBoolean(false);
        private IScanViewModelListener listener;

        public ScanViewModel()
        {
            // Register self as a listener to get informed whenever the tracking session is updated.
            this.dataCaptureManager.BarcodeTracking.AddListener(this);
        }

        public DataCaptureContext GetDataCaptureContext() => this.dataCaptureManager.DataCaptureContext;

        public BarcodeTracking BarcodeTracking => this.dataCaptureManager.BarcodeTracking;

        public Brush DefaultBrush => this.dataCaptureManager.DefaultBrush;

        public Camera Camera => this.dataCaptureManager.Camera;

        public void SetListener(IScanViewModelListener listener) => this.listener = listener;

        public void ResumeScanning()
        {
            if (!this.IsFrozen())
            {
                this.ResumeScanningInternal();
            }            
        }

        public void PauseScanning()
        {
            if (!this.IsFrozen())
            {
                this.PauseScanningInternal();
            }
        }

        public void StartFrameSource()
        {
            if (!this.IsFrozen())
            {
                this.StartFrameSourceInternal();
            }            
        }

        public void StopFrameSource()
        {
            if (!this.IsFrozen())
            {
                this.StopFrameSourceInternal();
            }            
        }

        public void ToggleFreeze()
        {
            if (!this.IsFrozen())
            {
                this.FreezeInternal();
            }
            else
            {
                this.UnfreezeInternal();
            }
        }

        public bool IsFrozen()
        {
            return this.frozen.Get();
        }

        #region IBarcodeTrackingListener
        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        { }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        { }

        public void OnSessionUpdated(
                    BarcodeTracking barcodeTracking,
                    BarcodeTrackingSession session,
                    IFrameData data)
        {
            if (this.IsFrozen() || this.listener == null)
            {
                return;
            }

            foreach (int identifier in session.RemovedTrackedBarcodes)
            {
                this.RemoveBubbleViewForIdentifierOnMainThread(identifier);
            }

            foreach (TrackedBarcode trackedBarcode in session.TrackedBarcodes.Values)
            {
                if (!string.IsNullOrEmpty(trackedBarcode.Barcode.Data))
                {
                    // We show or hide the bubble depending on its size compared to the device screen.
                    this.SetBubbleVisibilityOnMainThread(trackedBarcode, this.listener.ShouldShowBubble(trackedBarcode));
                }
            }
        }
        #endregion

        #region IBarcodeTrackingAdvancedOverlayListener
        public View ViewForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            return this.listener?.GetOrCreateViewForBubbleData(
                                      trackedBarcode,
                                      this.bubbleDataProvider.GetDataForBarcode(trackedBarcode.Barcode.Data),
                                      this.listener.ShouldShowBubble(trackedBarcode));
        }

        public Anchor AnchorForTrackedBarcode(
                    BarcodeTrackingAdvancedOverlay overlay,
                    TrackedBarcode trackedBarcode)
        {
            return Anchor.TopCenter;
        }

        public PointWithUnit OffsetForTrackedBarcode(
                    BarcodeTrackingAdvancedOverlay overlay,
                    TrackedBarcode trackedBarcode,
                    View view)
        {
            // We want to center the view on top of the barcode.
            return new PointWithUnit(
                    new FloatWithUnit(0f, MeasureUnit.Fraction),
                    new FloatWithUnit(-1f, MeasureUnit.Fraction));
        }
        #endregion

        protected override void OnCleared()
        {
            base.OnCleared();
            this.dataCaptureManager.BarcodeTracking.RemoveListener(this);
        }

        private void FreezeInternal()
        {
            this.frozen.Set(true);
            this.PauseScanningInternal();
            this.StopFrameSourceInternal();
            this.NotifyFrozenListenerInternal();
        }

        private void UnfreezeInternal()
        {
            this.frozen.Set(false);
            this.StartFrameSourceInternal();
            this.ResumeScanningInternal();
            this.NotifyFrozenListenerInternal();
        }

        private void StartFrameSourceInternal()
        {
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private void ResumeScanningInternal()
        {
            this.BarcodeTracking.Enabled = true;
        }

        private void PauseScanningInternal()
        {
            this.BarcodeTracking.Enabled = false;
        }

        private void StopFrameSourceInternal()
        {
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        private void NotifyFrozenListenerInternal()
        {
            this.listener?.OnFrozenChanged(this.frozen.Get());
        }

        private void SetBubbleVisibilityOnMainThread(TrackedBarcode barcode, bool visible)
        {
            this.mainHandler.Post(() =>
            {
                this.listener?.SetBubbleVisibility(barcode, visible);
            });
        }

        private void RemoveBubbleViewForIdentifierOnMainThread(int identifier)
        {
            this.mainHandler.Post(() => 
            {
                this.listener?.RemoveBubbleView(identifier);
            });
        }
    }
}