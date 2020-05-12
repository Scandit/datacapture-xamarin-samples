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

using System.Threading.Tasks;
using Android.OS;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Base.MeasureUnits;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Rectangular
{
    public class LocationRectangularHeightMeasureUnitFragment : MeasureUnitFragment
    {
        private LocationRectangularHeightMeasureUnitViewModel viewModel;

        public static LocationRectangularHeightMeasureUnitFragment Create()
        {
            return new LocationRectangularHeightMeasureUnitFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(LocationRectangularHeightMeasureUnitViewModel))) as LocationRectangularHeightMeasureUnitViewModel;
        }

        public override FloatWithUnit CurrentFloatWithUnit => this.viewModel.CurrentHeight;

        protected override string GetTitle() => this.Context.GetString(Resource.String.height);

        protected override async Task UpdateValueAsync(float value)
        {
            await this.viewModel.UpdateHeightValueAsync(value);
            this.RefreshMeasureUnitAdapterData();
        }

        protected override async Task UpdateValueAsync(MeasureUnit value)
        {
            await this.viewModel.UpdateHeightMeasureUnitAsync(value);
            this.RefreshMeasureUnitAdapterData();
        }
    }
}