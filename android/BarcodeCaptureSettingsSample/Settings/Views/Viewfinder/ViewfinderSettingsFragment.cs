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
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder
{
    public class ViewfinderSettingsFragment : NavigationFragment
    {
        private ViewfinderSettingsViewModel viewModel;

        private RecyclerView recyclerViewfinderTypes;
        private View cardRectangular, cardSpecifications, cardAnimation, cardMeasures, cardLaserline, cardAimer;
        private View containerHeight, containerWidth, containerSizeSpec, containerHeightAspect,
                     containerWidthAspect, containerColor, containerRectangularDisabledColor, containerEnabledColor, containerDisabledColor,
                     containerLaserlineStyle, containerLaserlineWidth, containerRectangularStyle, containerRectangularLineStyle,
                     containerShorterDimension, containerShorterDimensionAspect, containerAimerFrameColor, containerAimerDotColor;
        private TextView textType, textColor, textRectangularDisabledColor, textSizeSpecification, textWidth, textHeight,
                         textEnabledColor, textDisabledColor, textLaserlineStyle, textLaserlineWidth, textRectangularStyle, textAimerFrameColor,
                         textAimerDotColor, textRectangularLineStyle;
        private EditText editHeightAspect, editWidthAspect, editShorterDimension, editLongerDimensionAspect, editRectangularDimming;
        private Switch switchRectangularAnimation, switchRectangularLooping;

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

            this.cardRectangular = view.FindViewById<View>(Resource.Id.card_rectangular);
            this.cardSpecifications = view.FindViewById<View>(Resource.Id.card_size_specification);
            this.cardAnimation = view.FindViewById<View>(Resource.Id.card_animation);
            this.cardMeasures = view.FindViewById<View>(Resource.Id.card_measures);
            this.cardLaserline = view.FindViewById<View>(Resource.Id.card_laserline);
            this.cardAimer = view.FindViewById<View>(Resource.Id.card_aimer);

            this.textType = view.FindViewById<TextView>(Resource.Id.text_viewfinder_type);
            this.textColor = view.FindViewById<TextView>(Resource.Id.text_color);
            this.textRectangularDisabledColor = view.FindViewById<TextView>(Resource.Id.text_rectangular_disabled_color);
            this.textRectangularStyle = view.FindViewById<TextView>(Resource.Id.text_rectangular_style);
            this.textRectangularLineStyle = view.FindViewById<TextView>(Resource.Id.text_rectangular_line_style);
            this.editRectangularDimming = view.FindViewById<EditText>(Resource.Id.edit_rectangular_dimming);
            this.switchRectangularAnimation = view.FindViewById<Switch>(Resource.Id.switch_rectangular_animation);
            this.switchRectangularLooping = view.FindViewById<Switch>(Resource.Id.switch_rectangular_looping);
            this.textSizeSpecification = view.FindViewById<TextView>(Resource.Id.text_size_specification);
            this.textWidth = view.FindViewById<TextView>(Resource.Id.text_width);
            this.textHeight = view.FindViewById<TextView>(Resource.Id.text_height);
            this.editHeightAspect = view.FindViewById<EditText>(Resource.Id.edit_height);
            this.editWidthAspect = view.FindViewById<EditText>(Resource.Id.edit_width);
            this.editShorterDimension = view.FindViewById<EditText>(Resource.Id.edit_shorter_dimension);
            this.editLongerDimensionAspect = view.FindViewById<EditText>(Resource.Id.edit_longer_dimension_aspect);

            this.containerColor = view.FindViewById(Resource.Id.container_rectangular_color);
            this.containerRectangularDisabledColor = view.FindViewById(Resource.Id.container_rectangular_disabled_color);
            this.containerHeight = view.FindViewById(Resource.Id.container_height);
            this.containerWidth = view.FindViewById(Resource.Id.container_width);
            this.containerHeightAspect = view.FindViewById(Resource.Id.container_height_aspect);
            this.containerWidthAspect = view.FindViewById(Resource.Id.container_width_aspect);
            this.containerSizeSpec = view.FindViewById(Resource.Id.container_size_specification);
            this.containerRectangularStyle = view.FindViewById(Resource.Id.container_rectangular_style);
            this.containerRectangularLineStyle = view.FindViewById(Resource.Id.container_rectangular_line_style);
            this.containerShorterDimension = view.FindViewById(Resource.Id.container_shorter_dimension);
            this.containerShorterDimensionAspect = view.FindViewById(Resource.Id.container_longer_dimension_aspect);

            this.containerLaserlineWidth = view.FindViewById<View>(Resource.Id.container_laserline_width);
            this.textLaserlineWidth = view.FindViewById<TextView>(Resource.Id.text_laserline_width);
            this.containerLaserlineStyle = view.FindViewById(Resource.Id.container_laserline_style);
            this.textLaserlineStyle = view.FindViewById<TextView>(Resource.Id.text_laserline_style);
            this.textEnabledColor = view.FindViewById<TextView>(Resource.Id.text_enabled_color);
            this.textDisabledColor = view.FindViewById<TextView>(Resource.Id.text_disabled_color);
            this.containerEnabledColor = view.FindViewById<View>(Resource.Id.container_enabled_color);
            this.containerDisabledColor = view.FindViewById<View>(Resource.Id.container_disabled_color);

            this.containerAimerFrameColor = view.FindViewById<View>(Resource.Id.container_aimer_frame_color);
            this.containerAimerDotColor = view.FindViewById<View>(Resource.Id.container_aimer_dot_color);
            this.textAimerFrameColor = view.FindViewById<TextView>(Resource.Id.text_aimer_frame_color);
            this.textAimerDotColor = view.FindViewById<TextView>(Resource.Id.text_aimer_dot_color);

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

            this.containerRectangularStyle.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowRectangularStyleMenu();
            };

            this.containerRectangularLineStyle.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowRectangularLineStyleMenu();
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

            this.editShorterDimension.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyShorterDimensionChange(this.editShorterDimension.Text);
                    this.DismissKeyboard(this.editShorterDimension);
                    this.editShorterDimension.ClearFocus();
                }
            };

            this.editLongerDimensionAspect.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyLongerDimensionAspectChange(this.editLongerDimensionAspect.Text);
                    this.DismissKeyboard(this.editLongerDimensionAspect);
                    this.editLongerDimensionAspect.ClearFocus();
                }
            };

            this.editRectangularDimming.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyRectangularDimmingChange(this.editRectangularDimming.Text);
                    this.DismissKeyboard(this.editRectangularDimming);
                    this.editRectangularDimming.ClearFocus();
                }
            };

            this.switchRectangularAnimation.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.SetRectangularAnimationEnabled(args.IsChecked);
                this.RefreshRectangularAnimation(args.IsChecked);
            };

            this.switchRectangularLooping.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.SetRectangularLoopingEnabled(args.IsChecked);
            };

            this.containerLaserlineStyle.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowLaserlineStyleMenu();
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

            this.containerAimerFrameColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowAimerFrameColorMenu();
            };

            this.containerAimerDotColor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowAimerDotColorMenu();
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
                }
                this.ShowHideSubSettings();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ApplyShorterDimensionChange(string text)
        {
            if (float.TryParse(text, out float result))
            {
                switch (SettingsManager.Instance.CurrentViewfinder)
                {
                    case RectangularViewfinder _:
                        this.viewModel.SetRectangularViewfinderShorterDimension(result);
                        break;
                }

                this.ShowHideSubSettings();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ApplyLongerDimensionAspectChange(string text)
        {
            if (float.TryParse(text, out float result))
            {
                switch (SettingsManager.Instance.CurrentViewfinder)
                {
                    case RectangularViewfinder _:
                        this.viewModel.SetRectangularViewfinderLongerDimensionAspect(result);
                        break;
                }

                this.ShowHideSubSettings();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ApplyRectangularDimmingChange(string text)
        {
            if (float.TryParse(text, out float result))
            {
                switch (SettingsManager.Instance.CurrentViewfinder)
                {
                    case RectangularViewfinder _:
                        this.viewModel.SetRectangularDimming(result);
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
            this.textRectangularStyle.Text = viewfinder.Style.Name();
            this.textRectangularLineStyle.Text = viewfinder.LineStyle.Name();
            this.textSizeSpecification.Text = this.Context.GetString((int)viewfinder.SizeSpecification);

            this.RefreshHeight(viewfinder.Height);
            this.RefreshWidth(viewfinder.Width);
            this.RefreshHeightAspect(viewfinder.HeightAspect);
            this.RefreshWidthAspect(viewfinder.WidthAspect);

            if (viewfinder.ShorterDimension != null)
            {
                this.RefreshShorterDimension(viewfinder.ShorterDimension.Value);
                this.RefreshLongerDimensionAspect(viewfinder.LongerDimensionAspect);
            }

            this.RefreshDimming(viewfinder.Dimming);
            this.RefreshRectangularAnimation(viewfinder.Animation);
            this.RefreshRectangularLooping(viewfinder.Looping);
        }

        private void RefreshLaserlineViewfinderData(ViewfinderTypeLaserline viewfinder)
        {
            this.textType.Text = this.Context.GetString(viewfinder.DisplayNameResourceId);
            this.textLaserlineStyle.Text = viewfinder.Style.Name();
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
            }
        }

        private void RefreshWidth(FloatWithUnit width)
        {
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.textWidth.Text = width.GetStringWithUnit(this.Context);
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
            }
        }

        private void RefreshShorterDimension(float shorterDimension)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.editShorterDimension.Text = string.Format(textFormat, shorterDimension);
                    break;
            }
        }

        private void RefreshDimming(float dimming)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.editRectangularDimming.Text = string.Format(textFormat, dimming);
                    break;
            }
        }

        private void RefreshRectangularAnimation(bool animation)
        {
            this.switchRectangularAnimation.Checked = animation;
            this.switchRectangularLooping.Visibility = (animation ? ViewStates.Visible : ViewStates.Gone);
        }

        private void RefreshRectangularLooping(bool looping)
        {
            this.switchRectangularLooping.Checked = looping;
        }

        private void RefreshLongerDimensionAspect(float longerDimensionAspect)
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            switch (SettingsManager.Instance.CurrentViewfinder)
            {
                case RectangularViewfinder _:
                    this.editLongerDimensionAspect.Text = string.Format(textFormat, longerDimensionAspect);
                    break;
            }
        }

        private void RefreshAimerViewfinderData(ViewfinderTypeAimer viewfinder)
        {
            this.textType.Text = this.Context.GetString(viewfinder.DisplayNameResourceId);
            this.textAimerFrameColor.Text = this.Context.GetString(viewfinder.FrameColor.DisplayNameResourceId);
            this.textAimerDotColor.Text = this.Context.GetString(viewfinder.DotColor.DisplayNameResourceId);
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
            else if (viewfinderType is ViewfinderTypeAimer aimer)
            {
                this.SetupForAimerViewfinder(aimer);
            }
        }

        private void SetupForNoViewfinder()
        {
            this.textType.Visibility = ViewStates.Gone;
            this.cardRectangular.Visibility = ViewStates.Gone;
            this.cardAnimation.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardAimer.Visibility = ViewStates.Gone;
        }

        private void SetupForRectangularViewfinder(ViewfinderTypeRectangular viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardRectangular.Visibility = ViewStates.Visible;
            this.cardAnimation.Visibility = ViewStates.Visible;
            this.cardSpecifications.Visibility = ViewStates.Visible;
            this.cardMeasures.Visibility = ViewStates.Visible;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardAimer.Visibility = ViewStates.Gone;

            SizeSpecification spec = viewfinder.SizeSpecification;

            switch (spec)
            {
                case SizeSpecification.WidthAndHeight:
                    this.containerHeight.Visibility = ViewStates.Visible;
                    this.containerWidth.Visibility = ViewStates.Visible;
                    this.containerHeightAspect.Visibility = ViewStates.Gone;
                    this.containerWidthAspect.Visibility = ViewStates.Gone;
                    this.containerShorterDimension.Visibility = ViewStates.Gone;
                    this.containerShorterDimensionAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.HeightAndWidthAspect:
                    this.containerHeight.Visibility = ViewStates.Visible;
                    this.containerWidth.Visibility = ViewStates.Gone;
                    this.containerHeightAspect.Visibility = ViewStates.Gone;
                    this.containerWidthAspect.Visibility = ViewStates.Visible;
                    this.containerShorterDimension.Visibility = ViewStates.Gone;
                    this.containerShorterDimensionAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.WidthAndHeightAspect:
                    this.containerHeight.Visibility = ViewStates.Gone;
                    this.containerWidth.Visibility = ViewStates.Visible;
                    this.containerHeightAspect.Visibility = ViewStates.Visible;
                    this.containerWidthAspect.Visibility = ViewStates.Gone;
                    this.containerShorterDimension.Visibility = ViewStates.Gone;
                    this.containerShorterDimensionAspect.Visibility = ViewStates.Gone;
                    break;
                case SizeSpecification.ShorterDimensionAndAspect:
                    this.containerHeight.Visibility = ViewStates.Gone;
                    this.containerWidth.Visibility = ViewStates.Gone;
                    this.containerHeightAspect.Visibility = ViewStates.Gone;
                    this.containerWidthAspect.Visibility = ViewStates.Gone;
                    this.containerShorterDimension.Visibility = ViewStates.Visible;
                    this.containerShorterDimensionAspect.Visibility = ViewStates.Visible;
                    break;
            }

            this.RefreshRectangularViewfinderData(viewfinder);
        }

        private void SetupForLaserlineViewfinder(ViewfinderTypeLaserline viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardRectangular.Visibility = ViewStates.Gone;
            this.cardAnimation.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Visible;
            this.cardAimer.Visibility = ViewStates.Gone;

            this.RefreshLaserlineViewfinderData(viewfinder);
        }

        private void SetupForAimerViewfinder(ViewfinderTypeAimer viewfinder)
        {
            this.textType.Visibility = ViewStates.Visible;
            this.cardRectangular.Visibility = ViewStates.Gone;
            this.cardAnimation.Visibility = ViewStates.Gone;
            this.cardSpecifications.Visibility = ViewStates.Gone;
            this.cardMeasures.Visibility = ViewStates.Gone;
            this.cardLaserline.Visibility = ViewStates.Gone;
            this.cardAimer.Visibility = ViewStates.Visible;

            this.RefreshAimerViewfinderData(viewfinder);
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

            var availableColors = ViewfinderTypeRectangular.EnabledColors.GetAllForStyle(this.viewModel.RectangularViewfinderStyle);
            for (int i = 0; i < availableColors.Count; i++)
            {
                UiColor color = availableColors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderColor(availableColors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowRectangularDisabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerRectangularDisabledColor, GravityFlags.End);

            var availableColors = ViewfinderTypeRectangular.DisabledColors.GetAllForStyle(this.viewModel.RectangularViewfinderStyle);
            for (int i = 0; i < availableColors.Count; i++)
            {
                UiColor color = availableColors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderDisabledColor(availableColors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowRectangularStyleMenu()
        {
            PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerRectangularStyle, GravityFlags.End);

            RectangularViewfinderStyle[] values = RectangularViewfinderStyle.Values();
            for (int i = 0; i < values.Count(); i++)
            {
                RectangularViewfinderStyle style = values[i];
                menu.Menu.Add(0, i, i, style.Name());
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedStyle = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderStyle(RectangularViewfinderStyle.Values()[selectedStyle]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowRectangularLineStyleMenu()
        {
            PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerRectangularLineStyle, GravityFlags.End);

            RectangularViewfinderLineStyle[] values = RectangularViewfinderLineStyle.Values();
            for (int i = 0; i < values.Length; i++)
            {
                RectangularViewfinderLineStyle style = values[i];
                menu.Menu.Add(0, i, i, style.Name());
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedStyle = args.Item.ItemId;
                this.viewModel.SetRectangularViewfinderLineStyle(
                    RectangularViewfinderLineStyle.Values()[selectedStyle]
                );
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowEnabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerEnabledColor, GravityFlags.End);

            var availableColors = ViewfinderTypeLaserline.EnabledColors.GetAllForStyle(this.viewModel.LaserlineViewfinderStyle);
            for (int i = 0; i < availableColors.Count; i++)
            {
                UiColor color = availableColors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetLaserlineViewfinderEnabledColor(availableColors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowDisabledColorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerDisabledColor, GravityFlags.End);

            var availableColors = ViewfinderTypeLaserline.DisabledColors.GetAllForStyle(this.viewModel.LaserlineViewfinderStyle);
            for (int i = 0; i < availableColors.Count; i++)
            {
                UiColor color = availableColors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetLaserlineViewfinderDisabledColor(availableColors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowAimerFrameColorMenu()
        {
            PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerAimerFrameColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeAimer.FrameColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeAimer.FrameColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetAimerViewfinderFrameColor(ViewfinderTypeAimer.FrameColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowAimerDotColorMenu()
        {
            PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerAimerDotColor, GravityFlags.End);

            for (int i = 0; i < ViewfinderTypeAimer.DotColors.Colors.Count; i++)
            {
                UiColor color = ViewfinderTypeAimer.DotColors.Colors[i];
                menu.Menu.Add(0, i, i, this.Context.GetString(color.DisplayNameResourceId));
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedColor = args.Item.ItemId;
                this.viewModel.SetAimerViewfinderDotColor(ViewfinderTypeAimer.DotColors.Colors[selectedColor]);
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        private void BuildAndShowLaserlineStyleMenu()
        {
            PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerLaserlineStyle, GravityFlags.End);

            LaserlineViewfinderStyle[] values = LaserlineViewfinderStyle.Values();
            for (int i = 0; i < values.Length; i++)
            {
                LaserlineViewfinderStyle style = values[i];
                menu.Menu.Add(0, i, i, style.Name());
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedStyle = args.Item.ItemId;
                this.viewModel.SetLaserlineViewfinderStyle(
                    LaserlineViewfinderStyle.Values()[selectedStyle]
                );
                this.ShowHideSubSettings();
            };

            menu.Show();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.Viewfinder);
    }
}
