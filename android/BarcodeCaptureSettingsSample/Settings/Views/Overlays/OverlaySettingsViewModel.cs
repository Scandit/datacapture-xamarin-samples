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

using AndroidX.Lifecycle;
using Android.Graphics;
using System.Collections.Generic;
using Scandit.DataCapture.Core.UI.Style;
using System.Linq;
using Scandit.DataCapture.Barcode.UI.Overlay;

namespace BarcodeCaptureSettingsSample.Settings.Views.Overlays
{
    public class OverlaySettingsViewModel : ViewModel
    {
        private static readonly int Red = Color.ParseColor("#FFFF0000");
        private static readonly int TransparentRed = Color.ParseColor("#33FF0000");
        private static readonly int Green = Color.ParseColor("#FF00FF00");
        private static readonly int TransparentGreen = Color.ParseColor("#3300FF00");

        private readonly SettingsManager settingsManager = SettingsManager.Instance;

        public IList<OverlaySettingsBrush> AvailableBrushes { get; private set; }

        public OverlayStyleEntry[] Entries
        {
            get => new[]
            {
                new OverlayStyleEntry(
                    BarcodeCaptureOverlayStyle.Legacy,
                    settingsManager.OverlayStyle == BarcodeCaptureOverlayStyle.Legacy
                ),
                new OverlayStyleEntry(
                    BarcodeCaptureOverlayStyle.Frame,
                    settingsManager.OverlayStyle == BarcodeCaptureOverlayStyle.Frame
                ),
            };
        }

        public OverlaySettingsViewModel()
        {
            float strokeWidth = this.settingsManager.DefaultBrush.StrokeWidth;
            this.AvailableBrushes = new List<OverlaySettingsBrush> 
            {
                new OverlaySettingsBrush(this.settingsManager.DefaultBrush, Resource.String._default),
                new OverlaySettingsBrush(TransparentRed, Red, strokeWidth, Resource.String.red),
                new OverlaySettingsBrush(TransparentGreen, Green, strokeWidth, Resource.String.green)
            };
        }

        public OverlaySettingsBrush CurrentBrush
        {
            get
            {
                return this.GetSettingsBrush(this.settingsManager.CurrentBrush) ?? this.AvailableBrushes[0];
            }
            set
            {
                Brush brush = this.GetSettingsBrush(value.Brush)?.Brush ?? this.AvailableBrushes[0].Brush;
                this.settingsManager.CurrentBrush = brush;
            }
        }

        private OverlaySettingsBrush GetSettingsBrush(Brush brush)
        {
            static bool eq(Brush first, Brush other)
            {
                if (first == null || other == null)
                {
                    return false;
                }

                return first.FillColor == other.FillColor &&
                       first.StrokeColor == other.StrokeColor &&
                       first.StrokeWidth == other.StrokeWidth;
            };

            return this.AvailableBrushes.Where(item => eq(item.Brush, brush)).FirstOrDefault();
        }

        public OverlayStyleEntry CurrentStyle
        {
            get => this.Entries.First((style) => style.enabled);
            set => settingsManager.OverlayStyle = value.style;
        }
    }
}
