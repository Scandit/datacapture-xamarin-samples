﻿/*
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

using Android.Views;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Base.MeasureUnits
{
    public class MeasureUnitViewHolder : TwoTextsAndIconViewHolder
    {
        public MeasureUnitViewHolder(View itemView) : base(itemView, Resource.Id.text_field, Resource.Id.text_field_2)
        { }

        public void Bind(MeasureUnitItem entry)
        {
            entry.RequireNotNull(nameof(entry));

            this.SetFirstTextView(entry.MeasureUnit.Name());
            this.SetIcon(entry.Enabled ? Resource.Drawable.ic_check : 0);
        }
    }
}