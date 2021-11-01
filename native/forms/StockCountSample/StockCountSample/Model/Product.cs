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

namespace StockCountSample.Model
{
    public class Product : IEquatable<Product>
    {
        public static Product Create(string code, string symbology) =>
            new Product(code, symbology);

        public string Code { get; }

        public string Symbology { get; }

        private Product(string code, string symbology)
        {
            this.Code = code;
            this.Symbology = symbology;
        }

        public bool Equals(Product other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as Product) != null && this.Equals((Product)obj);
        }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode() ^ this.Symbology.GetHashCode();
        }
    }
}
