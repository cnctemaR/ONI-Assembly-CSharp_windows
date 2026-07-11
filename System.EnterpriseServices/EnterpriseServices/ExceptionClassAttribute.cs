using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExceptionClassAttribute : Attribute
	{
		public ExceptionClassAttribute(string name)
		{
			this.name = name;
		}

		public string Value
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
