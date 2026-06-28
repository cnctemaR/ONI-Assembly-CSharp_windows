using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class CommonAcl : GenericAcl
	{
		internal CommonAcl(bool isContainer, bool isDS, byte revision)
			: this(isContainer, isDS, revision, 10)
		{
		}

		internal CommonAcl(bool isContainer, bool isDS, byte revision, int capacity)
		{
			this.is_container = isContainer;
			this.is_ds = isDS;
			this.revision = revision;
			this.list = new List<GenericAce>(capacity);
		}

		[MonoTODO]
		public sealed override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public sealed override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		[MonoTODO]
		public bool IsCanonical
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsContainer
		{
			get
			{
				return this.is_container;
			}
		}

		public bool IsDS
		{
			get
			{
				return this.is_ds;
			}
		}

		public sealed override GenericAce this[int index]
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

		public sealed override byte Revision
		{
			get
			{
				return this.revision;
			}
		}

		[MonoTODO]
		public sealed override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void Purge(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveInheritedAces()
		{
			throw new NotImplementedException();
		}

		private const int default_capacity = 10;

		private bool is_container;

		private bool is_ds;

		private byte revision;

		private List<GenericAce> list;
	}
}
