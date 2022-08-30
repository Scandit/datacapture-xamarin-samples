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

using System;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public static class UnitExtenstions
    {
        private static readonly FloatWithUnit FullFraction = new FloatWithUnit { Value = 1, Unit = MeasureUnit.Fraction };
        public static readonly SizeWithUnit FullSize = new SizeWithUnit { Width = FullFraction, Height = FullFraction };

        public static MarginsWithUnit NewWithTop(this MarginsWithUnit margins, FloatWithUnit topMargin)
        {
            return new MarginsWithUnit()
            {
                Left = margins.Left,
                Top = topMargin,
                Right = margins.Right,
                Bottom = margins.Bottom
            };
        }

        public static MarginsWithUnit NewWithRight(this MarginsWithUnit margins, FloatWithUnit rightMargin)
        {
            return new MarginsWithUnit()
            {
                Left = margins.Left,
                Top = margins.Top,
                Right = rightMargin,
                Bottom = margins.Bottom
            };
        }

        public static MarginsWithUnit NewWithBottom(this MarginsWithUnit margins, FloatWithUnit bottomMargin)
        {
            return new MarginsWithUnit()
            {
                Left = margins.Left,
                Top = margins.Top,
                Right = margins.Right,
                Bottom = bottomMargin
            };
        }

        public static MarginsWithUnit NewWithLeft(this MarginsWithUnit margins, FloatWithUnit leftMargin)
        {
            return new MarginsWithUnit()
            {
                Left = leftMargin,
                Top = margins.Top,
                Right = margins.Right,
                Bottom = margins.Bottom
            };
        }

        public static PointWithUnit NewWithX(this PointWithUnit pointWithUnit, FloatWithUnit x)
        {
            return new PointWithUnit()
            {
                X = x,
                Y = pointWithUnit.Y
            };
        }

        public static PointWithUnit NewWithY(this PointWithUnit pointWithUnit, FloatWithUnit y)
        {
            return new PointWithUnit()
            {
                X = pointWithUnit.X,
                Y = y
            };
        }

        public static string GetValueString(this object obj, int decimalPlaces = 2)
        {
            return obj switch
            {
                null => string.Empty,
                nfloat value => NumberFormatter.Instance.FormatNFloat(value, decimalPlaces),
                _ => obj.ToString(),
            };
        }
    }
}
