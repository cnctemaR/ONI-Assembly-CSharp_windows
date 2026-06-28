using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(false)]
	public sealed class ApplicationActivationAttribute : Attribute, IConfigurationAttribute
	{
		public ApplicationActivationAttribute(ActivationOption opt)
		{
			this.opt = opt;
		}

		[MonoTODO]
		bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
		{
			throw new NotImplementedException();
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

		public string SoapMailbox
		{
			get
			{
				return this.soapMailbox;
			}
			set
			{
				this.soapMailbox = value;
			}
		}

		public string SoapVRoot
		{
			get
			{
				return this.soapVRoot;
			}
			set
			{
				this.soapVRoot = value;
			}
		}

		public ActivationOption Value
		{
			get
			{
				return this.opt;
			}
		}

		private ActivationOption opt;

		private string soapMailbox;

		private string soapVRoot;
	}
}
