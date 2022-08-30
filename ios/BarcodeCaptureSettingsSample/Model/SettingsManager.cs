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
using BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture;
using BarcodeCaptureSettingsSample.DataSource.Settings.Camera;
using BarcodeCaptureSettingsSample.DataSource.Settings.View.Logo;
using BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder;
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Control;
using Scandit.DataCapture.Core.UI.Gestures;
using Scandit.DataCapture.Core.UI.Viewfinder;
using Brush = Scandit.DataCapture.Core.UI.Style.Brush;

namespace BarcodeCaptureSettingsSample.Model
{
    public sealed class SettingsManager
    {
        private static readonly Lazy<SettingsManager> lazy = new Lazy<SettingsManager>(() => new SettingsManager());

        private FloatWithUnit rectangularWidth = FloatWithUnit.Zero;

        private FloatWithUnit rectangularHeight = FloatWithUnit.Zero;

        private nfloat rectangularShorterDimension = .0f;

        private nfloat rectangularWidthAspect = .0f;

        private nfloat rectangularHeightAspect = .0f;

        private nfloat rectangularLongerDimensioAspect = .0f;

        private nfloat rectangularViewfinderDimming = .0f;

        private FloatWithUnit spotlightWidth = FloatWithUnit.Zero;

        private FloatWithUnit spotlightHeight = FloatWithUnit.Zero;

        private nfloat spotlightWidthAspect = .0f;

        private nfloat spotlightHeightAspect = .0f;

        private RectangularViewfinderColor rectangularViewfinderColor = RectangularViewfinderColor.Default;

        private RectangularViewfinderDisabledColor rectangularViewfinderDisabledColor = RectangularViewfinderDisabledColor.Default;

        private RectangularViewfinderAnimation rectangularViewfinderAnimation = null;

        private LaserlineViewfinderEnabledColor laserlineViewfinderEnabledColor = LaserlineViewfinderEnabledColor.Default;

        private LaserlineViewfinderAnimatedEnabledColor laserlineViewfinderAnimatedEnabledColor = LaserlineViewfinderAnimatedEnabledColor.Default;

        private LaserlineViewfinderDisabledColor laserlineViewfinderDisabledColor = LaserlineViewfinderDisabledColor.Default;

        private AimerViewfinderFrameColor aimerViewfinderFrameColor = AimerViewfinderFrameColor.Default;

        private AimerViewfinderDotColor aimerViewfinderDotColor = AimerViewfinderDotColor.Default;

        private Camera internalCamera = Camera.GetDefaultCamera();

        private TorchStateType internalTorchState = TorchStateType.Off;

        private readonly TorchSwitchControl internalTorchSwitch = new TorchSwitchControl();

        private readonly ZoomSwitchControl zoomSwitchControl = new ZoomSwitchControl();

        private bool torchSwitchShown = false;

        private bool zoomSwitchEnabled = false;

        // Use the recommended camera settings for the BarcodeCapture mode.
        private readonly CameraSettings cameraSettings = BarcodeCapture.RecommendedCameraSettings;

        private DataCaptureView captureView;

        private ViewfinderKind viewfinderKind = ViewfinderKind.None;

        public static SettingsManager Instance => lazy.Value;

        public bool ContinuousModeEnabled { get; set; }

        public DataCaptureContext Context { get; private set; }

        public BarcodeCaptureSettings BarcodeCaptureSettings { get; set; }

        public BarcodeCapture BarcodeCapture { get; set; }

        public DataCaptureView CaptureView
        {
            get => captureView;
            set
            {
                this.captureView = value;
                // Add a barcode capture overlay to the data capture view
                //to render the location of captured barcodes on top of the
                // video preview.
                // This is optional, but recommended for better visual feedback.
                this.Overlay = BarcodeCaptureOverlay.Create(this.BarcodeCapture, this.captureView, BarcodeCaptureOverlayStyle.Frame);
            }
        }

        public BarcodeCaptureOverlay Overlay { get; set; }

        public Brush DefaultBrush => BarcodeCaptureOverlay.DefaultBrushForStyle(this.OverlayStyle);

