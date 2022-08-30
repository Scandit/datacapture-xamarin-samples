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

using System.Collections.Generic;
using System.Linq;
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class ViewfinderDataSource : IDataSource
    {
        private readonly Section viewfinderType;

        private readonly Section rectangularSettings;

        private readonly Section rectangularSizeType;

        private Section laserlineSettings;

        private readonly Section aimerSettings;

        private readonly FloatWithUnitRow rectangularWidth;

        private readonly FloatWithUnitRow rectangularHeight;

        private readonly FloatRow rectangularShorterDimension;

        private readonly FloatRow rectangularWidthAspect;

        private readonly FloatRow rectangularHeightAspect;

        private readonly FloatRow rectangularLongerDimensioApsect;

        public ViewfinderDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;

            this.rectangularWidth = FloatWithUnitRow.Create(
                "Width",
                () => SettingsManager.Instance.RectangularWidth, 
                unit => SettingsManager.Instance.RectangularWidth = unit,
                this.DataSourceListener
            );

            this.rectangularHeight = FloatWithUnitRow.Create(
                "Height",
                () => SettingsManager.Instance.RectangularHeight,
                unit => SettingsManager.Instance.RectangularHeight = unit,
                this.DataSourceListener
            );

            this.rectangularShorterDimension = FloatRow.Create(
                "Shorter Dimension",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.RectangularShorterDimension),
                () => SettingsManager.Instance.RectangularShorterDimension,
                aspect => SettingsManager.Instance.RectangularShorterDimension = aspect
            );

            this.rectangularWidthAspect = FloatRow.Create(
                "Width Aspect",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.RectangularWidthAspect),
                () => SettingsManager.Instance.RectangularWidthAspect,
                aspect => SettingsManager.Instance.RectangularWidthAspect = aspect
            );
            
            this.rectangularHeightAspect = FloatRow.Create(
                "Height Aspect",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.RectangularHeightAspect),
                () => SettingsManager.Instance.RectangularHeightAspect,
                aspect => SettingsManager.Instance.RectangularHeightAspect = aspect 
            );

            this.rectangularLongerDimensioApsect = FloatRow.Create(
                "Longer Dimension Aspect",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.RectangularLongerDimensionAspect),
                () => SettingsManager.Instance.RectangularLongerDimensionAspect,
                aspect => SettingsManager.Instance.RectangularLongerDimensionAspect = aspect
            );

            this.rectangularSettings = this.CreateRectangularSettings();
            this.rectangularSizeType = this.CreateRectangularSizeType();
            this.laserlineSettings = this.CreateLaserlineSettings();
            this.aimerSettings = this.CreateAimerSettings();
            this.viewfinderType = this.CreateTypeSection();
        }

        private Section CreateRectangularSizeType()
        {
            return new Section(new[] 
            {
                ChoiceRow<RectangularSizeSpecification>.Create(
                    "Size Specification",
                    Enumeration.GetAll<RectangularSizeSpecification>().ToArray(),
                    () => SettingsManager.Instance.ViewfinderSizeSpecification,
                    spec => SettingsManager.Instance.ViewfinderSizeSpecification = spec, 
                    this.DataSourceListener)
            });
        }

        private Section CreateRectangularAnimation()
        {
            var animationOption = SwitchRow.Create(
                    "Animation",
                    () => SettingsManager.Instance.RectangularViewfinderAnimation != null,
                    enabled =>
                    {
                        SettingsManager.Instance.RectangularViewfinderAnimation = enabled ?
                          new RectangularViewfinderAnimation(looping: false) : null;
                        this.DataSourceListener.OnDataChange();
                    });

            var loopingOption = SwitchRow.Create(
                    "Looping",
                    () => SettingsManager.Instance.RectangularViewfinderAnimation?.Looping ?? false,
                    enabled =>
                    {
                        SettingsManager.Instance.RectangularViewfinderAnimation =
                            new RectangularViewfinderAnimation(looping: enabled);
                    });

            if (SettingsManager.Instance.RectangularViewfinderAnimation != null)
            {
                return new Section(new[] { animationOption, loopingOption });
            }

            return new Section(new[] { animationOption });
        }

        private Section CreateRectangularSettings()
        {
            return new Section(new Row[]
            {
                ChoiceRow<RectangularViewfinderStyleType>.Create(
                    "Style",
                    Enumeration.GetAll<RectangularViewfinderStyleType>().ToArray(),
                    () => SettingsManager.Instance.RectangularViewfinderStyleType,
                    newStyle =>
                    {
                        SettingsManager.Instance.RectangularViewfinderStyleType = newStyle;
                        SettingsManager.Instance.ViewfinderKind = ViewfinderKind.UpdateRectangularStyle(newStyle);
                    },
                    this.DataSourceListener),
                ChoiceRow<RectangularViewfinderLineStyleType>.Create(
                    "Line Style",
                    Enumeration.GetAll<RectangularViewfinderLineStyleType>().ToArray(),
                    () => SettingsManager.Instance.RectangularViewfinderLineStyleType,
                    newLineStyle =>
                    {
                        SettingsManager.Instance.RectangularViewfinderLineStyleType = newLineStyle;
                        SettingsManager.Instance.ViewfinderKind = ViewfinderKind.UpdateRectangularStyle(
                            SettingsManager.Instance.RectangularViewfinderStyleType, newLineStyle);
                    },
                    this.DataSourceListener),
                FloatRow.Create(
                    "Dimming (0.0 - 1.0)",
                    () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.RectangularViewfinderDimming),
                    () => SettingsManager.Instance.RectangularViewfinderDimming,
                    value => SettingsManager.Instance.RectangularViewfinderDimming = value
                    ),
                ChoiceRow<RectangularViewfinderColor>.Create(
                    "Color",
                    Enumeration.GetAll<RectangularViewfinderColor>().ToArray(),
                    () => SettingsManager.Instance.RectangularViewfinderColor,
                    newColor => SettingsManager.Instance.RectangularViewfinderColor = newColor,
                    this.DataSourceListener),
                ChoiceRow<RectangularViewfinderDisabledColor>.Create(
                     "Disabled Color",
                     Enumeration.GetAll<RectangularViewfinderDisabledColor>().ToArray(),
                     () => SettingsManager.Instance.RectangularViewfinderDisabledColor,
                     newColor => SettingsManager.Instance.RectangularViewfinderDisabledColor = newColor,
                     this.DataSourceListener
                 )
            }, "Rectangular");
        }

        private Section CreateTypeSection()
        {
            return new Section(new []
            {
                BoolOptionRow.Create(
                    "None",
                    () => SettingsManager.Instance.ViewfinderKind == ViewfinderKind.None,
                    (_) => SettingsManager.Instance.ViewfinderKind = ViewfinderKind.None,
                    this.DataSourceListener
                ),
                BoolOptionRow.Create(
                    "Rectangular",
                    () => SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Rectangular,
                    (_) => SettingsManager.Instance.ViewfinderKind = ViewfinderKind.Rectangular,
                    this.DataSourceListener
                ),
                BoolOptionRow.Create(
                    "Laserline",
                    () => SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Laserline,
                    (_) => SettingsManager.Instance.ViewfinderKind = ViewfinderKind.Laserline,
                    this.DataSourceListener
                ),
                BoolOptionRow.Create(
                    "Aimer",
                    () => SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Aimer,
                    (_) => SettingsManager.Instance.ViewfinderKind = ViewfinderKind.Aimer,
                    this.DataSourceListener
                )
            }, "Type");
        }

        private Section CreateLaserlineSettings()
        {
             return new Section(new Row[] 
             {
                 ChoiceRow<LaserlineViewfinderStyleType>.Create(
                    "Style",
                    Enumeration.GetAll<LaserlineViewfinderStyleType>().ToArray(),
                    () => SettingsManager.Instance.LaserlineViewfinderStyleType,
                    newStyle =>
                    {
                        SettingsManager.Instance.LaserlineViewfinderStyleType = newStyle;
                        SettingsManager.Instance.ViewfinderKind = ViewfinderKind.UpdateLaserlineStyle(newStyle);
                        this.laserlineSettings = this.CreateLaserlineSettings();
                    },
                    this.DataSourceListener),
                 FloatWithUnitRow.Create(
                     "Width",
                     () => (SettingsManager.Instance.ViewfinderKind.Viewfinder as LaserlineViewfinder).Width,
                     unit => (SettingsManager.Instance.ViewfinderKind.Viewfinder as LaserlineViewfinder).Width = unit,
                     this.DataSourceListener
                 ),
                 this.LaserlineViewfinderEnabledColorRow(),
                 ChoiceRow<LaserlineViewfinderDisabledColor>.Create(
                     "Disabled Color",
                     Enumeration.GetAll<LaserlineViewfinderDisabledColor>().ToArray(),
                     () => SettingsManager.Instance.LaserlineViewfinderDisabledColor,
                     color => SettingsManager.Instance.LaserlineViewfinderDisabledColor = color,
                     this.DataSourceListener
                 )
             }, "Laserline");
        }

        private Row LaserlineViewfinderEnabledColorRow()
        {
            Row enabledColorRow;
            if (Equals(SettingsManager.Instance.LaserlineViewfinderStyleType, LaserlineViewfinderStyleType.Animated))
            {
                enabledColorRow = ChoiceRow<LaserlineViewfinderAnimatedEnabledColor>.Create(
                    "Enabled Color",
                    Enumeration.GetAll<LaserlineViewfinderAnimatedEnabledColor>().ToArray(),
                    () => SettingsManager.Instance.LaserlineViewfinderAnimatedEnabledColor,
                    color => SettingsManager.Instance.LaserlineViewfinderAnimatedEnabledColor = color,
                    this.DataSourceListener
                );
            }
            else
            {
                enabledColorRow = ChoiceRow<LaserlineViewfinderEnabledColor>.Create(
                    "Enabled Color",
                    Enumeration.GetAll<LaserlineViewfinderEnabledColor>().ToArray(),
                    () => SettingsManager.Instance.LaserlineViewfinderEnabledColor,
                    color => SettingsManager.Instance.LaserlineViewfinderEnabledColor = color,
                    this.DataSourceListener
                );
            }

            return enabledColorRow;
        }

        private Section CreateAimerSettings()
        {
            return new Section(new Row[]
            {
                 ChoiceRow<AimerViewfinderFrameColor>.Create(
                     "Frame Color",
                     Enumeration.GetAll<AimerViewfinderFrameColor>().ToArray(),
                     () => SettingsManager.Instance.AimerViewfinderFrameColor,
                     color => SettingsManager.Instance.AimerViewfinderFrameColor = color,
                     this.DataSourceListener
                 ),
                 ChoiceRow<AimerViewfinderDotColor>.Create(
                     "Dot Color",
                     Enumeration.GetAll<AimerViewfinderDotColor>().ToArray(),
                     () => SettingsManager.Instance.AimerViewfinderDotColor,
                     color => SettingsManager.Instance.AimerViewfinderDotColor = color,
                     this.DataSourceListener
                 )
            }, "Aimer");
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections
        {
            get
            {
                var sections = new List<Section> {this.viewfinderType};
                if (SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Rectangular)
                {
                    var sizeSpec = SettingsManager.Instance.ViewfinderSizeSpecification;
                    sections.AddRange(new List<Section>() {
                        this.rectangularSettings,
                        this.CreateRectangularAnimation(),
                        this.rectangularSizeType });

                    if (sizeSpec.Equals(RectangularSizeSpecification.WidthAndHeight))
                    {
                        sections.Add(new Section(new Row[]{this.rectangularWidth, this.rectangularHeight}));
                    }
                    else if (sizeSpec.Equals(RectangularSizeSpecification.WidthAndHeightAspect))
                    {
                        sections.Add(new Section(new Row[]{this.rectangularWidth, this.rectangularHeightAspect}));
                    }
                    else if (sizeSpec.Equals(RectangularSizeSpecification.HeightAndWidthAspect))
                    {
                        sections.Add(new Section(new Row[]{this.rectangularWidthAspect, this.rectangularHeight}));
                    }
                    else if (sizeSpec.Equals(RectangularSizeSpecification.ShorterDimensionAndAspectRatio))
                    {
                        sections.Add(new Section(new Row[]{this.rectangularShorterDimension, this.rectangularLongerDimensioApsect }));
                    }
                }
                else if (SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Laserline)
                {
                    sections.Add(this.laserlineSettings);
                }
                else if (SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Aimer)
                {
                    sections.Add(this.aimerSettings);
                }

                return sections.ToArray();
            }
        }
    }
}
