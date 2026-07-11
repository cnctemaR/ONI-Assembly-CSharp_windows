using System;

namespace System
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class MonoTODOAttribute : Attribute
	{
		public MonoTODOAttribute()
		{
		}

		public MonoTODOAttribute(string comment)
		{
		}

		public string Comment
		{
			get
			{
				throw null;
			}
		}
	}
}
