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
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class RectangularViewfinderLineStyleType : Enumeration
    {
        public static readonly RectangularViewfinderLineStyleType Light = new RectangularViewfinderLineStyleType(RectangularViewfinderLineStyle.Light);
        public static readonly RectangularViewfinderLineStyleType Bold = new RectangularViewfinderLineStyleType(RectangularViewfinderLineStyle.Bold);

        public RectangularViewfinderLineStyle LineStyle { get; }

        public RectangularViewfinderLineStyleType(RectangularViewfinderLineStyle lineStyle) : base((int)lineStyle, lineStyle.ToString())
        {
            this.LineStyle = lineStyle;
        }
    }
}
