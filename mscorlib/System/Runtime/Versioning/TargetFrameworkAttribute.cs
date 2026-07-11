using System;

namespace System.Runtime.Versioning
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class TargetFrameworkAttribute : Attribute
	{
		public TargetFrameworkAttribute(string frameworkName)
		{
			if (frameworkName == null)
			{
				throw new ArgumentNullException("frameworkName");
			}
			this._frameworkName = frameworkName;
		}

		public string FrameworkName
		{
			get
			{
				return this._frameworkName;
			}
		}

		public string FrameworkDisplayName
		{
			get
			{
				return this._frameworkDisplayName;
			}
			set
			{
				this._frameworkDisplayName = value;
			}
		}

		private string _frameworkName;

		private string _frameworkDisplayName;
	}
}
