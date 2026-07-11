using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public class IdentityReferenceCollection : IEnumerable, ICollection<IdentityReference>, IEnumerable<IdentityReference>
	{
		public IdentityReferenceCollection()
		{
			this._list = new ArrayList();
		}

		public IdentityReferenceCollection(int capacity)
		{
			this._list = new ArrayList(capacity);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IdentityReference this[int index]
		{
			get
			{
				if (index >= this._list.Count)
				{
					return null;
				}
				return (IdentityReference)this._list[index];
			}
			set
			{
				this._list[index] = value;
			}
		}

		public void Add(IdentityReference identity)
		{
			this._list.Add(identity);
		}

		public void Clear()
		{
			this._list.Clear();
		}

		public bool Contains(IdentityReference identity)
		{
			foreach (object obj in this._list)
			{
				IdentityReference identityReference = (IdentityReference)obj;
				if (identityReference.Equals(identity))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(IdentityReference[] array, int offset)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<IdentityReference> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(IdentityReference identity)
		{
			foreach (object obj in this._list)
			{
				IdentityReference identityReference = (IdentityReference)obj;
				if (identityReference.Equals(identity))
				{
					this._list.Remove(identityReference);
					return true;
				}
			}
			return false;
		}

		public IdentityReferenceCollection Translate(Type targetType)
		{
			throw new NotImplementedException();
		}

		public IdentityReferenceCollection Translate(Type targetType, bool forceSuccess)
		{
			throw new NotImplementedException();
		}

		private ArrayList _list;
	}
}
