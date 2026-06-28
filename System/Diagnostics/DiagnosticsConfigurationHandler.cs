using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace System.Diagnostics
{
	[Obsolete("This class is obsoleted")]
	public class DiagnosticsConfigurationHandler : global::System.Configuration.IConfigurationSectionHandler
	{
		public DiagnosticsConfigurationHandler()
		{
			this.elementHandlers["assert"] = new DiagnosticsConfigurationHandler.ElementHandler(this.AddAssertNode);
			this.elementHandlers["switches"] = new DiagnosticsConfigurationHandler.ElementHandler(this.AddSwitchesNode);
			this.elementHandlers["trace"] = new DiagnosticsConfigurationHandler.ElementHandler(this.AddTraceNode);
			this.elementHandlers["sources"] = new DiagnosticsConfigurationHandler.ElementHandler(this.AddSourcesNode);
		}

		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			IDictionary dictionary;
			if (parent == null)
			{
				dictionary = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			}
			else
			{
				dictionary = (IDictionary)((ICloneable)parent).Clone();
			}
			if (dictionary.Contains(".__TraceInfoSettingsKey__."))
			{
				this.configValues = (TraceImplSettings)dictionary[".__TraceInfoSettingsKey__."];
			}
			else
			{
				dictionary.Add(".__TraceInfoSettingsKey__.", this.configValues = new TraceImplSettings());
			}
			foreach (object obj in section.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType xmlNodeType = xmlNode.NodeType;
				if (xmlNodeType == XmlNodeType.Element)
				{
					if (!(xmlNode.LocalName != "sharedListeners"))
					{
						this.AddTraceListeners(dictionary, xmlNode, this.GetSharedListeners(dictionary));
					}
				}
			}
			foreach (object obj2 in section.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj2;
				XmlNodeType nodeType = xmlNode2.NodeType;
				XmlNodeType xmlNodeType = nodeType;
				if (xmlNodeType != XmlNodeType.Element)
				{
					if (xmlNodeType != XmlNodeType.Comment && xmlNodeType != XmlNodeType.Whitespace)
					{
						this.ThrowUnrecognizedElement(xmlNode2);
					}
				}
				else if (!(xmlNode2.LocalName == "sharedListeners"))
				{
					DiagnosticsConfigurationHandler.ElementHandler elementHandler = (DiagnosticsConfigurationHandler.ElementHandler)this.elementHandlers[xmlNode2.Name];
					if (elementHandler != null)
					{
						elementHandler(dictionary, xmlNode2);
					}
					else
					{
						this.ThrowUnrecognizedElement(xmlNode2);
					}
				}
			}
			return dictionary;
		}

		private void AddAssertNode(IDictionary d, XmlNode node)
		{
			XmlAttributeCollection attributes = node.Attributes;
			string attribute = this.GetAttribute(attributes, "assertuienabled", false, node);
			string attribute2 = this.GetAttribute(attributes, "logfilename", false, node);
			this.ValidateInvalidAttributes(attributes, node);
			if (attribute != null)
			{
				try
				{
					d["assertuienabled"] = bool.Parse(attribute);
				}
				catch (Exception ex)
				{
					throw new global::System.Configuration.ConfigurationException("The `assertuienabled' attribute must be `true' or `false'", ex, node);
				}
			}
			if (attribute2 != null)
			{
				d["logfilename"] = attribute2;
			}
			DefaultTraceListener defaultTraceListener = (DefaultTraceListener)this.configValues.Listeners["Default"];
			if (defaultTraceListener != null)
			{
				if (attribute != null)
				{
					defaultTraceListener.AssertUiEnabled = (bool)d["assertuienabled"];
				}
				if (attribute2 != null)
				{
					defaultTraceListener.LogFileName = attribute2;
				}
			}
			if (node.ChildNodes.Count > 0)
			{
				this.ThrowUnrecognizedElement(node.ChildNodes[0]);
			}
		}

		private void AddSwitchesNode(IDictionary d, XmlNode node)
		{
			this.ValidateInvalidAttributes(node.Attributes, node);
			IDictionary dictionary = new Hashtable();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						XmlAttributeCollection attributes = xmlNode.Attributes;
						string name = xmlNode.Name;
						if (name == null)
						{
							goto IL_013B;
						}
						if (DiagnosticsConfigurationHandler.<>f__switch$map5 == null)
						{
							DiagnosticsConfigurationHandler.<>f__switch$map5 = new Dictionary<string, int>(3)
							{
								{ "add", 0 },
								{ "remove", 1 },
								{ "clear", 2 }
							};
						}
						int num;
						if (!DiagnosticsConfigurationHandler.<>f__switch$map5.TryGetValue(name, out num))
						{
							goto IL_013B;
						}
						switch (num)
						{
						case 0:
						{
							string text = this.GetAttribute(attributes, "name", true, xmlNode);
							string attribute = this.GetAttribute(attributes, "value", true, xmlNode);
							dictionary[text] = DiagnosticsConfigurationHandler.GetSwitchValue(text, attribute);
							break;
						}
						case 1:
						{
							string text = this.GetAttribute(attributes, "name", true, xmlNode);
							dictionary.Remove(text);
							break;
						}
						case 2:
							dictionary.Clear();
							break;
						default:
							goto IL_013B;
						}
						IL_0147:
						this.ValidateInvalidAttributes(attributes, xmlNode);
						continue;
						IL_013B:
						this.ThrowUnrecognizedElement(xmlNode);
						goto IL_0147;
					}
					this.ThrowUnrecognizedNode(xmlNode);
				}
			}
			d[node.Name] = dictionary;
		}

		private static object GetSwitchValue(string name, string value)
		{
			return value;
		}

		private void AddTraceNode(IDictionary d, XmlNode node)
		{
			this.AddTraceAttributes(d, node);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						if (xmlNode.Name == "listeners")
						{
							this.AddTraceListeners(d, xmlNode, this.configValues.Listeners);
						}
						else
						{
							this.ThrowUnrecognizedElement(xmlNode);
						}
						this.ValidateInvalidAttributes(xmlNode.Attributes, xmlNode);
					}
					else
					{
						this.ThrowUnrecognizedNode(xmlNode);
					}
				}
			}
		}

		private void AddTraceAttributes(IDictionary d, XmlNode node)
		{
			XmlAttributeCollection attributes = node.Attributes;
			string attribute = this.GetAttribute(attributes, "autoflush", false, node);
			string attribute2 = this.GetAttribute(attributes, "indentsize", false, node);
			this.ValidateInvalidAttributes(attributes, node);
			if (attribute != null)
			{
				bool flag = false;
				try
				{
					flag = bool.Parse(attribute);
					d["autoflush"] = flag;
				}
				catch (Exception ex)
				{
					throw new global::System.Configuration.ConfigurationException("The `autoflush' attribute must be `true' or `false'", ex, node);
				}
				this.configValues.AutoFlush = flag;
			}
			if (attribute2 != null)
			{
				int num = 0;
				try
				{
					num = int.Parse(attribute2);
					d["indentsize"] = num;
				}
				catch (Exception ex2)
				{
					throw new global::System.Configuration.ConfigurationException("The `indentsize' attribute must be an integral value.", ex2, node);
				}
				this.configValues.IndentSize = num;
			}
		}

		private TraceListenerCollection GetSharedListeners(IDictionary d)
		{
			TraceListenerCollection traceListenerCollection = d["sharedListeners"] as TraceListenerCollection;
			if (traceListenerCollection == null)
			{
				traceListenerCollection = new TraceListenerCollection();
				d["sharedListeners"] = traceListenerCollection;
			}
			return traceListenerCollection;
		}

		private void AddSourcesNode(IDictionary d, XmlNode node)
		{
			this.ValidateInvalidAttributes(node.Attributes, node);
			Hashtable hashtable = d["sources"] as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				d["sources"] = hashtable;
			}
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						if (xmlNode.Name == "source")
						{
							this.AddTraceSource(d, hashtable, xmlNode);
						}
						else
						{
							this.ThrowUnrecognizedElement(xmlNode);
						}
					}
					else
					{
						this.ThrowUnrecognizedNode(xmlNode);
					}
				}
			}
		}

		private void AddTraceSource(IDictionary d, Hashtable sources, XmlNode node)
		{
			string text = null;
			SourceLevels sourceLevels = SourceLevels.Error;
			global::System.Collections.Specialized.StringDictionary stringDictionary = new global::System.Collections.Specialized.StringDictionary();
			foreach (object obj in node.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				string name = xmlAttribute.Name;
				if (name != null)
				{
					if (DiagnosticsConfigurationHandler.<>f__switch$map6 == null)
					{
						DiagnosticsConfigurationHandler.<>f__switch$map6 = new Dictionary<string, int>(2)
						{
							{ "name", 0 },
							{ "switchValue", 1 }
						};
					}
					int num;
					if (DiagnosticsConfigurationHandler.<>f__switch$map6.TryGetValue(name, out num))
					{
						if (num == 0)
						{
							text = xmlAttribute.Value;
							continue;
						}
						if (num == 1)
						{
							sourceLevels = (SourceLevels)((int)Enum.Parse(typeof(SourceLevels), xmlAttribute.Value));
							continue;
						}
					}
				}
				stringDictionary[xmlAttribute.Name] = xmlAttribute.Value;
			}
			if (text == null)
			{
				throw new global::System.Configuration.ConfigurationException("Mandatory attribute 'name' is missing in 'source' element.");
			}
			if (sources.ContainsKey(text))
			{
				return;
			}
			TraceSourceInfo traceSourceInfo = new TraceSourceInfo(text, sourceLevels, this.configValues);
			sources.Add(traceSourceInfo.Name, traceSourceInfo);
			foreach (object obj2 in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj2;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						if (xmlNode.Name == "listeners")
						{
							this.AddTraceListeners(d, xmlNode, traceSourceInfo.Listeners);
						}
						else
						{
							this.ThrowUnrecognizedElement(xmlNode);
						}
						this.ValidateInvalidAttributes(xmlNode.Attributes, xmlNode);
					}
					else
					{
						this.ThrowUnrecognizedNode(xmlNode);
					}
				}
			}
		}

		private void AddTraceListeners(IDictionary d, XmlNode listenersNode, TraceListenerCollection listeners)
		{
			this.ValidateInvalidAttributes(listenersNode.Attributes, listenersNode);
			foreach (object obj in listenersNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						XmlAttributeCollection attributes = xmlNode.Attributes;
						string name = xmlNode.Name;
						if (name == null)
						{
							goto IL_0111;
						}
						if (DiagnosticsConfigurationHandler.<>f__switch$map7 == null)
						{
							DiagnosticsConfigurationHandler.<>f__switch$map7 = new Dictionary<string, int>(3)
							{
								{ "add", 0 },
								{ "remove", 1 },
								{ "clear", 2 }
							};
						}
						int num;
						if (!DiagnosticsConfigurationHandler.<>f__switch$map7.TryGetValue(name, out num))
						{
							goto IL_0111;
						}
						switch (num)
						{
						case 0:
							this.AddTraceListener(d, xmlNode, attributes, listeners);
							break;
						case 1:
						{
							string attribute = this.GetAttribute(attributes, "name", true, xmlNode);
							this.RemoveTraceListener(attribute);
							break;
						}
						case 2:
							this.configValues.Listeners.Clear();
							break;
						default:
							goto IL_0111;
						}
						IL_011D:
						this.ValidateInvalidAttributes(attributes, xmlNode);
						continue;
						IL_0111:
						this.ThrowUnrecognizedElement(xmlNode);
						goto IL_011D;
					}
					this.ThrowUnrecognizedNode(xmlNode);
				}
			}
		}

		private void AddTraceListener(IDictionary d, XmlNode child, XmlAttributeCollection attributes, TraceListenerCollection listeners)
		{
			string attribute = this.GetAttribute(attributes, "name", true, child);
			string attribute2 = this.GetAttribute(attributes, "type", false, child);
			if (attribute2 == null)
			{
				TraceListener traceListener = this.GetSharedListeners(d)[attribute];
				if (traceListener == null)
				{
					throw new global::System.Configuration.ConfigurationException(string.Format("Shared trace listener {0} does not exist.", attribute));
				}
				if (attributes.Count != 0)
				{
					throw new ConfigurationErrorsException(string.Format("Listener '{0}' references a shared listener and can only have a 'Name' attribute.", attribute));
				}
				listeners.Add(traceListener, this.configValues);
				return;
			}
			else
			{
				Type type = Type.GetType(attribute2);
				if (type == null)
				{
					throw new global::System.Configuration.ConfigurationException(string.Format("Invalid Type Specified: {0}", attribute2));
				}
				string attribute3 = this.GetAttribute(attributes, "initializeData", false, child);
				object[] array;
				Type[] array2;
				if (attribute3 != null)
				{
					array = new object[] { attribute3 };
					array2 = new Type[] { typeof(string) };
				}
				else
				{
					array = null;
					array2 = Type.EmptyTypes;
				}
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
				if (type.Assembly == base.GetType().Assembly)
				{
					bindingFlags |= BindingFlags.NonPublic;
				}
				ConstructorInfo constructor = type.GetConstructor(bindingFlags, null, array2, null);
				if (constructor == null)
				{
					throw new global::System.Configuration.ConfigurationException("Couldn't find constructor for class " + attribute2);
				}
				TraceListener traceListener2 = (TraceListener)constructor.Invoke(array);
				traceListener2.Name = attribute;
				string attribute4 = this.GetAttribute(attributes, "traceOutputOptions", false, child);
				if (attribute4 != null)
				{
					if (attribute4 != attribute4.Trim())
					{
						throw new ConfigurationErrorsException(string.Format("Invalid value '{0}' for 'traceOutputOptions'.", attribute4), child);
					}
					TraceOptions traceOptions;
					try
					{
						traceOptions = (TraceOptions)((int)Enum.Parse(typeof(TraceOptions), attribute4));
					}
					catch (ArgumentException)
					{
						throw new ConfigurationErrorsException(string.Format("Invalid value '{0}' for 'traceOutputOptions'.", attribute4), child);
					}
					traceListener2.TraceOutputOptions = traceOptions;
				}
				string[] supportedAttributes = traceListener2.GetSupportedAttributes();
				if (supportedAttributes != null)
				{
					foreach (string text in supportedAttributes)
					{
						string attribute5 = this.GetAttribute(attributes, text, false, child);
						if (attribute5 != null)
						{
							traceListener2.Attributes.Add(text, attribute5);
						}
					}
				}
				listeners.Add(traceListener2, this.configValues);
				return;
			}
		}

		private void RemoveTraceListener(string name)
		{
			try
			{
				this.configValues.Listeners.Remove(name);
			}
			catch (ArgumentException)
			{
			}
			catch (Exception ex)
			{
				throw new global::System.Configuration.ConfigurationException(string.Format("Unknown error removing listener: {0}", name), ex);
			}
		}

		private string GetAttribute(XmlAttributeCollection attrs, string attr, bool required, XmlNode node)
		{
			XmlAttribute xmlAttribute = attrs[attr];
			string text = null;
			if (xmlAttribute != null)
			{
				text = xmlAttribute.Value;
				if (required)
				{
					this.ValidateAttribute(attr, text, node);
				}
				attrs.Remove(xmlAttribute);
			}
			else if (required)
			{
				this.ThrowMissingAttribute(attr, node);
			}
			return text;
		}

		private void ValidateAttribute(string attribute, string value, XmlNode node)
		{
			if (value == null || value.Length == 0)
			{
				throw new global::System.Configuration.ConfigurationException(string.Format("Required attribute '{0}' cannot be empty.", attribute), node);
			}
		}

		private void ValidateInvalidAttributes(XmlAttributeCollection c, XmlNode node)
		{
			if (c.Count != 0)
			{
				this.ThrowUnrecognizedAttribute(c[0].Name, node);
			}
		}

		private void ThrowMissingAttribute(string attribute, XmlNode node)
		{
			throw new global::System.Configuration.ConfigurationException(string.Format("Required attribute '{0}' not found.", attribute), node);
		}

		private void ThrowUnrecognizedNode(XmlNode node)
		{
			throw new global::System.Configuration.ConfigurationException(string.Format("Unrecognized node `{0}'; nodeType={1}", node.Name, node.NodeType), node);
		}

		private void ThrowUnrecognizedElement(XmlNode node)
		{
			throw new global::System.Configuration.ConfigurationException(string.Format("Unrecognized element '{0}'.", node.Name), node);
		}

		private void ThrowUnrecognizedAttribute(string attribute, XmlNode node)
		{
			throw new global::System.Configuration.ConfigurationException(string.Format("Unrecognized attribute '{0}' on element <{1}/>.", attribute, node.Name), node);
		}

		private TraceImplSettings configValues;

		private IDictionary elementHandlers = new Hashtable();

		private delegate void ElementHandler(IDictionary d, XmlNode node);
	}
}
