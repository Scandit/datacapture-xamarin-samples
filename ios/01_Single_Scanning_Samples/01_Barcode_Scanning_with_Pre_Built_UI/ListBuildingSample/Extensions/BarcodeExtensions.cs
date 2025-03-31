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
using CoreGraphics;
using Scandit.DataCapture.Barcode.Data;
using UIKit;

namespace ListBuildingSample.Extensions
{
    public static class BarcodeExtensions
    {
        public static float CroppedImagePadding = 1.2f;

        public static CGRect GetBarcodeLocation(this Barcode barcode, UIImage? frame)
        {
            if (frame == null)
            {
                return CGRect.Empty;
            }

            // safety check when input bitmap is too small
            if (frame.Size.Width == 1 && frame.Size.Height == 1)
            {
                return new CGRect(0, 0, frame.Size.Width, frame.Size.Height);
            }

            var points = new List<CGPoint>
            {
                barcode.Location.BottomLeft,
                barcode.Location.TopLeft,
                barcode.Location.TopRight,
                barcode.Location.BottomRight
            };

            var minX = points.Min((point) => point.X);
            var minY = points.Min((point) => point.Y);
            var maxX = points.Max((point) => point.X);
            var maxY = points.Max((point) => point.Y);

            var center = new CGPoint((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            var largerSize = Math.Max(maxY - minY, maxX - minX);

            var height = largerSize * CroppedImagePadding;
            var width = largerSize * CroppedImagePadding;

            var x = (center.X - largerSize * (CroppedImagePadding / 2));
            var y = (center.Y - largerSize * (CroppedImagePadding / 2));

            if ((y + height) > frame.Size.Height) // safety check
            {
                y -= (y + height) - frame.Size.Height;
            }

            if ((x + width) > frame.Size.Width) // safety check
            {
                x -= (x + width) - frame.Size.Width;
            }

            if (y <= 0 || x <= 0)
            {
                return new CGRect(0, 0, frame.Size.Width, frame.Size.Height);
            }

            return new CGRect(x, y, width, height);
        }
    }
}
