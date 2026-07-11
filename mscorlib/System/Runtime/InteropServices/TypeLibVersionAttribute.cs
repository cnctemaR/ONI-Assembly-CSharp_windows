using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class TypeLibVersionAttribute : Attribute
	{
		public TypeLibVersionAttribute(int major, int minor)
		{
			this.major = major;
			this.minor = minor;
		}

		public int MajorVersion
		{
			get
			{
				return this.major;
			}
		}

		public int MinorVersion
		{
			get
			{
				return this.minor;
			}
		}

		private int major;

		private int minor;
	}
}
