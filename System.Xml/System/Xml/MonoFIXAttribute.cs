using System;

namespace System.Xml
{
	[AttributeUsage(AttributeTargets.All)]
	internal class MonoFIXAttribute : Attribute
	{
		public MonoFIXAttribute()
		{
		}

		public MonoFIXAttribute(string comment)
		{
			this.comment = comment;
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
		}

		private string comment;
	}
}
