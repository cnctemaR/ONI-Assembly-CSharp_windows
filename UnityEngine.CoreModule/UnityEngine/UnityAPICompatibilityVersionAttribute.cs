using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class UnityAPICompatibilityVersionAttribute : Attribute
	{
		public UnityAPICompatibilityVersionAttribute(string version)
		{
			this._version = version;
		}

		public string version
		{
			get
			{
				return this._version;
			}
		}

		private string _version;
	}
}
