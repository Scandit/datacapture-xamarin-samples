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
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Base.MeasureUnits
{ 
    public abstract class MeasureUnitFragment : NavigationFragment
    {
        private static readonly MeasureUnit[] MeasureUnits = { MeasureUnit.Dip, MeasureUnit.Fraction, MeasureUnit.Pixel };

        protected TextView textValueLabel;
        protected EditText editValue;
        protected RecyclerView recycler;
        protected MeasureUnitAdapter adapter;

        public abstract FloatWithUnit CurrentFloatWithUnit { get; }

        protected virtual Task UpdateValueAsync(float value)
        {
            return Task.CompletedTask;
        }

        protected virtual Task UpdateValueAsync(MeasureUnit value)
        {
            return Task.CompletedTask;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_measure_unit, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.textValueLabel = view.FindViewById<TextView>(Resource.Id.text_value_label);
            this.textValueLabel.SetText(Resource.String.value);
            this.editValue = view.FindViewById<EditText>(Resource.Id.edit_value);
            this.editValue.EditorAction += async (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    await this.ApplyChangeAsync(this.editValue.Text);
                    this.DismissKeyboard(this.editValue);
                    this.editValue.ClearFocus();
                }
            };
            async Task onClickCallback(MeasureUnitItem entry)
            {
                await this.UpdateValueAsync(entry.MeasureUnit);
            }
            this.adapter = new MeasureUnitAdapter(this.GetItems(), onClickCallback);
            this.recycler = view.FindViewById<RecyclerView>(Resource.Id.recycler_measure_units);
            this.recycler.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            this.recycler.SetAdapter(this.adapter);
            this.DisplayCurrentValue();
        }

        private async Task ApplyChangeAsync(string text)
        {
            if (float.TryParse(text, out float result))
            {
                await this.UpdateValueAsync(result);
            }
            else
            { 
                this.ShowInvalidNumberToast();
            }
        }

        private void ShowInvalidNumberToast()
        {
            Toast.MakeText(this.RequireContext(), Resource.String.number_not_valid, ToastLength.Long).Show();
        }

        protected void DisplayCurrentValue()
        {
            this.editValue.Text = this.CurrentFloatWithUnit.GetStringWithoutUnit(this.Context);
        }

        protected void RefreshMeasureUnitAdapterData()
        {
            this.DisplayCurrentValue();
            this.adapter.UpdateData(this.GetItems());
        }

        protected override bool ShouldShowBackButton() => true;

        private MeasureUnitItem[] GetItems()
        {
            MeasureUnitItem selector(MeasureUnit unit)
            {
                return new MeasureUnitItem(unit, unit.Equals(this.CurrentFloatWithUnit.Unit));
            }

            return MeasureUnits.Select(selector).ToArray();
        }
    }
}