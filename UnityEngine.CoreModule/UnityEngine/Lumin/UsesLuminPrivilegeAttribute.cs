using System;

namespace UnityEngine.Lumin
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class UsesLuminPrivilegeAttribute : Attribute
	{
		public UsesLuminPrivilegeAttribute(string privilege)
		{
			this.m_Privilege = privilege;
		}

		public string privilege
		{
			get
			{
				return this.m_Privilege;
			}
		}

		private readonly string m_Privilege;
	}
}
