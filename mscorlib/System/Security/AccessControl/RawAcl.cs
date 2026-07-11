using System;
using System.Collections.Generic;

namespace System.Security.AccessControl
{
	public sealed class RawAcl : GenericAcl
	{
		public RawAcl(byte revision, int capacity)
		{
			this.revision = revision;
			this.list = new List<GenericAce>(capacity);
		}

		public RawAcl(byte[] binaryForm, int offset)
			: this(0, 10)
		{
		}

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public override GenericAce this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public override byte Revision
		{
			get
			{
				return this.revision;
			}
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public void InsertAce(int index, GenericAce ace)
		{
			if (ace == null)
			{
				throw new ArgumentNullException("ace");
			}
			this.list.Insert(index, ace);
		}

		public void RemoveAce(int index)
		{
			this.list.RemoveAt(index);
		}

		private byte revision;

		private List<GenericAce> list;
	}
}
