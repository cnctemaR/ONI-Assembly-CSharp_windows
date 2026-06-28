using System;

namespace System.ComponentModel.Design
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class HelpKeywordAttribute : Attribute
	{
		public HelpKeywordAttribute()
		{
		}

		public HelpKeywordAttribute(string keyword)
		{
			this.contextKeyword = keyword;
		}

		public HelpKeywordAttribute(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			this.contextKeyword = t.FullName;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			HelpKeywordAttribute helpKeywordAttribute = other as HelpKeywordAttribute;
			return helpKeywordAttribute != null && helpKeywordAttribute.contextKeyword == this.contextKeyword;
		}

		public override int GetHashCode()
		{
			return (this.contextKeyword == null) ? 0 : this.contextKeyword.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.contextKeyword == null;
		}

		public string HelpKeyword
		{
			get
			{
				return this.contextKeyword;
			}
		}

		public static readonly HelpKeywordAttribute Default;

		private string contextKeyword;
	}
}
