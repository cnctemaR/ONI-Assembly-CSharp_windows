using System;
using System.Xml;

namespace System.Configuration
{
	internal class SectionGroupInfo : ConfigInfo
	{
		public SectionGroupInfo()
		{
			this.Type = typeof(ConfigurationSectionGroup);
		}

		public SectionGroupInfo(string groupName, string typeName)
		{
			this.Name = groupName;
			this.TypeName = typeName;
		}

		public void AddChild(ConfigInfo data)
		{
			data.Parent = this;
			if (data is SectionInfo)
			{
				if (this.sections == null)
				{
					this.sections = new ConfigInfoCollection();
				}
				this.sections[data.Name] = data;
			}
			else
			{
				if (this.groups == null)
				{
					this.groups = new ConfigInfoCollection();
				}
				this.groups[data.Name] = data;
			}
		}

		public void Clear()
		{
			if (this.sections != null)
			{
				this.sections.Clear();
			}
			if (this.groups != null)
			{
				this.groups.Clear();
			}
		}

		public bool HasChild(string name)
		{
			return (this.sections != null && this.sections[name] != null) || (this.groups != null && this.groups[name] != null);
		}

		public void RemoveChild(string name)
		{
			if (this.sections != null)
			{
				this.sections.Remove(name);
			}
			if (this.groups != null)
			{
				this.groups.Remove(name);
			}
		}

		public SectionInfo GetChildSection(string name)
		{
			if (this.sections != null)
			{
				return this.sections[name] as SectionInfo;
			}
			return null;
		}

		public SectionGroupInfo GetChildGroup(string name)
		{
			if (this.groups != null)
			{
				return this.groups[name] as SectionGroupInfo;
			}
			return null;
		}

		public ConfigInfoCollection Sections
		{
			get
			{
				if (this.sections == null)
				{
					return SectionGroupInfo.emptyList;
				}
				return this.sections;
			}
		}

		public ConfigInfoCollection Groups
		{
			get
			{
				if (this.groups == null)
				{
					return SectionGroupInfo.emptyList;
				}
				return this.groups;
			}
		}

