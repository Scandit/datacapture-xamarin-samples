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

using System.Collections.Generic;
using System.Linq;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public static class ShortSetExtensions
    {
        public static ICollection<short> NewSetWithMinimum(this ICollection<short> set, short value)
        {
            var minimum = set.Min();
            if (value == minimum)
            {
                return set;
            }
            set.Add(value);

            if (value < minimum)
            {
                var rangeToAdd = Enumerable.Range(value, minimum - value).Select(i => (short)i);
                set.ToHashSet().UnionWith(rangeToAdd);
            }
            else
            {
                var rangeToSubtract = Enumerable.Range(minimum, value - minimum).Select(i => (short)i);
                set.ToHashSet().ExceptWith(rangeToSubtract);
            }

            return set;
        }

        public static ICollection<short> NewSetWithMaximum(this ICollection<short> set, short value)
        {
            var maximum = set.Max();
            if (value == maximum)
            {
                return set;
            }
            set.Add(value);

            if (value > maximum)
            {

                var rangeToAdd = Enumerable.Range(maximum, value - maximum).Select(i => (short)i);
                set.ToHashSet().UnionWith(rangeToAdd);
            }
            else
            {
                var rangeToSubtract = Enumerable.Range(value + 1, maximum - value).Select(i => (short)i);
                set.ToHashSet().ExceptWith(rangeToSubtract);
            }

            return set;
        }
    }
}