        private SettingsManager()
        {
            // The barcode capturing process is configured through barcode
            // capture settings and are then applied to the barcode capture
            // instance that manages barcode recognition.
            this.BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // Create data capture context using your license key and
            // set the camera as the frame source.
            this.Context = DataCaptureContextExtensions.LicensedContext;
            this.Context.SetFrameSourceAsync(this.internalCamera);

            // Create new barcode capture mode with the settings from above.
            this.BarcodeCapture = BarcodeCapture.Create(this.Context, this.BarcodeCaptureSettings);

            // Make sure that references to some settings are actually
            // the current settings.
            this.internalCamera?.ApplySettingsAsync(this.cameraSettings);
            if (this.internalCamera != null)
            {
                this.internalCamera.DesiredTorchState = this.internalTorchState.TorchState;
            }
        }

        #region Symbologies

        public bool AnySymbologyEnabled => this.BarcodeCaptureSettings.EnabledSymbologies.Count > 0;

        public void EnableAllSymbologies()
        {
            var symbologyies = new HashSet<Symbology>(SymbologyExtensions.AllValues);
            this.BarcodeCaptureSettings.EnableSymbologies(symbologyies);
            this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
        }

        public void DisableAllSymbologies()
        {
            foreach (var symbology in this.BarcodeCaptureSettings.EnabledSymbologies)
            {
                this.BarcodeCaptureSettings.EnableSymbology(symbology, false);
            }
            this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
        }

        public bool IsSymbologyEnabled(Symbology symbology)
        {
            return this.BarcodeCaptureSettings.EnabledSymbologies.Contains(symbology);
        }

        public SymbologySettings GetSymbologySettings(Symbology symbology)
        {
            return this.BarcodeCaptureSettings.GetSymbologySettings(symbology);
        }

        public void SymbologySettingsChanged()
        {
            // Apply barcode capture settings, which includes symbology
            // settings that might've been potentially updated.
            this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
        }

        #endregion

        #region FEEDBACK

        public Feedback Feedback
        {
            get => this.BarcodeCapture.Feedback.Success;
            set => this.BarcodeCapture.Feedback.Success = value;
        }

        public Vibration Vibration { get; set; } = VibrationType.Default.Vibration;

        #endregion

        #region LOCATION SELECTION

        public ILocationSelection LocationSelection
        {
            get => BarcodeCaptureSettings.LocationSelection;
            set
            {
                this.BarcodeCaptureSettings.LocationSelection = value;
                this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
            }
        }

        #endregion

        #region BARCODE CAPTURE DUPLICATE FILTER
        public TimeSpan DuplicateFilter
        {
            get => this.BarcodeCaptureSettings.CodeDuplicateFilter;
            set
            {
                this.BarcodeCaptureSettings.CodeDuplicateFilter = value;
                this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
            }
        }
        #endregion

        #region COMPOSITE TYPE SETTINGS
        public CompositeType EnabledCompositeTypes
        {
            get => this.BarcodeCaptureSettings.EnabledCompositeTypes;
            set
            {
                this.BarcodeCaptureSettings.EnabledCompositeTypes = value;
                this.BarcodeCaptureSettings.EnableSymbologies(value);
                this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
            }
        }

        public CompositeType GetEnabledCompositeTypes()
        {
            return this.BarcodeCaptureSettings.EnabledCompositeTypes;
        }
        #endregion

        #region CAMERA

        public Camera Camera
        {
            get => this.internalCamera;
            set
            {
                // ⚠️ The new camera might be null, e.g. the device
                // doesn't have a specific or any camera.
                this.internalCamera = value;
                this.internalCamera?.ApplySettingsAsync(this.cameraSettings);
                this.Context.SetFrameSourceAsync(this.internalCamera);
            }
        }

        public TorchStateType TorchStateType
        {
            get => this.internalTorchState;
            set
            {
                this.internalTorchState = value;
                this.Camera.DesiredTorchState = value.TorchState;
            }
        }

        public VideoResolution PreferredResolution
        {
            get => this.cameraSettings.PreferredResolution;
            set
            {
                this.cameraSettings.PreferredResolution = value;
                this.Camera.ApplySettingsAsync(this.cameraSettings);
            }
        }

        public nfloat ZoomFactor
        {
            get => this.cameraSettings.ZoomFactor;
            set
            {
                this.cameraSettings.ZoomFactor = value;
                this.Camera.ApplySettingsAsync(this.cameraSettings);
            }
        }

