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
using System.Linq;
using System.Threading.Tasks;
using BarcodeCaptureSettingsSample.Base.UiColors;
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI.Style;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample
{
    public class SettingsManager
    {
        private const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private static readonly Lazy<SettingsManager> instance = new Lazy<SettingsManager>(
            valueFactory: () => new SettingsManager(),
            isThreadSafe: true);

        public static SettingsManager Instance { get { return instance.Value; } }
        public DataCaptureContext DataCaptureContext { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }
        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureOverlay BarcodeCaptureOverlay { get; private set; }

        #region View Settings
        public Anchor LogoAnchor { get; set; } = Anchor.BottomRight;
        public FloatWithUnit AnchorXOffset { get; set; } = new FloatWithUnit(0f, MeasureUnit.Fraction);
        public FloatWithUnit AnchorYOffset { get; set; } = new FloatWithUnit(0f, MeasureUnit.Fraction);
        public bool TorchButtonEnabled { get; set; } = false;
        #endregion

        #region ScanResult Settings
        public bool ContinuousScanningEnabled { get; set; } = false;
        #endregion

        #region ScanArea Settings
        public MarginsWithUnit ScanAreaMargins { get; set; } = new MarginsWithUnit(
            new FloatWithUnit(0, MeasureUnit.Dip),
            new FloatWithUnit(0, MeasureUnit.Dip),
            new FloatWithUnit(0, MeasureUnit.Dip),
            new FloatWithUnit(0, MeasureUnit.Dip)
        );

        public bool ShouldShowScanAreaGuides
        {
            get { return this.BarcodeCaptureOverlay.ShouldShowScanAreaGuides; }
            set { this.BarcodeCaptureOverlay.ShouldShowScanAreaGuides = value; }
        }

        public void SetScanAreaTopMargin(FloatWithUnit topMargin)
        {
            this.ScanAreaMargins = new MarginsWithUnit(
                this.ScanAreaMargins.Left,
                topMargin,
                this.ScanAreaMargins.Right,
                this.ScanAreaMargins.Bottom);
        }

        public void SetScanAreaRightMargin(FloatWithUnit rightMargin)
        {
            this.ScanAreaMargins = new MarginsWithUnit(
                this.ScanAreaMargins.Left,
                this.ScanAreaMargins.Top,
                rightMargin,
                this.ScanAreaMargins.Bottom);
        }

        public void SetScanAreaBottomMargin(FloatWithUnit bottomMargin)
        {
            this.ScanAreaMargins = new MarginsWithUnit(
                this.ScanAreaMargins.Left,
                this.ScanAreaMargins.Top,
                this.ScanAreaMargins.Right,
                bottomMargin);
        }

        public void SetScanAreaLeftMargin(FloatWithUnit leftMargin)
        {
            this.ScanAreaMargins = new MarginsWithUnit(
                leftMargin,
                this.ScanAreaMargins.Top,
                this.ScanAreaMargins.Right,
                this.ScanAreaMargins.Bottom);
        }
        #endregion

        #region Viewfinder Settings
        public IViewfinder CurrentViewfinder
        {
            get
            {
                return this.BarcodeCaptureOverlay.Viewfinder;
            }
            set
            {
                this.BarcodeCaptureOverlay.Viewfinder = value;
            }
        }
        public FloatWithUnit RectangularViewfinderWidth { get; set; }
        public FloatWithUnit RectangularViewfinderHeight { get; set; }
        public float RectangularViewfinderWidthAspect { get; set; }
        public float RectangularViewfinderHeightAspect { get; set; }
        public UiColor RectangularViewfinderColor { get; set; } = ViewfinderTypeRectangular.DefaultColor;
        public UiColor RectangularViewfinderDisabledColor { get; set; } = ViewfinderTypeRectangular.DisabledColors.Default;
        public SizeSpecification RectangularViewfinderSizeSpecification { get; set; } = SizeSpecification.WidthAndHeight;
        public FloatWithUnit LaserlineViewfinderWidth { get; set; } = new FloatWithUnit(0.75f, MeasureUnit.Fraction);
        public UiColor LaserlineViewfinderEnabledColor { get; set; } = ViewfinderTypeLaserline.EnabledColors.Default;
        public UiColor LaserlineViewfinderDisabledColor { get; set; } = ViewfinderTypeLaserline.DisabledColors.Default;
        public UiColor SpotlightViewfinderBackgroundColor { get; set; } = ViewfinderTypeSpotlight.BackgroundColors.Default;
        public UiColor SpotlightViewfinderEnabledColor { get; set; } = ViewfinderTypeSpotlight.EnabledColors.Default;
        public UiColor SpotlightViewfinderDisabledColor { get; set; } = ViewfinderTypeSpotlight.DisabledColors.Default;
        public SizeSpecification SpotlightViewfinderSizeSpecification { get; set; } = SizeSpecification.WidthAndHeight;
        public FloatWithUnit SpotlightViewfinderWidth { get; set; }
        public FloatWithUnit SpotlightViewfinderHeight { get; set; }
        public float SpotlightViewfinderWidthAspect { get; set; }
        public float SpotlightViewfinderHeightAspect { get; set; }

        #endregion

        #region Overlay Settings
        public Brush DefaultBrush => BarcodeCaptureOverlay.DefaultBrush;

        public Brush CurrentBrush
        {
            get
            {
                return this.BarcodeCaptureOverlay.Brush;
            }
            set
            {
                this.BarcodeCaptureOverlay.Brush = value;
            }
        }
        #endregion

        #region Point of Interest Settings
        public PointWithUnit PointOfInterest { get; set; } = new PointWithUnit(
            new FloatWithUnit(0.5f, MeasureUnit.Fraction),
            new FloatWithUnit(0.5f, MeasureUnit.Fraction)
        );
        #endregion

        #region Location Settings
        public ILocationSelection LocationSelection => this.BarcodeCaptureSettings.LocationSelection;
        public float LocationSelectionRadiusValue { get; set; }
        public MeasureUnit LocationSelectionRadiusMeasureUnit { get; set; } = MeasureUnit.Dip;
        public SizeSpecification LocationSelectionRectangularSizeSpecification { get; set; } = SizeSpecification.WidthAndHeight;
        public FloatWithUnit LocationSelectionRectangularWidth { get; set; } = new FloatWithUnit(0f, MeasureUnit.Dip);
        public FloatWithUnit LocationSelectionRectangularHeight { get; set; } = new FloatWithUnit(0f, MeasureUnit.Dip);
        public float LocationSelectionRectangularWidthAspect { get; set; }
        public float LocationSelectionRectangularHeightAspect { get; set; }

        public async Task SetLocationSelectionAsync(ILocationSelection locationSelection)
        {
            this.BarcodeCaptureSettings.LocationSelection = locationSelection;
            await this.ApplyBarcodeCaptureSettingsAsync();
        }

        public async Task SetLocationSelectionRadiusValueAsync(float value)
        {
            if (this.LocationSelection is RadiusLocationSelection)
            {
                MeasureUnit currentUnit = ((RadiusLocationSelection)this.LocationSelection).Radius.Unit;
                FloatWithUnit newRadius = new FloatWithUnit(value, currentUnit);
                this.LocationSelectionRadiusValue = value;
                await this.SetLocationSelectionAsync(RadiusLocationSelection.Create(newRadius));
            }
        }

        public async Task SetLocationSelectionRadiusMeasureUnitAsync(MeasureUnit value)
        {
            if (this.LocationSelection is RadiusLocationSelection)
            {
                float currentValue = ((RadiusLocationSelection)this.LocationSelection).Radius.Value;
                FloatWithUnit newRadius = new FloatWithUnit(currentValue, value);
                this.LocationSelectionRadiusMeasureUnit = value;
                await this.SetLocationSelectionAsync(RadiusLocationSelection.Create(newRadius));
            }
        }
        #endregion

        #region Feedback Settings
        public bool SoundEnabled
        {
            get
            {
                return this.BarcodeCapture.Feedback.Success.Sound != null;
            }
            set
            {
                var currentVibration = this.BarcodeCapture.Feedback.Success.Vibration;
                this.BarcodeCapture.Feedback.Success = new Feedback(currentVibration, value == true ? Sound.DefaultSound : null);
            }
        }

        public bool VibrationEnabled
        {
            get
            {
                return this.BarcodeCapture.Feedback.Success.Vibration != null;
            }
            set
            {
                var currentSound = this.BarcodeCapture.Feedback.Success.Sound;
                this.BarcodeCapture.Feedback.Success = new Feedback(value == true ? Vibration.DefaultVibration : null, currentSound);
            }
        }
        #endregion

        #region Symbology Settings
        public SymbologySettings GetSymbologySettings(Symbology symbology)
        {
            return this.BarcodeCaptureSettings.GetSymbologySettings(symbology);
        }

        public bool IsSymbologyEnabled(string symbologyIdentifier)
        {
            Symbology symbology = SymbologyDescription.ForIdentifier(symbologyIdentifier).Symbology;
            return this.IsSymbologyEnabled(symbology);
        }

        public bool IsSymbologyEnabled(Symbology symbology)
        {
            return this.GetSymbologySettings(symbology).Enabled;
        }

        public async Task EnableAllSymbologies(bool enabled)
        {
            foreach (SymbologyDescription description in SymbologyDescription.All())
            {
                this.BarcodeCaptureSettings.EnableSymbology(description.Symbology, enabled);
            }

            await this.ApplyBarcodeCaptureSettingsAsync();
        }

        public async Task EnableSymbologyAsync(Symbology symbology, bool enabled, bool updateBarcodeCaptureSettings = false)
        {
            this.BarcodeCaptureSettings.EnableSymbology(symbology, enabled);
            if (updateBarcodeCaptureSettings)
            {
                await this.ApplyBarcodeCaptureSettingsAsync();
            }
        }

        public IEnumerable<Symbology> EnabledSymbologies => this.BarcodeCaptureSettings.EnabledSymbologies;

        public bool IsColorInverted(Symbology symbology)
        {
            return this.GetSymbologySettings(symbology).ColorInvertedEnabled;
        }

        public async Task SetColorInvertedAsync(Symbology symbology, bool colorInvertible)
        {
            this.GetSymbologySettings(symbology).ColorInvertedEnabled = colorInvertible;
            await this.ApplyBarcodeCaptureSettingsAsync();
        }

        public bool IsExtensionEnabled(Symbology symbology, string extension)
        {
            return this.GetSymbologySettings(symbology).IsExtensionEnabled(extension);
        }

        public async Task SetExtensionEnabledAsync(Symbology symbology, string extension, bool enabled)
        {
            this.GetSymbologySettings(symbology).SetExtensionEnabled(extension, enabled);
            await this.ApplyBarcodeCaptureSettingsAsync();
        }

        public short GetMinSymbolCount(Symbology symbology)
        {
            return this.GetSymbologySettings(symbology).ActiveSymbolCounts.Min();
        }

        public async Task SetMinSymbolCountAsync(Symbology symbology, short minSymbolCount)
        {
            SymbologySettings symbologySettings = this.GetSymbologySettings(symbology);
            short maxSymbolCount = symbologySettings.ActiveSymbolCounts.Max();
            await this.SetSymbolCountAsync(symbologySettings, minSymbolCount, maxSymbolCount);
        }

        public short GetMaxSymbolCount(Symbology symbology)
        {
            return this.GetSymbologySettings(symbology).ActiveSymbolCounts.Max();
        }
        
        public async Task SetMaxSymbolCountAsync(Symbology symbology, short maxSymbolCount)
        {
            SymbologySettings symbologySettings = this.GetSymbologySettings(symbology);
            short minSymbologyCount = symbologySettings.ActiveSymbolCounts.Min();
            await this.SetSymbolCountAsync(symbologySettings, minSymbologyCount, maxSymbolCount);
        }

        private async Task SetSymbolCountAsync(SymbologySettings symbologySettings, short minSymbolCount, short maxSymbolCount)
        {
            HashSet<short> symbolCount = new HashSet<short>();
            
            if (minSymbolCount >= maxSymbolCount)
            {
                symbolCount.Add(maxSymbolCount);
            }
            else
            {
                for (short i = minSymbolCount; i <= maxSymbolCount; i++)
                {
                    symbolCount.Add(i);
                }
            }
            
            symbologySettings.ActiveSymbolCounts = symbolCount;
            await this.ApplyBarcodeCaptureSettingsAsync();
        }
        #endregion

        #region Camera Settings

        public TorchState TorchState { get; private set; }
        public CameraPosition CameraPosition { get; private set; } = CameraPosition.WorldFacing;
        public Camera CurrentCamera { get; private set; } = Camera.GetDefaultCamera();
        public CameraSettings CameraSettings { get; } = BarcodeCapture.RecommendedCameraSettings;

        public async Task SetCameraPositionAsync(CameraPosition cameraPosition)
        {
            if (this.CameraPosition != cameraPosition)
            {
                this.CameraPosition = cameraPosition;
                Camera camera = Camera.GetCamera(cameraPosition);
                if (camera != null)
                {
                    await camera.ApplySettingsAsync(this.CameraSettings);
                    await this.DataCaptureContext.SetFrameSourceAsync(camera);
                    this.CurrentCamera = camera;
                }
            }
        }

        public void SetTorchState(TorchState torchState)
        {
            this.TorchState = torchState;
            if (this.CurrentCamera != null)
            {
                this.CurrentCamera.DesiredTorchState = torchState;
            }
        }

        public VideoResolution VideoResolution => this.CameraSettings.PreferredResolution;

        public async Task SetVideoResolutionAsync(VideoResolution videoResolution)
        {
            this.CameraSettings.PreferredResolution = videoResolution;
            await this.ApplyCameraSettingsAsync();
        }

        public float MaxFrameRate => this.CameraSettings.MaxFrameRate;

        public async Task SetMaxFrameRateAsync(float value)
        {
            this.CameraSettings.MaxFrameRate = value;
            await this.ApplyCameraSettingsAsync();
        }
        
        public float ZoomFactor => this.CameraSettings.ZoomFactor;

        public async Task SetZoomFactorAsync(float value)
        {
            this.CameraSettings.ZoomFactor = value;
            await this.ApplyCameraSettingsAsync();
        }

        public float ZoomGestureZoomFactor => this.CameraSettings.ZoomGestureZoomFactor;

        public async Task SetZoomGestureZoomFactor(float value)
        {
            this.CameraSettings.ZoomGestureZoomFactor = value;
            await this.ApplyCameraSettingsAsync();
        }

        public FocusGestureStrategy FocusGestureStrategy => this.CameraSettings.FocusGestureStrategy;
        
        public async Task SetFocusGestureStrategy(FocusGestureStrategy strategy)
        {
            this.CameraSettings.FocusGestureStrategy = strategy;
            await this.ApplyCameraSettingsAsync();
        }
        #endregion

        private SettingsManager()
        {
            this.CurrentCamera?.ApplySettingsAsync(this.CameraSettings);

            // The barcode capturing process is configured through barcode capture settings
            // which are then applied to the barcode capture instance that manages barcode recognition.
            this.BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // Create data capture context using your license key and set the camera as the frame source.
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);
            this.DataCaptureContext.SetFrameSourceAsync(this.CurrentCamera);

            // Create new barcode capture mode with the settings from above.
            this.BarcodeCapture = BarcodeCapture.Create(this.DataCaptureContext, this.BarcodeCaptureSettings);
            this.BarcodeCapture.Enabled = true;

            // Create a new overlay with the barcode capture from above, and retrieve the default brush.
            this.BarcodeCaptureOverlay = BarcodeCaptureOverlay.Create(this.BarcodeCapture, null);

            // Create a temporary RectangularViewfinder instance to get default values for width and height.
            using RectangularViewfinder tempRectangularViewfinder = RectangularViewfinder.Create();
            this.RectangularViewfinderWidth = tempRectangularViewfinder.SizeWithUnitAndAspect.WidthAndHeight.Width;
            this.RectangularViewfinderHeight = tempRectangularViewfinder.SizeWithUnitAndAspect.WidthAndHeight.Height;

            // Create a temporary SpotlightViewfinder instance to get default values for width and height.
            using SpotlightViewfinder tempSpotlightViewfinder = SpotlightViewfinder.Create();
            this.SpotlightViewfinderWidth = tempSpotlightViewfinder.SizeWithUnitAndAspect.WidthAndHeight.Width;
            this.SpotlightViewfinderHeight = tempSpotlightViewfinder.SizeWithUnitAndAspect.WidthAndHeight.Height;
        }

        public async Task ApplyBarcodeCaptureSettingsAsync()
        {
            await this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
        }

        public async Task ApplyCameraSettingsAsync()
        {
            if (this.CurrentCamera != null)
            {
                await this.CurrentCamera.ApplySettingsAsync(this.CameraSettings);
            }
        }
    }
}
