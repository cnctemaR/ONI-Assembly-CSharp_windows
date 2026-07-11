using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography
{
	public sealed class OidCollection : ICollection, IEnumerable
	{
		public OidCollection()
		{
			this.m_list = new ArrayList();
		}

		public int Add(Oid oid)
		{
			return this.m_list.Add(oid);
		}

		public Oid this[int index]
		{
			get
			{
				return this.m_list[index] as Oid;
			}
		}

		public Oid this[string oid]
		{
			get
			{
				string text = X509Utils.FindOidInfoWithFallback(2U, oid, OidGroup.All);
				if (text == null)
				{
					text = oid;
				}
				foreach (object obj in this.m_list)
				{
					Oid oid2 = (Oid)obj;
					if (oid2.Value == text)
					{
						return oid2;
					}
				}
				return null;
			}
		}

		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public OidEnumerator GetEnumerator()
		{
			return new OidEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OidEnumerator(this);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException(global::SR.GetString("Only single dimensional arrays are supported for the requested action."));
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", global::SR.GetString("Index was out of range. Must be non-negative and less than the size of the collection."));
			}
			if (index + this.Count > array.Length)
			{
				throw new ArgumentException(global::SR.GetString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
				index++;
			}
		}

		public void CopyTo(Oid[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

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

		private ArrayList m_list;
	}
}
