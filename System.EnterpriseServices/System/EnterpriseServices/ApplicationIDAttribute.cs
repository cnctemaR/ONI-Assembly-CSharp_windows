using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	public sealed class ApplicationIDAttribute : Attribute, IConfigurationAttribute
	{
		public ApplicationIDAttribute(string guid)
		{
			this.guid = new Guid(guid);
		}

		bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
		{
			return false;
		}

		bool IConfigurationAttribute.Apply(Hashtable cache)
		{
			return false;
		}

		bool IConfigurationAttribute.IsValidTarget(string s)
		{
			return s == "Application";
		}

		public Guid Value
		{
			get
			{
				return this.guid;
			}
		}

		private Guid guid;
	}
}
