using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class DefaultDllImportSearchPathsAttribute : Attribute
	{
		public DefaultDllImportSearchPathsAttribute(DllImportSearchPath paths)
		{
			this._paths = paths;
		}

		public DllImportSearchPath Paths
		{
			get
			{
				return this._paths;
			}
		}

		internal DllImportSearchPath _paths;
	}
}
