using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyProductAttribute : Attribute
	{
		public AssemblyProductAttribute(string product)
		{
			this.m_product = product;
		}

		public string Product
		{
			get
			{
				return this.m_product;
			}
		}

		private string m_product;
	}
}
