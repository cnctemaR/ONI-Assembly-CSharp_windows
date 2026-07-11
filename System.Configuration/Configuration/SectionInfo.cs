using System;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Configuration
{
	internal class SectionInfo : ConfigInfo
	{
		public SectionInfo()
		{
		}

		public SectionInfo(string sectionName, SectionInformation info)
		{
			this.Name = sectionName;
			this.TypeName = info.Type;
			this.allowLocation = info.AllowLocation;
			this.allowDefinition = info.AllowDefinition;
			this.allowExeDefinition = info.AllowExeDefinition;
			this.requirePermission = info.RequirePermission;
			this.restartOnExternalChanges = info.RestartOnExternalChanges;
		}

		public override object CreateInstance()
		{
			object obj = base.CreateInstance();
			ConfigurationSection configurationSection = obj as ConfigurationSection;
			if (configurationSection != null)
			{
				configurationSection.SectionInformation.AllowLocation = this.allowLocation;
				configurationSection.SectionInformation.AllowDefinition = this.allowDefinition;
				configurationSection.SectionInformation.AllowExeDefinition = this.allowExeDefinition;
				configurationSection.SectionInformation.RequirePermission = this.requirePermission;
				configurationSection.SectionInformation.RestartOnExternalChanges = this.restartOnExternalChanges;
				configurationSection.SectionInformation.SetName(this.Name);
			}
			return obj;
		}

		public override bool HasDataContent(Configuration config)
		{
			return config.GetSectionInstance(this, false) != null || config.GetSectionXml(this) != null;
		}

		public override bool HasConfigContent(Configuration cfg)
		{
			return base.StreamName == cfg.FileName;
		}

		public override void ReadConfig(Configuration cfg, string streamName, XmlReader reader)
		{
			base.StreamName = streamName;
			this.ConfigHost = cfg.ConfigHost;
			while (reader.MoveToNextAttribute())
			{
				string name = reader.Name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num <= 1766272347U)
				{
					if (num != 1066839313U)
					{
						if (num != 1361572173U)
						{
							if (num != 1766272347U)
							{
								goto IL_0290;
							}
							if (!(name == "requirePermission"))
							{
								goto IL_0290;
							}
							string value = reader.Value;
							bool flag = value == "true";
							if (!flag && value != "false")
							{
								base.ThrowException("Invalid attribute value", reader);
							}
							this.requirePermission = flag;
							continue;
						}
						else if (!(name == "type"))
						{
							goto IL_0290;
						}
					}
					else
					{
						if (!(name == "allowLocation"))
						{
							goto IL_0290;
						}
						string value2 = reader.Value;
						this.allowLocation = value2 == "true";
						if (!this.allowLocation && value2 != "false")
						{
							base.ThrowException("Invalid attribute value", reader);
							continue;
						}
						continue;
					}
				}
				else
				{
					if (num <= 1931054735U)
					{
						if (num != 1841158919U)
						{
							if (num != 1931054735U)
							{
								goto IL_0290;
							}
							if (!(name == "allowExeDefinition"))
							{
								goto IL_0290;
							}
						}
						else
						{
							if (!(name == "restartOnExternalChanges"))
							{
								goto IL_0290;
							}
							string value3 = reader.Value;
							bool flag2 = value3 == "true";
							if (!flag2 && value3 != "false")
							{
								base.ThrowException("Invalid attribute value", reader);
							}
							this.restartOnExternalChanges = flag2;
							continue;
						}
					}
					else if (num != 2369371622U)
					{
						if (num != 3263379011U)
						{
							goto IL_0290;
						}
						if (!(name == "allowDefinition"))
						{
							goto IL_0290;
						}
						string value4 = reader.Value;
						try
						{
							this.allowDefinition = (ConfigurationAllowDefinition)Enum.Parse(typeof(ConfigurationAllowDefinition), value4);
							continue;
						}
						catch
						{
							base.ThrowException("Invalid attribute value", reader);
							continue;
						}
					}
					else
					{
						if (!(name == "name"))
						{
							goto IL_0290;
						}
						this.Name = reader.Value;
						if (this.Name == "location")
						{
							base.ThrowException("location is a reserved section name", reader);
							continue;
						}
						continue;
					}
					string value5 = reader.Value;
					try
					{
						this.allowExeDefinition = (ConfigurationAllowExeDefinition)Enum.Parse(typeof(ConfigurationAllowExeDefinition), value5);
						continue;
					}
					catch
					{
						base.ThrowException("Invalid attribute value", reader);
						continue;
					}
				}
				this.TypeName = reader.Value;
				continue;
				IL_0290:
				base.ThrowException(string.Format("Unrecognized attribute: {0}", reader.Name), reader);
			}
			if (this.Name == null || this.TypeName == null)
			{
				base.ThrowException("Required attribute missing", reader);
			}
			reader.MoveToElement();
			reader.Skip();
		}

		public override void WriteConfig(Configuration cfg, XmlWriter writer, ConfigurationSaveMode mode)
		{
			writer.WriteStartElement("section");
			writer.WriteAttributeString("name", this.Name);
			writer.WriteAttributeString("type", this.TypeName);
			if (!this.allowLocation)
			{
				writer.WriteAttributeString("allowLocation", "false");
			}
			if (this.allowDefinition != ConfigurationAllowDefinition.Everywhere)
			{
				writer.WriteAttributeString("allowDefinition", this.allowDefinition.ToString());
			}
			if (this.allowExeDefinition != ConfigurationAllowExeDefinition.MachineToApplication)
			{
				writer.WriteAttributeString("allowExeDefinition", this.allowExeDefinition.ToString());
			}
			if (!this.requirePermission)
			{
				writer.WriteAttributeString("requirePermission", "false");
			}
			writer.WriteEndElement();
		}

		public override void ReadData(Configuration config, XmlReader reader, bool overrideAllowed)
		{
			if (!config.HasFile && !this.allowLocation)
			{
				throw new ConfigurationErrorsException("The configuration section <" + this.Name + "> cannot be defined inside a <location> element.", reader);
			}
			if (!config.ConfigHost.IsDefinitionAllowed(config.ConfigPath, this.allowDefinition, this.allowExeDefinition))
			{
				object obj = ((this.allowExeDefinition != ConfigurationAllowExeDefinition.MachineToApplication) ? this.allowExeDefinition : this.allowDefinition);
				throw new ConfigurationErrorsException(string.Concat(new object[] { "The section <", this.Name, "> can't be defined in this configuration file (the allowed definition context is '", obj, "')." }), reader);
			}
			if (config.GetSectionXml(this) != null)
			{
				base.ThrowException("The section <" + this.Name + "> is defined more than once in the same configuration file.", reader);
			}
			config.SetSectionXml(this, reader.ReadOuterXml());
		}

		public override void WriteData(Configuration config, XmlWriter writer, ConfigurationSaveMode mode)
		{
			ConfigurationSection sectionInstance = config.GetSectionInstance(this, false);
			string text;
			if (sectionInstance != null)
			{
				ConfigurationSection configurationSection = ((config.Parent != null) ? config.Parent.GetSectionInstance(this, false) : null);
				text = sectionInstance.SerializeSection(configurationSection, this.Name, mode);
				string externalDataXml = sectionInstance.ExternalDataXml;
				string filePath = config.FilePath;
				if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(externalDataXml))
				{
					using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Path.GetDirectoryName(filePath), sectionInstance.SectionInformation.ConfigSource)))
					{
						streamWriter.Write(externalDataXml);
					}
				}
				if (sectionInstance.SectionInformation.IsProtected)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("<{0} configProtectionProvider=\"{1}\">\n", this.Name, sectionInstance.SectionInformation.ProtectionProvider.Name);
					stringBuilder.Append(config.ConfigHost.EncryptSection(text, sectionInstance.SectionInformation.ProtectionProvider, ProtectedConfiguration.Section));
					stringBuilder.AppendFormat("</{0}>", this.Name);
					text = stringBuilder.ToString();
				}
			}
			else
			{
				text = config.GetSectionXml(this);
			}
			if (!string.IsNullOrEmpty(text))
			{
				writer.WriteRaw(text);
			}
		}

		internal override void Merge(ConfigInfo data)
		{
		}

		internal override bool HasValues(Configuration config, ConfigurationSaveMode mode)
		{
			ConfigurationSection sectionInstance = config.GetSectionInstance(this, false);
			if (sectionInstance == null)
			{
				return false;
			}
			ConfigurationSection configurationSection = ((config.Parent != null) ? config.Parent.GetSectionInstance(this, false) : null);
			return sectionInstance.HasValues(configurationSection, mode);
		}

		internal override void ResetModified(Configuration config)
		{
			ConfigurationSection sectionInstance = config.GetSectionInstance(this, false);
			if (sectionInstance != null)
			{
				sectionInstance.ResetModified();
			}
		}

		private bool allowLocation = true;

		private bool requirePermission = true;

		private bool restartOnExternalChanges;

		private ConfigurationAllowDefinition allowDefinition = ConfigurationAllowDefinition.Everywhere;

		private ConfigurationAllowExeDefinition allowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;
	}
}
