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

#nullable enable

using System;
using System.Reactive.Linq;
using System.Threading;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using LabelCaptureSimpleSample.ViewModels;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Control;
using Scandit.DataCapture.Label.UI.Overlay.Validation;

namespace LabelCaptureSimpleSample.Views.Fragments
{
    public class ScanFragment : Fragment
    {
        private readonly ScanViewModel viewModel;

        private IDisposable subscription = null!;
        private DataCaptureView dataCaptureView = null!;
        private LabelCaptureValidationFlowOverlay validationFlowOverlay = null!;

        private ScanFragment(ScanViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public static ScanFragment Create(ScanViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var scanFragment = new ScanFragment(viewModel);
            return scanFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? layout = inflater.Inflate(Resource.Layout.fragment_scan, container, false);

            if (layout == null)
            {
                throw new ArgumentNullException(
                    nameof(layout), $"Cannot inflate the layout {nameof(Resource.Layout.fragment_scan)}");
            }

            this.AttachDataCaptureView(layout);
            return layout;
        }

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.subscription = this.viewModel.Messages
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(message =>
                {
                    if (this.IsResumed)
                    {
                        this.ShowLabelCapturedDialog(message);
                    }
                });
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            this.subscription.Dispose();
        }

        public override void OnPause()
        {
            base.OnPause();
            this.viewModel.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            this.viewModel.OnResume();
        }

        private void AttachDataCaptureView(View root)
        {
            ViewGroup? dataCaptureViewContainer = root.FindViewById<ViewGroup>(Resource.Id.data_capture_view_container)
                ?? throw new ArgumentNullException(nameof(Resource.Id.data_capture_view_container));

            var labelCaptureOverlay = this.viewModel.BuildOverlay(this.RequireContext());
            this.validationFlowOverlay = this.viewModel.GetValidationFlowOverlay(this.RequireContext());
            this.dataCaptureView = DataCaptureView.Create(this.RequireContext(), this.viewModel.DataCaptureContext);
            this.dataCaptureView.AddControl(new TorchSwitchControl(this.RequireContext()));
            this.dataCaptureView.AddOverlay(labelCaptureOverlay);
            this.dataCaptureView.AddOverlay(this.validationFlowOverlay);

            dataCaptureViewContainer.AddView(
                this.dataCaptureView,
                new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        private void ShowLabelCapturedDialog(string message)
        {
            if (this.ParentFragmentManager.FindFragmentByTag(LabelCapturedFragment.TAG) == null)
            {
                var dialog = LabelCapturedFragment.Create(message);
                dialog.OnDismissed += (sender, args) => this.OnDialogDismissed();
                dialog.Show(this.ParentFragmentManager, LabelCapturedFragment.TAG);
            }
        }

        private void OnDialogDismissed()
        {
            this.viewModel.OnDialogDismissed();
        }
    }
}
