using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace System.Configuration
{
	internal class ConfigHelper
	{
		internal static IDictionary GetDictionary(IDictionary prev, XmlNode region, string nameAtt, string valueAtt)
		{
			Hashtable hashtable;
			if (prev == null)
			{
				hashtable = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			}
			else
			{
				Hashtable hashtable2 = (Hashtable)prev;
				hashtable = (Hashtable)hashtable2.Clone();
			}
			ConfigHelper.CollectionWrapper collectionWrapper = new ConfigHelper.CollectionWrapper(hashtable);
			collectionWrapper = ConfigHelper.GoGetThem(collectionWrapper, region, nameAtt, valueAtt);
			if (collectionWrapper == null)
			{
				return null;
			}
			return collectionWrapper.UnWrap() as IDictionary;
		}

		internal static ConfigNameValueCollection GetNameValueCollection(global::System.Collections.Specialized.NameValueCollection prev, XmlNode region, string nameAtt, string valueAtt)
		{
			ConfigNameValueCollection configNameValueCollection = new ConfigNameValueCollection(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			if (prev != null)
			{
				configNameValueCollection.Add(prev);
			}
			ConfigHelper.CollectionWrapper collectionWrapper = new ConfigHelper.CollectionWrapper(configNameValueCollection);
			collectionWrapper = ConfigHelper.GoGetThem(collectionWrapper, region, nameAtt, valueAtt);
			if (collectionWrapper == null)
			{
				return null;
			}
			return collectionWrapper.UnWrap() as ConfigNameValueCollection;
		}

		private static ConfigHelper.CollectionWrapper GoGetThem(ConfigHelper.CollectionWrapper result, XmlNode region, string nameAtt, string valueAtt)
		{
			if (region.Attributes != null && region.Attributes.Count != 0 && (region.Attributes.Count != 1 || region.Attributes[0].Name != "xmlns"))
			{
				throw new ConfigurationException("Unknown attribute", region);
			}
			XmlNodeList childNodes = region.ChildNodes;
			foreach (object obj in childNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType != XmlNodeType.Element)
					{
						throw new ConfigurationException("Only XmlElement allowed", xmlNode);
					}
					string name = xmlNode.Name;
					if (name == "clear")
					{
						if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
						{
							throw new ConfigurationException("Unknown attribute", xmlNode);
						}
						result.Clear();
					}
					else if (name == "remove")
					{
						XmlNode xmlNode2 = null;
						if (xmlNode.Attributes != null)
						{
							xmlNode2 = xmlNode.Attributes.RemoveNamedItem(nameAtt);
						}
						if (xmlNode2 == null)
						{
							throw new ConfigurationException("Required attribute not found", xmlNode);
						}
						if (xmlNode2.Value == string.Empty)
						{
							throw new ConfigurationException("Required attribute is empty", xmlNode);
						}
						if (xmlNode.Attributes.Count != 0)
						{
							throw new ConfigurationException("Unknown attribute", xmlNode);
						}
						result.Remove(xmlNode2.Value);
					}
					else
					{
						if (!(name == "add"))
						{
							throw new ConfigurationException("Unknown element", xmlNode);
						}
						XmlNode xmlNode2 = null;
						if (xmlNode.Attributes != null)
						{
							xmlNode2 = xmlNode.Attributes.RemoveNamedItem(nameAtt);
						}
						if (xmlNode2 == null)
						{
							throw new ConfigurationException("Required attribute not found", xmlNode);
						}
						if (xmlNode2.Value == string.Empty)
						{
							throw new ConfigurationException("Required attribute is empty", xmlNode);
						}
						XmlNode xmlNode3 = xmlNode.Attributes.RemoveNamedItem(valueAtt);
						if (xmlNode3 == null)
						{
							throw new ConfigurationException("Required attribute not found", xmlNode);
						}
						if (xmlNode.Attributes.Count != 0)
						{
							throw new ConfigurationException("Unknown attribute", xmlNode);
						}
						result[xmlNode2.Value] = xmlNode3.Value;
					}
				}
			}
			return result;
		}

		private class CollectionWrapper
		{
			public CollectionWrapper(IDictionary dict)
			{
				this.dict = dict;
				this.isDict = true;
			}

			public CollectionWrapper(global::System.Collections.Specialized.NameValueCollection collection)
			{
				this.collection = collection;
				this.isDict = false;
			}

			public void Remove(string s)
			{
				if (this.isDict)
				{
					this.dict.Remove(s);
				}
				else
				{
					this.collection.Remove(s);
				}
			}

			public void Clear()
			{
				if (this.isDict)
				{
					this.dict.Clear();
				}
				else
				{
					this.collection.Clear();
				}
			}

			public string this[string key]
			{
				set
				{
					if (this.isDict)
					{
						this.dict[key] = value;
					}
					else
					{
						this.collection[key] = value;
					}
				}
			}

			public object UnWrap()
			{
				if (this.isDict)
				{
					return this.dict;
				}
				return this.collection;
			}

			private IDictionary dict;

			private global::System.Collections.Specialized.NameValueCollection collection;

			private bool isDict;
		}
	}
}
