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
using System.Collections.Generic;
using BarcodeCaptureSettingsSample.Base.UiColors;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeAimer : ViewfinderType
    {
        public static class FrameColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { Default, UiColor.Blue, UiColor.Red });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using AimerViewfinder aimerViewfinder = AimerViewfinder.Create();
                return new UiColor(aimerViewfinder.FrameColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static class DotColors
        {
            private static readonly Lazy<IList<UiColor>> colors = new Lazy<IList<UiColor>>(() => new[] { Default, UiColor.Blue, UiColor.Red });
            private static readonly Lazy<UiColor> defaultColor = new Lazy<UiColor>(() =>
            {
                using AimerViewfinder aimerViewfinder = AimerViewfinder.Create();
                return new UiColor(aimerViewfinder.DotColor, Resource.String._default);
            });

            public static IList<UiColor> Colors { get { return colors.Value; } }

            public static UiColor Default { get { return defaultColor.Value; } }
        }

        public static ViewfinderTypeAimer FromCurrentViewfinderAndSettings(
                IViewfinder currentViewfinder, SettingsManager settingsManager)
        {
            return new ViewfinderTypeAimer(
                    currentViewfinder is AimerViewfinder,
                    settingsManager.AimerViewfinderFrameColor,
                    settingsManager.AimerViewfinderDotColor
            );
        }

        public UiColor FrameColor { get; set; }

        public UiColor DotColor { get; set; }

        private ViewfinderTypeAimer(
                bool enabled,
                UiColor frameColor,
                UiColor dotColor) : base(Resource.String.aimer, enabled)
        {        
            this.FrameColor = frameColor;
            this.DotColor = dotColor;
        }

        public override IViewfinder Build()
        {        
            AimerViewfinder viewfinder = AimerViewfinder.Create();
            viewfinder.FrameColor = this.FrameColor.Color;
            viewfinder.DotColor = this.DotColor.Color;
            return viewfinder;
        }
    }
}