        public nfloat ZoomGestureZoomFactor
        {
            get => this.cameraSettings.ZoomGestureZoomFactor;
            set
            {
                this.cameraSettings.ZoomGestureZoomFactor = value;
                this.Camera.ApplySettingsAsync(this.cameraSettings);
            }
        }

        public FocusGestureStrategy FocusGestureStrategy
        {
            get => this.cameraSettings.FocusGestureStrategy;
            set
            {
                this.cameraSettings.FocusGestureStrategy = value;
                this.Camera.ApplySettingsAsync(this.cameraSettings);
            }
        }

        public FocusRange FocusRange
        {
            get => this.cameraSettings.FocusRange;
            set
            {
                this.cameraSettings.FocusRange = value;
                this.Camera.ApplySettingsAsync(this.cameraSettings);
            }
        }

        #endregion

        #region Scan Areas

        public MarginsWithUnit ScanAreaMargins
        {
            get => this.CaptureView.ScanAreaMargins;
            set => this.CaptureView.ScanAreaMargins = value;
        }

        public bool ShouldShowScanAreaGuides
        {
            get => this.Overlay.ShouldShowScanAreaGuides;
            set => this.Overlay.ShouldShowScanAreaGuides = value;
        }

        #endregion

        #region Point of Interest

        public PointWithUnit PointOfInterest
        {
            get => this.CaptureView.PointOfInterest;
            set => this.CaptureView.PointOfInterest = value;
        }

        #endregion

        #region Overlay

        public BarcodeCaptureOverlayStyle OverlayStyle
        {
            get => Overlay.Style;
            set
            {
                var shouldShowScanAreaGuides = Overlay.ShouldShowScanAreaGuides;
                var viewfinder = Overlay.Viewfinder;
                this.CaptureView.RemoveOverlay(this.Overlay);
                var overlay = BarcodeCaptureOverlay.Create(this.BarcodeCapture, this.CaptureView, value);
                overlay.ShouldShowScanAreaGuides = shouldShowScanAreaGuides;
                overlay.Viewfinder = viewfinder;
                this.Overlay = overlay;
            }
        }

        public Brush Brush
        {
            get => this.Overlay.Brush;
            set => this.Overlay.Brush = value;
        }

        #endregion

        #region Logo

        public NamedAnchor LogoAnchor
        {
            get => NamedAnchor.Create(this.CaptureView.LogoAnchor);
            set => this.CaptureView.LogoAnchor = value.Anchor;
        }

        public PointWithUnit LogoOffset
        {
            get => this.CaptureView.LogoOffset;
            set => this.CaptureView.LogoOffset = value;
        }

        #endregion

        #region Controls

        public bool TorchSwitchShown
        {
            get => this.torchSwitchShown;
            set
            {
                this.torchSwitchShown = value;
                if (value)
                {
                    this.CaptureView.AddControl(this.internalTorchSwitch);
                }
                else
                {
                    this.CaptureView.RemoveControl(this.internalTorchSwitch);
                }
            }
        }

        public bool IsZoomSwitchButtonEnabled
        {
            get => this.zoomSwitchEnabled;
            set
            {
                this.zoomSwitchEnabled = value;
                if (value)
                {
                    this.CaptureView.AddControl(this.zoomSwitchControl);
                }
                else
                {
                    this.CaptureView.RemoveControl(this.zoomSwitchControl);
                }
            }
        }

        #endregion

        #region Viewfinder

