using System;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace System.Configuration
{
	public class LocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		public LocalFileSettingsProvider()
		{
			this.impl = new CustomizableFileSettingsProvider();
		}

		[MonoTODO]
		[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery)]
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			return this.impl.GetPreviousVersion(context, property);
		}

		[MonoTODO]
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			return this.impl.GetPropertyValues(context, properties);
		}

		public override void Initialize(string name, NameValueCollection values)
		{
			if (name == null)
			{
				name = "LocalFileSettingsProvider";
			}
			if (values != null)
			{
				this.impl.ApplicationName = values["applicationName"];
			}
			base.Initialize(name, values);
		}

		[MonoTODO]
		public void Reset(SettingsContext context)
		{
			this.impl.Reset(context);
		}

		[MonoTODO]
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			this.impl.SetPropertyValues(context, values);
		}

		[MonoTODO]
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			this.impl.Upgrade(context, properties);
		}

		public override string ApplicationName
		{
			get
			{
				return this.impl.ApplicationName;
			}
			set
			{
				this.impl.ApplicationName = value;
			}
		}

		private CustomizableFileSettingsProvider impl;
	}
}
