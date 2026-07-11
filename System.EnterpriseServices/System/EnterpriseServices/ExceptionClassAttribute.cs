using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
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