		public override bool HasDataContent(Configuration config)
		{
			foreach (ConfigInfoCollection configInfoCollection in new object[] { this.Sections, this.Groups })
			{
				foreach (object obj in configInfoCollection)
				{
					string text = (string)obj;
					ConfigInfo configInfo = configInfoCollection[text];
					if (configInfo.HasDataContent(config))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool HasConfigContent(Configuration cfg)
		{
			if (base.StreamName == cfg.FileName)
			{
				return true;
			}
			foreach (ConfigInfoCollection configInfoCollection in new object[] { this.Sections, this.Groups })
			{
				foreach (object obj in configInfoCollection)
				{
					string text = (string)obj;
					ConfigInfo configInfo = configInfoCollection[text];
					if (configInfo.HasConfigContent(cfg))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void ReadConfig(Configuration cfg, string streamName, XmlReader reader)
		{
			base.StreamName = streamName;
			this.ConfigHost = cfg.ConfigHost;
			if (reader.LocalName != "configSections")
			{
				while (reader.MoveToNextAttribute())
				{
					if (reader.Name == "name")
					{
						this.Name = reader.Value;
					}
					else if (reader.Name == "type")
					{
						this.TypeName = reader.Value;
						this.Type = null;
					}
					else
					{
						base.ThrowException("Unrecognized attribute", reader);
					}
				}
				if (this.Name == null)
				{
					base.ThrowException("sectionGroup must have a 'name' attribute", reader);
				}
				if (this.Name == "location")
				{
					base.ThrowException("location is a reserved section name", reader);
				}
			}
			if (this.TypeName == null)
			{
				this.TypeName = "System.Configuration.ConfigurationSectionGroup";
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Skip();
				}
				else
				{
					string localName = reader.LocalName;
					ConfigInfo configInfo = null;
					if (localName == "remove")
					{
						this.ReadRemoveSection(reader);
					}
					else if (localName == "clear")
					{
						if (reader.HasAttributes)
						{
							base.ThrowException("Unrecognized attribute.", reader);
						}
						this.Clear();
						reader.Skip();
					}
					else
					{
						if (localName == "section")
						{
							configInfo = new SectionInfo();
						}
						else if (localName == "sectionGroup")
						{
							configInfo = new SectionGroupInfo();
						}
						else
						{
							base.ThrowException("Unrecognized element: " + reader.Name, reader);
						}
						configInfo.ReadConfig(cfg, streamName, reader);
						ConfigInfo configInfo2 = this.Groups[configInfo.Name];
						if (configInfo2 == null)
						{
							configInfo2 = this.Sections[configInfo.Name];
						}
						if (configInfo2 != null)
						{
							if (configInfo2.GetType() != configInfo.GetType())
							{
								base.ThrowException("A section or section group named '" + configInfo.Name + "' already exists", reader);
							}
							configInfo2.Merge(configInfo);
							configInfo2.StreamName = streamName;
						}
						else
						{
							this.AddChild(configInfo);
						}
					}
				}
			}
			reader.ReadEndElement();
		}

		public override void WriteConfig(Configuration cfg, XmlWriter writer, ConfigurationSaveMode mode)
		{
			if (this.Name != null)
			{
				writer.WriteStartElement("sectionGroup");
				writer.WriteAttributeString("name", this.Name);
				if (this.TypeName != null && this.TypeName != string.Empty && this.TypeName != "System.Configuration.ConfigurationSectionGroup")
				{
					writer.WriteAttributeString("type", this.TypeName);
				}
			}
			else
			{
				writer.WriteStartElement("configSections");
			}
			foreach (ConfigInfoCollection configInfoCollection in new object[] { this.Sections, this.Groups })
			{
				foreach (object obj in configInfoCollection)
				{
					string text = (string)obj;
					ConfigInfo configInfo = configInfoCollection[text];
					if (configInfo.HasConfigContent(cfg))
					{
						configInfo.WriteConfig(cfg, writer, mode);
					}
				}
			}
			writer.WriteEndElement();
		}

		private void ReadRemoveSection(XmlReader reader)
		{
			if (!reader.MoveToNextAttribute() || reader.Name != "name")
			{
				base.ThrowException("Unrecognized attribute.", reader);
			}
			string value = reader.Value;
			if (string.IsNullOrEmpty(value))
			{
				base.ThrowException("Empty name to remove", reader);
			}
			reader.MoveToElement();
			if (!this.HasChild(value))
			{
				base.ThrowException("No factory for " + value, reader);
			}
			this.RemoveChild(value);
			reader.Skip();
		}

		public void ReadRootData(XmlReader reader, Configuration config, bool overrideAllowed)
		{
			reader.MoveToContent();
			this.ReadContent(reader, config, overrideAllowed, true);
		}

		public override void ReadData(Configuration config, XmlReader reader, bool overrideAllowed)
		{
			reader.MoveToContent();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				this.ReadContent(reader, config, overrideAllowed, false);
				reader.MoveToContent();
				reader.ReadEndElement();
			}
			else
			{
				reader.Read();
			}
		}

		private void ReadContent(XmlReader reader, Configuration config, bool overrideAllowed, bool root)
		{
			while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Skip();
				}
				else if (reader.LocalName == "dllmap")
				{
					reader.Skip();
				}
				else if (reader.LocalName == "location")
				{
					if (!root)
					{
						base.ThrowException("<location> elements are only allowed in <configuration> elements.", reader);
					}
					string attribute = reader.GetAttribute("allowOverride");
					bool flag = attribute == null || attribute.Length == 0 || bool.Parse(attribute);
					string attribute2 = reader.GetAttribute("path");
					if (attribute2 != null && attribute2.Length > 0)
					{
						string text = reader.ReadOuterXml();
						string[] array = attribute2.Split(new char[] { ',' });
						foreach (string text2 in array)
						{
							string text3 = text2.Trim();
							if (config.Locations.Find(text3) != null)
							{
								base.ThrowException("Sections must only appear once per config file.", reader);
							}
							ConfigurationLocation configurationLocation = new ConfigurationLocation(text3, text, config, flag);
							config.Locations.Add(configurationLocation);
						}
					}
					else
					{
						this.ReadData(config, reader, flag);
					}
				}
				else
				{
					ConfigInfo configInfo = this.GetConfigInfo(reader, this);
					if (configInfo != null)
					{
						configInfo.ReadData(config, reader, overrideAllowed);
					}
					else
					{
						base.ThrowException("Unrecognized configuration section <" + reader.LocalName + ">", reader);
					}
				}
			}
		}

		private ConfigInfo GetConfigInfo(XmlReader reader, SectionGroupInfo current)
		{
			ConfigInfo configInfo = null;
			if (current.sections != null)
			{
				configInfo = current.sections[reader.LocalName];
			}
			if (configInfo != null)
			{
				return configInfo;
			}
			if (current.groups != null)
			{
				configInfo = current.groups[reader.LocalName];
			}
			if (configInfo != null)
			{
				return configInfo;
			}
			if (current.groups == null)
			{
				return null;
			}
			foreach (object obj in current.groups.AllKeys)
			{
				string text = (string)obj;
				configInfo = this.GetConfigInfo(reader, (SectionGroupInfo)current.groups[text]);
				if (configInfo != null)
				{
					return configInfo;
				}
			}
			return null;
		}

		internal override void Merge(ConfigInfo newData)
		{
			SectionGroupInfo sectionGroupInfo = newData as SectionGroupInfo;
			if (sectionGroupInfo == null)
			{
				return;
			}
			if (sectionGroupInfo.sections != null && sectionGroupInfo.sections.Count > 0)
			{
				foreach (object obj in sectionGroupInfo.sections.AllKeys)
				{
					string text = (string)obj;
					ConfigInfo configInfo = this.sections[text];
					if (configInfo == null)
					{
						this.sections.Add(text, sectionGroupInfo.sections[text]);
					}
				}
			}
			if (sectionGroupInfo.groups != null && sectionGroupInfo.sections != null && sectionGroupInfo.sections.Count > 0)
			{
				foreach (object obj2 in sectionGroupInfo.groups.AllKeys)
				{
					string text2 = (string)obj2;
					ConfigInfo configInfo = this.groups[text2];
					if (configInfo == null)
					{
						this.groups.Add(text2, sectionGroupInfo.groups[text2]);
					}
				}
			}
		}

		public void WriteRootData(XmlWriter writer, Configuration config, ConfigurationSaveMode mode)
		{
			this.WriteContent(writer, config, mode, false);
		}

		public override void WriteData(Configuration config, XmlWriter writer, ConfigurationSaveMode mode)
		{
			writer.WriteStartElement(this.Name);
			this.WriteContent(writer, config, mode, true);
			writer.WriteEndElement();
		}

		public void WriteContent(XmlWriter writer, Configuration config, ConfigurationSaveMode mode, bool writeElem)
		{
			foreach (ConfigInfoCollection configInfoCollection in new object[] { this.Sections, this.Groups })
			{
				foreach (object obj in configInfoCollection)
				{
					string text = (string)obj;
					ConfigInfo configInfo = configInfoCollection[text];
					if (configInfo.HasDataContent(config))
					{
						configInfo.WriteData(config, writer, mode);
					}
				}
			}
		}

		private ConfigInfoCollection sections;

		private ConfigInfoCollection groups;

		private static ConfigInfoCollection emptyList = new ConfigInfoCollection();
	}
}
