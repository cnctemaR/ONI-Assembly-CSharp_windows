using System;
using System.ComponentModel;

namespace System.Data.Odbc
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class OdbcDescriptionAttribute : DescriptionAttribute
	{
		public OdbcDescriptionAttribute(string description)
			: base(description)
		{
			this.description = description;
		}

		public override string Description
		{
			get
			{
				return this.description;
			}
		}

		private string description;
	}
}
