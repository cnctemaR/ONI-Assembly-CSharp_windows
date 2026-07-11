using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using Unity;

namespace System.Threading
{
	[Serializable]
	public sealed class CompressedStack : ISerializable
	{
		internal CompressedStack(int length)
		{
			if (length > 0)
			{
				this._list = new ArrayList(length);
			}
		}

		internal CompressedStack(CompressedStack cs)
		{
			if (cs != null && cs._list != null)
			{
				this._list = (ArrayList)cs._list.Clone();
			}
		}

		[ComVisible(false)]
		public CompressedStack CreateCopy()
		{
			return new CompressedStack(this);
		}

		public static CompressedStack Capture()
		{
			throw new NotSupportedException();
		}

		[SecurityCritical]
		public static CompressedStack GetCompressedStack()
		{
			throw new NotSupportedException();
		}

		[MonoTODO("incomplete")]
		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		[SecurityCritical]
		public static void Run(CompressedStack compressedStack, ContextCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		internal bool Equals(CompressedStack cs)
		{
			if (this.IsEmpty())
			{
				return cs.IsEmpty();
			}
			return !cs.IsEmpty() && this._list.Count == cs._list.Count;
		}

		internal bool IsEmpty()
		{
			return this._list == null || this._list.Count == 0;
		}

		internal IList List
		{
			get
			{
				return this._list;
			}
		}

		internal CompressedStack()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ArrayList _list;
	}
}
