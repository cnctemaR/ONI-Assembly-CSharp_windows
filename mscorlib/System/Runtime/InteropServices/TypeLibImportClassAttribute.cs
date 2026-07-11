using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	public sealed class TypeLibImportClassAttribute : Attribute
	{
		public TypeLibImportClassAttribute(Type importClass)
		{
			this._importClassName = importClass.ToString();
		}

		public string Value
		{
			get
			{
				return this._importClassName;
			}
		}

		internal string _importClassName;
	}
}
