using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
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
