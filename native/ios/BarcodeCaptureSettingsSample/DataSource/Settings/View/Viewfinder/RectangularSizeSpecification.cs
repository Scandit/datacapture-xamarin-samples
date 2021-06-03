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

using BarcodeCaptureSettingsSample.DataSource.Other;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class RectangularSizeSpecification : Enumeration
    {
        public static readonly RectangularSizeSpecification WidthAndHeight = new RectangularSizeSpecification(0, "Width and Height");
        public static readonly RectangularSizeSpecification WidthAndHeightAspect = new RectangularSizeSpecification(1, "Width and Height Aspect");
        public static readonly RectangularSizeSpecification HeightAndWidthAspect = new RectangularSizeSpecification(2, "Height and Width Aspect");
        public static readonly RectangularSizeSpecification ShorterDimensionAndAspectRatio = new RectangularSizeSpecification(3, "Shorter Dimension and Aspect");

        public RectangularSizeSpecification(int key, string name) : base(key, name) { }
    }
}
