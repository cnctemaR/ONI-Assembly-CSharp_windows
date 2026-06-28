using System;
using System.ComponentModel;

namespace System.Data.Odbc
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class OdbcCategoryAttribute : CategoryAttribute
	{
		public OdbcCategoryAttribute(string category)
		{
			this.category = category;
		}

		public new string Category
		{
			get
			{
				return this.category;
			}
		}

		[MonoTODO]
		protected override string GetLocalizedString(string value)
		{
			throw new NotImplementedException();
		}

		private string category;
	}
}
