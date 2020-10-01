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
    public class ViewfinderKind : Enumeration
    {
        public static readonly ViewfinderKind None = new ViewfinderKind(0, "None", null);
        public static readonly ViewfinderKind Rectangular = new ViewfinderKind(1, "Rectangular", RectangularViewfinder.Create());
        public static readonly ViewfinderKind Laserline = new ViewfinderKind(2, "Laserline", LaserlineViewfinder.Create());
        public static readonly ViewfinderKind Spotlight = new ViewfinderKind(3, "Spotlight", SpotlightViewfinder.Create());

        public ViewfinderKind(int key, string value, IViewfinder viewfinder) : base(key, value)
        {
            this.Viewfinder = viewfinder;
        }

        public IViewfinder Viewfinder { get; }
    }
}