        public ViewfinderKind ViewfinderKind 
        {
            get => this.viewfinderKind;
            set
            {
                this.viewfinderKind = value;
                this.Overlay.Viewfinder = value.Viewfinder;
                if (value.Viewfinder is RectangularViewfinder rectangular)
                {
                    this.ViewfinderSizeSpecification = rectangular.SizeWithUnitAndAspect.SizeSpecification();
                    switch(rectangular.SizeWithUnitAndAspect.SizingMode)
                    {
                        case SizingMode.WidthAndHeight:
                        {
                            var widthAndHeight = rectangular.SizeWithUnitAndAspect.WidthAndHeight;
                            this.RectangularWidth = widthAndHeight.Width;
                            this.RectangularHeight = widthAndHeight.Height;
                            break;
                        }
                        case SizingMode.WidthAndAspectRatio:
                        {
                            this.RectangularWidthAspect = rectangular.SizeWithUnitAndAspect.WidthAndAspectRatio.Aspect;
                            break;
                        }
                        case SizingMode.HeightAndAspectRatio:
                        {
                            this.RectangularHeightAspect = rectangular.SizeWithUnitAndAspect.HeightAndAspectRatio.Aspect;
                            break;
                        }
                        case SizingMode.ShorterDimensionAndAspectRatio:
                        {
                            var shorterDimensionAndAspect = rectangular.SizeWithUnitAndAspect.ShorterDimensionAndAspectRatio;
                            this.RectangularShorterDimension = shorterDimensionAndAspect.Size.Value;
                            this.RectangularLongerDimensionAspect = shorterDimensionAndAspect.Aspect;
                            break;
                        }
                    }
                }
            }
        }
        
        public FloatWithUnit RectangularWidth
        {
            get => this.rectangularWidth;
            set
            {
                this.rectangularWidth = value;
                this.UpdateViewfinderSize();
            }
        }

