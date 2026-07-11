using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	public sealed class TypeLibImportClassAttribute : Attribute
	{
		public TypeLibImportClassAttribute(Type importClass)
		{
			this._importClass = importClass.ToString();
		}

		public string Value
		{
			get
			{
				return this._importClass;
			}
		}

		private string _importClass;
	}
}
