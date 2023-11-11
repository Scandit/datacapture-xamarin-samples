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
using System.IO;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using AndroidX.Lifecycle;
using IdCaptureExtendedSample.Result.Presenters;
using Scandit.DataCapture.ID.Data;
using Object = Java.Lang.Object;

namespace IdCaptureExtendedSample.Result
{
    public class ResultViewModel : ViewModel, IParcelable, IParcelableCreator
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

        private ResultViewModel(Parcel parcel)
        {
            var entries = parcel.ReadArrayList(this.Class.ClassLoader) as IList<ResultEntry>;

            int idFaceImageBytesSize = parcel.ReadInt();
            byte[] idFaceImageBytes = new byte[idFaceImageBytesSize];
            parcel.ReadByteArray(idFaceImageBytes);

            int idFrontImageBytesSize = parcel.ReadInt();
            byte[] idFrontImageBytes = new byte[idFrontImageBytesSize];
            parcel.ReadByteArray(idFrontImageBytes);

            int idBackImageBytesSize = parcel.ReadInt();
            byte[] idBackImageBytes = new byte[idBackImageBytesSize];
            parcel.ReadByteArray(idBackImageBytes);

            this.UiState = new ResultUiState(entries)
            {
                FaceImage = ConvertBytesToImage(idFaceImageBytes),
                IdFrontImage = ConvertBytesToImage(idFrontImageBytes),
                IdBackImage = ConvertBytesToImage(idBackImageBytes)
            };
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteList(this.UiState.Data);

            byte[] idFaceImageBytes = this.GetFaceImageBytes();
            byte[] idFrontImageBytes = this.GetIdBackImageBytes();
            byte[] idBackImageBytes = this.GetIdBackImageBytes();

            if (idFaceImageBytes != null)
            {
                dest.WriteInt(idFaceImageBytes.Length);
                dest.WriteByteArray(idFaceImageBytes);
            }
            if (idFrontImageBytes != null)
            {
                dest.WriteInt(idFrontImageBytes.Length);
                dest.WriteByteArray(idFrontImageBytes);
            }
            if (idBackImageBytes != null)
            {
                dest.WriteInt(idBackImageBytes.Length);
                dest.WriteByteArray(idBackImageBytes);
            }
        }

        public Object CreateFromParcel(Parcel source)
        {
            return new ResultViewModel(source);
        }

        public Object[] NewArray(int size)
        {
            return new ResultViewModel[size];
        }

        public byte[] GetFaceImageBytes()
        {
            return ConvertImageToBytes(this.UiState.FaceImage);
        }

        public byte[] GetIdFrontImageBytes()
        {
            return ConvertImageToBytes(this.UiState.IdFrontImage);
        }

        public byte[] GetIdBackImageBytes()
        {
            return ConvertImageToBytes(this.UiState.IdBackImage);
        }

        private static byte[] ConvertImageToBytes(Bitmap image)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Png, 100, memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                memoryStream.Flush();
                return byteArray;
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        private static Bitmap ConvertBytesToImage(byte[] bytes)
        {
            if (bytes != null)
            {
                return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            }
            else
            {
                return null;
            }
        }
    }
}
