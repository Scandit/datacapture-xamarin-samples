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
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Core.UI.Style;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class NamedBrush : Enumeration
    {
        public static readonly NamedBrush Default = new NamedBrush(0, "Default", BrushExtensions.DefaultBrush);
        public static readonly NamedBrush Red = new NamedBrush(1, "Red", BrushExtensions.RedBrush);
        public static readonly NamedBrush Green = new NamedBrush(2, "Green", BrushExtensions.GreenBrush);
        
        public NamedBrush(int id, string name, Brush brush) : base(id, name)
        {
            this.Brush = brush;
        }

        public Brush Brush { get; }

        public static NamedBrush Create(Brush brush)
        {
            if (brush.StrokeColor.Equals(BrushExtensions.RedBrush.StrokeColor))
            {
                return Red;
            }
            else if (brush.StrokeColor.Equals(BrushExtensions.GreenBrush.StrokeColor))
            {
                return Green;
            }
            return Default;
        }
    }
}
