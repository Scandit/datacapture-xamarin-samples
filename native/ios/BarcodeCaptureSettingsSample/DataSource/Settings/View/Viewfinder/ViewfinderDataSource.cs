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

        private readonly Section laserlineSettings;

        private readonly Section spotlightSettings;

        private readonly Section spotlightSizeType;
        
        private readonly FloatWithUnitRow rectangularWidth;

        private readonly FloatWithUnitRow rectangularHeight;

        private readonly FloatRow rectangularWidthAspect;

        private readonly FloatRow rectangularHeightAspect;

        private readonly FloatWithUnitRow spotlightWidth;

        private readonly FloatWithUnitRow spotlightHeight;

        private readonly FloatRow spotlightWidthAspect;

        private readonly FloatRow spotlightHeightAspect;

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

            this.spotlightWidth = FloatWithUnitRow.Create(
                "Width",
                () => SettingsManager.Instance.SpotlightWidth,
                unit => SettingsManager.Instance.SpotlightWidth = unit,
                this.DataSourceListener
            );

            this.spotlightHeight = FloatWithUnitRow.Create(
                "Height",
                () => SettingsManager.Instance.SpotlightHeight,
                unit => SettingsManager.Instance.SpotlightHeight = unit,
                this.DataSourceListener
            );

            this.spotlightWidthAspect = FloatRow.Create(
                "Width Aspect",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.SpotlightWidthAspect),
                () => SettingsManager.Instance.SpotlightWidthAspect,
                aspect => SettingsManager.Instance.SpotlightWidthAspect = aspect
            );

            this.spotlightHeightAspect = FloatRow.Create(
                "Height Aspect",
                () => NumberFormatter.Instance.FormatNFloat(SettingsManager.Instance.SpotlightHeightAspect),
                () => SettingsManager.Instance.SpotlightHeightAspect,
                aspect => SettingsManager.Instance.SpotlightHeightAspect = aspect
            );

            this.rectangularSettings = this.CreateRectangularSettings();
            this.rectangularSizeType = this.CreateRectangularSizeType();
            this.laserlineSettings = this.CreateLaserlineSettings();
            this.spotlightSettings = this.CreateSpotlightSettings();
            this.spotlightSizeType = this.CreateSpotlightSizeType();
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

        private Section CreateRectangularSettings()
        {
            return new Section(new Row[]
            {
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
                    "Spotlight",
                    () => SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Spotlight,
                    (_) => SettingsManager.Instance.ViewfinderKind = ViewfinderKind.Spotlight,
                    this.DataSourceListener)
            }, "Type");
        }

        private Section CreateLaserlineSettings()
        {
             return new Section(new Row[] 
             {
                 FloatWithUnitRow.Create(
                     "Width",
                     () => (SettingsManager.Instance.ViewfinderKind.Viewfinder as LaserlineViewfinder).Width,
                     unit => (SettingsManager.Instance.ViewfinderKind.Viewfinder as LaserlineViewfinder).Width = unit,
                     this.DataSourceListener
                 ),
                 ChoiceRow<LaserlineViewfinderEnabledColor>.Create(
                     "Enabled Color",
                     Enumeration.GetAll<LaserlineViewfinderEnabledColor>().ToArray(),
                     () => SettingsManager.Instance.LaserlineViewfinderEnabledColor,
                     color => SettingsManager.Instance.LaserlineViewfinderEnabledColor = color,
                     this.DataSourceListener
                 ),
                 ChoiceRow<LaserlineViewfinderDisabledColor>.Create(
                     "Disabled Color",
                     Enumeration.GetAll<LaserlineViewfinderDisabledColor>().ToArray(),
                     () => SettingsManager.Instance.LaserlineViewfinderDisabledColor,
                     color => SettingsManager.Instance.LaserlineViewfinderDisabledColor = color,
                     this.DataSourceListener
                 )
             }, "Laserline");
        }

        private Section CreateSpotlightSettings()
        {
            return new Section(new Row[]
            {
                 ChoiceRow<SpotlightViewfinderBackgroundColor>.Create(
                     "Background Color",
                     Enumeration.GetAll<SpotlightViewfinderBackgroundColor>().ToArray(),
                     () => SettingsManager.Instance.SpotlightViewfinderBackgroundColor,
                     color => SettingsManager.Instance.SpotlightViewfinderBackgroundColor = color,
                     this.DataSourceListener
                 ),
                 ChoiceRow<SpotlightViewfinderEnabledColor>.Create(
                     "Enabled Color",
                     Enumeration.GetAll<SpotlightViewfinderEnabledColor>().ToArray(),
                     () => SettingsManager.Instance.SpotlightViewfinderEnabledColor,
                     color => SettingsManager.Instance.SpotlightViewfinderEnabledColor = color,
                     this.DataSourceListener
                 ),
                 ChoiceRow<SpotlightViewfinderDisabledColor>.Create(
                     "Disabled Color",
                     Enumeration.GetAll<SpotlightViewfinderDisabledColor>().ToArray(),
                     () => SettingsManager.Instance.SpotlightViewfinderDisabledColor,
                     color => SettingsManager.Instance.SpotlightViewfinderDisabledColor = color,
                     this.DataSourceListener
                 )
            }, "Spotlight");
        }

        private Section CreateSpotlightSizeType()
        {
            return new Section(new[]
            {
                ChoiceRow<SpotlightSizeSpecification>.Create(
                    "Size Specification",
                    Enumeration.GetAll<SpotlightSizeSpecification>().ToArray(),
                    () => SettingsManager.Instance.SpotlightViewfinderSizeSpecification,
                    spec => SettingsManager.Instance.SpotlightViewfinderSizeSpecification = spec,
                    this.DataSourceListener)
            });
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
                    sections.AddRange(new List<Section>() {this.rectangularSettings, this.rectangularSizeType});
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
                }
                else if (SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Laserline)
                {
                    sections.Add(this.laserlineSettings);
                }
                else if (SettingsManager.Instance.ViewfinderKind == ViewfinderKind.Spotlight)
                {
                    var sizeSpec = SettingsManager.Instance.SpotlightViewfinderSizeSpecification;
                    sections.AddRange(new List<Section>() { this.spotlightSettings, this.spotlightSizeType });
                    if (sizeSpec.Equals(SpotlightSizeSpecification.WidthAndHeight))
                    {
                        sections.Add(new Section(new Row[] { this.spotlightWidth, this.spotlightHeight }));
                    }
                    else if (sizeSpec.Equals(SpotlightSizeSpecification.WidthAndHeightAspect))
                    {
                        sections.Add(new Section(new Row[] { this.spotlightWidth, this.spotlightHeightAspect }));
                    }
                    else if (sizeSpec.Equals(SpotlightSizeSpecification.HeightAndWidthAspect))
                    {
                        sections.Add(new Section(new Row[] { this.spotlightWidthAspect, this.spotlightHeight }));
                    }
                }

                return sections.ToArray();
            }
        }
    }
}
