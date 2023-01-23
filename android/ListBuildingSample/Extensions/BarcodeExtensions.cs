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
using Android.Graphics;
using Scandit.DataCapture.Barcode.Data;
using Point = Scandit.DataCapture.Core.Common.Geometry.Point;

namespace ListBuildingSample.Extensions
{
    public static class BarcodeExtensions
    {
        public static float CroppedImagePadding = 1.2f;

        public static Bitmap? GetBarcodeImage(this Barcode barcode, Bitmap? frame)
        {
            if (frame == null)
            {
                return null;
            }

            // safety check when input bitmap is too small
            if (frame.Width == 1 && frame.Height == 1)
            {                
                return frame;
            }

            var points = new List<Point>
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

            var center = new Point((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            var largerSize = Math.Max(maxY - minY, maxX - minX);

            int height = (int)(largerSize * CroppedImagePadding);
            int width = (int)(largerSize * CroppedImagePadding);

            int x = (int)(center.X - largerSize * (CroppedImagePadding / 2));
            int y = (int)(center.Y - largerSize * (CroppedImagePadding / 2));

            if ((y + height) > frame.Height) // safety check
            {
                y -= (y + height) - frame.Height;
            }

            if ((x + width) > frame.Width) // safety check
            {
                x -= (x + width) - frame.Width;
            }

            if (y <= 0 || x <= 0)
            {
                return frame;
            }

            Matrix matrix = new Matrix();
            matrix.PostRotate(90f);

            return Bitmap.CreateBitmap(frame, x, y, width, height, matrix, true);
        }
    }
}
