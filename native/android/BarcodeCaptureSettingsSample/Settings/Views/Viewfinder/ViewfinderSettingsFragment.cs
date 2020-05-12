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
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Base.UiColors;
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder
{
    public class ViewfinderSettingsFragment : NavigationFragment
    {
        private ViewfinderSettingsViewModel viewModel;

        private RecyclerView recyclerViewfinderTypes;
        private View cardColor, cardSpecifications, cardMeasures, cardLaserline;
        private View containerHeight, containerWidth, containerSizeSpec, containerHeightAspect,
                     containerWidthAspect, containerColor, containerEnabledColor, containerDisabledColor,
                     containerLaserlineWidth;
        private TextView textType, textColor, textSizeSpecification, textWidth, textHeight,
                         textEnabledColor, textDisabledColor, textLaserlineWidth;
        private EditText editHeightAspect, editWidthAspect;

        private ViewfinderTypeAdapter adapter;

        public static ViewfinderSettingsFragment Create()
        {
            return new ViewfinderSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ViewfinderSettingsViewModel))) as ViewfinderSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_viewfinder_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.recyclerViewfinderTypes = view.FindViewById<RecyclerView>(Resource.Id.recycler_viewfinder_types);
            this.SetupRecyclerTypes();

            this.cardColor = view.FindViewById<View>(Resource.Id.card_color);
            this.cardSpecifications = view.FindViewById<View>(Resource.Id.card_size_specification);
            this.cardMeasures = view.FindViewById<View>(Resource.Id.card_measures);
            this.cardLaserline = view.FindViewById<View>(Resource.Id.card_laserline);

            this.textType = view.FindViewById<TextView>(Resource.Id.text_viewfinder_type);
            this.textColor = view.FindViewById<TextView>(Resource.Id.text_color);
            this.textSizeSpecification = view.FindViewById<TextView>(Resource.Id.text_size_specification);
            this.textWidth = view.FindViewById<TextView>(Resource.Id.text_width);
            this.textHeight = view.FindViewById<TextView>(Resource.Id.text_height);
            this.editHeightAspect = view.FindViewById<EditText>(Resource.Id.edit_height);
            this.editWidthAspect = view.FindViewById<EditText>(Resource.Id.edit_width);

            this.containerColor = view.FindViewById(Resource.Id.container_color);
            this.containerHeight = view.FindViewById(Resource.Id.container_height);
            this.containerWidth = view.FindViewById(Resource.Id.container_width);
            this.containerHeightAspect = view.FindViewById(Resource.Id.container_height_aspect);
            this.containerWidthAspect = view.FindViewById(Resource.Id.container_width_aspect);
            this.containerSizeSpec = view.FindViewById(Resource.Id.container_size_specification);

            this.containerLaserlineWidth = view.FindViewById<View>(Resource.Id.container_laserline_width);
            this.textLaserlineWidth = view.FindViewById<TextView>(Resource.Id.text_laserline_width);
            this.textEnabledColor = view.FindViewById<TextView>(Resource.Id.text_enabled_color);
            this.textDisabledColor = view.FindViewById<TextView>(Resource.Id.text_disabled_color);
            this.containerEnabledColor = view.FindViewById<View>(Resource.Id.container_enabled_color);
            this.containerDisabledColor = view.FindViewById<View>(Resource.Id.container_disabled_color);

            this.RegisterEventHandlers();
            this.ShowHideSubSettings();
        }

        private void SetupRecyclerTypes()
        {
            this.adapter = new ViewfinderTypeAdapter(this.viewModel.ViewfinderTypes.ToArray(), onClickCallback: (ViewfinderType viewfinder) =>
            {
                this.viewModel.SetViewfinderType(viewfinder);
                this.RefreshRecyclerTypesData();
                this.ShowHideSubSettings();
            });
            this.recyclerViewfinderTypes.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            this.recyclerViewfinderTypes.SetAdapter(this.adapter);
        }

        private void RegisterEventHandlers()
        {
            this.containerColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowColorMenu();
            };

            this.containerHeight.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ViewfinderRectangleHeightMeasureFragment.Create(), true, null);
            };

            this.containerWidth.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ViewfinderRectangleWidthMeasureFragment.Create(), true, null);
            };

            this.containerSizeSpec.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowSizeSpecificationMenu();
            };

            this.editHeightAspect.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyHeightChange(this.editHeightAspect.Text);
                    this.DismissKeyboard(this.editHeightAspect);
                    this.editHeightAspect.ClearFocus();
                }
            };

            this.editWidthAspect.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyWidthChange(this.editWidthAspect.Text);
                    this.DismissKeyboard(this.editWidthAspect);
                    this.editWidthAspect.ClearFocus();
                }
            };

            this.containerLaserlineWidth.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ViewfinderLaserlineWidthMeasureFragment.Create(), true, null);
            };

            this.containerEnabledColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowEnabledColorMenu();
            };

            this.containerDisabledColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowDisabledColorMenu();
            };
        }

        private void ApplyHeightChange(string text)
        {
            if (float.TryParse(text, out float result))
            {

                this.viewModel.SetRectangularViewfinderHeightAspect(result);
                this.ShowHideSubSettings();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ApplyWidthChange(string text)
        {
            if (float.TryParse(text, out float result))
            {
                this.viewModel.SetRectangularViewfinderWidthAspect(result);
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

        private void RefreshRecyclerTypesData()
        {
            this.adapter.UpdateData(this.viewModel.ViewfinderTypes.ToArray());
        }

        private void RefreshRectangularViewfinderData(ViewfinderTypeRectangular viewfinder)
        {
            this.textType.Text = this.Context.GetString(viewfinder.DisplayNameResourceId);
            this.textColor.Text = this.Context.GetString(viewfinder.Color.DisplayNameResourceId);
            this.textSizeSpecification.Text = this.Context.GetString((int)viewfinder.SizeSpecification);

            this.RefreshHeight(viewfinder.Height);
            this.RefreshWidth(viewfinder.Width);
            this.RefreshHeightAspect(viewfinder.HeightAspect);
            this.RefreshWidthAspect(viewfinder.WidthAspect);
        }

        private void RefreshLaserlineViewfinderData(ViewfinderTypeLaserline viewfinder)
        {
            this.textType.Text = this.Context.GetString(viewfinder.DisplayNameResourceId);
            this.textLaserlineWidth.Text = viewfinder.Width.GetStringWithUnit(this.Context);
            this.textEnabledColor.Text = this.Context.GetString(viewfinder.EnabledColor.DisplayNameResourceId);
            this.textDisabledColor.Text = this.Context.GetString(viewfinder.DisabledColor.DisplayNameResourceId);
        }

        private void RefreshHeight(FloatWithUnit height)
        {
            this.textHeight.Text = height.GetStringWithUnit(this.Context);
        }

        private void RefreshWidth(FloatWithUnit width)
        {
            this.textWidth.Text = width.GetStringWithUnit(this.Context);
        }

        private void RefreshHeightAspect(float heightAspect)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            editHeightAspect.Text = string.Format(textFormat, heightAspect);
        }

        private void RefreshWidthAspect(float widthAspect)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            editWidthAspect.Text = string.Format(textFormat, widthAspect);
        }

        private void ShowHideSubSettings()
        {
            ViewfinderType viewfinderType = this.viewModel.GetCurrentViewfinderType();
            if (viewfinderType is ViewfinderTypeNone)
            {
                this.SetupForNoViewfinder();
            }
            else if (viewfinderType is ViewfinderTypeRectangular)
            {
                this.SetupForRectangularViewfinder((ViewfinderTypeRectangular)viewfinderType);
            }
            else if (viewfinderType is ViewfinderTypeLaserline)
            {
                this.SetupForLaserlineViewfinder((ViewfinderTypeLaserline)viewfinderType);
            }
        }

        private void SetupForNoViewfinder()
        {
            this.textType.Visibility = ViewStates.Gone;
            this.cardColor.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Gone;
        }

        private void SetupForRectangularViewfinder(ViewfinderTypeRectangular viewfinder)
        {
            textType.Visibility = ViewStates.Visible;
            cardColor.Visibility = ViewStates.Visible;
            cardSpecifications.Visibility = ViewStates.Visible;
            cardMeasures.Visibility = ViewStates.Visible;
            cardLaserline.Visibility = ViewStates.Gone;

            SizeSpecification spec = viewfinder.SizeSpecification;

            switch (spec)
            {
                case SizeSpecification.WidthAndHeight:
                    containerHeight.Visibility = ViewStates.Visible;
                    containerWidth.Visibility = ViewStates.Visible;
                    containerHeightAspect.Visibility = ViewStates.Gone;
                    containerWidthAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    containerHeight.Visibility = ViewStates.Visible;
                    containerWidth.Visibility = ViewStates.Gone;
                    containerHeightAspect.Visibility = ViewStates.Gone;
                    containerWidthAspect.Visibility = ViewStates.Visible;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    containerHeight.Visibility = ViewStates.Gone;
                    containerWidth.Visibility = ViewStates.Visible;
                    containerHeightAspect.Visibility = ViewStates.Visible;
                    containerWidthAspect.Visibility = ViewStates.Gone;
                    break;
            }

            this.RefreshRectangularViewfinderData(viewfinder);
        }

        private void SetupForLaserlineViewfinder(ViewfinderTypeLaserline viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardColor.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Visible;

            this.RefreshLaserlineViewfinderData(viewfinder);
        }

        private void BuildAndShowSizeSpecificationMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), containerSizeSpec, GravityFlags.End);
            
            System.Collections.IList sizeSpecificationItems = Enum.GetValues(typeof(SizeSpecification));
            for (int i = 0; i < sizeSpecificationItems.Count; i++)
            {
                int itemId = (int)sizeSpecificationItems[i];
                menu.Menu.Add(0, itemId, i, this.Context.GetString(itemId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedSizeSpec = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderSizeSpec((SizeSpecification)selectedSizeSpec);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), containerColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeRectangular.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeRectangular.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderColor(ViewfinderTypeRectangular.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowEnabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), containerEnabledColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeLaserline.EnabledColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeLaserline.EnabledColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetLaserlineViewfinderEnabledColor(ViewfinderTypeLaserline.EnabledColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowDisabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), containerDisabledColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeLaserline.DisabledColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeLaserline.DisabledColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetLaserlineViewfinderDisabledColor(ViewfinderTypeLaserline.DisabledColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.Viewfinder);
    }
}
