using System;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class SatelliteContractVersionAttribute : Attribute
	{
		public SatelliteContractVersionAttribute(string version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._version = version;
		}

		public string Version
		{
			get
			{
				return this._version;
			}
		}

		private string _version;
	}
}
