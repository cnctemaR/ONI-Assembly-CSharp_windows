using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	[ComVisible(true)]
	[Serializable]
	public sealed class DefaultMemberAttribute : Attribute
	{
		public DefaultMemberAttribute(string memberName)
		{
			this.m_memberName = memberName;
		}

		public string MemberName
		{
			get
			{
				return this.m_memberName;
			}
		}

		private string m_memberName;
	}
}
