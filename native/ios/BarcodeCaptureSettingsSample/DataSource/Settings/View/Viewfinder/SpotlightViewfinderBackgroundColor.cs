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
    public class SpotlightViewfinderBackgroundColor : Enumeration
    {
        public static readonly SpotlightViewfinderBackgroundColor Default = new SpotlightViewfinderBackgroundColor(0, "Default", SpotlightViewfinder.Create().BackgroundColor);
        public static readonly SpotlightViewfinderBackgroundColor Blue = new SpotlightViewfinderBackgroundColor(1, "Blue", new UIColor(red: 0.4f, green: 0.8f, blue: 0.8f, alpha: 0.6f));
        public static readonly SpotlightViewfinderBackgroundColor Black = new SpotlightViewfinderBackgroundColor(2, "Green", new UIColor(red: 0.4f, green: 0.6f, blue: 0.2f, alpha: 0.6f));

        public UIColor UIColor { get; }

        public SpotlightViewfinderBackgroundColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }
}
