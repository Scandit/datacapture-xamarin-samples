﻿/*
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
using System.Collections.Generic;
using Android.Graphics;

namespace IdCaptureExtendedSample.Result
{
    public class ResultUiState
    {
        public Bitmap FaceImage { get; set; }
        public Bitmap IdFrontImage { get; set; }
        public Bitmap IdBackImage { get; set; }
        public ArrayList Data { get; private set; }

        public ResultUiState(IList<ResultEntry> results)
        {
            this.Data = new ArrayList(results.Count);

            foreach(var result in results)
            {
                this.Data.Add(result);
            }
        }
    }
}
