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

using Android.Content;
using Scandit.DataCapture.Core.Common.Geometry;

namespace MatrixScanBubblesSample.Scan.Bubbles
{
    public class BubbleSizeManager
    {
        private const float ScreenPercentageWidthRequired = 0.1f;
        private readonly float displayWidth;

        public BubbleSizeManager(Context context)
        {
            this.displayWidth = context.Resources.DisplayMetrics.WidthPixels;
        }

        // We want to show the bubble overlay only if the barcode takes >= 10% of the screen width.
        public bool IsBarcodeLargeEnoughForBubble(Quadrilateral barcodeLocation)
        {
            float topRightX = barcodeLocation.TopRight.X;
            float topLeftX = barcodeLocation.TopLeft.X;
            float bottomRightX = barcodeLocation.BottomRight.X;
            float bottomLeftX = barcodeLocation.BottomLeft.X;
            float avgWidth = ((topRightX - bottomLeftX) + (bottomRightX - topLeftX)) / 2;

            return (avgWidth / displayWidth) >= ScreenPercentageWidthRequired;
        }
    }
}