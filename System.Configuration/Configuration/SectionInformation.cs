using System;

namespace System.Configuration
{
	public sealed class SectionInformation
	{
		[MonoTODO("default value for require_permission")]
		internal SectionInformation()
		{
			this.allow_definition = ConfigurationAllowDefinition.Everywhere;
			this.allow_location = true;
			this.allow_override = true;
			this.inherit_on_child_apps = true;
			this.restart_on_external_changes = true;
		}

		internal string ConfigFilePath { get; set; }

		public ConfigurationAllowDefinition AllowDefinition
		{
			get
			{
				return this.allow_definition;
			}
			set
			{
				this.allow_definition = value;
			}
		}

		public ConfigurationAllowExeDefinition AllowExeDefinition
		{
			get
			{
				return this.allow_exe_definition;
			}
			set
			{
				this.allow_exe_definition = value;
			}
		}

		public bool AllowLocation
		{
			get
			{
				return this.allow_location;
			}
			set
			{
				this.allow_location = value;
			}
		}

		public bool AllowOverride
		{
			get
			{
				return this.allow_override;
			}
			set
			{
				this.allow_override = value;
			}
		}

		public string ConfigSource
		{
			get
			{
				return this.config_source;
			}
			set
			{
				this.config_source = value;
			}
		}

		public bool ForceSave
		{
			get
			{
				return this.force_update;
			}
			set
			{
				this.force_update = value;
			}
		}

		public bool InheritInChildApplications
		{
			get
			{
				return this.inherit_on_child_apps;
			}
			set
			{
				this.inherit_on_child_apps = value;
			}
		}

		[MonoTODO]
		public bool IsDeclarationRequired
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public bool IsDeclared
		{
			get
			{
				return this.is_declared;
			}
		}

		[MonoTODO]
		public bool IsLocked
		{
			get
			{
				return this.is_locked;
			}
		}

		public bool IsProtected
		{
			get
			{
				return this.protection_provider != null;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ProtectedConfigurationProvider ProtectionProvider
		{
			get
			{
				return this.protection_provider;
			}
		}

		[MonoTODO]
		public bool RequirePermission
		{
			get
			{
				return this.require_permission;
			}
			set
			{
				this.require_permission = value;
			}
		}

		[MonoTODO]
		public bool RestartOnExternalChanges
		{
			get
			{
				return this.restart_on_external_changes;
			}
			set
			{
				this.restart_on_external_changes = value;
			}
		}

		[MonoTODO]
		public string SectionName
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type_name;
			}
			set
			{
				this.type_name = value;
			}
		}

		public ConfigurationSection GetParentSection()
		{
			return this.parent;
		}

		internal void SetParentSection(ConfigurationSection parent)
		{
			this.parent = parent;
		}

		public string GetRawXml()
		{
			return this.raw_xml;
		}

		public void ProtectSection(string provider)
		{
			this.protection_provider = ProtectedConfiguration.GetProvider(provider, true);
		}

		[MonoTODO]
		public void ForceDeclaration(bool require)
		{
		}

		public void ForceDeclaration()
		{
			this.ForceDeclaration(true);
		}

		[MonoTODO]
		public void RevertToParent()
		{
			throw new NotImplementedException();
		}

		public void UnprotectSection()
		{
			this.protection_provider = null;
		}

		public void SetRawXml(string xml)
		{
			this.raw_xml = xml;
		}

		[MonoTODO]
		internal void SetName(string name)
		{
			this.name = name;
		}

		private ConfigurationSection parent;

		private ConfigurationAllowDefinition allow_definition = ConfigurationAllowDefinition.Everywhere;

		private ConfigurationAllowExeDefinition allow_exe_definition = ConfigurationAllowExeDefinition.MachineToApplication;

		private bool allow_location;

		private bool allow_override;

		private bool inherit_on_child_apps;

		private bool restart_on_external_changes;

		private bool require_permission;

		private string config_source;

		private bool force_update;

		private bool is_declared;

		private bool is_locked;

		private string name;

		private string type_name;

		private string raw_xml;

		private ProtectedConfigurationProvider protection_provider;
	}
}
