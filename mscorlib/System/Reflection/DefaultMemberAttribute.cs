using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	[Serializable]
	public sealed class DefaultMemberAttribute : Attribute
	{
		public DefaultMemberAttribute(string memberName)
		{
			this.member_name = memberName;
		}

		public string MemberName
		{
			get
			{
				return this.member_name;
			}
		}

		private string member_name;
	}
}
