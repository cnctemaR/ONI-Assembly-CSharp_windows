using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyFlagsAttribute : Attribute
	{
		[Obsolete("")]
		[CLSCompliant(false)]
		public AssemblyFlagsAttribute(uint flags)
		{
			this.flags = flags;
		}

		[Obsolete("")]
		public AssemblyFlagsAttribute(int assemblyFlags)
		{
			this.flags = (uint)assemblyFlags;
		}

		public AssemblyFlagsAttribute(AssemblyNameFlags assemblyFlags)
		{
			this.flags = (uint)assemblyFlags;
		}

		[CLSCompliant(false)]
		[Obsolete("")]
		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public int AssemblyFlags
		{
			get
			{
				return (int)this.flags;
			}
		}

		private uint flags;
	}
}
