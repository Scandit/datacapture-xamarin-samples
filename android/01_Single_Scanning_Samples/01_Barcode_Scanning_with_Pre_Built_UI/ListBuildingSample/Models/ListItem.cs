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
using Android.Graphics;
using Scandit.DataCapture.Barcode.Data;

namespace ListBuildingSample.Models
{
    /// <summary>
    /// This class represents scanned barcode, with all information needed by ListItemViewHolder.
    /// </summary>
    public class ListItem: IEquatable<ListItem>
    {
        private readonly Symbology symbology;

        /// <summary>
        /// The image of the scanned barcode.
        /// </summary>
        public Bitmap Image { get; }

        /// <summary>
        /// Sequence number for scanned barcode.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The symbology of scanned barcode.
        /// </summary>
        public string Symbology => SymbologyDescription.Create(this.symbology).ReadableName;

        /// <summary>
        /// Data contained in scanned barcode.
        /// </summary>
        public string Data;

        public ListItem(Bitmap image, int number, Symbology symbology, string data)
        {
            this.Image = image;
            this.Number = number;
            this.symbology = symbology;
            this.Data = data;
        }

        public bool Equals(ListItem other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as ListItem) != null && this.Equals((ListItem)obj);
        }

        public override int GetHashCode()
        {
            return this.Number.GetHashCode() ^ this.Symbology.GetHashCode();
        }
    }
}
