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
			this.name = product;
		}

		public string Product
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
