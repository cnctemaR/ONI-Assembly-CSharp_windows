using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class ObfuscateAssemblyAttribute : Attribute
	{
		public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
		{
			this.strip = true;
			this.is_private = assemblyIsPrivate;
		}

		public bool AssemblyIsPrivate
		{
			get
			{
				return this.is_private;
			}
		}

		public bool StripAfterObfuscation
		{
			get
			{
				return this.strip;
			}
			set
			{
				this.strip = value;
			}
		}

		private bool is_private;

		private bool strip;
	}
}
