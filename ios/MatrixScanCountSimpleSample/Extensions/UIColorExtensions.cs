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

using System.Drawing;
using CoreGraphics;
using UIKit;

namespace MatrixScanCountSimpleSample.Extensions
{
    public static class UIColorExtensions
    {
        public static UIImage ToImage(this UIColor color, int width = 1, int height = 1)
        {
            var rect = new CGRect(x: 0.0f, y: 0.0f, width: width, height: height);
            UIGraphics.BeginImageContext(rect.Size);
            var context = UIGraphics.GetCurrentContext();

            context.SetFillColor(color.CGColor);
            context.FillRect(rect);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }
    }
}
