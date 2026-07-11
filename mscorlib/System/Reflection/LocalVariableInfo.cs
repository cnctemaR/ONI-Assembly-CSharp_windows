using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public class LocalVariableInfo
	{
		protected LocalVariableInfo()
		{
		}

		public virtual bool IsPinned
		{
			get
			{
				return this.is_pinned;
			}
		}

		public virtual int LocalIndex
		{
			get
			{
				return (int)this.position;
			}
		}

		public virtual Type LocalType
		{
			get
			{
				return this.type;
			}
		}

		public override string ToString()
		{
			if (this.is_pinned)
			{
				return string.Format("{0} ({1}) (pinned)", this.type, this.position);
			}
			return string.Format("{0} ({1})", this.type, this.position);
		}

		internal Type type;

		internal bool is_pinned;

		internal ushort position;
	}
}
