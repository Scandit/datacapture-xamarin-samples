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
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Transitions;
using MatrixScanBubblesSample.Scan.Bubbles.Data;

namespace MatrixScanBubblesSample.Scan.Bubbles
{
    public class Bubble
    {
        private readonly View containerShelfData;
        private readonly TextView textCode;
        private readonly Transition transition = new Fade();
        private bool showShelfData = true;

        private class OnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action onClickAction;

            public OnClickListener(Action onClickAction) => this.onClickAction = onClickAction;

            public void OnClick(View view) => this.onClickAction?.Invoke();
        }

        public View Root { get; }

        public Bubble(Context context, BubbleData data, string code)
        {
            this.Root = View.Inflate(context, Resource.Layout.bubble, null);
            this.Root.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            this.containerShelfData = this.Root.FindViewById(Resource.Id.shelf_container);
            this.textCode = this.Root.FindViewById<TextView>(Resource.Id.text_code);
            TextView textShelfData = this.Root.FindViewById<TextView>(Resource.Id.text_shelf_data);
            string textFormat = context.GetString(Resource.String.shelf_text);
            textShelfData.Text = string.Format(textFormat, data.ShelfCount, data.BackroomCount);

            this.transition.SetDuration(100);

            // We want to show the scanned code when tapping on the bubble. To do so, it's enough to
            // just add a regular View.OnClickListener on the view.
            this.Root.SetOnClickListener(new OnClickListener(() => this.ToggleText()));         
            this.textCode.Text = code;
            this.ShowCurrentData();
        }

        public void Show()
        {
            this.StartTransition();
            this.Root.Visibility = ViewStates.Visible;
        }

        public void Hide()
        {
            this.StartTransition();
            this.Root.Visibility = ViewStates.Gone;
        }

        private void StartTransition()
        {
            TransitionManager.BeginDelayedTransition((ViewGroup)this.Root, this.transition);
        }

        private void ToggleText()
        {
            this.showShelfData = !this.showShelfData;
            this.ShowCurrentData();
        }

        private void ShowCurrentData()
        {
            if (this.showShelfData)
            {
                this.ShowShelfDataInternal();
            }
            else
            {
                this.ShowCodeInternal();
            }
        }

        private void ShowShelfDataInternal()
        {
            this.containerShelfData.Visibility = ViewStates.Visible;
            this.textCode.Visibility = ViewStates.Gone;
        }

        private void ShowCodeInternal()
        {
            this.containerShelfData.Visibility = ViewStates.Gone;
            this.textCode.Visibility = ViewStates.Visible;
        }
    }
}