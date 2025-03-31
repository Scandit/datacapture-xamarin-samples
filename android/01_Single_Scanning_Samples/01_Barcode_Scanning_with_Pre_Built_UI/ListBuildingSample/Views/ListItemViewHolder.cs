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
using ListBuildingSample.Models;

namespace ListBuildingSample.Views
{
    public class ListItemViewHolder : RecyclerView.ViewHolder
    {
        private TextView title;
        private TextView description;
        private ImageView icon;

        public ListItemViewHolder(View itemView) : base(itemView)
        {
            this.title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            this.description = itemView.FindViewById<TextView>(Resource.Id.item_description);
            this.icon = itemView.FindViewById<ImageView>(Resource.Id.item_icon);
        }

        public void Bind(ListItem result, int position)
        {
            this.title.Text = $"Item {result.Number}";
            this.description.Text = $"{result.Symbology}: {result.Data}";
            this.icon.SetImageBitmap(result.Image);
        }
    }
}
