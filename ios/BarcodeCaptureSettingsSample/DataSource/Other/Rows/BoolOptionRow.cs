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
using BarcodeCaptureSettingsSample.Extensions;
using Foundation;

namespace BarcodeCaptureSettingsSample.DataSource.Other.Rows
{
    public class BoolOptionRow : OptionRow<bool>
    {
        protected BoolOptionRow(string title,
            Func<bool> getter,
            Action<bool> setter,
            Action<Tuple<OptionRow<bool>, NSIndexPath>> onSelect) : base(title, getter, setter, onSelect) { }

        public override void CellSelected(Row row, NSIndexPath indexPath)
        {
            var tuple = new Tuple<OptionRow<bool>, NSIndexPath>(this, indexPath);
            this.OnSelect?.Invoke(tuple);
        }

        public static BoolOptionRow Create(string title,
                                           Func<bool> getter,
                                           Action<bool> setter,
                                           IDataSourceListener dataSourceListener)
        {
            getter.RequireNotNull(nameof(getter));
            setter.RequireNotNull(nameof(setter));
            dataSourceListener.RequireNotNull(nameof(dataSourceListener));
            return new BoolOptionRow(title, getter, b =>
            {
                setter(b);
                dataSourceListener.OnDataChange();
            }, tuple =>
            {
                var row = tuple.Item1;
                row.Setter(!row.Getter());
            });
        }
    }
}
