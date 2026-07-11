using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	public sealed class ApplicationNameAttribute : Attribute, IConfigurationAttribute
	{
		public ApplicationNameAttribute(string name)
		{
			this.name = name;
		}

		bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
		{
			return false;
		}

		[MonoTODO]
		bool IConfigurationAttribute.Apply(Hashtable cache)
		{
			throw new NotImplementedException();
		}

		bool IConfigurationAttribute.IsValidTarget(string s)
		{
			return s == "Application";
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
