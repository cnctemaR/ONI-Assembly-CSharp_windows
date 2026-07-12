using System;

namespace UnityEngine.Lumin
{
	[Obsolete("Lumin is no longer supported in Unity 2022.2")]
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
