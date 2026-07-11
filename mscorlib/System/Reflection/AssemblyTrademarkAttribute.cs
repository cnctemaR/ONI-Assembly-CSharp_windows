using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyTrademarkAttribute : Attribute
	{
		public AssemblyTrademarkAttribute(string trademark)
		{
			this.m_trademark = trademark;
		}

		public string Trademark
		{
			get
			{
				return this.m_trademark;
			}
		}

		private string m_trademark;
	}
}
