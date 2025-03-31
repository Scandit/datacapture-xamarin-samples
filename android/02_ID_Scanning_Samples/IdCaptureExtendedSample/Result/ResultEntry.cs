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

using Android.OS;
using Android.Runtime;
using Java.Lang;

namespace IdCaptureExtendedSample.Result
{
    public class ResultEntry : Object, IParcelable, IParcelableCreator
    {
        public string Title { get; private set; }
        public string Value { get; private set; }

        public ResultEntry(string value, string title)
        {
            this.Title = title;
            this.Value = value;
        }

        private ResultEntry(Parcel parcel)
        {
            this.Title = parcel.ReadString();
            this.Value = parcel.ReadString();
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(this.Title);
            dest.WriteString(this.Value);
        }

        public Object CreateFromParcel(Parcel source)
        {
            return new ResultEntry(source);
        }

        public Object[] NewArray(int size)
        {
            return new ResultEntry[size];
        }
    }
}
