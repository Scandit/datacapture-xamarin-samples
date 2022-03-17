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

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using System.Collections;

namespace IdCaptureExtendedSample.Result
{
    public class ResultListAdapter : ListAdapter
    { 
        public ResultListAdapter(IList entries) : base(new ItemCallback())
        {
            this.SubmitList(entries);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new ResultListViewHolder(
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.result_item, parent, false)
            );
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ResultListViewHolder viewHolder)
            {
                viewHolder.Bind(this.GetItem(position) as ResultEntry);
            }
        }
    }

    public class ResultListViewHolder : RecyclerView.ViewHolder
    {
        public ResultListViewHolder(View itemView) : base(itemView)
        { }

        public void Bind(ResultEntry item)
        {
            if (item != null)
            {
                this.ItemView.FindViewById<TextView>(Resource.Id.text_value).Text = GetValue(item);
                this.ItemView.FindViewById<TextView>(Resource.Id.text_key).Text = item.Title;
            }
        }

        private static string GetValue(ResultEntry item)
        {
            return !string.IsNullOrEmpty(item.Value) ? item.Value : string.Empty;
        }
    }

    public class ItemCallback : DiffUtil.ItemCallback
    {
        public override bool AreContentsTheSame(Object oldItem, Object newItem)
        {
            if (object.ReferenceEquals(oldItem, newItem))
            {
                return true;
            }

            if (oldItem == null || newItem == null)
            {
                return false;
            }

            if (oldItem is ResultEntry oldResultEntry && newItem is ResultEntry newResultEntry)
            {
                return string.Equals(oldResultEntry.Value, newResultEntry.Value);
            }

            return false;
        }

        public override bool AreItemsTheSame(Object oldItem, Object newItem)
        {
            if (object.ReferenceEquals(oldItem, newItem))
            {
                return true;
            }

            if (oldItem == null || newItem == null)
            {
                return false;
            }

            if (oldItem is ResultEntry oldResultEntry && newItem is ResultEntry newResultEntry)
            {
                return string.Equals(oldResultEntry.Title, newResultEntry.Title);
            }

            return false;
        }
    }
}
