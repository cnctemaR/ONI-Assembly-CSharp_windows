using System;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration
{
	internal class ConfigurationData
	{
		private Hashtable FileCache
		{
			get
			{
				if (this.cache != null)
				{
					return this.cache;
				}
				this.cache = new Hashtable();
				return this.cache;
			}
		}

		public ConfigurationData()
			: this(null)
		{
		}

		public ConfigurationData(ConfigurationData parent)
		{
			this.parent = ((parent == this) ? null : parent);
			this.factories = new Hashtable();
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public bool Load(string fileName)
		{
			this.fileName = fileName;
			if (fileName == null || !File.Exists(fileName))
			{
				return false;
			}
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
				if (this.InitRead(xmlTextReader))
				{
					this.ReadConfigFile(xmlTextReader);
				}
			}
			catch (ConfigurationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("Error reading " + fileName, ex);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return true;
		}

		public bool LoadString(string data)
		{
			if (data == null)
			{
				return false;
			}
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(new StringReader(data));
				if (this.InitRead(xmlTextReader))
				{
					this.ReadConfigFile(xmlTextReader);
				}
			}
			catch (ConfigurationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("Error reading " + this.fileName, ex);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return true;
		}

		private object GetHandler(string sectionName)
		{
			Hashtable hashtable = this.factories;
			object obj2;
			lock (hashtable)
			{
				object obj = this.factories[sectionName];
				if (obj == null || obj == ConfigurationData.removedMark)
				{
					if (this.parent != null)
					{
						obj2 = this.parent.GetHandler(sectionName);
					}
					else
					{
						obj2 = null;
					}
				}
				else if (obj is IConfigurationSectionHandler)
				{
					obj2 = (IConfigurationSectionHandler)obj;
				}
				else
				{
					obj = this.CreateNewHandler(sectionName, (SectionData)obj);
					this.factories[sectionName] = obj;
					obj2 = obj;
				}
			}
			return obj2;
		}

		private object CreateNewHandler(string sectionName, SectionData section)
		{
			Type type = Type.GetType(section.TypeName);
			if (type == null)
			{
				throw new ConfigurationException("Cannot get Type for " + section.TypeName);
			}
			object obj = Activator.CreateInstance(type, true);
			if (obj == null)
			{
				throw new ConfigurationException("Cannot get instance for " + type);
			}
			return obj;
		}

		private XmlDocument GetInnerDoc(XmlDocument doc, int i, string[] sectionPath)
		{
			if (++i >= sectionPath.Length)
			{
				return doc;
			}
			if (doc.DocumentElement == null)
			{
				return null;
			}
			for (XmlNode xmlNode = doc.DocumentElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == sectionPath[i])
				{
					ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
					configXmlDocument.Load(new StringReader(xmlNode.OuterXml));
					return this.GetInnerDoc(configXmlDocument, i, sectionPath);
				}
			}
			return null;
		}

		private XmlDocument GetDocumentForSection(string sectionName)
		{
			ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
			if (this.pending == null)
			{
				return configXmlDocument;
			}
			string[] array = sectionName.Split(new char[] { '/' });
			string text = this.pending[array[0]] as string;
			if (text == null)
			{
				return configXmlDocument;
			}
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(text));
			xmlTextReader.MoveToContent();
			configXmlDocument.LoadSingleElement(this.fileName, xmlTextReader);
			return this.GetInnerDoc(configXmlDocument, 0, array);
		}

		private object GetConfigInternal(string sectionName)
		{
			object handler = this.GetHandler(sectionName);
			IConfigurationSectionHandler configurationSectionHandler = handler as IConfigurationSectionHandler;
			if (configurationSectionHandler == null)
			{
				return handler;
			}
			object obj = null;
			if (this.parent != null)
			{
				obj = this.parent.GetConfig(sectionName);
			}
			XmlDocument documentForSection = this.GetDocumentForSection(sectionName);
			if (documentForSection == null || documentForSection.DocumentElement == null)
			{
				return obj;
			}
			return configurationSectionHandler.Create(obj, this.fileName, documentForSection.DocumentElement);
		}

		public object GetConfig(string sectionName)
		{
			ConfigurationData configurationData = this;
			object obj;
			lock (configurationData)
			{
				obj = this.FileCache[sectionName];
			}
			if (obj == ConfigurationData.emptyMark)
			{
				return null;
			}
			if (obj != null)
			{
				return obj;
			}
			configurationData = this;
			lock (configurationData)
			{
				obj = this.GetConfigInternal(sectionName);
				this.FileCache[sectionName] = ((obj == null) ? ConfigurationData.emptyMark : obj);
			}
			return obj;
		}

		private object LookForFactory(string key)
		{
			object obj = this.factories[key];
			if (obj != null)
			{
				return obj;
			}
			if (this.parent != null)
			{
				return this.parent.LookForFactory(key);
			}
			return null;
		}

		private bool InitRead(XmlTextReader reader)
		{
			reader.MoveToContent();
			if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
			{
				this.ThrowException("Configuration file does not have a valid root element", reader);
			}
			if (reader.HasAttributes)
			{
				this.ThrowException("Unrecognized attribute in root element", reader);
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return false;
			}
			reader.Read();
			reader.MoveToContent();
			return reader.NodeType != XmlNodeType.EndElement;
		}

		private void MoveToNextElement(XmlTextReader reader)
		{
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					return;
				}
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment && nodeType != XmlNodeType.SignificantWhitespace && nodeType != XmlNodeType.EndElement)
				{
					this.ThrowException("Unrecognized element", reader);
				}
			}
		}

		private void ReadSection(XmlTextReader reader, string sectionName)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			bool flag = false;
			string text5 = null;
			bool flag2 = true;
			AllowDefinition allowDefinition = AllowDefinition.Everywhere;
			while (reader.MoveToNextAttribute())
			{
				string name = reader.Name;
				if (name != null)
				{
					if (name == "allowLocation")
					{
						if (text3 != null)
						{
							this.ThrowException("Duplicated allowLocation attribute.", reader);
						}
						text3 = reader.Value;
						flag2 = text3 == "true";
						if (!flag2 && text3 != "false")
						{
							this.ThrowException("Invalid attribute value", reader);
						}
					}
					else if (name == "requirePermission")
					{
						if (text5 != null)
						{
							this.ThrowException("Duplicated requirePermission attribute.", reader);
						}
						text5 = reader.Value;
						flag = text5 == "true";
						if (!flag && text5 != "false")
						{
							this.ThrowException("Invalid attribute value", reader);
						}
					}
					else
					{
						if (name == "allowDefinition")
						{
							if (text4 != null)
							{
								this.ThrowException("Duplicated allowDefinition attribute.", reader);
							}
							text4 = reader.Value;
							try
							{
								allowDefinition = (AllowDefinition)Enum.Parse(typeof(AllowDefinition), text4);
								continue;
							}
							catch
							{
								this.ThrowException("Invalid attribute value", reader);
								continue;
							}
						}
						if (name == "type")
						{
							if (text2 != null)
							{
								this.ThrowException("Duplicated type attribute.", reader);
							}
							text2 = reader.Value;
						}
						else if (name == "name")
						{
							if (text != null)
							{
								this.ThrowException("Duplicated name attribute.", reader);
							}
							text = reader.Value;
							if (text == "location")
							{
								this.ThrowException("location is a reserved section name", reader);
							}
						}
						else
						{
							this.ThrowException("Unrecognized attribute.", reader);
						}
					}
				}
			}
			if (text == null || text2 == null)
			{
				this.ThrowException("Required attribute missing", reader);
			}
			if (sectionName != null)
			{
				text = sectionName + "/" + text;
			}
			reader.MoveToElement();
			object obj = this.LookForFactory(text);
			if (obj != null && obj != ConfigurationData.removedMark)
			{
				this.ThrowException("Already have a factory for " + text, reader);
			}
			SectionData sectionData = new SectionData(text, text2, flag2, allowDefinition, flag);
			sectionData.FileName = this.fileName;
			this.factories[text] = sectionData;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.Read();
				reader.MoveToContent();
				if (reader.NodeType != XmlNodeType.EndElement)
				{
					this.ReadSections(reader, text);
				}
				reader.ReadEndElement();
			}
			reader.MoveToContent();
		}

		private void ReadRemoveSection(XmlTextReader reader, string sectionName)
		{
			if (!reader.MoveToNextAttribute() || reader.Name != "name")
			{
				this.ThrowException("Unrecognized attribute.", reader);
			}
			string text = reader.Value;
			if (text == null || text.Length == 0)
			{
				this.ThrowException("Empty name to remove", reader);
			}
			reader.MoveToElement();
			if (sectionName != null)
			{
				text = sectionName + "/" + text;
			}
			object obj = this.LookForFactory(text);
			if (obj != null && obj == ConfigurationData.removedMark)
			{
				this.ThrowException("No factory for " + text, reader);
			}
			this.factories[text] = ConfigurationData.removedMark;
			this.MoveToNextElement(reader);
		}

		private void ReadSectionGroup(XmlTextReader reader, string configSection)
		{
			if (!reader.MoveToNextAttribute())
			{
				this.ThrowException("sectionGroup must have a 'name' attribute.", reader);
			}
			string text = null;
			do
			{
				if (reader.Name == "name")
				{
					if (text != null)
					{
						this.ThrowException("Duplicate 'name' attribute.", reader);
					}
					text = reader.Value;
				}
				else if (reader.Name != "type")
				{
					this.ThrowException("Unrecognized attribute.", reader);
				}
			}
			while (reader.MoveToNextAttribute());
			if (text == null)
			{
				this.ThrowException("No 'name' attribute.", reader);
			}
			if (text == "location")
			{
				this.ThrowException("location is a reserved section name", reader);
			}
			if (configSection != null)
			{
				text = configSection + "/" + text;
			}
			object obj = this.LookForFactory(text);
			if (obj != null && obj != ConfigurationData.removedMark && obj != ConfigurationData.groupMark)
			{
				this.ThrowException("Already have a factory for " + text, reader);
			}
			this.factories[text] = ConfigurationData.groupMark;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				reader.MoveToContent();
				return;
			}
			reader.Read();
			reader.MoveToContent();
			if (reader.NodeType != XmlNodeType.EndElement)
			{
				this.ReadSections(reader, text);
			}
			reader.ReadEndElement();
			reader.MoveToContent();
		}

		private void ReadSections(XmlTextReader reader, string configSection)
		{
			int depth = reader.Depth;
			reader.MoveToContent();
			while (reader.Depth == depth)
			{
				string name = reader.Name;
				if (name == "section")
				{
					this.ReadSection(reader, configSection);
				}
				else if (name == "remove")
				{
					this.ReadRemoveSection(reader, configSection);
				}
				else if (name == "clear")
				{
					if (reader.HasAttributes)
					{
						this.ThrowException("Unrecognized attribute.", reader);
					}
					this.factories.Clear();
					this.MoveToNextElement(reader);
				}
				else if (name == "sectionGroup")
				{
					this.ReadSectionGroup(reader, configSection);
				}
				else
				{
					this.ThrowException("Unrecognized element: " + reader.Name, reader);
				}
				reader.MoveToContent();
			}
		}

		private void StorePending(string name, XmlTextReader reader)
		{
			if (this.pending == null)
			{
				this.pending = new Hashtable();
			}
			this.pending[name] = reader.ReadOuterXml();
		}

		private void ReadConfigFile(XmlTextReader reader)
		{
			reader.MoveToContent();
			while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
			{
				string name = reader.Name;
				if (name == "configSections")
				{
					if (reader.HasAttributes)
					{
						this.ThrowException("Unrecognized attribute in <configSections>.", reader);
					}
					if (reader.IsEmptyElement)
					{
						reader.Skip();
					}
					else
					{
						reader.Read();
						reader.MoveToContent();
						if (reader.NodeType != XmlNodeType.EndElement)
						{
							this.ReadSections(reader, null);
						}
						reader.ReadEndElement();
					}
				}
				else if (name != null && name != "")
				{
					this.StorePending(name, reader);
					this.MoveToNextElement(reader);
				}
				else
				{
					this.MoveToNextElement(reader);
				}
				reader.MoveToContent();
			}
		}

		private void ThrowException(string text, XmlTextReader reader)
		{
			throw new ConfigurationException(text, this.fileName, reader.LineNumber);
		}

		private ConfigurationData parent;

		private Hashtable factories;

		private static object removedMark = new object();

		private static object emptyMark = new object();

		private Hashtable pending;

		private string fileName;

		private static object groupMark = new object();

		private Hashtable cache;
	}
}
