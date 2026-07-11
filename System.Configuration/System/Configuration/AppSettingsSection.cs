using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	public sealed class AppSettingsSection : ConfigurationSection
	{
		static AppSettingsSection()
		{
			AppSettingsSection._properties.Add(AppSettingsSection._propFile);
			AppSettingsSection._properties.Add(AppSettingsSection._propSettings);
		}

		protected internal override bool IsModified()
		{
			return this.Settings.IsModified();
		}

		[MonoInternalNote("file path?  do we use a System.Configuration api for opening it?  do we keep it open?  do we open it writable?")]
		protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			base.DeserializeElement(reader, serializeCollectionKey);
			if (this.File != "")
			{
				try
				{
					string text = this.File;
					if (!Path.IsPathRooted(text))
					{
						text = Path.Combine(Path.GetDirectoryName(base.Configuration.FilePath), text);
					}
					FileStream fileStream = global::System.IO.File.OpenRead(text);
					XmlReader xmlReader = new ConfigXmlTextReader(fileStream, text);
					base.DeserializeElement(xmlReader, serializeCollectionKey);
					fileStream.Close();
				}
				catch
				{
				}
			}
		}

		protected internal override void Reset(ConfigurationElement parentSection)
		{
			AppSettingsSection appSettingsSection = parentSection as AppSettingsSection;
			if (appSettingsSection != null)
			{
				this.Settings.Reset(appSettingsSection.Settings);
			}
		}

		[MonoTODO]
		protected internal override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
		{
			if (this.File == "")
			{
				return base.SerializeSection(parentElement, name, saveMode);
			}
			throw new NotImplementedException();
		}

		[ConfigurationProperty("file", DefaultValue = "")]
		public string File
		{
			get
			{
				return (string)base[AppSettingsSection._propFile];
			}
			set
			{
				base[AppSettingsSection._propFile] = value;
			}
		}

		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public KeyValueConfigurationCollection Settings
		{
			get
			{
				return (KeyValueConfigurationCollection)base[AppSettingsSection._propSettings];
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AppSettingsSection._properties;
			}
		}

		protected internal override object GetRuntimeObject()
		{
			KeyValueInternalCollection keyValueInternalCollection = new KeyValueInternalCollection();
			foreach (string text in this.Settings.AllKeys)
			{
				KeyValueConfigurationElement keyValueConfigurationElement = this.Settings[text];
				keyValueInternalCollection.Add(keyValueConfigurationElement.Key, keyValueConfigurationElement.Value);
			}
			if (!ConfigurationManager.ConfigurationSystem.SupportsUserConfig)
			{
				keyValueInternalCollection.SetReadOnly();
			}
			return keyValueInternalCollection;
		}

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propFile = new ConfigurationProperty("file", typeof(string), "", new StringConverter(), null, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propSettings = new ConfigurationProperty("", typeof(KeyValueConfigurationCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
