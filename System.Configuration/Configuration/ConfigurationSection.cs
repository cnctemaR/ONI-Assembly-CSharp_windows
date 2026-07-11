using System;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	public abstract class ConfigurationSection : ConfigurationElement
	{
		internal string ExternalDataXml
		{
			get
			{
				return this.externalDataXml;
			}
		}

		internal IConfigurationSectionHandler SectionHandler
		{
			get
			{
				return this.section_handler;
			}
			set
			{
				this.section_handler = value;
			}
		}

		[MonoTODO]
		public SectionInformation SectionInformation
		{
			get
			{
				if (this.sectionInformation == null)
				{
					this.sectionInformation = new SectionInformation();
				}
				return this.sectionInformation;
			}
		}

		internal object ConfigContext
		{
			get
			{
				return this._configContext;
			}
			set
			{
				this._configContext = value;
			}
		}

		[MonoTODO("Provide ConfigContext. Likely the culprit of bug #322493")]
		protected internal virtual object GetRuntimeObject()
		{
			if (this.SectionHandler == null)
			{
				return this;
			}
			ConfigurationSection configurationSection = ((this.sectionInformation != null) ? this.sectionInformation.GetParentSection() : null);
			object obj = ((configurationSection != null) ? configurationSection.GetRuntimeObject() : null);
			if (base.RawXml == null)
			{
				return obj;
			}
			try
			{
				XmlReader xmlReader = new ConfigXmlTextReader(new StringReader(base.RawXml), base.Configuration.FilePath);
				this.DoDeserializeSection(xmlReader);
				if (!string.IsNullOrEmpty(this.SectionInformation.ConfigSource))
				{
					string text = this.SectionInformation.ConfigFilePath;
					if (!string.IsNullOrEmpty(text))
					{
						text = Path.GetDirectoryName(text);
					}
					else
					{
						text = string.Empty;
					}
					string text2 = Path.Combine(text, this.SectionInformation.ConfigSource);
					if (File.Exists(text2))
					{
						base.RawXml = File.ReadAllText(text2);
						this.SectionInformation.SetRawXml(base.RawXml);
					}
				}
			}
			catch
			{
			}
			XmlDocument xmlDocument = new ConfigurationXmlDocument();
			xmlDocument.LoadXml(base.RawXml);
			return this.SectionHandler.Create(obj, this.ConfigContext, xmlDocument.DocumentElement);
		}

		[MonoTODO]
		protected internal override bool IsModified()
		{
			return base.IsModified();
		}

		[MonoTODO]
		protected internal override void ResetModified()
		{
			base.ResetModified();
		}

		private ConfigurationElement CreateElement(Type t)
		{
			ConfigurationElement configurationElement = (ConfigurationElement)Activator.CreateInstance(t);
			configurationElement.Init();
			configurationElement.Configuration = base.Configuration;
			if (this.IsReadOnly())
			{
				configurationElement.SetReadOnly();
			}
			return configurationElement;
		}

		private void DoDeserializeSection(XmlReader reader)
		{
			reader.MoveToContent();
			string text = null;
			string text2 = null;
			while (reader.MoveToNextAttribute())
			{
				string localName = reader.LocalName;
				if (localName == "configProtectionProvider")
				{
					text = reader.Value;
				}
				else if (localName == "configSource")
				{
					text2 = reader.Value;
				}
			}
			if (text != null)
			{
				ProtectedConfigurationProvider provider = ProtectedConfiguration.GetProvider(text, true);
				XmlDocument xmlDocument = new ConfigurationXmlDocument();
				reader.MoveToElement();
				xmlDocument.Load(new StringReader(reader.ReadInnerXml()));
				reader = new XmlNodeReader(provider.Decrypt(xmlDocument));
				this.SectionInformation.ProtectSection(text);
				reader.MoveToContent();
			}
			if (text2 != null)
			{
				this.SectionInformation.ConfigSource = text2;
			}
			this.SectionInformation.SetRawXml(base.RawXml);
			if (this.SectionHandler == null)
			{
				this.DeserializeElement(reader, false);
			}
		}

		[MonoInternalNote("find the proper location for the decryption stuff")]
		protected internal virtual void DeserializeSection(XmlReader reader)
		{
			try
			{
				this.DoDeserializeSection(reader);
			}
			catch (ConfigurationErrorsException ex)
			{
				throw new ConfigurationErrorsException(string.Format("Error deserializing configuration section {0}: {1}", this.SectionInformation.Name, ex.Message));
			}
		}

		internal void DeserializeConfigSource(string basePath)
		{
			string configSource = this.SectionInformation.ConfigSource;
			if (string.IsNullOrEmpty(configSource))
			{
				return;
			}
			if (Path.IsPathRooted(configSource))
			{
				throw new ConfigurationErrorsException("The configSource attribute must be a relative physical path.");
			}
			if (this.HasLocalModifications())
			{
				throw new ConfigurationErrorsException("A section using 'configSource' may contain no other attributes or elements.");
			}
			string text = Path.Combine(basePath, configSource);
			if (!File.Exists(text))
			{
				base.RawXml = null;
				this.SectionInformation.SetRawXml(null);
				throw new ConfigurationErrorsException(string.Format("Unable to open configSource file '{0}'.", text));
			}
			base.RawXml = File.ReadAllText(text);
			this.SectionInformation.SetRawXml(base.RawXml);
			this.DeserializeElement(new ConfigXmlTextReader(new StringReader(base.RawXml), text), false);
		}

		protected internal virtual string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
		{
			this.externalDataXml = null;
			ConfigurationElement configurationElement;
			if (parentElement != null)
			{
				configurationElement = this.CreateElement(base.GetType());
				configurationElement.Unmerge(this, parentElement, saveMode);
			}
			else
			{
				configurationElement = this;
			}
			configurationElement.PrepareSave(parentElement, saveMode);
			bool flag = configurationElement.HasValues(parentElement, saveMode);
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					if (flag)
					{
						configurationElement.SerializeToXmlElement(xmlTextWriter, name);
					}
					else if (saveMode == ConfigurationSaveMode.Modified && configurationElement.IsModified())
					{
						xmlTextWriter.WriteStartElement(name);
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.Close();
				}
				text = stringWriter.ToString();
			}
			string configSource = this.SectionInformation.ConfigSource;
			if (string.IsNullOrEmpty(configSource))
			{
				return text;
			}
			this.externalDataXml = text;
			string text2;
			using (StringWriter stringWriter2 = new StringWriter())
			{
				bool flag2 = !string.IsNullOrEmpty(name);
				using (XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter2))
				{
					if (flag2)
					{
						xmlTextWriter2.WriteStartElement(name);
					}
					xmlTextWriter2.WriteAttributeString("configSource", configSource);
					if (flag2)
					{
						xmlTextWriter2.WriteEndElement();
					}
				}
				text2 = stringWriter2.ToString();
			}
			return text2;
		}

		private SectionInformation sectionInformation;

		private IConfigurationSectionHandler section_handler;

		private string externalDataXml;

		private object _configContext;
	}
}
