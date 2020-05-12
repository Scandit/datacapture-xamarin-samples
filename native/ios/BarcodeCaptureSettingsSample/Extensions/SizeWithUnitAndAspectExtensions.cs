﻿/*
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

using BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public static class SizeWithUnitAndAspectExtensions
    {
        public static RectangularSizeSpecification SizeSpecification(this SizeWithUnitAndAspect sizeWithUnitAndAspect) => sizeWithUnitAndAspect.SizingMode switch
        {
            SizingMode.WidthAndHeight => RectangularSizeSpecification.WidthAndHeight,
            SizingMode.WidthAndAspectRatio => RectangularSizeSpecification.WidthAndHeightAspect,
            SizingMode.HeightAndAspectRatio => RectangularSizeSpecification.HeightAndWidthAspect,
            _ => RectangularSizeSpecification.WidthAndHeight,
        };
    }
}
