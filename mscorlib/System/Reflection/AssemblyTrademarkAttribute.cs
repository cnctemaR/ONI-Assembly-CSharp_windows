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
			this.name = trademark;
		}

		public string Trademark
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
