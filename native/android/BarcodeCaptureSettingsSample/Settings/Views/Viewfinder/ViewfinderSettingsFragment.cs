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
using System.Collections;
using System.Collections.Generic;
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
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types.SpotlightHeight;
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types.SpotlightWidth;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder
{
    public class ViewfinderSettingsFragment : NavigationFragment
    {
        private ViewfinderSettingsViewModel viewModel;

        private RecyclerView recyclerViewfinderTypes;
        private View cardColor, cardSpecifications, cardMeasures, cardLaserline, cardSpotlightColor,
                     cardSpotlightSizeSpecification, cardSpotlightMeasures;
        private View containerHeight, containerWidth, containerSizeSpec, containerHeightAspect,
                     containerWidthAspect, containerColor, containerRectangularDisabledColor, containerEnabledColor, containerDisabledColor,
                     containerLaserlineWidth, containerSpotlightSizeSpec, containerSpotlightBackgroundColor, containerSpotlightEnabledColor,
                     containerSpotlightDisabledColor, containerSpotlightWidth, containerSpotlightHeight,
                     containerSpotlightHeightAspect, containerSpotlightWidthAspect;
        private TextView textType, textColor, textRectangularDisabledColor, textSizeSpecification, textWidth, textHeight,
                         textEnabledColor, textDisabledColor, textLaserlineWidth, textSpotlightSizeSpecification,
                         textSpotlightBackgroundColor, textSpotlightEnabledColor, textSpotlightDisabledColor,
                         textSpotlightWidth, textSpotlightHeight;
        private EditText editHeightAspect, editWidthAspect, editSpotlightHeightAspect, editSpotlightWidthAspect;

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
            this.cardSpotlightColor = view.FindViewById<View>(Resource.Id.card_spotlight_color);
            this.cardSpotlightMeasures = view.FindViewById<View>(Resource.Id.card_spotlight_measures);
            this.cardSpotlightSizeSpecification = view.FindViewById<View>(Resource.Id.card_spotlight_size_specification);

            this.textType = view.FindViewById<TextView>(Resource.Id.text_viewfinder_type);
            this.textColor = view.FindViewById<TextView>(Resource.Id.text_color);
            this.textRectangularDisabledColor = view.FindViewById<TextView>(Resource.Id.text_rectangular_disabled_color);
            this.textSizeSpecification = view.FindViewById<TextView>(Resource.Id.text_size_specification);
            this.textWidth = view.FindViewById<TextView>(Resource.Id.text_width);
            this.textHeight = view.FindViewById<TextView>(Resource.Id.text_height);
            this.editHeightAspect = view.FindViewById<EditText>(Resource.Id.edit_height);
            this.editWidthAspect = view.FindViewById<EditText>(Resource.Id.edit_width);
            this.textSpotlightSizeSpecification = view.FindViewById<TextView>(Resource.Id.text_spotlight_size_specification);
            this.textSpotlightBackgroundColor = view.FindViewById<TextView>(Resource.Id.text_spotlight_background_color);
            this.textSpotlightEnabledColor = view.FindViewById<TextView>(Resource.Id.text_spotlight_enabled_color);
            this.textSpotlightDisabledColor = view.FindViewById<TextView>(Resource.Id.text_spotlight_disabled_color);
            this.textSpotlightWidth = view.FindViewById<TextView>(Resource.Id.text_spotlight_width);
            this.textSpotlightHeight = view.FindViewById<TextView>(Resource.Id.text_spotlight_height);
            this.editSpotlightHeightAspect = view.FindViewById<EditText>(Resource.Id.edit_spotlight_height);
            this.editSpotlightWidthAspect = view.FindViewById<EditText>(Resource.Id.edit_spotlight_width);

            this.containerColor = view.FindViewById(Resource.Id.container_color);
            this.containerRectangularDisabledColor = view.FindViewById(Resource.Id.container_rectangular_disabled_color);
            this.containerHeight = view.FindViewById(Resource.Id.container_height);
            this.containerWidth = view.FindViewById(Resource.Id.container_width);
            this.containerHeightAspect = view.FindViewById(Resource.Id.container_height_aspect);
            this.containerWidthAspect = view.FindViewById(Resource.Id.container_width_aspect);
            this.containerSizeSpec = view.FindViewById(Resource.Id.container_size_specification);
            this.containerSpotlightSizeSpec = view.FindViewById(Resource.Id.container_spotlight_size_specification);
            this.containerSpotlightBackgroundColor = view.FindViewById(Resource.Id.container_spotlight_background_color);
            this.containerSpotlightEnabledColor = view.FindViewById(Resource.Id.container_spotlight_enabled_color);
            this.containerSpotlightDisabledColor = view.FindViewById(Resource.Id.container_spotlight_disabled_color);
            this.containerSpotlightHeight = view.FindViewById(Resource.Id.container_spotlight_height);
            this.containerSpotlightWidth = view.FindViewById(Resource.Id.container_spotlight_width);
            this.containerSpotlightHeightAspect = view.FindViewById(Resource.Id.container_spotlight_height_aspect);
            this.containerSpotlightWidthAspect = view.FindViewById(Resource.Id.container_spotlight_width_aspect);

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

            this.containerRectangularDisabledColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowRectangularDisabledColorMenu();
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

            this.containerSpotlightSizeSpec.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowSpotlightSizeSpecificationMenu();
            };

            this.containerSpotlightBackgroundColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowSpotlightBackgroundColorMenu();
            };
            
            this.containerSpotlightEnabledColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowSpotlightEnabledColorMenu();
            };

            this.containerSpotlightDisabledColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowSpotlightDisabledColorMenu();
            };
            
            this.containerSpotlightHeight.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ViewfinderSpotlightHeightMeasureFragment.Create(), true, null);
            };

            this.containerSpotlightWidth.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ViewfinderSpotlightWidthMeasureFragment.Create(), true, null);
            };

            this.editSpotlightHeightAspect.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyHeightChange(this.editHeightAspect.Text);
                    this.DismissKeyboard(this.editHeightAspect);
                    this.editHeightAspect.ClearFocus();
                }
            };

            this.editSpotlightWidthAspect.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyWidthChange(this.editWidthAspect.Text);
                    this.DismissKeyboard(this.editWidthAspect);
                    this.editWidthAspect.ClearFocus();
                }
            };
        }

        private void ApplyHeightChange(string text)
        {
            if (float.TryParse(text, out float result))
            {
                switch (SettingsManager.Instance.CurrentViewfinder)
                {
                    case RectangularViewfinder _:
                        this.viewModel.SetRectangularViewfinderHeightAspect(result);
                        break;
                    case SpotlightViewfinder _:
                        this.viewModel.SetSpotlightViewfinderHeightAspect(result);
                        break;
                }

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
                switch (SettingsManager.Instance.CurrentViewfinder)
                {
                    case RectangularViewfinder _:
                        this.viewModel.SetRectangularViewfinderWidthAspect(result);
                        break;
                    case SpotlightViewfinder _:
                        this.viewModel.SetSpotlightViewfinderWidthAspect(result);
                        break;
                }
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
            this.textRectangularDisabledColor.Text = this.Context.GetString(viewfinder.DisabledColor.DisplayNameResourceId);
            this.textSizeSpecification.Text = this.Context.GetString((int)viewfinder.SizeSpecification);

            this.RefreshHeight(viewfinder.Height);
            this.RefreshWidth(viewfinder.Width);
            this.RefreshHeightAspect(viewfinder.HeightAspect);
            this.RefreshWidthAspect(viewfinder.WidthAspect);
        }

        private void RefreshSpotlightViewfinderData(ViewfinderTypeSpotlight viewfinder)
        {
            this.textType.Text = this.Context.GetString(viewfinder.DisplayNameResourceId);
            this.textSpotlightBackgroundColor.Text = this.Context.GetString(viewfinder.BackgroundColor.DisplayNameResourceId);
            this.textSpotlightEnabledColor.Text = this.Context.GetString(viewfinder.EnabledColor.DisplayNameResourceId);
            this.textSpotlightDisabledColor.Text = this.Context.GetString(viewfinder.DisabledColor.DisplayNameResourceId);
            this.textSpotlightSizeSpecification.Text = this.Context.GetString((int)viewfinder.SizeSpecification);

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
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.textHeight.Text = height.GetStringWithUnit(this.Context);
                    break;
                case SpotlightViewfinder _:
                    this.textSpotlightHeight.Text = height.GetStringWithUnit(this.Context);
                    break;
            }
        }

        private void RefreshWidth(FloatWithUnit width)
        {
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.textWidth.Text = width.GetStringWithUnit(this.Context);
                    break;
                case SpotlightViewfinder _:
                    this.textSpotlightWidth.Text = width.GetStringWithUnit(this.Context);
                    break;
            }
        }

        private void RefreshHeightAspect(float heightAspect)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.editHeightAspect.Text = string.Format(textFormat, heightAspect);
                    break;
                case SpotlightViewfinder _:
                    this.editSpotlightHeightAspect.Text = string.Format(textFormat, heightAspect);
                    break;
            }
        }

        private void RefreshWidthAspect(float widthAspect)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.editWidthAspect.Text = string.Format(textFormat, widthAspect);
                    break;
                case SpotlightViewfinder _:
                    this.editSpotlightWidthAspect.Text = string.Format(textFormat, widthAspect);
                    break;
            }
        }

        private void ShowHideSubSettings()
        {
            ViewfinderType viewfinderType = this.viewModel.GetCurrentViewfinderType();
            if (viewfinderType is ViewfinderTypeNone)
            {
                this.SetupForNoViewfinder();
            }
            else if (viewfinderType is ViewfinderTypeRectangular rectangular)
            {
                this.SetupForRectangularViewfinder(rectangular);
            }
            else if (viewfinderType is ViewfinderTypeLaserline laserline)
            {
                this.SetupForLaserlineViewfinder(laserline);
            }
            else if (viewfinderType is ViewfinderTypeSpotlight spotlight)
            {
                this.SetupForSpotlightViewfinder(spotlight);
            }
        }

        private void SetupForNoViewfinder()
        {
            this.textType.Visibility = ViewStates.Gone;
            this.cardColor.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardSpotlightColor.Visibility = ViewStates.Gone;
            this.cardSpotlightSizeSpecification.Visibility = ViewStates.Gone;
            this.cardSpotlightMeasures.Visibility = ViewStates.Gone;
        }

        private void SetupForRectangularViewfinder(ViewfinderTypeRectangular viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardColor.Visibility = ViewStates.Visible;
            this.cardSpecifications.Visibility = ViewStates.Visible;
            this.cardMeasures.Visibility = ViewStates.Visible;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardSpotlightColor.Visibility = ViewStates.Gone;
            this.cardSpotlightSizeSpecification.Visibility = ViewStates.Gone;
            this.cardSpotlightMeasures.Visibility = ViewStates.Gone;

            SizeSpecification spec = viewfinder.SizeSpecification;

            switch (spec)
            {
                case SizeSpecification.WidthAndHeight:
                    this.containerHeight.Visibility = ViewStates.Visible;
                    this.containerWidth.Visibility = ViewStates.Visible;
                    this.containerHeightAspect.Visibility = ViewStates.Gone;
                    this.containerWidthAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    this.containerHeight.Visibility = ViewStates.Visible;
                    this.containerWidth.Visibility = ViewStates.Gone;
                    this.containerHeightAspect.Visibility = ViewStates.Gone;
                    this.containerWidthAspect.Visibility = ViewStates.Visible;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    this.containerHeight.Visibility = ViewStates.Gone;
                    this.containerWidth.Visibility = ViewStates.Visible;
                    this.containerHeightAspect.Visibility = ViewStates.Visible;
                    this.containerWidthAspect.Visibility = ViewStates.Gone;
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
            this.cardSpotlightColor.Visibility = ViewStates.Gone;
            this.cardSpotlightSizeSpecification.Visibility = ViewStates.Gone;
            this.cardSpotlightMeasures.Visibility = ViewStates.Gone;

            this.RefreshLaserlineViewfinderData(viewfinder);
        }

        private void SetupForSpotlightViewfinder(ViewfinderTypeSpotlight viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardColor.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardSpotlightColor.Visibility = ViewStates.Visible;
            this.cardSpotlightSizeSpecification.Visibility = ViewStates.Visible;
            this.cardSpotlightMeasures.Visibility = ViewStates.Visible;

            SizeSpecification spec = viewfinder.SizeSpecification;

            switch (spec)
            {
                case SizeSpecification.WidthAndHeight:
                    this.containerSpotlightHeight.Visibility = ViewStates.Visible;
                    this.containerSpotlightWidth.Visibility = ViewStates.Visible;
                    this.containerSpotlightHeightAspect.Visibility = ViewStates.Gone;
                    this.containerSpotlightWidthAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    this.containerSpotlightHeight.Visibility = ViewStates.Visible;
                    this.containerSpotlightWidth.Visibility = ViewStates.Gone;
                    this.containerSpotlightHeightAspect.Visibility = ViewStates.Gone;
                    this.containerSpotlightWidthAspect.Visibility = ViewStates.Visible;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    this.containerSpotlightHeight.Visibility = ViewStates.Gone;
                    this.containerSpotlightWidth.Visibility = ViewStates.Visible;
                    this.containerSpotlightHeightAspect.Visibility = ViewStates.Visible;
                    this.containerSpotlightWidthAspect.Visibility = ViewStates.Gone;
                    break;
            }

            this.RefreshSpotlightViewfinderData(viewfinder);
        }

        private void BuildAndShowSizeSpecificationMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerSizeSpec, GravityFlags.End);
            
            IList sizeSpecificationItems = Enum.GetValues(typeof(SizeSpecification));
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
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerColor, GravityFlags.End);

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

        private void BuildAndShowRectangularDisabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerRectangularDisabledColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeRectangular.DisabledColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeRectangular.DisabledColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderDisabledColor(ViewfinderTypeRectangular.DisabledColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowEnabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerEnabledColor, GravityFlags.End);

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
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerDisabledColor, GravityFlags.End);

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

        private void BuildAndShowSpotlightSizeSpecificationMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerSpotlightSizeSpec, GravityFlags.End);

            IList sizeSpecificationItems = Enum.GetValues(typeof(SizeSpecification));
            for (int i = 0; i < sizeSpecificationItems.Count; i++)
            {
                int itemId = (int)sizeSpecificationItems[i];
                menu.Menu.Add(0, itemId, i, this.Context.GetString(itemId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedSizeSpec = args.Item.ItemId;
                this.viewModel.SetSpotlightViewfinderSizeSpec((SizeSpecification)selectedSizeSpec);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowSpotlightBackgroundColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerSpotlightBackgroundColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeSpotlight.BackgroundColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeSpotlight.BackgroundColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetSpotlightViewfinderBackgroundColor(ViewfinderTypeSpotlight.BackgroundColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowSpotlightEnabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerSpotlightEnabledColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeSpotlight.EnabledColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeSpotlight.EnabledColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetSpotlightViewfinderEnabledColor(ViewfinderTypeSpotlight.EnabledColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowSpotlightDisabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerSpotlightDisabledColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeSpotlight.DisabledColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeSpotlight.DisabledColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetSpotlightViewfinderDisabledColor(ViewfinderTypeSpotlight.DisabledColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.Viewfinder);
    }
}
