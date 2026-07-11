using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CngKeyCreationParameters
	{
		public CngExportPolicies? ExportPolicy
		{
			get
			{
				return this.m_exportPolicy;
			}
			set
			{
				this.m_exportPolicy = value;
			}
		}

		public CngKeyCreationOptions KeyCreationOptions
		{
			get
			{
				return this.m_keyCreationOptions;
			}
			set
			{
				this.m_keyCreationOptions = value;
			}
		}

		public CngKeyUsages? KeyUsage
		{
			get
			{
				return this.m_keyUsage;
			}
			set
			{
				this.m_keyUsage = value;
			}
		}

		public IntPtr ParentWindowHandle
		{
			get
			{
				return this.m_parentWindowHandle;
			}
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
			set
			{
				this.m_parentWindowHandle = value;
			}
		}

		public CngPropertyCollection Parameters
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
			get
			{
				return this.m_parameters;
			}
		}

		internal CngPropertyCollection ParametersNoDemand
		{
			get
			{
				return this.m_parameters;
			}
		}

		public CngProvider Provider
		{
			get
			{
				return this.m_provider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_provider = value;
			}
		}

		public CngUIPolicy UIPolicy
		{
			get
			{
				return this.m_uiPolicy;
			}
			[SecuritySafeCritical]
			[HostProtection(SecurityAction.LinkDemand, UI = true)]
			[UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.SafeSubWindows)]
			set
			{
				this.m_uiPolicy = value;
			}
		}

		private CngExportPolicies? m_exportPolicy;

		private CngKeyCreationOptions m_keyCreationOptions;

		private CngKeyUsages? m_keyUsage;

		private CngPropertyCollection m_parameters = new CngPropertyCollection();

		private IntPtr m_parentWindowHandle;

		private CngProvider m_provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;

		private CngUIPolicy m_uiPolicy;
	}
}
