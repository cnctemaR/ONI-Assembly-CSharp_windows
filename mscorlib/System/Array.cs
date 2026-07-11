using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public abstract class Array : IEnumerable, ICloneable, ICollection, IList
	{
		private Array()
		{
		}

		object IList.this[int index]
		{
			get
			{
				if (index >= this.Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				if (this.Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Only single dimension arrays are supported."));
				}
				return this.GetValueImpl(index);
			}
			set
			{
				if (index >= this.Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				if (this.Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Only single dimension arrays are supported."));
				}
				this.SetValueImpl(value, index);
			}
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			Array.Clear(this, this.GetLowerBound(0), this.Length);
		}

		bool IList.Contains(object value)
		{
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				if (object.Equals(this.GetValueImpl(i), value))
				{
					return true;
				}
			}
			return false;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		int IList.IndexOf(object value)
		{
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				if (object.Equals(this.GetValueImpl(i), value))
				{
					return i + this.GetLowerBound(0);
				}
			}
			return this.GetLowerBound(0) - 1;
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		int ICollection.Count
		{
			get
			{
				return this.Length;
			}
		}

		internal int InternalArray__ICollection_get_Count()
		{
			return this.Length;
		}

		internal bool InternalArray__ICollection_get_IsReadOnly()
		{
			return true;
		}

		internal IEnumerator<T> InternalArray__IEnumerable_GetEnumerator<T>()
		{
			return new Array.InternalEnumerator<T>(this);
		}

		internal void InternalArray__ICollection_Clear()
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal void InternalArray__ICollection_Add<T>(T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal bool InternalArray__ICollection_Remove<T>(T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal bool InternalArray__ICollection_Contains<T>(T item)
		{
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				T t;
				this.GetGenericValueImpl<T>(i, out t);
				if (item == null)
				{
					return t == null;
				}
				if (item.Equals(t))
				{
					return true;
				}
			}
			return false;
		}

		internal void InternalArray__ICollection_CopyTo<T>(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index + this.GetLength(0) > array.GetLowerBound(0) + array.GetLength(0))
			{
				throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value has to be >= 0."));
			}
			Array.Copy(this, this.GetLowerBound(0), array, index, this.GetLength(0));
		}

		internal void InternalArray__Insert<T>(int index, T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal void InternalArray__RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal int InternalArray__IndexOf<T>(T item)
		{
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = this.Length;
			int i = 0;
			while (i < length)
			{
				T t;
				this.GetGenericValueImpl<T>(i, out t);
				if (item == null)
				{
					if (t == null)
					{
						return i + this.GetLowerBound(0);
					}
					return this.GetLowerBound(0) - 1;
				}
				else
				{
					if (t.Equals(item))
					{
						return i + this.GetLowerBound(0);
					}
					i++;
				}
			}
			return this.GetLowerBound(0) - 1;
		}

		internal T InternalArray__get_Item<T>(int index)
		{
			if (index >= this.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			T t;
			this.GetGenericValueImpl<T>(index, out t);
			return t;
		}

		internal void InternalArray__set_Item<T>(int index, T item)
		{
			if (index >= this.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			object[] array = this as object[];
			if (array != null)
			{
				array[index] = item;
				return;
			}
			this.SetGenericValueImpl<T>(index, ref item);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetGenericValueImpl<T>(int pos, out T value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetGenericValueImpl<T>(int pos, ref T value);

		public int Length
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				int num = this.GetLength(0);
				for (int i = 1; i < this.Rank; i++)
				{
					num *= this.GetLength(i);
				}
				return num;
			}
		}

		[ComVisible(false)]
		public long LongLength
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return (long)this.Length;
			}
		}

		public int Rank
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.GetRank();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetRank();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLength(int dimension);

		[ComVisible(false)]
		public long GetLongLength(int dimension)
		{
			return (long)this.GetLength(dimension);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLowerBound(int dimension);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetValue(params int[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValue(object value, params int[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object GetValueImpl(int pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetValueImpl(object value, int pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool FastCopy(Array source, int source_idx, Array dest, int dest_idx, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Array CreateInstanceImpl(Type elementType, int[] lengths, int[] bounds);

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new Array.SimpleEnumerator(this);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int GetUpperBound(int dimension)
		{
			return this.GetLowerBound(dimension) + this.GetLength(dimension) - 1;
		}

		public object GetValue(int index)
		{
			if (this.Rank != 1)
			{
				throw new ArgumentException(Locale.GetText("Array was not a one-dimensional array."));
			}
			if (index < this.GetLowerBound(0) || index > this.GetUpperBound(0))
			{
				throw new IndexOutOfRangeException(Locale.GetText("Index has to be between upper and lower bound of the array."));
			}
			return this.GetValueImpl(index - this.GetLowerBound(0));
		}

		public object GetValue(int index1, int index2)
		{
			int[] array = new int[] { index1, index2 };
			return this.GetValue(array);
		}

		public object GetValue(int index1, int index2, int index3)
		{
			int[] array = new int[] { index1, index2, index3 };
			return this.GetValue(array);
		}

		[ComVisible(false)]
		public object GetValue(long index)
		{
			if (index < 0L || index > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return this.GetValue((int)index);
		}

		[ComVisible(false)]
		public object GetValue(long index1, long index2)
		{
			if (index1 < 0L || index1 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0L || index2 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return this.GetValue((int)index1, (int)index2);
		}

		[ComVisible(false)]
		public object GetValue(long index1, long index2, long index3)
		{
			if (index1 < 0L || index1 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0L || index2 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index3 < 0L || index3 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index3", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return this.GetValue((int)index1, (int)index2, (int)index3);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index)
		{
			if (index < 0L || index > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			this.SetValue(value, (int)index);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index1, long index2)
		{
			if (index1 < 0L || index1 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0L || index2 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			int[] array = new int[]
			{
				(int)index1,
				(int)index2
			};
			this.SetValue(value, array);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index1, long index2, long index3)
		{
			if (index1 < 0L || index1 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0L || index2 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index3 < 0L || index3 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index3", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			int[] array = new int[]
			{
				(int)index1,
				(int)index2,
				(int)index3
			};
			this.SetValue(value, array);
		}

		public void SetValue(object value, int index)
		{
			if (this.Rank != 1)
			{
				throw new ArgumentException(Locale.GetText("Array was not a one-dimensional array."));
			}
			if (index < this.GetLowerBound(0) || index > this.GetUpperBound(0))
			{
				throw new IndexOutOfRangeException(Locale.GetText("Index has to be >= lower bound and <= upper bound of the array."));
			}
			this.SetValueImpl(value, index - this.GetLowerBound(0));
		}

		public void SetValue(object value, int index1, int index2)
		{
			int[] array = new int[] { index1, index2 };
			this.SetValue(value, array);
		}

		public void SetValue(object value, int index1, int index2, int index3)
		{
			int[] array = new int[] { index1, index2, index3 };
			this.SetValue(value, array);
		}

		public static Array CreateInstance(Type elementType, int length)
		{
			int[] array = new int[] { length };
			return Array.CreateInstance(elementType, array);
		}

		public static Array CreateInstance(Type elementType, int length1, int length2)
		{
			int[] array = new int[] { length1, length2 };
			return Array.CreateInstance(elementType, array);
		}

		public static Array CreateInstance(Type elementType, int length1, int length2, int length3)
		{
			int[] array = new int[] { length1, length2, length3 };
			return Array.CreateInstance(elementType, array);
		}

		public static Array CreateInstance(Type elementType, params int[] lengths)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			if (lengths.Length > 255)
			{
				throw new TypeLoadException();
			}
			int[] array = null;
			elementType = elementType.UnderlyingSystemType;
			if (!elementType.IsSystemType)
			{
				throw new ArgumentException("Type must be a type provided by the runtime.", "elementType");
			}
			if (elementType.Equals(typeof(void)))
			{
				throw new NotSupportedException("Array type can not be void");
			}
			if (elementType.ContainsGenericParameters)
			{
				throw new NotSupportedException("Array type can not be an open generic type");
			}
			return Array.CreateInstanceImpl(elementType, lengths, array);
		}

		public static Array CreateInstance(Type elementType, int[] lengths, int[] lowerBounds)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			if (lowerBounds == null)
			{
				throw new ArgumentNullException("lowerBounds");
			}
			elementType = elementType.UnderlyingSystemType;
			if (!elementType.IsSystemType)
			{
				throw new ArgumentException("Type must be a type provided by the runtime.", "elementType");
			}
			if (elementType.Equals(typeof(void)))
			{
				throw new NotSupportedException("Array type can not be void");
			}
			if (elementType.ContainsGenericParameters)
			{
				throw new NotSupportedException("Array type can not be an open generic type");
			}
			if (lengths.Length < 1)
			{
				throw new ArgumentException(Locale.GetText("Arrays must contain >= 1 elements."));
			}
			if (lengths.Length != lowerBounds.Length)
			{
				throw new ArgumentException(Locale.GetText("Arrays must be of same size."));
			}
			for (int i = 0; i < lowerBounds.Length; i++)
			{
				if (lengths[i] < 0)
				{
					throw new ArgumentOutOfRangeException("lengths", Locale.GetText("Each value has to be >= 0."));
				}
				if ((long)lowerBounds[i] + (long)lengths[i] > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("lengths", Locale.GetText("Length + bound must not exceed Int32.MaxValue."));
				}
			}
			if (lengths.Length > 255)
			{
				throw new TypeLoadException();
			}
			return Array.CreateInstanceImpl(elementType, lengths, lowerBounds);
		}

		private static int[] GetIntArray(long[] values)
		{
			int num = values.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				long num2 = values[i];
				if (num2 < 0L || num2 > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("values", Locale.GetText("Each value has to be >= 0 and <= Int32.MaxValue."));
				}
				array[i] = (int)num2;
			}
			return array;
		}

		public static Array CreateInstance(Type elementType, params long[] lengths)
		{
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			return Array.CreateInstance(elementType, Array.GetIntArray(lengths));
		}

		[ComVisible(false)]
		public object GetValue(params long[] indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			return this.GetValue(Array.GetIntArray(indices));
		}

		[ComVisible(false)]
		public void SetValue(object value, params long[] indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetValue(value, Array.GetIntArray(indices));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (value == null)
			{
				return -1;
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (!(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("value does not support IComparable."));
			}
			return Array.DoBinarySearch(array, array.GetLowerBound(0), array.GetLength(0), value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, object value, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (comparer == null && value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("comparer is null and value does not support IComparable."));
			}
			return Array.DoBinarySearch(array, array.GetLowerBound(0), array.GetLength(0), value, comparer);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, int index, int length, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.GetLowerBound(0) + array.GetLength(0) - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("value does not support IComparable"));
			}
			return Array.DoBinarySearch(array, index, length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, int index, int length, object value, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.GetLowerBound(0) + array.GetLength(0) - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (comparer == null && value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("comparer is null and value does not support IComparable."));
			}
			return Array.DoBinarySearch(array, index, length, value, comparer);
		}

		private static int DoBinarySearch(Array array, int index, int length, object value, IComparer comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer.Default;
			}
			int i = index;
			int num = index + length - 1;
			try
			{
				while (i <= num)
				{
					int num2 = i + (num - i) / 2;
					object valueImpl = array.GetValueImpl(num2);
					int num3 = comparer.Compare(valueImpl, value);
					if (num3 == 0)
					{
						return num2;
					}
					if (num3 > 0)
					{
						num = num2 - 1;
					}
					else
					{
						i = num2 + 1;
					}
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(Locale.GetText("Comparer threw an exception."), ex);
			}
			return ~i;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void Clear(Array array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (length < 0)
			{
				throw new IndexOutOfRangeException("length < 0");
			}
			int lowerBound = array.GetLowerBound(0);
			if (index < lowerBound)
			{
				throw new IndexOutOfRangeException("index < lower bound");
			}
			index -= lowerBound;
			if (index > array.Length - length)
			{
				throw new IndexOutOfRangeException("index + length > size");
			}
			Array.ClearInternal(array, index, length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearInternal(Array a, int index, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object Clone();

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, Array destinationArray, int length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			Array.Copy(sourceArray, sourceArray.GetLowerBound(0), destinationArray, destinationArray.GetLowerBound(0), length);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (sourceIndex < 0)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", Locale.GetText("Value has to be >= 0."));
			}
			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", Locale.GetText("Value has to be >= 0."));
			}
			if (Array.FastCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length))
			{
				return;
			}
			int num = sourceIndex - sourceArray.GetLowerBound(0);
			int num2 = destinationIndex - destinationArray.GetLowerBound(0);
			if (num > sourceArray.Length - length)
			{
				throw new ArgumentException("length");
			}
			if (num2 > destinationArray.Length - length)
			{
				string text = "Destination array was not long enough. Check destIndex and length, and the array's lower bounds";
				throw new ArgumentException(text, string.Empty);
			}
			if (sourceArray.Rank != destinationArray.Rank)
			{
				throw new RankException(Locale.GetText("Arrays must be of same size."));
			}
			Type elementType = sourceArray.GetType().GetElementType();
			Type elementType2 = destinationArray.GetType().GetElementType();
			if (!object.ReferenceEquals(sourceArray, destinationArray) || num > num2)
			{
				for (int i = 0; i < length; i++)
				{
					object valueImpl = sourceArray.GetValueImpl(num + i);
					try
					{
						destinationArray.SetValueImpl(valueImpl, num2 + i);
					}
					catch
					{
						if (elementType.Equals(typeof(object)))
						{
							throw new InvalidCastException();
						}
						throw new ArrayTypeMismatchException(string.Format(Locale.GetText("(Types: source={0};  target={1})"), elementType.FullName, elementType2.FullName));
					}
				}
			}
			else
			{
				for (int j = length - 1; j >= 0; j--)
				{
					object valueImpl2 = sourceArray.GetValueImpl(num + j);
					try
					{
						destinationArray.SetValueImpl(valueImpl2, num2 + j);
					}
					catch
					{
						if (elementType.Equals(typeof(object)))
						{
							throw new InvalidCastException();
						}
						throw new ArrayTypeMismatchException(string.Format(Locale.GetText("(Types: source={0};  target={1})"), elementType.FullName, elementType2.FullName));
					}
				}
			}
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			if (sourceIndex < -2147483648L || sourceIndex > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", Locale.GetText("Must be in the Int32 range."));
			}
			if (destinationIndex < -2147483648L || destinationIndex > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", Locale.GetText("Must be in the Int32 range."));
			}
			if (length < 0L || length > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			Array.Copy(sourceArray, (int)sourceIndex, destinationArray, (int)destinationIndex, (int)length);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, Array destinationArray, long length)
		{
			if (length < 0L || length > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			Array.Copy(sourceArray, destinationArray, (int)length);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.IndexOf(array, value, 0, array.Length);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.IndexOf(array, value, startIndex, array.Length - startIndex);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex - 1 > array.GetUpperBound(0) - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (object.Equals(array.GetValueImpl(i), value))
				{
					return i;
				}
			}
			return array.GetLowerBound(0) - 1;
		}

		public void Initialize()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length == 0)
			{
				return array.GetLowerBound(0) - 1;
			}
			return Array.LastIndexOf(array, value, array.Length - 1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.LastIndexOf(array, value, startIndex, startIndex - array.GetLowerBound(0) + 1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int lowerBound = array.GetLowerBound(0);
			if (array.Length == 0)
			{
				return lowerBound - 1;
			}
			if (count < 0 || startIndex < lowerBound || startIndex > array.GetUpperBound(0) || startIndex - count + 1 < lowerBound)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = startIndex; i >= startIndex - count + 1; i--)
			{
				if (object.Equals(array.GetValueImpl(i), value))
				{
					return i;
				}
			}
			return lowerBound - 1;
		}

		private static Array.Swapper get_swapper(Array array)
		{
			if (array is int[])
			{
				return new Array.Swapper(array.int_swapper);
			}
			if (array is double[])
			{
				return new Array.Swapper(array.double_swapper);
			}
			if (array is object[])
			{
				return new Array.Swapper(array.obj_swapper);
			}
			return new Array.Swapper(array.slow_swapper);
		}

		private static Array.Swapper get_swapper<T>(T[] array)
		{
			if (array is int[])
			{
				return new Array.Swapper(array.int_swapper);
			}
			if (array is double[])
			{
				return new Array.Swapper(array.double_swapper);
			}
			return new Array.Swapper(array.slow_swapper);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Reverse(Array array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Reverse(array, array.GetLowerBound(0), array.GetLength(0));
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Reverse(Array array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0) || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (index > array.GetUpperBound(0) + 1 - length)
			{
				throw new ArgumentException();
			}
			int num = index + length - 1;
			object[] array2 = array as object[];
			if (array2 != null)
			{
				while (index < num)
				{
					object obj = array2[index];
					array2[index] = array2[num];
					array2[num] = obj;
					index++;
					num--;
				}
				return;
			}
			int[] array3 = array as int[];
			if (array3 != null)
			{
				while (index < num)
				{
					int num2 = array3[index];
					array3[index] = array3[num];
					array3[num] = num2;
					index++;
					num--;
				}
				return;
			}
			double[] array4 = array as double[];
			if (array4 != null)
			{
				while (index < num)
				{
					double num3 = array4[index];
					array4[index] = array4[num];
					array4[num] = num3;
					index++;
					num--;
				}
				return;
			}
			Array.Swapper swapper = Array.get_swapper(array);
			while (index < num)
			{
				swapper(index, num);
				index++;
				num--;
			}
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort(array, null, array.GetLowerBound(0), array.GetLength(0), null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Array.Sort(keys, items, keys.GetLowerBound(0), keys.GetLength(0), null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort(array, null, array.GetLowerBound(0), array.GetLength(0), comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, int index, int length)
		{
			Array.Sort(array, null, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, IComparer comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Array.Sort(keys, items, keys.GetLowerBound(0), keys.GetLength(0), comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, int index, int length)
		{
			Array.Sort(keys, items, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, int index, int length, IComparer comparer)
		{
			Array.Sort(array, null, index, length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, int index, int length, IComparer comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (keys.Rank > 1 || (items != null && items.Rank > 1))
			{
				throw new RankException();
			}
			if (items != null && keys.GetLowerBound(0) != items.GetLowerBound(0))
			{
				throw new ArgumentException();
			}
			if (index < keys.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (keys.Length - (index + keys.GetLowerBound(0)) < length || (items != null && index > items.Length - length))
			{
				throw new ArgumentException();
			}
			if (length <= 1)
			{
				return;
			}
			if (comparer == null)
			{
				Array.Swapper swapper;
				if (items == null)
				{
					swapper = null;
				}
				else
				{
					swapper = Array.get_swapper(items);
				}
				if (keys is double[])
				{
					Array.combsort(keys as double[], index, length, swapper);
					return;
				}
				if (!(keys is uint[]) && keys is int[])
				{
					Array.combsort(keys as int[], index, length, swapper);
					return;
				}
				if (keys is char[])
				{
					Array.combsort(keys as char[], index, length, swapper);
					return;
				}
			}
			try
			{
				int num = index + length - 1;
				Array.qsort(keys, items, index, num, comparer);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(Locale.GetText("The comparer threw an exception."), ex);
			}
		}

		private void int_swapper(int i, int j)
		{
			int[] array = this as int[];
			int num = array[i];
			array[i] = array[j];
			array[j] = num;
		}

		private void obj_swapper(int i, int j)
		{
			object[] array = this as object[];
			object obj = array[i];
			array[i] = array[j];
			array[j] = obj;
		}

		private void slow_swapper(int i, int j)
		{
			object valueImpl = this.GetValueImpl(i);
			this.SetValueImpl(this.GetValue(j), i);
			this.SetValueImpl(valueImpl, j);
		}

		private void double_swapper(int i, int j)
		{
			double[] array = this as double[];
			double num = array[i];
			array[i] = array[j];
			array[j] = num;
		}

		private static int new_gap(int gap)
		{
			gap = gap * 10 / 13;
			if (gap == 9 || gap == 10)
			{
				return 11;
			}
			if (gap < 1)
			{
				return 1;
			}
			return gap;
		}

		private static void combsort(double[] array, int start, int size, Array.Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = Array.new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						double num4 = array[i];
						array[i] = array[num3];
						array[num3] = num4;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void combsort(int[] array, int start, int size, Array.Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = Array.new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						int num4 = array[i];
						array[i] = array[num3];
						array[num3] = num4;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void combsort(char[] array, int start, int size, Array.Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = Array.new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						char c = array[i];
						array[i] = array[num3];
						array[num3] = c;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void qsort(Array keys, Array items, int low0, int high0, IComparer comparer)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int num3 = num + (num2 - num) / 2;
			object valueImpl = keys.GetValueImpl(num3);
			for (;;)
			{
				while (num < high0 && Array.compare(keys.GetValueImpl(num), valueImpl, comparer) < 0)
				{
					num++;
				}
				while (num2 > low0 && Array.compare(valueImpl, keys.GetValueImpl(num2), comparer) < 0)
				{
					num2--;
				}
				if (num > num2)
				{
					break;
				}
				Array.swap(keys, items, num, num2);
				num++;
				num2--;
			}
			if (low0 < num2)
			{
				Array.qsort(keys, items, low0, num2, comparer);
			}
			if (num < high0)
			{
				Array.qsort(keys, items, num, high0, comparer);
			}
		}

		private static void swap(Array keys, Array items, int i, int j)
		{
			object obj = keys.GetValueImpl(i);
			keys.SetValueImpl(keys.GetValue(j), i);
			keys.SetValueImpl(obj, j);
			if (items != null)
			{
				obj = items.GetValueImpl(i);
				items.SetValueImpl(items.GetValueImpl(j), i);
				items.SetValueImpl(obj, j);
			}
		}

		private static int compare(object value1, object value2, IComparer comparer)
		{
			if (value1 == null)
			{
				return (value2 != null) ? (-1) : 0;
			}
			if (value2 == null)
			{
				return 1;
			}
			if (comparer == null)
			{
				return ((IComparable)value1).CompareTo(value2);
			}
			return comparer.Compare(value1, value2);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort<T, T>(array, null, 0, array.Length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Array.Sort<TKey, TValue>(keys, items, 0, keys.Length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort<T, T>(array, null, 0, array.Length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, IComparer<TKey> comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Array.Sort<TKey, TValue>(keys, items, 0, keys.Length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort<T, T>(array, null, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length)
		{
			Array.Sort<TKey, TValue>(keys, items, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, int index, int length, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort<T, T>(array, null, index, length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length, IComparer<TKey> comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (keys.Length - index < length || (items != null && index > items.Length - length))
			{
				throw new ArgumentException();
			}
			if (length <= 1)
			{
				return;
			}
			if (comparer == null)
			{
				Array.Swapper swapper;
				if (items == null)
				{
					swapper = null;
				}
				else
				{
					swapper = Array.get_swapper<TValue>(items);
				}
				if (keys is double[])
				{
					Array.combsort(keys as double[], index, length, swapper);
					return;
				}
				if (!(keys is uint[]) && keys is int[])
				{
					Array.combsort(keys as int[], index, length, swapper);
					return;
				}
				if (keys is char[])
				{
					Array.combsort(keys as char[], index, length, swapper);
					return;
				}
			}
			try
			{
				int num = index + length - 1;
				Array.qsort<TKey, TValue>(keys, items, index, num, comparer);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(Locale.GetText("The comparer threw an exception."), ex);
			}
		}

		public static void Sort<T>(T[] array, Comparison<T> comparison)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Array.Sort<T>(array, array.Length, comparison);
		}

		internal static void Sort<T>(T[] array, int length, Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			if (length <= 1 || array.Length <= 1)
			{
				return;
			}
			try
			{
				int num = 0;
				int num2 = length - 1;
				Array.qsort<T>(array, num, num2, comparison);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(Locale.GetText("Comparison threw an exception."), ex);
			}
		}

		private static void qsort<K, V>(K[] keys, V[] items, int low0, int high0, IComparer<K> comparer)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int num3 = num + (num2 - num) / 2;
			K k = keys[num3];
			for (;;)
			{
				while (num < high0 && Array.compare<K>(keys[num], k, comparer) < 0)
				{
					num++;
				}
				while (num2 > low0 && Array.compare<K>(k, keys[num2], comparer) < 0)
				{
					num2--;
				}
				if (num > num2)
				{
					break;
				}
				Array.swap<K, V>(keys, items, num, num2);
				num++;
				num2--;
			}
			if (low0 < num2)
			{
				Array.qsort<K, V>(keys, items, low0, num2, comparer);
			}
			if (num < high0)
			{
				Array.qsort<K, V>(keys, items, num, high0, comparer);
			}
		}

		private static int compare<T>(T value1, T value2, IComparer<T> comparer)
		{
			if (comparer != null)
			{
				return comparer.Compare(value1, value2);
			}
			if (value1 == null)
			{
				return (value2 != null) ? (-1) : 0;
			}
			if (value2 == null)
			{
				return 1;
			}
			if (value1 is IComparable<T>)
			{
				return ((IComparable<T>)((object)value1)).CompareTo(value2);
			}
			if (value1 is IComparable)
			{
				return ((IComparable)((object)value1)).CompareTo(value2);
			}
			string text = Locale.GetText("No IComparable or IComparable<{0}> interface found.");
			throw new InvalidOperationException(string.Format(text, typeof(T)));
		}

		private static void qsort<T>(T[] array, int low0, int high0, Comparison<T> comparison)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int num3 = num + (num2 - num) / 2;
			T t = array[num3];
			for (;;)
			{
				while (num < high0 && comparison(array[num], t) < 0)
				{
					num++;
				}
				while (num2 > low0 && comparison(t, array[num2]) < 0)
				{
					num2--;
				}
				if (num > num2)
				{
					break;
				}
				Array.swap<T>(array, num, num2);
				num++;
				num2--;
			}
			if (low0 < num2)
			{
				Array.qsort<T>(array, low0, num2, comparison);
			}
			if (num < high0)
			{
				Array.qsort<T>(array, num, high0, comparison);
			}
		}

		private static void swap<K, V>(K[] keys, V[] items, int i, int j)
		{
			K k = keys[i];
			keys[i] = keys[j];
			keys[j] = k;
			if (items != null)
			{
				V v = items[i];
				items[i] = items[j];
				items[j] = v;
			}
		}

		private static void swap<T>(T[] array, int i, int j)
		{
			T t = array[i];
			array[i] = array[j];
			array[j] = t;
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (this.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index + this.GetLength(0) > array.GetLowerBound(0) + array.GetLength(0))
			{
				throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value has to be >= 0."));
			}
			Array.Copy(this, this.GetLowerBound(0), array, index, this.GetLength(0));
		}

		[ComVisible(false)]
		public void CopyTo(Array array, long index)
		{
			if (index < 0L || index > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			this.CopyTo(array, (int)index);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Resize<T>(ref T[] array, int newSize)
		{
			Array.Resize<T>(ref array, (array != null) ? array.Length : 0, newSize);
		}

		internal static void Resize<T>(ref T[] array, int length, int newSize)
		{
			if (newSize < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array == null)
			{
				array = new T[newSize];
				return;
			}
			if (array.Length == newSize)
			{
				return;
			}
			T[] array2 = new T[newSize];
			Array.Copy(array, array2, Math.Min(newSize, length));
			array = array2;
		}

		public static bool TrueForAll<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T t in array)
			{
				if (!match(t))
				{
					return false;
				}
			}
			return true;
		}

		public static void ForEach<T>(T[] array, Action<T> action)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T t in array)
			{
				action(t);
			}
		}

		public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			TOutput[] array2 = new TOutput[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = converter(array[i]);
			}
			return array2;
		}

		public static int FindLastIndex<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.FindLastIndex<T>(array, 0, array.Length, match);
		}

		public static int FindLastIndex<T>(T[] array, int startIndex, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			return Array.FindLastIndex<T>(array, startIndex, array.Length - startIndex, match);
		}

		public static int FindLastIndex<T>(T[] array, int startIndex, int count, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			if (startIndex > array.Length || startIndex + count > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = startIndex + count - 1; i >= startIndex; i--)
			{
				if (match(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindIndex<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.FindIndex<T>(array, 0, array.Length, match);
		}

		public static int FindIndex<T>(T[] array, int startIndex, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.FindIndex<T>(array, startIndex, array.Length - startIndex, match);
		}

		public static int FindIndex<T>(T[] array, int startIndex, int count, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			if (startIndex > array.Length || startIndex + count > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = startIndex; i < startIndex + count; i++)
			{
				if (match(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.BinarySearch<T>(array, 0, array.Length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, T value, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.BinarySearch<T>(array, 0, array.Length, value, comparer);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, int index, int length, T value)
		{
			return Array.BinarySearch<T>(array, index, length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, int index, int length, T value, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.Length - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			int i = index;
			int num = index + length - 1;
			try
			{
				while (i <= num)
				{
					int num2 = i + (num - i) / 2;
					int num3 = comparer.Compare(value, array[num2]);
					if (num3 == 0)
					{
						return num2;
					}
					if (num3 < 0)
					{
						num = num2 - 1;
					}
					else
					{
						i = num2 + 1;
					}
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(Locale.GetText("Comparer threw an exception."), ex);
			}
			return ~i;
		}

		public static int IndexOf<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.IndexOf<T>(array, value, 0, array.Length);
		}

		public static int IndexOf<T>(T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.IndexOf<T>(array, value, startIndex, array.Length - startIndex);
		}

		public static int IndexOf<T>(T[] array, T value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex - 1 > array.GetUpperBound(0) - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = startIndex + count;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = startIndex; i < num; i++)
			{
				if (@default.Equals(array[i], value))
				{
					return i;
				}
			}
			return -1;
		}

		public static int LastIndexOf<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length == 0)
			{
				return -1;
			}
			return Array.LastIndexOf<T>(array, value, array.Length - 1);
		}

		public static int LastIndexOf<T>(T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return Array.LastIndexOf<T>(array, value, startIndex, startIndex + 1);
		}

		public static int LastIndexOf<T>(T[] array, T value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex > array.GetUpperBound(0) || startIndex - count + 1 < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException();
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = startIndex; i >= startIndex - count + 1; i--)
			{
				if (@default.Equals(array[i], value))
				{
					return i;
				}
			}
			return -1;
		}

		public static T[] FindAll<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			int num = 0;
			T[] array2 = new T[array.Length];
			foreach (T t in array)
			{
				if (match(t))
				{
					array2[num++] = t;
				}
			}
			Array.Resize<T>(ref array2, num);
			return array2;
		}

		public static bool Exists<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T t in array)
			{
				if (match(t))
				{
					return true;
				}
			}
			return false;
		}

		public static ReadOnlyCollection<T> AsReadOnly<T>(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return new ReadOnlyCollection<T>(new Array.ArrayReadOnlyList<T>(array));
		}

		public static T Find<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T t in array)
			{
				if (match(t))
				{
					return t;
				}
			}
			return default(T);
		}

		public static T FindLast<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			for (int i = array.Length - 1; i >= 0; i--)
			{
				if (match(array[i]))
				{
					return array[i];
				}
			}
			return default(T);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void ConstrainedCopy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
		}

		internal struct InternalEnumerator<T> : IEnumerator, IDisposable, IEnumerator<T>
		{
			internal InternalEnumerator(Array array)
			{
				this.array = array;
				this.idx = -2;
			}

			void IEnumerator.Reset()
			{
				this.idx = -2;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this.idx = -2;
			}

			public bool MoveNext()
			{
				if (this.idx == -2)
				{
					this.idx = this.array.Length;
				}
				return this.idx != -1 && --this.idx != -1;
			}

			public T Current
			{
				get
				{
					if (this.idx == -2)
					{
						throw new InvalidOperationException("Enumeration has not started. Call MoveNext");
					}
					if (this.idx == -1)
					{
						throw new InvalidOperationException("Enumeration already finished");
					}
					return this.array.InternalArray__get_Item<T>(this.array.Length - 1 - this.idx);
				}
			}

			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private Array array;

			private int idx;
		}

		internal class SimpleEnumerator : IEnumerator, ICloneable
		{
			public SimpleEnumerator(Array arrayToEnumerate)
			{
				this.enumeratee = arrayToEnumerate;
				this.currentpos = -1;
				this.length = arrayToEnumerate.Length;
			}

			public object Current
			{
				get
				{
					if (this.currentpos < 0)
					{
						throw new InvalidOperationException(Locale.GetText("Enumeration has not started."));
					}
					if (this.currentpos >= this.length)
					{
						throw new InvalidOperationException(Locale.GetText("Enumeration has already ended"));
					}
					return this.enumeratee.GetValueImpl(this.currentpos);
				}
			}

			public bool MoveNext()
			{
				if (this.currentpos < this.length)
				{
					this.currentpos++;
				}
				return this.currentpos < this.length;
			}

			public void Reset()
			{
				this.currentpos = -1;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			private Array enumeratee;

			private int currentpos;

			private int length;
		}

		private class ArrayReadOnlyList<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
		{
			public ArrayReadOnlyList(T[] array)
			{
				this.array = array;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public T this[int index]
			{
				get
				{
					if (index >= this.array.Length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.array[index];
				}
				set
				{
					throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
				}
			}

			public int Count
			{
				get
				{
					return this.array.Length;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(T item)
			{
				throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
			}

			public void Clear()
			{
				throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
			}

			public bool Contains(T item)
			{
				return Array.IndexOf<T>(this.array, item) >= 0;
			}

			public void CopyTo(T[] array, int index)
			{
				this.array.CopyTo(array, index);
			}

			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < this.array.Length; i++)
				{
					yield return this.array[i];
				}
				yield break;
			}

			public int IndexOf(T item)
			{
				return Array.IndexOf<T>(this.array, item);
			}

			public void Insert(int index, T item)
			{
				throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
			}

			public bool Remove(T item)
			{
				throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
			}

			public void RemoveAt(int index)
			{
				throw Array.ArrayReadOnlyList<T>.ReadOnlyError();
			}

			private static Exception ReadOnlyError()
			{
				return new NotSupportedException("This collection is read-only.");
			}

			private T[] array;
		}

		private delegate void Swapper(int i, int j);
	}
}
