using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	[Serializable]
	public class HashSet<T> : IEnumerable, ISerializable, IDeserializationCallback, ICollection<T>, IEnumerable<T>
	{
		public HashSet()
		{
			this.Init(10, null);
		}

		public HashSet(IEqualityComparer<T> comparer)
		{
			this.Init(10, comparer);
		}

		public HashSet(IEnumerable<T> collection)
			: this(collection, null)
		{
		}

		public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int num = 0;
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				num = collection2.Count;
			}
			this.Init(num, comparer);
			foreach (T t in collection)
			{
				this.Add(t);
			}
		}

		protected HashSet(SerializationInfo info, StreamingContext context)
		{
			this.si = info;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<T>.CopyTo(T[] array, int index)
		{
			this.CopyTo(array, index);
		}

		void ICollection<T>.Add(T item)
		{
			this.Add(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private void Init(int capacity, IEqualityComparer<T> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.comparer = comparer ?? EqualityComparer<T>.Default;
			if (capacity == 0)
			{
				capacity = 10;
			}
			capacity = (int)((float)capacity / 0.9f) + 1;
			this.InitArrays(capacity);
			this.generation = 0;
		}

		private void InitArrays(int size)
		{
			this.table = new int[size];
			this.links = new HashSet<T>.Link[size];
			this.empty_slot = -1;
			this.slots = new T[size];
			this.touched = 0;
			this.threshold = (int)((float)this.table.Length * 0.9f);
			if (this.threshold == 0 && this.table.Length > 0)
			{
				this.threshold = 1;
			}
		}

		private bool SlotsContainsAt(int index, int hash, T item)
		{
			HashSet<T>.Link link;
			for (int num = this.table[index] - 1; num != -1; num = link.Next)
			{
				link = this.links[num];
				if (link.HashCode == hash && ((hash != -2147483648 || (item != null && this.slots[num] != null)) ? this.comparer.Equals(item, this.slots[num]) : (item == null && null == this.slots[num])))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0, this.count);
		}

		public void CopyTo(T[] array, int index)
		{
			this.CopyTo(array, index, this.count);
		}

		public void CopyTo(T[] array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
			int num = 0;
			int num2 = 0;
			while (num < this.touched && num2 < count)
			{
				if (this.GetLinkHashCode(num) != 0)
				{
					array[index++] = this.slots[num];
				}
				num++;
			}
		}

		private void Resize()
		{
			int num = HashSet<T>.PrimeHelper.ToPrime((this.table.Length << 1) | 1);
			int[] array = new int[num];
			HashSet<T>.Link[] array2 = new HashSet<T>.Link[num];
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.links[num2].Next)
				{
					int num3 = (array2[num2].HashCode = this.GetItemHashCode(this.slots[num2]));
					int num4 = (num3 & int.MaxValue) % num;
					array2[num2].Next = array[num4] - 1;
					array[num4] = num2 + 1;
				}
			}
			this.table = array;
			this.links = array2;
			T[] array3 = new T[num];
			Array.Copy(this.slots, 0, array3, 0, this.touched);
			this.slots = array3;
			this.threshold = (int)((float)num * 0.9f);
		}

		private int GetLinkHashCode(int index)
		{
			return this.links[index].HashCode & int.MinValue;
		}

		private int GetItemHashCode(T item)
		{
			if (item == null)
			{
				return int.MinValue;
			}
			return this.comparer.GetHashCode(item) | int.MinValue;
		}

		public bool Add(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			int num = (itemHashCode & int.MaxValue) % this.table.Length;
			if (this.SlotsContainsAt(num, itemHashCode, item))
			{
				return false;
			}
			if (++this.count > this.threshold)
			{
				this.Resize();
				num = (itemHashCode & int.MaxValue) % this.table.Length;
			}
			int num2 = this.empty_slot;
			if (num2 == -1)
			{
				num2 = this.touched++;
			}
			else
			{
				this.empty_slot = this.links[num2].Next;
			}
			this.links[num2].HashCode = itemHashCode;
			this.links[num2].Next = this.table[num] - 1;
			this.table[num] = num2 + 1;
			this.slots[num2] = item;
			this.generation++;
			return true;
		}

		public IEqualityComparer<T> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		public void Clear()
		{
			this.count = 0;
			Array.Clear(this.table, 0, this.table.Length);
			Array.Clear(this.slots, 0, this.slots.Length);
			Array.Clear(this.links, 0, this.links.Length);
			this.empty_slot = -1;
			this.touched = 0;
			this.generation++;
		}

		public bool Contains(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			int num = (itemHashCode & int.MaxValue) % this.table.Length;
			return this.SlotsContainsAt(num, itemHashCode, item);
		}

		public bool Remove(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			int num = (itemHashCode & int.MaxValue) % this.table.Length;
			int num2 = this.table[num] - 1;
			if (num2 == -1)
			{
				return false;
			}
			int num3 = -1;
			do
			{
				HashSet<T>.Link link = this.links[num2];
				if (link.HashCode == itemHashCode && ((itemHashCode != -2147483648 || (item != null && this.slots[num2] != null)) ? this.comparer.Equals(this.slots[num2], item) : (item == null && null == this.slots[num2])))
				{
					break;
				}
				num3 = num2;
				num2 = link.Next;
			}
			while (num2 != -1);
			if (num2 == -1)
			{
				return false;
			}
			this.count--;
			if (num3 == -1)
			{
				this.table[num] = this.links[num2].Next + 1;
			}
			else
			{
				this.links[num3].Next = this.links[num2].Next;
			}
			this.links[num2].Next = this.empty_slot;
			this.empty_slot = num2;
			this.links[num2].HashCode = 0;
			this.slots[num2] = default(T);
			this.generation++;
			return true;
		}

		public int RemoveWhere(Predicate<T> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			int num = 0;
			T[] array = new T[this.count];
			this.CopyTo(array, 0);
			foreach (T t in array)
			{
				if (predicate(t))
				{
					this.Remove(t);
					num++;
				}
			}
			return num;
		}

		public void TrimExcess()
		{
			this.Resize();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			T[] array = new T[this.count];
			this.CopyTo(array, 0);
			foreach (T t in array)
			{
				if (!other.Contains(t))
				{
					this.Remove(t);
				}
			}
			foreach (T t2 in other)
			{
				if (!this.Contains(t2))
				{
					this.Remove(t2);
				}
			}
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in other)
			{
				this.Remove(t);
			}
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
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
			if (this.count != other.Count<T>())
			{
				return false;
			}
			foreach (T t in this)
			{
				if (!other.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in other)
			{
				if (this.Contains(t))
				{
					this.Remove(t);
				}
				else
				{
					this.Add(t);
				}
			}
		}

		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in other)
			{
				this.Add(t);
			}
		}

		private bool CheckIsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in this)
			{
				if (!other.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			return this.count == 0 || (this.count <= other.Count<T>() && this.CheckIsSubsetOf(other));
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			return this.count == 0 || (this.count < other.Count<T>() && this.CheckIsSubsetOf(other));
		}

		private bool CheckIsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T t in other)
			{
				if (!this.Contains(t))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			return this.count >= other.Count<T>() && this.CheckIsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			return this.count > other.Count<T>() && this.CheckIsSupersetOf(other);
		}

		[MonoTODO]
		public static IEqualityComparer<HashSet<T>> CreateSetComparer()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void OnDeserialization(object sender)
		{
			if (this.si == null)
			{
				return;
			}
			throw new NotImplementedException();
		}

		public HashSet<T>.Enumerator GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		private const int INITIAL_SIZE = 10;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = -2147483648;

		private int[] table;

		private HashSet<T>.Link[] links;

		private T[] slots;

		private int touched;

		private int empty_slot;

		private int count;

		private int threshold;

		private IEqualityComparer<T> comparer;

		private SerializationInfo si;

		private int generation;

		private struct Link
		{
			public int HashCode;

			public int Next;
		}

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			internal Enumerator(HashSet<T> hashset)
			{
				this.hashset = hashset;
				this.stamp = hashset.generation;
			}

			object IEnumerator.Current
			{
				get
				{
					this.CheckState();
					if (this.next <= 0)
					{
						throw new InvalidOperationException("Current is not valid");
					}
					return this.current;
				}
			}

			void IEnumerator.Reset()
			{
				this.CheckState();
				this.next = 0;
			}

			public bool MoveNext()
			{
				this.CheckState();
				if (this.next < 0)
				{
					return false;
				}
				while (this.next < this.hashset.touched)
				{
					int num = this.next++;
					if (this.hashset.GetLinkHashCode(num) != 0)
					{
						this.current = this.hashset.slots[num];
						return true;
					}
				}
				this.next = -1;
				return false;
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			public void Dispose()
			{
				this.hashset = null;
			}

			private void CheckState()
			{
				if (this.hashset == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.hashset.generation != this.stamp)
				{
					throw new InvalidOperationException("HashSet have been modified while it was iterated over");
				}
			}

			private HashSet<T> hashset;

			private int next;

			private int stamp;

			private T current;
		}

		private static class PrimeHelper
		{
			private static bool TestPrime(int x)
			{
				if ((x & 1) != 0)
				{
					int num = (int)Math.Sqrt((double)x);
					for (int i = 3; i < num; i += 2)
					{
						if (x % i == 0)
						{
							return false;
						}
					}
					return true;
				}
				return x == 2;
			}

			private static int CalcPrime(int x)
			{
				for (int i = (x & -2) - 1; i < 2147483647; i += 2)
				{
					if (HashSet<T>.PrimeHelper.TestPrime(i))
					{
						return i;
					}
				}
				return x;
			}

			public static int ToPrime(int x)
			{
				for (int i = 0; i < HashSet<T>.PrimeHelper.primes_table.Length; i++)
				{
					if (x <= HashSet<T>.PrimeHelper.primes_table[i])
					{
						return HashSet<T>.PrimeHelper.primes_table[i];
					}
				}
				return HashSet<T>.PrimeHelper.CalcPrime(x);
			}

			private static readonly int[] primes_table = new int[]
			{
				11, 19, 37, 73, 109, 163, 251, 367, 557, 823,
				1237, 1861, 2777, 4177, 6247, 9371, 14057, 21089, 31627, 47431,
				71143, 106721, 160073, 240101, 360163, 540217, 810343, 1215497, 1823231, 2734867,
				4102283, 6153409, 9230113, 13845163
			};
		}
	}
}
