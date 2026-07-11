using System;

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	[Conditional("CODE_ANALYSIS")]
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

		private string category;

		private string checkId;

		private string justification;

		private string messageId;

		private string scope;

		private string target;
	}
}
