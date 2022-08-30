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
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder;
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture
{
    public class LocationSelectionDataSource : IDataSource
    {
        public LocationSelectionDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections => this.CreateSections();

        private Section[] CreateSections()
        {
            var sections = new List<Section>() {new Section(new []
            {
                OptionRow<bool>.Create("None",
                    () => SettingsManager.Instance.LocationSelection == null, 
                    tuple => SettingsManager.Instance.LocationSelection = null, 
                    this.DataSourceListener
                ),
                OptionRow<bool>.Create("Radius",
                    () => SettingsManager.Instance.LocationSelection is RadiusLocationSelection,
                    _ => SettingsManager.Instance.LocationSelection = RadiusLocationSelection.Create(FloatWithUnit.Zero),
                    this.DataSourceListener
                ),
                OptionRow<bool>.Create("Rectangular",
                    () => SettingsManager.Instance.LocationSelection is RectangularLocationSelection,
                    _ => SettingsManager.Instance.LocationSelection = RectangularLocationSelection.Create(SizeWithUnit.Zero),
                    this.DataSourceListener
                )
            }, "Type")};

            switch (SettingsManager.Instance.LocationSelection)
            {
                case RadiusLocationSelection radiusLocationSelection:
                {
                     var radiusSettings = new Section(new[]
                     {
                         FloatWithUnitRow.Create(
                             "Size",
                             () => radiusLocationSelection.Radius,
                             value => SettingsManager.Instance.LocationSelection = RadiusLocationSelection.Create(value),
                             this.DataSourceListener
                         )
                     }, "Radius");
                     sections.Add(radiusSettings);
                     break;
                }
                case RectangularLocationSelection rectangularLocationSelection:
                {
                    var rectangularSettings = new Section(new []
                    {
                        ChoiceRow<RectangularSizeSpecification>.Create(
                            "Size Specification",
                            new [] {
                                RectangularSizeSpecification.WidthAndHeight,
                                RectangularSizeSpecification.WidthAndHeightAspect,
                                RectangularSizeSpecification.HeightAndWidthAspect},
                            () => this.GetCurrentSizeSpecification(),
                            specification => this.SetCurrentSizeSpecification(specification),
                            this.DataSourceListener
                        ), 
                    }, "Rectangular");
                    sections.Add(rectangularSettings);
                    switch (rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode)
                    {
                        case SizingMode.WidthAndHeight:
                        {
                            sections.Add(new Section(new[]
                            {
                                this.CreateRectangularWidthRow(),
                                this.CreateRectangularHeightRow()
                            }));
                            break;
                        }
                        case SizingMode.WidthAndAspectRatio:
                        {
                            sections.Add(new Section(new Row[]
                            {
                                this.CreateRectangularWidthRow(),
                                this.CreateHeightAspectRow()
                            }));
                            break;
                        }
                        case SizingMode.HeightAndAspectRatio:
                        {
                            sections.Add(new Section(new Row[]
                            {
                                this.CreateRectangularHeightRow(),
                                this.CreateWidthAspectRow()
                            }));
                            break;
                        }
                    }
                    break;
                }
            }

            return sections.ToArray();
        }

        private FloatWithUnitRow CreateRectangularWidthRow()
        {
            return FloatWithUnitRow.Create(
                "Width",
                () => this.GetRectangularWidth(),
                value => this.SetRectangularWidth(value),
                this.DataSourceListener
            );
        }

        private FloatWithUnitRow CreateRectangularHeightRow()
        {
            return FloatWithUnitRow.Create(
                "Height",
                () => this.GetRectangularHeight(),
                value => this.SetRectangularHeight(value),
                this.DataSourceListener
            );
        }

        private FloatRow CreateHeightAspectRow()
        {
            return FloatRow.Create(
                "Height Aspect",
                () => NumberFormatter.Instance.FormatNFloat(this.GetRectangularAspect()),
                () => this.GetRectangularAspect(),
                value => this.SetRectangularAspect(value)
            );
        }

        private FloatRow CreateWidthAspectRow()
        {
            return FloatRow.Create(
                "Width Aspect",
                () => NumberFormatter.Instance.FormatNFloat(this.GetRectangularAspect()),
                () => this.GetRectangularAspect(),
                value => this.SetRectangularAspect(value)
            );
        }

        private RectangularSizeSpecification GetCurrentSizeSpecification()
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                throw new InvalidOperationException("The current location selection is not RectangularLocationSelection");
            }
            return rectangularLocationSelection.SizeWithUnitAndAspect.SizeSpecification();
        }

        private void SetCurrentSizeSpecification(RectangularSizeSpecification specification)
        {
            if (specification.Equals(RectangularSizeSpecification.WidthAndHeight))
            {
                SettingsManager.Instance.LocationSelection = RectangularLocationSelection.Create(UnitExtenstions.FullSize);
            }
            else if (specification.Equals(RectangularSizeSpecification.WidthAndHeightAspect))
            {
                SettingsManager.Instance.LocationSelection = RectangularLocationSelection.CreateWithWidthAndAspectRatio(FloatWithUnit.Zero, 1);
            }
            else if (specification.Equals(RectangularSizeSpecification.HeightAndWidthAspect))
            {
                SettingsManager.Instance.LocationSelection = RectangularLocationSelection.CreateWithHeightAndAspectRatio(FloatWithUnit.Zero, 1);
            }
        }

        private FloatWithUnit GetRectangularWidth()
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return FloatWithUnit.Zero;
            }
            return rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.WidthAndAspectRatio => rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndAspectRatio.Size,
                SizingMode.WidthAndHeight => rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndHeight.Width,
                _ => FloatWithUnit.Zero
            };
        }

        private void SetRectangularWidth(FloatWithUnit width)
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return;
            }
            ILocationSelection locationSelection = rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.WidthAndHeight =>
                    SettingsManager.Instance.LocationSelection =
                        RectangularLocationSelection.Create(
                            new SizeWithUnit { Width = width, Height = rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndHeight.Height }),
                SizingMode.WidthAndAspectRatio =>
                    SettingsManager.Instance.LocationSelection =
                        RectangularLocationSelection.CreateWithWidthAndAspectRatio(width, rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndAspectRatio.Aspect),
                _ => SettingsManager.Instance.LocationSelection
            };
            SettingsManager.Instance.LocationSelection = locationSelection;
        }

        private FloatWithUnit GetRectangularHeight()
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return FloatWithUnit.Zero;
            }
            return rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.HeightAndAspectRatio => rectangularLocationSelection.SizeWithUnitAndAspect.HeightAndAspectRatio.Size,
                SizingMode.WidthAndHeight => rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndHeight.Height,
                _ => FloatWithUnit.Zero
            };
        }

        private void SetRectangularHeight(FloatWithUnit height)
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return;
            }
            ILocationSelection locationSelection = rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.WidthAndHeight =>
                    RectangularLocationSelection.Create(
                        new SizeWithUnit { Width = rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndHeight.Width, Height = height }),
                SizingMode.HeightAndAspectRatio =>
                        RectangularLocationSelection.CreateWithHeightAndAspectRatio(
                            height, rectangularLocationSelection.SizeWithUnitAndAspect.HeightAndAspectRatio.Aspect),
                _ => rectangularLocationSelection
            };
            SettingsManager.Instance.LocationSelection = locationSelection;
        }

        private nfloat GetRectangularAspect()
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return .0f;
            }
            return rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.WidthAndAspectRatio => rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndAspectRatio.Aspect,
                SizingMode.HeightAndAspectRatio => rectangularLocationSelection.SizeWithUnitAndAspect.HeightAndAspectRatio.Aspect,
                _ => .0f
            };
        }

        private void SetRectangularAspect(nfloat aspect)
        {
            if (!(SettingsManager.Instance.LocationSelection is RectangularLocationSelection rectangularLocationSelection))
            {
                return;
            }
            ILocationSelection locationSelection = rectangularLocationSelection.SizeWithUnitAndAspect.SizingMode switch
            {
                SizingMode.WidthAndAspectRatio =>
                    RectangularLocationSelection.CreateWithWidthAndAspectRatio(
                        rectangularLocationSelection.SizeWithUnitAndAspect.WidthAndAspectRatio.Size,
                        aspect
                    ),
                SizingMode.HeightAndAspectRatio =>
                    RectangularLocationSelection.CreateWithHeightAndAspectRatio(
                        rectangularLocationSelection.SizeWithUnitAndAspect.HeightAndAspectRatio.Size,
                        aspect
                    ),
                _ => rectangularLocationSelection
            };
            SettingsManager.Instance.LocationSelection = locationSelection;
        }
    }
}
