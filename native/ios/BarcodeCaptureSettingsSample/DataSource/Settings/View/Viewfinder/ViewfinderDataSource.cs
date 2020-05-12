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
        
        private readonly FloatWithUnitRow rectangularWidth;

        private readonly FloatWithUnitRow rectangularHeight;

        private readonly FloatRow rectangularWidthAspect;

        private readonly FloatRow rectangularHeightAspect;

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

            this.rectangularSettings = this.CreateRectangularSettings();
            this.rectangularSizeType = this.CreateRectangularSizeType();
            this.laserlineSettings = this.CreateLaserlineSettings();
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
            return new Section(new[]
            {
                ChoiceRow<RectangularViewfinderColor>.Create(
                    "Color",
                    Enumeration.GetAll<RectangularViewfinderColor>().ToArray(),
                    () => SettingsManager.Instance.RectangularViewfinderColor,
                    newColor => SettingsManager.Instance.RectangularViewfinderColor = newColor,
                    this.DataSourceListener)
            }, "Rectangular");
        }

        private Section CreateTypeSection()
        {
            return new Section(new []
            {
                BoolOptionRow.Create(
                    "None",
                    () => SettingsManager.Instance.Viewfinder == null,
                    (_) => SettingsManager.Instance.Viewfinder = null,
                    this.DataSourceListener
                ),
                BoolOptionRow.Create(
                    "Rectangular",
                    () => SettingsManager.Instance.Viewfinder is RectangularViewfinder,
                    (_) => SettingsManager.Instance.Viewfinder = RectangularViewfinder.Create(),
                    this.DataSourceListener
                ),
                BoolOptionRow.Create(
                    "Laserline",
                    () => SettingsManager.Instance.Viewfinder is LaserlineViewfinder,
                    (_) => SettingsManager.Instance.Viewfinder = LaserlineViewfinder.Create(),
                    this.DataSourceListener
                )
            }, "Type");
        }

        private Section CreateLaserlineSettings()
        {
             return new Section(new Row[] 
             {
                 FloatWithUnitRow.Create(
                     "Width",
                     () => (SettingsManager.Instance.Viewfinder as LaserlineViewfinder).Width,
                     unit => (SettingsManager.Instance.Viewfinder as LaserlineViewfinder).Width = unit,
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

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections
        {
            get
            {
                var sections = new List<Section> {this.viewfinderType};
                if (SettingsManager.Instance.Viewfinder is RectangularViewfinder)
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
                else if (SettingsManager.Instance.Viewfinder is LaserlineViewfinder)
                {
                    sections.Add(this.laserlineSettings);
                }

                return sections.ToArray();
            }
        }
    }
}
