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
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    public class ViewHolder : RecyclerView.ViewHolder
    {
        private readonly TextView textData;
        private readonly TextView textSymbology;

        public ViewHolder(View itemView) : base(itemView)
        {
            var color = new Color(ContextCompat.GetColor(itemView.Context, Resource.Color.scanditBlue));

            this.textData = itemView.FindViewById<TextView>(Android.Resource.Id.Text1);
            this.textSymbology = itemView.FindViewById<TextView>(Android.Resource.Id.Text2);
            this.textSymbology.SetTextColor(color);
        }

        public void Bind(Barcode result)
        {
            if (result != null)
            {
                this.textData.Text = !string.IsNullOrEmpty(result.Data) ? result.Data : string.Empty;

                // Get the human readable name of the symbology.
                string symbology = SymbologyDescription.Create(result.Symbology).ReadableName;
                this.textSymbology.Text = symbology;
            }
        }
    }

    public class SplitViewAdapter : RecyclerView.Adapter
    {
        private readonly IList<Barcode> results;

        public SplitViewAdapter(IList<Barcode> results)
        {
            this.results = results ?? throw new ArgumentNullException(nameof(results));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new ViewHolder(inflater.Inflate(Android.Resource.Layout.SimpleListItem2, parent, false));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as ViewHolder)?.Bind(this.results[position]);
        }

        public override int ItemCount => this.results.Count;
    }
}
