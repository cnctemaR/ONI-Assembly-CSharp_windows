using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T>, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
	{
		public HashSet()
			: this(EqualityComparer<T>.Default)
		{
		}

		public HashSet(IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			this._comparer = comparer;
			this._lastIndex = 0;
			this._count = 0;
			this._freeList = -1;
			this._version = 0;
		}

		public HashSet(int capacity)
			: this(capacity, EqualityComparer<T>.Default)
		{
		}

		public HashSet(IEnumerable<T> collection)
			: this(collection, EqualityComparer<T>.Default)
		{
		}

		public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
			: this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			HashSet<T> hashSet = collection as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				this.CopyFrom(hashSet);
				return;
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			int num = ((collection2 == null) ? 0 : collection2.Count);
			this.Initialize(num);
			this.UnionWith(collection);
			if (this._count > 0 && this._slots.Length / this._count > 3)
			{
				this.TrimExcess();
			}
		}

		protected HashSet(SerializationInfo info, StreamingContext context)
		{
			this._siInfo = info;
		}

		private void CopyFrom(HashSet<T> source)
		{
			int count = source._count;
			if (count == 0)
			{
				return;
			}
			int num = source._buckets.Length;
			if (HashHelpers.ExpandPrime(count + 1) >= num)
			{
				this._buckets = (int[])source._buckets.Clone();
				this._slots = (HashSet<T>.Slot[])source._slots.Clone();
				this._lastIndex = source._lastIndex;
				this._freeList = source._freeList;
			}
			else
			{
				int lastIndex = source._lastIndex;
				HashSet<T>.Slot[] slots = source._slots;
				this.Initialize(count);
				int num2 = 0;
				for (int i = 0; i < lastIndex; i++)
				{
					int hashCode = slots[i].hashCode;
					if (hashCode >= 0)
					{
						this.AddValue(num2, hashCode, slots[i].value);
						num2++;
					}
				}
				this._lastIndex = num2;
			}
			this._count = count;
		}

		public HashSet(int capacity, IEqualityComparer<T> comparer)
			: this(comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity > 0)
			{
				this.Initialize(capacity);
			}
		}

		void ICollection<T>.Add(T item)
		{
			this.AddIfNotPresent(item);
		}

		public void Clear()
		{
			if (this._lastIndex > 0)
			{
				Array.Clear(this._slots, 0, this._lastIndex);
				Array.Clear(this._buckets, 0, this._buckets.Length);
				this._lastIndex = 0;
				this._count = 0;
				this._freeList = -1;
			}
			this._version++;
		}

		public bool Contains(T item)
		{
			if (this._buckets != null)
			{
				int num = this.InternalGetHashCode(item);
				for (int i = this._buckets[num % this._buckets.Length] - 1; i >= 0; i = this._slots[i].next)
				{
					if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, item))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, this._count);
		}

		public bool Remove(T item)
		{
			if (this._buckets != null)
			{
				int num = this.InternalGetHashCode(item);
				int num2 = num % this._buckets.Length;
				int num3 = -1;
				for (int i = this._buckets[num2] - 1; i >= 0; i = this._slots[i].next)
				{
					if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, item))
					{
						if (num3 < 0)
						{
							this._buckets[num2] = this._slots[i].next + 1;
						}
						else
						{
							this._slots[num3].next = this._slots[i].next;
						}
						this._slots[i].hashCode = -1;
						if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
						{
							this._slots[i].value = default(T);
						}
						this._slots[i].next = this._freeList;
						this._count--;
						this._version++;
						if (this._count == 0)
						{
							this._lastIndex = 0;
							this._freeList = -1;
						}
						else
						{
							this._freeList = i;
						}
						return true;
					}
					num3 = i;
				}
			}
			return false;
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public HashSet<T>.Enumerator GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this._version);
			info.AddValue("Comparer", this._comparer, typeof(IComparer<T>));
			info.AddValue("Capacity", (this._buckets == null) ? 0 : this._buckets.Length);
			if (this._buckets != null)
			{
				T[] array = new T[this._count];
				this.CopyTo(array);
				info.AddValue("Elements", array, typeof(T[]));
			}
		}

		public virtual void OnDeserialization(object sender)
		{
			if (this._siInfo == null)
			{
				return;
			}
			int @int = this._siInfo.GetInt32("Capacity");
			this._comparer = (IEqualityComparer<T>)this._siInfo.GetValue("Comparer", typeof(IEqualityComparer<T>));
			this._freeList = -1;
			if (@int != 0)
			{
				this._buckets = new int[@int];
				this._slots = new HashSet<T>.Slot[@int];
				T[] array = (T[])this._siInfo.GetValue("Elements", typeof(T[]));
				if (array == null)
				{
					throw new SerializationException("The Keys for this dictionary are missing.");
				}
				for (int i = 0; i < array.Length; i++)
				{
					this.AddIfNotPresent(array[i]);
				}
			}
			else
			{
				this._buckets = null;
			}
			this._version = this._siInfo.GetInt32("Version");
			this._siInfo = null;
		}

		public bool Add(T item)
		{
			return this.AddIfNotPresent(item);
		}

		public bool TryGetValue(T equalValue, out T actualValue)
		{
			if (this._buckets != null)
			{
				int num = this.InternalIndexOf(equalValue);
				if (num >= 0)
				{
					actualValue = this._slots[num].value;
					return true;
				}
			}
			actualValue = default(T);
			return false;
		}

		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in other)
			{
				this.AddIfNotPresent(t);
			}
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return;
			}
			if (other == this)
			{
				return;
			}
			ICollection<T> collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					this.Clear();
					return;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					this.IntersectWithHashSetWithSameEC(hashSet);
					return;
				}
			}
			this.IntersectWithEnumerable(other);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			foreach (T t in other)
			{
				this.Remove(t);
			}
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				this.UnionWith(other);
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				this.SymmetricExceptWithUniqueHashSet(hashSet);
				return;
			}
			this.SymmetricExceptWithEnumerable(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return true;
			}
			if (other == this)
			{
				return true;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				return this._count <= hashSet.Count && this.IsSubsetOfHashSetWithSameEC(hashSet);
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount >= 0;
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return false;
			}
			ICollection<T> collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return false;
				}
				if (this._count == 0)
				{
					return collection.Count > 0;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					return this._count < hashSet.Count && this.IsSubsetOfHashSetWithSameEC(hashSet);
				}
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount > 0;
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return true;
			}
			ICollection<T> collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet) && hashSet.Count > this._count)
				{
					return false;
				}
			}
			return this.ContainsAllElements(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return false;
			}
			if (other == this)
			{
				return false;
			}
			ICollection<T> collection = other as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return true;
				}
				HashSet<T> hashSet = other as HashSet<T>;
				if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
				{
					return hashSet.Count < this._count && this.ContainsAllElements(hashSet);
				}
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
			return elementCount.uniqueCount < this._count && elementCount.unfoundCount == 0;
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._count == 0)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			foreach (T t in other)
			{
				if (this.Contains(t))
				{
					return true;
				}
			}
			return false;
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other == this)
			{
				return true;
			}
			HashSet<T> hashSet = other as HashSet<T>;
			if (hashSet != null && HashSet<T>.AreEqualityComparersEqual(this, hashSet))
			{
				return this._count == hashSet.Count && this.ContainsAllElements(hashSet);
			}
			ICollection<T> collection = other as ICollection<T>;
			if (collection != null && this._count == 0 && collection.Count > 0)
			{
				return false;
			}
			HashSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
			return elementCount.uniqueCount == this._count && elementCount.unfoundCount == 0;
		}

		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0, this._count);
		}

		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Non negative number is required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "Non negative number is required.");
			}
			if (arrayIndex > array.Length || count > array.Length - arrayIndex)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			int num = 0;
			int num2 = 0;
			while (num2 < this._lastIndex && num < count)
			{
				if (this._slots[num2].hashCode >= 0)
				{
					array[arrayIndex + num] = this._slots[num2].value;
					num++;
				}
				num2++;
			}
		}

		public int RemoveWhere(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			int num = 0;
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					T value = this._slots[i].value;
					if (match(value) && this.Remove(value))
					{
						num++;
					}
				}
			}
			return num;
		}

		public IEqualityComparer<T> Comparer
		{
			get
			{
				return this._comparer;
			}
		}

		public void TrimExcess()
		{
			if (this._count == 0)
			{
				this._buckets = null;
				this._slots = null;
				this._version++;
				return;
			}
			int prime = HashHelpers.GetPrime(this._count);
			HashSet<T>.Slot[] array = new HashSet<T>.Slot[prime];
			int[] array2 = new int[prime];
			int num = 0;
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					array[num] = this._slots[i];
					int num2 = array[num].hashCode % prime;
					array[num].next = array2[num2] - 1;
					array2[num2] = num + 1;
					num++;
				}
			}
			this._lastIndex = num;
			this._slots = array;
			this._buckets = array2;
			this._freeList = -1;
		}

		public static IEqualityComparer<HashSet<T>> CreateSetComparer()
		{
			return new HashSetEqualityComparer<T>();
		}

		private void Initialize(int capacity)
		{
			int prime = HashHelpers.GetPrime(capacity);
			this._buckets = new int[prime];
			this._slots = new HashSet<T>.Slot[prime];
		}

		private void IncreaseCapacity()
		{
			int num = HashHelpers.ExpandPrime(this._count);
			if (num <= this._count)
			{
				throw new ArgumentException("HashSet capacity is too big.");
			}
			this.SetCapacity(num);
		}

		private void SetCapacity(int newSize)
		{
			HashSet<T>.Slot[] array = new HashSet<T>.Slot[newSize];
			if (this._slots != null)
			{
				Array.Copy(this._slots, 0, array, 0, this._lastIndex);
			}
			int[] array2 = new int[newSize];
			for (int i = 0; i < this._lastIndex; i++)
			{
				int num = array[i].hashCode % newSize;
				array[i].next = array2[num] - 1;
				array2[num] = i + 1;
			}
			this._slots = array;
			this._buckets = array2;
		}

		private bool AddIfNotPresent(T value)
		{
			if (this._buckets == null)
			{
				this.Initialize(0);
			}
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			for (int i = this._buckets[num2] - 1; i >= 0; i = this._slots[i].next)
			{
				if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, value))
				{
					return false;
				}
			}
			int num3;
			if (this._freeList >= 0)
			{
				num3 = this._freeList;
				this._freeList = this._slots[num3].next;
			}
			else
			{
				if (this._lastIndex == this._slots.Length)
				{
					this.IncreaseCapacity();
					num2 = num % this._buckets.Length;
				}
				num3 = this._lastIndex;
				this._lastIndex++;
			}
			this._slots[num3].hashCode = num;
			this._slots[num3].value = value;
			this._slots[num3].next = this._buckets[num2] - 1;
			this._buckets[num2] = num3 + 1;
			this._count++;
			this._version++;
			return true;
		}

		private void AddValue(int index, int hashCode, T value)
		{
			int num = hashCode % this._buckets.Length;
			this._slots[index].hashCode = hashCode;
			this._slots[index].value = value;
			this._slots[index].next = this._buckets[num] - 1;
			this._buckets[num] = index + 1;
		}

		private bool ContainsAllElements(IEnumerable<T> other)
		{
			foreach (T t in other)
			{
				if (!this.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other)
		{
			foreach (T t in this)
			{
				if (!other.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		private void IntersectWithHashSetWithSameEC(HashSet<T> other)
		{
			for (int i = 0; i < this._lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0)
				{
					T value = this._slots[i].value;
					if (!other.Contains(value))
					{
						this.Remove(value);
					}
				}
			}
		}

		private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
		{
			int lastIndex = this._lastIndex;
			int num = BitHelper.ToIntArrayLength(lastIndex);
			BitHelper bitHelper;
			checked
			{
				if (num <= 100)
				{
					bitHelper = new BitHelper(stackalloc int[unchecked((UIntPtr)num) * 4], num);
				}
				else
				{
					bitHelper = new BitHelper(new int[num], num);
				}
				foreach (T t in other)
				{
					int num2 = this.InternalIndexOf(t);
					if (num2 >= 0)
					{
						bitHelper.MarkBit(num2);
					}
				}
			}
			for (int i = 0; i < lastIndex; i++)
			{
				if (this._slots[i].hashCode >= 0 && !bitHelper.IsMarked(i))
				{
					this.Remove(this._slots[i].value);
				}
			}
		}

		private int InternalIndexOf(T item)
		{
			int num = this.InternalGetHashCode(item);
			for (int i = this._buckets[num % this._buckets.Length] - 1; i >= 0; i = this._slots[i].next)
			{
				if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, item))
				{
					return i;
				}
			}
			return -1;
		}

		private void SymmetricExceptWithUniqueHashSet(HashSet<T> other)
		{
			foreach (T t in other)
			{
				if (!this.Remove(t))
				{
					this.AddIfNotPresent(t);
				}
			}
		}

		private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other)
		{
			int lastIndex = this._lastIndex;
			int num = BitHelper.ToIntArrayLength(lastIndex);
			BitHelper bitHelper;
			checked
			{
				BitHelper bitHelper2;
				if (num <= 50)
				{
					bitHelper = new BitHelper(stackalloc int[unchecked((UIntPtr)num) * 4], num);
					bitHelper2 = new BitHelper(stackalloc int[unchecked((UIntPtr)num) * 4], num);
				}
				else
				{
					bitHelper = new BitHelper(new int[num], num);
					bitHelper2 = new BitHelper(new int[num], num);
				}
				foreach (T t in other)
				{
					int num2 = 0;
					if (this.AddOrGetLocation(t, out num2))
					{
						bitHelper2.MarkBit(num2);
					}
					else if (num2 < lastIndex && !bitHelper2.IsMarked(num2))
					{
						bitHelper.MarkBit(num2);
					}
				}
			}
			for (int i = 0; i < lastIndex; i++)
			{
				if (bitHelper.IsMarked(i))
				{
					this.Remove(this._slots[i].value);
				}
			}
		}

		private bool AddOrGetLocation(T value, out int location)
		{
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			for (int i = this._buckets[num2] - 1; i >= 0; i = this._slots[i].next)
			{
				if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, value))
				{
					location = i;
					return false;
				}
			}
			int num3;
			if (this._freeList >= 0)
			{
				num3 = this._freeList;
				this._freeList = this._slots[num3].next;
			}
			else
			{
				if (this._lastIndex == this._slots.Length)
				{
					this.IncreaseCapacity();
					num2 = num % this._buckets.Length;
				}
				num3 = this._lastIndex;
				this._lastIndex++;
			}
			this._slots[num3].hashCode = num;
			this._slots[num3].value = value;
			this._slots[num3].next = this._buckets[num2] - 1;
			this._buckets[num2] = num3 + 1;
			this._count++;
			this._version++;
			location = num3;
			return true;
		}

		private unsafe HashSet<T>.ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
		{
			HashSet<T>.ElementCount elementCount;
			if (this._count == 0)
			{
				int num = 0;
				using (IEnumerator<T> enumerator = other.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						num++;
					}
				}
				elementCount.uniqueCount = 0;
				elementCount.unfoundCount = num;
				return elementCount;
			}
			int num2 = BitHelper.ToIntArrayLength(this._lastIndex);
			BitHelper bitHelper;
			int num3;
			int num4;
			checked
			{
				if (num2 <= 100)
				{
					bitHelper = new BitHelper(stackalloc int[unchecked((UIntPtr)num2) * 4], num2);
				}
				else
				{
					bitHelper = new BitHelper(new int[num2], num2);
				}
				num3 = 0;
				num4 = 0;
			}
			foreach (T t2 in other)
			{
				int num5 = this.InternalIndexOf(t2);
				if (num5 >= 0)
				{
					if (!bitHelper.IsMarked(num5))
					{
						bitHelper.MarkBit(num5);
						num4++;
					}
				}
				else
				{
					num3++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
			elementCount.uniqueCount = num4;
			elementCount.unfoundCount = num3;
			return elementCount;
		}

		internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer)
		{
			if (set1 == null)
			{
				return set2 == null;
			}
			if (set2 == null)
			{
				return false;
			}
			if (!HashSet<T>.AreEqualityComparersEqual(set1, set2))
			{
				foreach (T t in set2)
				{
					bool flag = false;
					foreach (T t2 in set1)
					{
						if (comparer.Equals(t, t2))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}
			if (set1.Count != set2.Count)
			{
				return false;
			}
			foreach (T t3 in set2)
			{
				if (!set1.Contains(t3))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2)
		{
			return set1.Comparer.Equals(set2.Comparer);
		}

		private int InternalGetHashCode(T item)
		{
			if (item == null)
			{
				return 0;
			}
			return this._comparer.GetHashCode(item) & int.MaxValue;
		}

		private const int Lower31BitMask = 2147483647;

		private const int StackAllocThreshold = 100;

		private const int ShrinkThreshold = 3;

		private const string CapacityName = "Capacity";

		private const string ElementsName = "Elements";

		private const string ComparerName = "Comparer";

		private const string VersionName = "Version";

		private int[] _buckets;

		private HashSet<T>.Slot[] _slots;

		private int _count;

		private int _lastIndex;

		private int _freeList;

		private IEqualityComparer<T> _comparer;

		private int _version;

		private SerializationInfo _siInfo;

		internal struct ElementCount
		{
			internal int uniqueCount;

			internal int unfoundCount;
		}

		internal struct Slot
		{
			internal int hashCode;

			internal int next;

			internal T value;
		}

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(HashSet<T> set)
			{
				this._set = set;
				this._index = 0;
				this._version = set._version;
				this._current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this._version != this._set._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				while (this._index < this._set._lastIndex)
				{
					if (this._set._slots[this._index].hashCode >= 0)
					{
						this._current = this._set._slots[this._index].value;
						this._index++;
						return true;
					}
					this._index++;
				}
				this._index = this._set._lastIndex + 1;
				this._current = default(T);
				return false;
			}

			public T Current
			{
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index == this._set._lastIndex + 1)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				if (this._version != this._set._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._index = 0;
				this._current = default(T);
			}

			private HashSet<T> _set;

			private int _index;

			private int _version;

			private T _current;
		}
	}
}
