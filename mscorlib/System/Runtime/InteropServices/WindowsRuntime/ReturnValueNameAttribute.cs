using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public sealed class ReturnValueNameAttribute : Attribute
	{
		public ReturnValueNameAttribute(string name)
		{
			this.m_Name = name;
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		private string m_Name;
	}
}
