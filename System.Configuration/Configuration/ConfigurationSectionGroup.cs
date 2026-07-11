using System;

namespace System.Configuration
{
	public class ConfigurationSectionGroup
	{
		private Configuration Config
		{
			get
			{
				if (this.config == null)
				{
					throw new InvalidOperationException("ConfigurationSectionGroup cannot be edited until it is added to a Configuration instance as its descendant");
				}
				return this.config;
			}
		}

		internal void Initialize(Configuration config, SectionGroupInfo group)
		{
			if (this.initialized)
			{
				throw new SystemException("INTERNAL ERROR: this configuration section is being initialized twice: " + base.GetType());
			}
			this.initialized = true;
			this.config = config;
			this.group = group;
		}

		internal void SetName(string name)
		{
			this.name = name;
		}

		[MonoTODO]
		public void ForceDeclaration(bool force)
		{
			this.require_declaration = force;
		}

		public void ForceDeclaration()
		{
			this.ForceDeclaration(true);
		}

		[MonoTODO]
		public bool IsDeclared
		{
			get
			{
				return false;
			}
		}

		[MonoTODO]
		public bool IsDeclarationRequired
		{
			get
			{
				return this.require_declaration;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		[MonoInternalNote("Check if this is correct")]
		public string SectionGroupName
		{
			get
			{
				return this.group.XPath;
			}
		}

		public ConfigurationSectionGroupCollection SectionGroups
		{
			get
			{
				if (this.groups == null)
				{
					this.groups = new ConfigurationSectionGroupCollection(this.Config, this.group);
				}
				return this.groups;
			}
		}

		public ConfigurationSectionCollection Sections
		{
			get
			{
				if (this.sections == null)
				{
					this.sections = new ConfigurationSectionCollection(this.Config, this.group);
				}
				return this.sections;
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

		private bool require_declaration;

		private string name;

		private string type_name;

		private ConfigurationSectionCollection sections;

		private ConfigurationSectionGroupCollection groups;

		private Configuration config;

		private SectionGroupInfo group;

		private bool initialized;
	}
}
