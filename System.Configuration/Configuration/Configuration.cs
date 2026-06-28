using System;
using System.Collections;
using System.Configuration.Internal;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	public sealed class Configuration
	{
		internal Configuration(Configuration parent, string locationSubPath)
		{
			this.parent = parent;
			this.system = parent.system;
			this.rootGroup = parent.rootGroup;
			this.locationSubPath = locationSubPath;
			this.configPath = parent.ConfigPath;
		}

		internal Configuration(InternalConfigurationSystem system, string locationSubPath)
		{
			this.hasFile = true;
			this.system = system;
			system.InitForConfiguration(ref locationSubPath, out this.configPath, out this.locationConfigPath);
			Configuration configuration = null;
			if (locationSubPath != null)
			{
				configuration = new Configuration(system, locationSubPath);
				if (this.locationConfigPath != null)
				{
					configuration = configuration.FindLocationConfiguration(this.locationConfigPath, configuration);
				}
			}
			this.Init(system, this.configPath, configuration);
		}

		internal static event ConfigurationSaveEventHandler SaveStart;

		internal static event ConfigurationSaveEventHandler SaveEnd;

		internal Configuration FindLocationConfiguration(string relativePath, Configuration defaultConfiguration)
		{
			Configuration configuration = defaultConfiguration;
			if (!string.IsNullOrEmpty(this.LocationConfigPath))
			{
				Configuration parentWithFile = this.GetParentWithFile();
				if (parentWithFile != null)
				{
					string configPathFromLocationSubPath = this.system.Host.GetConfigPathFromLocationSubPath(this.configPath, relativePath);
					configuration = parentWithFile.FindLocationConfiguration(configPathFromLocationSubPath, defaultConfiguration);
				}
			}
			string text = this.configPath.Substring(1) + "/";
			if (relativePath.StartsWith(text, StringComparison.Ordinal))
			{
				relativePath = relativePath.Substring(text.Length);
			}
			ConfigurationLocation configurationLocation = this.Locations.Find(relativePath);
			if (configurationLocation == null)
			{
				return configuration;
			}
			configurationLocation.SetParentConfiguration(configuration);
			return configurationLocation.OpenConfiguration();
		}

		internal void Init(IConfigSystem system, string configPath, Configuration parent)
		{
			this.system = system;
			this.configPath = configPath;
			this.streamName = system.Host.GetStreamName(configPath);
			this.parent = parent;
			if (parent != null)
			{
				this.rootGroup = parent.rootGroup;
			}
			else
			{
				this.rootGroup = new SectionGroupInfo();
				this.rootGroup.StreamName = this.streamName;
			}
			if (this.streamName != null)
			{
				this.Load();
			}
		}

		internal Configuration Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		internal Configuration GetParentWithFile()
		{
			Configuration configuration = this.Parent;
			while (configuration != null && !configuration.HasFile)
			{
				configuration = configuration.Parent;
			}
			return configuration;
		}

		internal string FileName
		{
			get
			{
				return this.streamName;
			}
		}

		internal IInternalConfigHost ConfigHost
		{
			get
			{
				return this.system.Host;
			}
		}

		internal string LocationConfigPath
		{
			get
			{
				return this.locationConfigPath;
			}
		}

		internal string GetLocationSubPath()
		{
			Configuration configuration = this.parent;
			string text = null;
			while (configuration != null)
			{
				text = configuration.locationSubPath;
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				configuration = configuration.parent;
			}
			return text;
		}

		internal string ConfigPath
		{
			get
			{
				return this.configPath;
			}
		}

		public AppSettingsSection AppSettings
		{
			get
			{
				return (AppSettingsSection)this.GetSection("appSettings");
			}
		}

		public ConnectionStringsSection ConnectionStrings
		{
			get
			{
				return (ConnectionStringsSection)this.GetSection("connectionStrings");
			}
		}

		public string FilePath
		{
			get
			{
				if (this.streamName == null && this.parent != null)
				{
					return this.parent.FilePath;
				}
				return this.streamName;
			}
		}

		public bool HasFile
		{
			get
			{
				return this.hasFile;
			}
		}

		public ContextInformation EvaluationContext
		{
			get
			{
				if (this.evaluationContext == null)
				{
					object obj = this.system.Host.CreateConfigurationContext(this.configPath, this.GetLocationSubPath());
					this.evaluationContext = new ContextInformation(this, obj);
				}
				return this.evaluationContext;
			}
		}

		public ConfigurationLocationCollection Locations
		{
			get
			{
				if (this.locations == null)
				{
					this.locations = new ConfigurationLocationCollection();
				}
				return this.locations;
			}
		}

		public bool NamespaceDeclared
		{
			get
			{
				return this.rootNamespace != null;
			}
			set
			{
				this.rootNamespace = ((!value) ? null : "http://schemas.microsoft.com/.NetConfiguration/v2.0");
			}
		}

		public ConfigurationSectionGroup RootSectionGroup
		{
			get
			{
				if (this.rootSectionGroup == null)
				{
					this.rootSectionGroup = new ConfigurationSectionGroup();
					this.rootSectionGroup.Initialize(this, this.rootGroup);
				}
				return this.rootSectionGroup;
			}
		}

		public ConfigurationSectionGroupCollection SectionGroups
		{
			get
			{
				return this.RootSectionGroup.SectionGroups;
			}
		}

		public ConfigurationSectionCollection Sections
		{
			get
			{
				return this.RootSectionGroup.Sections;
			}
		}

		public ConfigurationSection GetSection(string path)
		{
			string[] array = path.Split(new char[] { '/' });
			if (array.Length == 1)
			{
				return this.Sections[array[0]];
			}
			ConfigurationSectionGroup configurationSectionGroup = this.SectionGroups[array[0]];
			int num = 1;
			while (configurationSectionGroup != null && num < array.Length - 1)
			{
				configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
				num++;
			}
			if (configurationSectionGroup != null)
			{
				return configurationSectionGroup.Sections[array[array.Length - 1]];
			}
			return null;
		}

		public ConfigurationSectionGroup GetSectionGroup(string path)
		{
			string[] array = path.Split(new char[] { '/' });
			ConfigurationSectionGroup configurationSectionGroup = this.SectionGroups[array[0]];
			int num = 1;
			while (configurationSectionGroup != null && num < array.Length)
			{
				configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
				num++;
			}
			return configurationSectionGroup;
		}

		internal ConfigurationSection GetSectionInstance(SectionInfo config, bool createDefaultInstance)
		{
			object obj = this.elementData[config];
			ConfigurationSection configurationSection = obj as ConfigurationSection;
			if (configurationSection != null || !createDefaultInstance)
			{
				return configurationSection;
			}
			object obj2 = config.CreateInstance();
			configurationSection = obj2 as ConfigurationSection;
			if (configurationSection == null)
			{
				configurationSection = new DefaultSection
				{
					SectionHandler = (obj2 as IConfigurationSectionHandler)
				};
			}
			configurationSection.Configuration = this;
			ConfigurationSection configurationSection2 = null;
			if (this.parent != null)
			{
				configurationSection2 = this.parent.GetSectionInstance(config, true);
				configurationSection.SectionInformation.SetParentSection(configurationSection2);
			}
			configurationSection.SectionInformation.ConfigFilePath = this.FilePath;
			configurationSection.ConfigContext = this.system.Host.CreateDeprecatedConfigContext(this.configPath);
			string text = obj as string;
			configurationSection.RawXml = text;
			configurationSection.Reset(configurationSection2);
			if (text != null && text == obj)
			{
				XmlTextReader xmlTextReader = new ConfigXmlTextReader(new StringReader(text), this.FilePath);
				configurationSection.DeserializeSection(xmlTextReader);
				xmlTextReader.Close();
				if (!string.IsNullOrEmpty(configurationSection.SectionInformation.ConfigSource) && !string.IsNullOrEmpty(this.FilePath))
				{
					configurationSection.DeserializeConfigSource(Path.GetDirectoryName(this.FilePath));
				}
			}
			this.elementData[config] = configurationSection;
			return configurationSection;
		}

		internal ConfigurationSectionGroup GetSectionGroupInstance(SectionGroupInfo group)
		{
			ConfigurationSectionGroup configurationSectionGroup = group.CreateInstance() as ConfigurationSectionGroup;
			if (configurationSectionGroup != null)
			{
				configurationSectionGroup.Initialize(this, group);
			}
			return configurationSectionGroup;
		}

		internal void SetConfigurationSection(SectionInfo config, ConfigurationSection sec)
		{
			this.elementData[config] = sec;
		}

		internal void SetSectionXml(SectionInfo config, string data)
		{
			this.elementData[config] = data;
		}

		internal string GetSectionXml(SectionInfo config)
		{
			return this.elementData[config] as string;
		}

		internal void CreateSection(SectionGroupInfo group, string name, ConfigurationSection sec)
		{
			if (group.HasChild(name))
			{
				throw new ConfigurationException("Cannot add a ConfigurationSection. A section or section group already exists with the name '" + name + "'");
			}
			if (!this.HasFile && !sec.SectionInformation.AllowLocation)
			{
				throw new ConfigurationErrorsException("The configuration section <" + name + "> cannot be defined inside a <location> element.");
			}
			if (!this.system.Host.IsDefinitionAllowed(this.configPath, sec.SectionInformation.AllowDefinition, sec.SectionInformation.AllowExeDefinition))
			{
				object obj = ((sec.SectionInformation.AllowExeDefinition == ConfigurationAllowExeDefinition.MachineToApplication) ? sec.SectionInformation.AllowDefinition : sec.SectionInformation.AllowExeDefinition);
				throw new ConfigurationErrorsException(string.Concat(new object[] { "The section <", name, "> can't be defined in this configuration file (the allowed definition context is '", obj, "')." }));
			}
			if (sec.SectionInformation.Type == null)
			{
				sec.SectionInformation.Type = this.system.Host.GetConfigTypeName(sec.GetType());
			}
			SectionInfo sectionInfo = new SectionInfo(name, sec.SectionInformation);
			sectionInfo.StreamName = this.streamName;
			sectionInfo.ConfigHost = this.system.Host;
			group.AddChild(sectionInfo);
			this.elementData[sectionInfo] = sec;
		}

		internal void CreateSectionGroup(SectionGroupInfo parentGroup, string name, ConfigurationSectionGroup sec)
		{
			if (parentGroup.HasChild(name))
			{
				throw new ConfigurationException("Cannot add a ConfigurationSectionGroup. A section or section group already exists with the name '" + name + "'");
			}
			if (sec.Type == null)
			{
				sec.Type = this.system.Host.GetConfigTypeName(sec.GetType());
			}
			sec.SetName(name);
			SectionGroupInfo sectionGroupInfo = new SectionGroupInfo(name, sec.Type);
			sectionGroupInfo.StreamName = this.streamName;
			sectionGroupInfo.ConfigHost = this.system.Host;
			parentGroup.AddChild(sectionGroupInfo);
			this.elementData[sectionGroupInfo] = sec;
			sec.Initialize(this, sectionGroupInfo);
		}

		internal void RemoveConfigInfo(ConfigInfo config)
		{
			this.elementData.Remove(config);
		}

		public void Save()
		{
			this.Save(ConfigurationSaveMode.Modified, false);
		}

		public void Save(ConfigurationSaveMode mode)
		{
			this.Save(mode, false);
		}

		public void Save(ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			ConfigurationSaveEventHandler saveStart = Configuration.SaveStart;
			ConfigurationSaveEventHandler saveEnd = Configuration.SaveEnd;
			object obj = null;
			Exception ex = null;
			Stream stream = this.system.Host.OpenStreamForWrite(this.streamName, null, ref obj);
			try
			{
				if (saveStart != null)
				{
					saveStart(this, new ConfigurationSaveEventArgs(this.streamName, true, null, obj));
				}
				this.Save(stream, mode, forceUpdateAll);
				this.system.Host.WriteCompleted(this.streamName, true, obj);
			}
			catch (Exception ex2)
			{
				ex = ex2;
				this.system.Host.WriteCompleted(this.streamName, false, obj);
				throw;
			}
			finally
			{
				stream.Close();
				if (saveEnd != null)
				{
					saveEnd(this, new ConfigurationSaveEventArgs(this.streamName, false, ex, obj));
				}
			}
		}

		public void SaveAs(string filename)
		{
			this.SaveAs(filename, ConfigurationSaveMode.Modified, false);
		}

		public void SaveAs(string filename, ConfigurationSaveMode mode)
		{
			this.SaveAs(filename, mode, false);
		}

		[MonoInternalNote("Detect if file has changed")]
		public void SaveAs(string filename, ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			string directoryName = Path.GetDirectoryName(Path.GetFullPath(filename));
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.Save(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write), mode, forceUpdateAll);
		}

		private void Save(Stream stream, ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StreamWriter(stream));
			xmlTextWriter.Formatting = Formatting.Indented;
			try
			{
				xmlTextWriter.WriteStartDocument();
				if (this.rootNamespace != null)
				{
					xmlTextWriter.WriteStartElement("configuration", this.rootNamespace);
				}
				else
				{
					xmlTextWriter.WriteStartElement("configuration");
				}
				if (this.rootGroup.HasConfigContent(this))
				{
					this.rootGroup.WriteConfig(this, xmlTextWriter, mode);
				}
				foreach (object obj in this.Locations)
				{
					ConfigurationLocation configurationLocation = (ConfigurationLocation)obj;
					if (configurationLocation.OpenedConfiguration == null)
					{
						xmlTextWriter.WriteRaw("\n");
						xmlTextWriter.WriteRaw(configurationLocation.XmlContent);
					}
					else
					{
						xmlTextWriter.WriteStartElement("location");
						xmlTextWriter.WriteAttributeString("path", configurationLocation.Path);
						if (!configurationLocation.AllowOverride)
						{
							xmlTextWriter.WriteAttributeString("allowOverride", "false");
						}
						configurationLocation.OpenedConfiguration.SaveData(xmlTextWriter, mode, forceUpdateAll);
						xmlTextWriter.WriteEndElement();
					}
				}
				this.SaveData(xmlTextWriter, mode, forceUpdateAll);
				xmlTextWriter.WriteEndElement();
			}
			finally
			{
				xmlTextWriter.Flush();
				xmlTextWriter.Close();
			}
		}

		private void SaveData(XmlTextWriter tw, ConfigurationSaveMode mode, bool forceUpdateAll)
		{
			this.rootGroup.WriteRootData(tw, this, mode);
		}

		private bool Load()
		{
			if (string.IsNullOrEmpty(this.streamName))
			{
				return true;
			}
			Stream stream = null;
			try
			{
				stream = (stream = this.system.Host.OpenStreamForRead(this.streamName));
			}
			catch
			{
				return false;
			}
			using (XmlTextReader xmlTextReader = new ConfigXmlTextReader(stream, this.streamName))
			{
				this.ReadConfigFile(xmlTextReader, this.streamName);
			}
			return true;
		}

		private void ReadConfigFile(XmlReader reader, string fileName)
		{
			reader.MoveToContent();
			if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
			{
				this.ThrowException("Configuration file does not have a valid root element", reader);
			}
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					if (reader.LocalName == "xmlns")
					{
						this.rootNamespace = reader.Value;
					}
					else
					{
						this.ThrowException(string.Format("Unrecognized attribute '{0}' in root element", reader.LocalName), reader);
					}
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			if (reader.LocalName == "configSections")
			{
				if (reader.HasAttributes)
				{
					this.ThrowException("Unrecognized attribute in <configSections>.", reader);
				}
				this.rootGroup.ReadConfig(this, fileName, reader);
			}
			this.rootGroup.ReadRootData(reader, this, true);
		}

		internal void ReadData(XmlReader reader, bool allowOverride)
		{
			this.rootGroup.ReadData(this, reader, allowOverride);
		}

		private void ThrowException(string text, XmlReader reader)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			throw new ConfigurationException(text, this.streamName, (xmlLineInfo == null) ? 0 : xmlLineInfo.LineNumber);
		}

		private Configuration parent;

		private Hashtable elementData = new Hashtable();

		private string streamName;

		private ConfigurationSectionGroup rootSectionGroup;

		private ConfigurationLocationCollection locations;

		private SectionGroupInfo rootGroup;

		private IConfigSystem system;

		private bool hasFile;

		private string rootNamespace;

		private string configPath;

		private string locationConfigPath;

		private string locationSubPath;

		private ContextInformation evaluationContext;
	}
}
