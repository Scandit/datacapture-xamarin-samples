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

namespace BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Right
{
    public class ScanAreaRightMarginFragment : MeasureUnitFragment
    {
        private ScanAreaRightMarginViewModel viewModel;

        public static ScanAreaRightMarginFragment Create()
        {
            return new ScanAreaRightMarginFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ScanAreaRightMarginViewModel))) as ScanAreaRightMarginViewModel;
        }

        public override FloatWithUnit CurrentFloatWithUnit => this.viewModel.Margin;

        protected override string GetTitle() => this.Context.GetString(Resource.String.right);

        protected override Task UpdateValueAsync(float value)
        {
            this.viewModel.SetMarginValue(value);
            this.RefreshMeasureUnitAdapterData();
            return Task.CompletedTask;
        }

        protected override Task UpdateValueAsync(MeasureUnit value)
        {
            this.viewModel.SetMarginUnit(value);
            this.RefreshMeasureUnitAdapterData();
            return Task.CompletedTask;
        }
    }
}