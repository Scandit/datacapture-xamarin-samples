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

namespace BarcodeCaptureSettingsSample.Settings.Views.PointOfInterests
{
    public class PointOfInterestYFragment : MeasureUnitFragment
    {
        private PointOfInterestYViewModel viewModel;

        public static PointOfInterestYFragment Create()
        {
            return new PointOfInterestYFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(PointOfInterestYViewModel))) as PointOfInterestYViewModel;
        }

        public override FloatWithUnit CurrentFloatWithUnit => this.viewModel.PointOfInterestY;

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString(Resource.String.y);

        protected override Task UpdateValueAsync(float value)
        {
            var currentUnit = this.viewModel.PointOfInterestY.Unit;
            this.viewModel.PointOfInterestY = new FloatWithUnit(value, currentUnit);
            return Task.CompletedTask;
        }

        protected override Task UpdateValueAsync(MeasureUnit value)
        {
            var currentValue = this.viewModel.PointOfInterestY.Value;
            this.viewModel.PointOfInterestY = new FloatWithUnit(currentValue, value);
            return Task.CompletedTask;
        }
    }
}