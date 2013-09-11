using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Kata02
{
    [TestClass]
    public class Kata02
    {
        //Classic iterative method
        public int Chop1(int element, ICollection<int> collection)
        {
            //Empty collection -> not found
            if (collection.Count == 0) return -1;

            var lower = 0;
            var upper = collection.Count - 1;
            int index, candidate;

            while (lower != upper)
            {
                index = lower + ((upper - lower) / 2);
                candidate = collection.ElementAt(index);

                if (candidate == element)
                {
                    return index;
                }
                else if (candidate < element)
                {
                    lower = index + 1;
                }
                else //if (candidate > element)
                {
                    upper = index;
                }
            }

            return (collection.ElementAt(upper) == element) ? upper : -1;
        }

        //Recursively passing smaller slices of the initial array
        public int Chop2(int element, ICollection<int> collection)
        {
            return SubChop2(element, new ArraySlice<int>(collection.ToArray()));
        }
        //...and its sub method required for passing array slices around
        private int SubChop2(int element, ArraySlice<int> array)
        {
            //Empty collection -> not found
            if (array.Count == 0) return -1;

            var index = array.Count / 2;
            var candidate = array[index];

            if (candidate == element)
            {
                return index;
            }
            else if (candidate < element)
            {
                var offset = index + 1;
                var position = SubChop2(element, array.Skip(index + 1));
                return position > -1 ? offset + position : -1;
            }
            else //if (candidate > element)
            {
                return SubChop2(element, array.Take(index));
            }
        }

        //Classic recursive method, passing boundaries around
        public int Chop3(int element, ICollection<int> collection)
        {
            //Empty collection -> not found
            if (collection.Count == 0) return -1;

            return this.SubChop3(element, collection, 0, collection.Count - 1);
        }
        //...and its sub method required for passing boundaries around
        private int SubChop3(int element, IEnumerable<int> collection, int lower, int upper)
        {
            //End case: lower == upper
            if (lower == upper)
            {
                if (collection.ElementAt(lower) == element) return lower;
                else return -1;
            }

            var index = lower + ((upper - lower) / 2);
            var candidate = collection.ElementAt(index);

            if (candidate == element)
            {
                return index;
            }
            else if (candidate < element)
            {
                return SubChop3(element, collection, index + 1, upper);
            }
            else //if (candidate > element)
            {
                return SubChop3(element, collection, lower, index);
            }
        }

        //Continuation Passing Style
        public int Chop4(int element, ICollection<int> collection)
        {
            //Empty collection -> not found
            if (collection.Count() == 0) return -1;

            int index = -1;

            SubChop4(element, collection, 0, collection.Count() - 1, i => index = i);

            return index;
        }
        //...and it's sub method
        private void SubChop4(int element, IEnumerable<int> collection, int lower, int upper, Action<int> continuation)
        {
            //End case: lower == upper
            if (lower == upper)
            {
                if (collection.ElementAt(lower) == element) continuation(lower);
                else continuation(-1);
            }
            else
            {
                var index = lower + ((upper - lower) / 2);
                var candidate = collection.ElementAt(index);

                if (candidate == element)
                {
                    continuation(index);
                }
                else if (candidate < element)
                {
                    SubChop4(element, collection, index + 1, upper, continuation);
                }
                else //if (candidate > element)
                {
                    SubChop4(element, collection, lower, index, continuation);
                }
            }
        }

        [TestMethod]
        public void TestChop()
        {
            Func<int, int[], int> chop = Chop2;

            Assert.AreEqual(-1, chop(3, new int[] { }));
            Assert.AreEqual(-1, chop(3, new int[] { 1 }));
            Assert.AreEqual(0, chop(1, new int[] { 1 }));

            Assert.AreEqual(0, chop(1, new int[] { 1, 3, 5 }));
            Assert.AreEqual(1, chop(3, new int[] { 1, 3, 5 }));
            Assert.AreEqual(2, chop(5, new int[] { 1, 3, 5 }));
            Assert.AreEqual(-1, chop(0, new int[] { 1, 3, 5 }));
            Assert.AreEqual(-1, chop(2, new int[] { 1, 3, 5 }));
            Assert.AreEqual(-1, chop(4, new int[] { 1, 3, 5 }));
            Assert.AreEqual(-1, chop(6, new int[] { 1, 3, 5 }));

            Assert.AreEqual(0, chop(1, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(1, chop(3, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(2, chop(5, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(3, chop(7, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, chop(0, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, chop(2, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, chop(4, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, chop(6, new int[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, chop(8, new int[] { 1, 3, 5, 7 }));

            Assert.AreEqual(-1, chop(2, new int[] { 1, 3, 5, 7, 8, 9, 10 }));
            Assert.AreEqual(2, chop(5, new int[] { 1, 3, 5, 7, 8, 9, 10 }));
            Assert.AreEqual(5, chop(9, new int[] { 1, 3, 5, 7, 8, 9, 10 }));
            Assert.AreEqual(0, chop(1, new int[] { 1, 3, 5, 7, 8, 9, 10 }));
            Assert.AreEqual(6, chop(10, new int[] { 1, 3, 5, 7, 8, 9, 10 }));
        }

        class ArraySlice<T>
        {
            T[] Array { get; set; }
            int Offset { get; set; }
            public int Count { get; private set; }

            public ArraySlice(T[] array) : this(array, 0, array.Length) { }

            ArraySlice(T[] array, int offset, int count)
            {
                this.Array = array;
                this.Offset = offset;
                this.Count = count;
            }

            public T this[int i]
            {
                get
                {
                    return this.Array[this.Offset + i];
                }
            }

            public ArraySlice<T> Slice(int offset, int count)
            {
                return new ArraySlice<T>(this.Array, offset, count);
            }

            public ArraySlice<T> Take(int count)
            {
                return this.Slice(this.Offset, count);
            }

            public ArraySlice<T> Skip(int offset)
            {
                return this.Slice(this.Offset + offset, this.Count - offset);
            }

            public T[] ToArray()
            {
                T[] array = new T[this.Count];
                System.Array.Copy(this.Array, this.Offset, array, 0, this.Count);
                return array;
            }
        }
    }
}
