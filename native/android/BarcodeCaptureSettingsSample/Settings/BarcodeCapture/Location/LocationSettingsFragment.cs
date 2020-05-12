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
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Radius;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.Rectangular;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location
{
    public class LocationSettingsFragment : NavigationFragment
    {
        private LocationSettingsViewModel viewModel;

        private View containerRadius, containerRectangular;
        private View containerRadiusSize, containerRectangularSpecification, containerRectangularWidth,
                containerRectangularHeight, containerRectangularWidthAspect,
                containerRectangularHeightAspect;
        private EditText editRectangularHeightAspect, editRectangularWidthAspect;
        private TextView textLocationType, textRadiusSize, textRectangularSizeSpecification,
                textRectangularWidth, textRectangularHeight;
        private LocationTypeAdapter adapter;

        public static LocationSettingsFragment Create()
        {
            return new LocationSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(LocationSettingsViewModel))) as LocationSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_location_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            RecyclerView recyclerLocationType = view.FindViewById<RecyclerView>(Resource.Id.recycler_location_type);
            recyclerLocationType.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            async Task onClickCallback(LocationType entry)
            {
                await this.viewModel.SetLocationTypeAsync(entry);
                this.ShowHideSubSettings();
                this.RefreshLocationTypesAdapterData();
            }
            this.adapter = new LocationTypeAdapter(this.viewModel.AllowedLocationTypes, onClickCallback);
            recyclerLocationType.SetAdapter(adapter);

            this.textLocationType = view.FindViewById<TextView>(Resource.Id.text_location_type);
            this.containerRadius = view.FindViewById<View>(Resource.Id.card_location_radius);
            this.containerRadiusSize = view.FindViewById<View>(Resource.Id.radius_size_container);
            this.textRadiusSize = view.FindViewById<TextView>(Resource.Id.text_radius_size);
            this.containerRectangular = view.FindViewById<View>(Resource.Id.card_location_rectangular_size);
            this.containerRectangularSpecification = view.FindViewById<View>(Resource.Id.container_rectangular_size_specification);
            this.containerRectangularWidth = view.FindViewById<View>(Resource.Id.container_rectangular_width);
            this.containerRectangularHeight = view.FindViewById<View>(Resource.Id.container_rectangular_height);
            this.containerRectangularWidthAspect = view.FindViewById<View>(Resource.Id.container_rectangular_width_aspect);
            this.containerRectangularHeightAspect = view.FindViewById<View>(Resource.Id.container_rectangular_height_aspect);
            this.editRectangularHeightAspect = view.FindViewById<EditText>(Resource.Id.edit_rectangular_height);
            this.editRectangularWidthAspect = view.FindViewById<EditText>(Resource.Id.edit_rectangular_width);
            this.textRectangularSizeSpecification = view.FindViewById<TextView>(Resource.Id.text_rectangular_size_specification);
            this.textRectangularWidth = view.FindViewById<TextView>(Resource.Id.text_rectangular_width);
            this.textRectangularHeight = view.FindViewById<TextView>(Resource.Id.text_rectangular_height);

            this.SetupListeners();
            this.ShowHideSubSettings();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)BarcodeCaptureSettingsType.LocationSelection);

        private void RefreshLocationTypesAdapterData()
        {
            this.adapter.UpdateData(this.viewModel.AllowedLocationTypes);
        }

        private void SetupListeners()
        {
            this.containerRadiusSize.Click += (object sender, EventArgs args) =>
            {
                this.GoToRadiusSizeFragment();
            };

            this.containerRectangularSpecification.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowRectangularSizeSpecificationMenu();
            };

            this.containerRectangularWidth.Click += (object sender, EventArgs args) =>
            {
                this.GoToRectangularWidthFragment();
            };

            this.containerRectangularHeight.Click += (object sender, EventArgs args) =>
            {
                this.GoToRectangularHeightFragment();
            };

            this.editRectangularWidthAspect.EditorAction += async (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    await this.ApplyWidthChangeAsync(this.editRectangularWidthAspect.Text);
                    this.DismissKeyboard(this.editRectangularWidthAspect);
                    this.editRectangularWidthAspect.ClearFocus();
                }
            };

            this.editRectangularHeightAspect.EditorAction += async (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    await this.ApplyHeightChangeAsync(this.editRectangularHeightAspect.Text);
                    this.DismissKeyboard(this.editRectangularHeightAspect);
                    this.editRectangularHeightAspect.ClearFocus();
                }
            };
        }

        private void GoToRadiusSizeFragment()
        {
            this.MoveToFragment(LocationRadiusMeasureUnitFragment.Create(), true, null);
        }

        private void GoToRectangularWidthFragment()
        {
            this.MoveToFragment(LocationRectangularWidthMeasureUnitFragment.Create(), true, null);
        }

        private void GoToRectangularHeightFragment()
        {
            this.MoveToFragment(LocationRectangularHeightMeasureUnitFragment.Create(), true, null);
        }

        private void ShowHideSubSettings()
        {
            switch (this.viewModel.GetCurrentLocationType())
            {
                case LocationTypeRadius typeRadius:
                    this.SetupForRadiusLocationSelection(typeRadius);
                    break;
                case LocationTypeRectangular typeRectangular:
                    this.SetupForRectangularLocationSelection(typeRectangular);
                    break;
                default:
                    this.SetupForNoLocationSelection();
                    break;
            }
        }

        private void SetupForNoLocationSelection()
        {
            this.textLocationType.Visibility = ViewStates.Gone;
            this.containerRadius.Visibility = ViewStates.Gone;
            this.containerRectangular.Visibility = ViewStates.Gone;
        }

        private void SetupForRadiusLocationSelection(LocationTypeRadius location)
        {
            this.textLocationType.Visibility = ViewStates.Visible;
            this.containerRadius.Visibility = ViewStates.Visible;
            this.containerRectangular.Visibility = ViewStates.Gone;

            this.RefreshRadiusLocationData(location);
        }

        private void SetupForRectangularLocationSelection(LocationTypeRectangular location)
        {
            this.textLocationType.Visibility = ViewStates.Visible;
            this.containerRadius.Visibility = ViewStates.Gone;
            this.containerRectangular.Visibility = ViewStates.Visible;

            switch (location.SizeSpecification)
            {
                case SizeSpecification.WidthAndHeight:
                    this.containerRectangularHeight.Visibility = ViewStates.Visible;
                    this.containerRectangularWidth.Visibility = ViewStates.Visible;
                    this.containerRectangularHeightAspect.Visibility = ViewStates.Gone;
                    this.containerRectangularWidthAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    this.containerRectangularHeight.Visibility = ViewStates.Visible;
                    this.containerRectangularWidth.Visibility = ViewStates.Gone;
                    this.containerRectangularHeightAspect.Visibility = ViewStates.Gone;
                    this.containerRectangularWidthAspect.Visibility = ViewStates.Visible;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    this.containerRectangularHeight.Visibility = ViewStates.Gone;
                    this.containerRectangularWidth.Visibility = ViewStates.Visible;
                    this.containerRectangularHeightAspect.Visibility = ViewStates.Visible;
                    this.containerRectangularWidthAspect.Visibility = ViewStates.Gone;
                    break;
            }

            this.RefreshRectangularLocationData(location);
        }

        private void RefreshRadiusLocationData(LocationTypeRadius location)
        {
            this.textLocationType.SetText(location.DisplayNameResourceId);
            string textFormat = this.Context.GetString(Resource.String.size_with_unit);
            this.textRadiusSize.Text = string.Format(textFormat, location.Radius, location.MeasureUnit.Name());
        }

        private void RefreshRectangularLocationData(LocationTypeRectangular location)
        {
            this.textLocationType.SetText(location.DisplayNameResourceId);
            this.textRectangularSizeSpecification.Text = this.Context.GetString((int)location.SizeSpecification);
            this.textRectangularWidth.Text = location.Width.GetStringWithUnit(this.Context);
            this.textRectangularHeight.Text = location.Height.GetStringWithUnit(this.Context);
            var textFormat = this.Context.GetString(Resource.String.size_no_unit);
            this.editRectangularWidthAspect.Text = string.Format(textFormat, location.WidthAspectRatio);
            this.editRectangularHeightAspect.Text = string.Format(textFormat, location.HeightAspectRatio);
        }

        private void BuildAndShowRectangularSizeSpecificationMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerRectangularSpecification, GravityFlags.End);

            System.Collections.IList sizeSpecificationItems = Enum.GetValues(typeof(SizeSpecification));
            for (int i = 0; i < sizeSpecificationItems.Count; i++)
            {
                int itemId = (int)sizeSpecificationItems[i];
                menu.Menu.Add(0, itemId, i, this.Context.GetString(itemId));
            }

            menu.MenuItemClick += async (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedSizeSpec = args.Item.ItemId;
                await this.viewModel.SetRectangularLocationSizeSpecAsync((SizeSpecification)selectedSizeSpec);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private async Task ApplyHeightChangeAsync(string text)
        {
            if (float.TryParse(text, out float result))
            {
                await this.viewModel.SetRectangularLocationHeightAspectAsync(result);
                this.ShowHideSubSettings();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }
        
        private async Task ApplyWidthChangeAsync(string text)
        {
            if (float.TryParse(text, out float result))
            {
                await this.viewModel.SetRectangularLocationWidthAspectAsync(result);
                this.ShowHideSubSettings();
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
    }
}
