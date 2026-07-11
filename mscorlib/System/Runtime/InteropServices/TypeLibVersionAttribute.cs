using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class TypeLibVersionAttribute : Attribute
	{
		public TypeLibVersionAttribute(int major, int minor)
		{
			this._major = major;
			this._minor = minor;
		}

		public int MajorVersion
		{
			get
			{
				return this._major;
			}
		}

		public int MinorVersion
		{
			get
			{
				return this._minor;
			}
		}

		internal int _major;

		internal int _minor;
	}
}
