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

using Android.Views;
using MatrixScanBubblesSample.Scan.Bubbles.Data;
using Scandit.DataCapture.Barcode.Tracking.Data;

namespace MatrixScanBubblesSample.Scan
{
    public interface IScanViewModelListener
    {
        bool ShouldShowBubble(TrackedBarcode barcode);

        View GetOrCreateViewForBubbleData(TrackedBarcode barcode, BubbleData bubbleData, bool visible);

        void SetBubbleVisibility(TrackedBarcode barcode, bool visible);

        void RemoveBubbleView(int identifier);

        void OnFrozenChanged(bool frozen);
    }
}
