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
using Xamarin.Forms;

namespace StockCountSample.Bridging
{
    public class DataCaptureView : Xamarin.Forms.View
    {
        public EventHandler PauseScanningRequested;
        public EventHandler ResumeScanningRequested;

        public static readonly BindableProperty LicenseKeyProperty = BindableProperty.Create(
            propertyName: "LicenseKey",
            returnType: typeof(string),
            declaringType: typeof(DataCaptureView)
        );

        public string LicenseKey
        {
            get => (string)this.GetValue(LicenseKeyProperty);
            set => this.SetValue(LicenseKeyProperty, value);
        }

        public static readonly BindableProperty TrackedBarcodesBrushProperty = BindableProperty.Create(
            propertyName: "TrackedBarcodesBrush",
            returnType: typeof(Brush),
            declaringType: typeof(DataCaptureView),
            defaultValue: Brush.Default
        );

        public static readonly BindableProperty NonEmptyListBrushProperty = BindableProperty.Create(
            propertyName: "NonEmptyListBrushProperty",
            returnType: typeof(Brush),
            declaringType: typeof(DataCaptureView),
            defaultValue: new Brush(Color.Green.MultiplyAlpha(0.3), Color.Green, 2)
        );

        public Brush TrackedBarcodesBrush
        {
            get => (Brush)this.GetValue(TrackedBarcodesBrushProperty);
            set => this.SetValue(TrackedBarcodesBrushProperty, value);
        }

        public void OnSleep()
        {
            this.PauseScanningRequested?.Invoke(this, EventArgs.Empty);
        }

        public void OnResume()
        {
            this.ResumeScanningRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
