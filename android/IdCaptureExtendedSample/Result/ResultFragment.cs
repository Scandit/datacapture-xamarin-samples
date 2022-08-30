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

using System.Collections;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result
{
    public class ResultFragment : Fragment
    {
        private ResultViewModel viewModel;

        private ImageView faceImage;
        private ImageView idFrontImage;
        private ImageView idBackImage;
        private ResultListAdapter resultAdapter;

        public static ResultFragment Create(CapturedId capturedId)
        {
            return new ResultFragment()
            {
                viewModel = new ResultViewModel(capturedId)
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View root = inflater.Inflate(Resource.Layout.result_screen, container, false);
            this.InitToolbar(root);

            this.faceImage = root.FindViewById<ImageView>(Resource.Id.image_face);
            this.idFrontImage = root.FindViewById<ImageView>(Resource.Id.image_id_front);
            this.idBackImage = root.FindViewById<ImageView>(Resource.Id.image_id_back);

            this.resultAdapter = new ResultListAdapter(new ArrayList());

            RecyclerView recyclerResult = root.FindViewById<RecyclerView>(Resource.Id.results_list);
            recyclerResult.AddItemDecoration(new DividerItemDecoration(this.Context, DividerItemDecoration.Vertical));
            recyclerResult.SetAdapter(this.resultAdapter);

            UpdateImage(this.viewModel.UiState.FaceImage, this.faceImage);
            UpdateImage(this.viewModel.UiState.IdFrontImage, this.idFrontImage);
            UpdateImage(this.viewModel.UiState.IdBackImage, this.idBackImage);

            this.resultAdapter.SubmitList(this.viewModel.UiState.Data);
            return root;
        }

        private void InitToolbar(View root)
        {
            AndroidX.AppCompat.Widget.Toolbar toolbar = root.FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            AppCompatActivity activity = (AppCompatActivity)this.RequireActivity();
            activity.SetSupportActionBar(toolbar);
            activity.SupportActionBar.SetTitle(Resource.String.app_name);
            activity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private static void UpdateImage(Bitmap image, ImageView target)
        {
            if (image != null)
            {
                target.Visibility = ViewStates.Visible;
                target.SetImageBitmap(image);
            }
            else
            {
                target.Visibility = ViewStates.Gone;
            }
        }
    }
}
