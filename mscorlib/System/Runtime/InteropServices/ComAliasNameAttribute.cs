using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class ComAliasNameAttribute : Attribute
	{
		public ComAliasNameAttribute(string alias)
		{
			this.val = alias;
		}

		public string Value
		{
			get
			{
				return this.val;
			}
		}

		private string val;
	}
}
