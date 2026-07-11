using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class OptionalFieldAttribute : Attribute
	{
		public int VersionAdded
		{
			get
			{
				return this.version_added;
			}
			set
			{
				this.version_added = value;
			}
		}

		private int version_added;
	}
}
