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
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
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

		public string HelpKeyword
		{
			get
			{
				return this.contextKeyword;
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this || (obj != null && obj is HelpKeywordAttribute && ((HelpKeywordAttribute)obj).HelpKeyword == this.HelpKeyword);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(HelpKeywordAttribute.Default);
		}

		public static readonly HelpKeywordAttribute Default = new HelpKeywordAttribute();

		private string contextKeyword;
	}
}
