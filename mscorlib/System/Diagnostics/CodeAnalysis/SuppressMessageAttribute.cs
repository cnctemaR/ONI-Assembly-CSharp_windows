using System;

namespace System.Diagnostics.CodeAnalysis
{
	[Conditional("CODE_ANALYSIS")]
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class SuppressMessageAttribute : Attribute
	{
		public SuppressMessageAttribute(string category, string checkId)
		{
			this.category = category;
			this.checkId = checkId;
		}

		public string Category
		{
			get
			{
				return this.category;
			}
		}

		public string CheckId
		{
			get
			{
				return this.checkId;
			}
		}

		public string Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		public string Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
			set
			{
				this.messageId = value;
			}
		}

		public string Justification
		{
			get
			{
				return this.justification;
			}
			set
			{
				this.justification = value;
			}
		}

		private string category;

		private string justification;

		private string checkId;

		private string scope;

		private string target;

		private string messageId;
	}
}