        public FloatWithUnit RectangularHeight
        {
            get => this.rectangularHeight;
            set
            {
                this.rectangularHeight = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat RectangularShorterDimension
        {
            get => this.rectangularShorterDimension;
            set
            {
                this.rectangularShorterDimension = value;
                this.UpdateViewfinderSize();
            }
        }
        
        public nfloat RectangularWidthAspect
        {
            get => this.rectangularWidthAspect;
            set
            {
                this.rectangularWidthAspect = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat RectangularHeightAspect
        {
            get => this.rectangularHeightAspect;
            set
            {
                this.rectangularHeightAspect = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat RectangularLongerDimensionAspect
        {
            get => this.rectangularLongerDimensioAspect;
            set
            {
                this.rectangularLongerDimensioAspect = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat RectangularViewfinderDimming
        {
            get => this.rectangularViewfinderDimming;
            set
            {
                this.rectangularViewfinderDimming = value;
                (this.ViewfinderKind.Viewfinder as RectangularViewfinder).Dimming = this.rectangularViewfinderDimming;
            }
        }

        public RectangularViewfinderColor RectangularViewfinderColor
        {
            get => this.rectangularViewfinderColor;
            set
            {
                this.rectangularViewfinderColor = value;
                (this.ViewfinderKind.Viewfinder as RectangularViewfinder).Color = this.RectangularViewfinderColor.UIColor;
            }
        }

        public RectangularViewfinderDisabledColor RectangularViewfinderDisabledColor
        {
            get => this.rectangularViewfinderDisabledColor;
            set
            {
                this.rectangularViewfinderDisabledColor = value;
                (this.ViewfinderKind.Viewfinder as RectangularViewfinder).DisabledColor = this.RectangularViewfinderDisabledColor.UIColor;
            }
        }

        public RectangularViewfinderStyleType RectangularViewfinderStyleType { get; set; } = RectangularViewfinderStyleType.Legacy;

        public RectangularViewfinderLineStyleType RectangularViewfinderLineStyleType { get; set; } = RectangularViewfinderLineStyleType.Light;

        public RectangularViewfinderAnimation RectangularViewfinderAnimation
        {
            get => this.rectangularViewfinderAnimation;
            set
            {
                this.rectangularViewfinderAnimation = value;
                (this.ViewfinderKind.Viewfinder as RectangularViewfinder).Animation = this.rectangularViewfinderAnimation;
            }
        }

        public LaserlineViewfinderStyleType LaserlineViewfinderStyleType { get; set; } = LaserlineViewfinderStyleType.Legacy;

        public LaserlineViewfinderEnabledColor LaserlineViewfinderEnabledColor
        {
            get => this.laserlineViewfinderEnabledColor;
            set
            {
                this.laserlineViewfinderEnabledColor = value;
                (this.ViewfinderKind.Viewfinder as LaserlineViewfinder).EnabledColor = this.LaserlineViewfinderEnabledColor.UIColor;
            }
        }

        public LaserlineViewfinderAnimatedEnabledColor LaserlineViewfinderAnimatedEnabledColor
        {
            get => this.laserlineViewfinderAnimatedEnabledColor;
            set
            {
                this.laserlineViewfinderAnimatedEnabledColor = value;
                (this.ViewfinderKind.Viewfinder as LaserlineViewfinder).EnabledColor = this.LaserlineViewfinderAnimatedEnabledColor.UIColor;
            }
        }

        public LaserlineViewfinderDisabledColor LaserlineViewfinderDisabledColor
        {
            get => this.laserlineViewfinderDisabledColor;
            set
            {
                this.laserlineViewfinderDisabledColor = value;
                (this.ViewfinderKind.Viewfinder as LaserlineViewfinder).DisabledColor = this.LaserlineViewfinderDisabledColor.UIColor;
            }
        }

        public AimerViewfinderFrameColor AimerViewfinderFrameColor
        {
            get => this.aimerViewfinderFrameColor;
            set
            {
                this.aimerViewfinderFrameColor = value;
                (this.ViewfinderKind.Viewfinder as AimerViewfinder).FrameColor = this.aimerViewfinderFrameColor.UIColor;
            }
        }

        public AimerViewfinderDotColor AimerViewfinderDotColor
        {
            get => this.aimerViewfinderDotColor;
            set
            {
                this.aimerViewfinderDotColor = value;
                (this.ViewfinderKind.Viewfinder as AimerViewfinder).DotColor = this.aimerViewfinderDotColor.UIColor;
            }
        }

        public RectangularSizeSpecification ViewfinderSizeSpecification = RectangularSizeSpecification.WidthAndHeight;

        public IZoomGesture SwipeToZoom
        {
            get => this.CaptureView.ZoomGesture;
            set => this.CaptureView.ZoomGesture = value;
        }

        public IFocusGesture TapToFocus
        {
            get => this.CaptureView.FocusGesture;
            set => this.CaptureView.FocusGesture = value;
        }

        public FloatWithUnit SpotlightWidth
        {
            get => this.spotlightWidth;
            set
            {
                this.spotlightWidth = value;
                this.UpdateViewfinderSize();
            }
        }

        public FloatWithUnit SpotlightHeight
        {
            get => this.spotlightHeight;
            set
            {
                this.spotlightHeight = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat SpotlightWidthAspect
        {
            get => this.spotlightWidthAspect;
            set
            {
                this.spotlightWidthAspect = value;
                this.UpdateViewfinderSize();
            }
        }

        public nfloat SpotlightHeightAspect
        {
            get => this.spotlightHeightAspect;
            set
            {
                this.spotlightHeightAspect = value;
                this.UpdateViewfinderSize();
            }
        }

        private void UpdateViewfinderSize()
        {
            if (this.ViewfinderKind == ViewfinderKind.Rectangular)
            {
                if (this.ViewfinderSizeSpecification.Equals(RectangularSizeSpecification.WidthAndHeight))
                {
                    var newSize = new SizeWithUnit()
                    {
                        Width = this.RectangularWidth,
                        Height = this.RectangularHeight
                    };
                    (ViewfinderKind.Rectangular.Viewfinder as RectangularViewfinder).SetSize(newSize);
                } else if (this.ViewfinderSizeSpecification.Equals(RectangularSizeSpecification.WidthAndHeightAspect))
                {
                    (ViewfinderKind.Rectangular.Viewfinder as RectangularViewfinder).SetWidthAndAspectRatio(this.RectangularWidth, this.RectangularHeightAspect);
                } else if (this.ViewfinderSizeSpecification.Equals(RectangularSizeSpecification.HeightAndWidthAspect))
                {
                    (ViewfinderKind.Rectangular.Viewfinder as RectangularViewfinder).SetHeightAndAspectRatio(this.RectangularHeight, this.RectangularWidthAspect);
                } else if (this.ViewfinderSizeSpecification.Equals(RectangularSizeSpecification.ShorterDimensionAndAspectRatio))
                {
                    (ViewfinderKind.Rectangular.Viewfinder as RectangularViewfinder).SetShorterDimensionAndAspectRatio(this.rectangularShorterDimension, this.rectangularLongerDimensioAspect);
                }
            }
        }

        #endregion
    }
}
