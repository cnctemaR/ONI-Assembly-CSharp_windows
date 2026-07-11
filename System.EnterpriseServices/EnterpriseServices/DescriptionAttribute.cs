using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
	[ComVisible(false)]
	public sealed class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute(string desc)
		{
		}
	}
}
