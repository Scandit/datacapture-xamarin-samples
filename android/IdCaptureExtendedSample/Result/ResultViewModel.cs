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

using AndroidX.Lifecycle;
using IdCaptureExtendedSample.Result.Presenters;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result
{
    public class ResultViewModel : ViewModel
    {
        private readonly ResultPresenterFactory factory = new ResultPresenterFactory();

        public ResultUiState UiState { get; private set; }

        public ResultViewModel(CapturedId capturedId)
        {
            IResultPresenter resultPresenter = this.factory.Create(capturedId);

            this.UiState = new ResultUiState(resultPresenter.Rows)
            {
                FaceImage = capturedId.GetImageBitmapForType(IdImageType.Face),
                IdFrontImage = capturedId.GetImageBitmapForType(IdImageType.IdFront),
                IdBackImage = capturedId.GetImageBitmapForType(IdImageType.IdBack)
            };
        }
    }
}
