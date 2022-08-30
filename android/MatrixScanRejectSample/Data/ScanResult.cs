//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using Scandit.DataCapture.Barcode.Data;

namespace MatrixScanRejectSample.Data
{
    public class ScanResult : Java.Lang.Object, IParcelable
    {
        public ScanResult(Barcode barcode)
        {
            using var symbologyDescription = SymbologyDescription.Create(barcode.Symbology);
            ReadableName = symbologyDescription.ReadableName;
            Data = barcode.Data;
        }

        public ScanResult(Parcel parcel)
        {
            ReadableName = parcel.ReadString();
            Data = parcel.ReadString();
        }

        public String ReadableName { get; private set; }

        public String Data { get;  private set; }

        public int DescribeContents()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            return obj is ScanResult result &&
                    ReadableName == ((ScanResult)obj).ReadableName &&
                   Data == result.Data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReadableName, Data);
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(ReadableName);
            dest.WriteString(Data);
        }

        private static readonly ParcelableCreator creator
            = new ParcelableCreator((parcel) => new ScanResult(parcel));

        [ExportField("CREATOR")]
        public static ParcelableCreator InitializeCreator()
        {
            return creator;
        }

        public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            private readonly Func<Parcel, ScanResult> _createFunc;

            public ParcelableCreator(Func<Parcel, ScanResult> createFromParcelFunc)
            {
                _createFunc = createFromParcelFunc;
            }

            public Java.Lang.Object CreateFromParcel(Parcel parcel)
            {
                return _createFunc(parcel);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new ScanResult[size];
            }
        }
    }
}
