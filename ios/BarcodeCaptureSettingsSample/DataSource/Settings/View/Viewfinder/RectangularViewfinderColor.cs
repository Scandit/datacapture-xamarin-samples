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

using BarcodeCaptureSettingsSample.DataSource.Other;
using Scandit.DataCapture.Core.UI.Viewfinder;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class RectangularViewfinderColor : Enumeration
    {
        public static readonly RectangularViewfinderColor Default =
            new RectangularViewfinderColor(0, "Default", RectangularViewfinder.Create(RectangularViewfinderStyle.Rounded).Color);
        public static readonly RectangularViewfinderColor Blue = new RectangularViewfinderColor(1, "Blue", UIColor.Blue);
        public static readonly RectangularViewfinderColor Black = new RectangularViewfinderColor(2, "Black", UIColor.Black);

        public UIColor UIColor { get; }

        public RectangularViewfinderColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }
}
