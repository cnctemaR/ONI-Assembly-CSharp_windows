using System;
using System.Collections;

namespace System.Security.AccessControl
{
	public abstract class GenericAcl : IEnumerable, ICollection
	{
		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((GenericAce[])array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public abstract int BinaryLength { get; }

		public abstract int Count { get; }

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public abstract GenericAce this[int index] { get; set; }

		public abstract byte Revision { get; }

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public void CopyTo(GenericAce[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || array.Length - index < this.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be non-negative integer and must not exceed array length - count");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array[i + index] = this[i];
			}
		}

		public abstract void GetBinaryForm(byte[] binaryForm, int offset);

		public AceEnumerator GetEnumerator()
		{
			return new AceEnumerator(this);
		}

		public static readonly byte AclRevision = 2;

		public static readonly byte AclRevisionDS = 4;

		public static readonly int MaxBinaryLength = 65536;
	}
}
