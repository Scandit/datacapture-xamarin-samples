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
using AndroidX.RecyclerView.Widget;

namespace BarcodeCaptureSettingsSample.Base
{ 
    public class TypedDiffUtilCallback<T> : DiffUtil.Callback where T : IEquatable<T>
    {
        private readonly T[] oldEntries;
        private readonly T[] newEntries;

        public override int NewListSize => newEntries.Length;

        public override int OldListSize => oldEntries.Length;

        public TypedDiffUtilCallback(T[] oldEntries, T[] newEntries)
        {
            this.oldEntries = oldEntries;
            this.newEntries = newEntries;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            return this.oldEntries[oldItemPosition].Equals(this.newEntries[newItemPosition]);
        }

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            return this.oldEntries[oldItemPosition].Equals(this.newEntries[newItemPosition]);
        }
    }
}
