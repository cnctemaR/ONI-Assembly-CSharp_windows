using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_LocalBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LocalBuilder : LocalVariableInfo, _LocalBuilder
	{
		void _LocalBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal LocalBuilder(Type t, ILGenerator ilgen)
		{
			this.type = t;
			this.ilgen = ilgen;
		}

		public void SetLocalSymInfo(string name, int startOffset, int endOffset)
		{
			this.name = name;
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}

		public void SetLocalSymInfo(string name)
		{
			this.SetLocalSymInfo(name, 0, 0);
		}

		public override Type LocalType
		{
			get
			{
				return this.type;
			}
		}

		public override bool IsPinned
		{
			get
			{
				return this.is_pinned;
			}
		}

		public override int LocalIndex
		{
			get
			{
				return (int)this.position;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal int StartOffset
		{
			get
			{
				return this.startOffset;
			}
		}

		internal int EndOffset
		{
			get
			{
				return this.endOffset;
			}
		}

		internal LocalBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string name;

		internal ILGenerator ilgen;

		private int startOffset;

		private int endOffset;
	}
}
