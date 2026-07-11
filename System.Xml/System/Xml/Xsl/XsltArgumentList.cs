using System;
using System.Collections;
using System.Security.Permissions;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public class XsltArgumentList
	{
		public XsltArgumentList()
		{
			this.extensionObjects = new Hashtable();
			this.parameters = new Hashtable();
		}

		public event XsltMessageEncounteredEventHandler XsltMessageEncountered;

		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
		public void AddExtensionObject(string namespaceUri, object extension)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentException("The namespaceUri is a null reference.");
			}
			if (namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("The namespaceUri is http://www.w3.org/1999/XSL/Transform.");
			}
			if (this.extensionObjects.Contains(namespaceUri))
			{
				throw new ArgumentException("The namespaceUri already has an extension object associated with it.");
			}
			this.extensionObjects[namespaceUri] = extension;
		}

		public void AddParam(string name, string namespaceUri, object parameter)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentException("The namespaceUri is a null reference.");
			}
			if (namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("The namespaceUri is http://www.w3.org/1999/XSL/Transform.");
			}
			if (name == null)
			{
				throw new ArgumentException("The parameter name is a null reference.");
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			if (this.parameters.Contains(xmlQualifiedName))
			{
				throw new ArgumentException("The namespaceUri already has a parameter associated with it.");
			}
			parameter = this.ValidateParam(parameter);
			this.parameters[xmlQualifiedName] = parameter;
		}

		public void Clear()
		{
			this.extensionObjects.Clear();
			this.parameters.Clear();
		}

		public object GetExtensionObject(string namespaceUri)
		{
			return this.extensionObjects[namespaceUri];
		}

		public object GetParam(string name, string namespaceUri)
		{
			if (name == null)
			{
				throw new ArgumentException("The parameter name is a null reference.");
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			return this.parameters[xmlQualifiedName];
		}

		public object RemoveExtensionObject(string namespaceUri)
		{
			object extensionObject = this.GetExtensionObject(namespaceUri);
			this.extensionObjects.Remove(namespaceUri);
			return extensionObject;
		}

		public object RemoveParam(string name, string namespaceUri)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			object param = this.GetParam(name, namespaceUri);
			this.parameters.Remove(xmlQualifiedName);
			return param;
		}

		private object ValidateParam(object parameter)
		{
			if (parameter is string)
			{
				return parameter;
			}
			if (parameter is bool)
			{
				return parameter;
			}
			if (parameter is double)
			{
				return parameter;
			}
			if (parameter is XPathNavigator)
			{
				return parameter;
			}
			if (parameter is XPathNodeIterator)
			{
				return parameter;
			}
			if (parameter is short)
			{
				return (double)((short)parameter);
			}
			if (parameter is ushort)
			{
				return (double)((ushort)parameter);
			}
			if (parameter is int)
			{
				return (double)((int)parameter);
			}
			if (parameter is long)
			{
				return (double)((long)parameter);
			}
			if (parameter is ulong)
			{
				return (ulong)parameter;
			}
			if (parameter is float)
			{
				return (double)((float)parameter);
			}
			if (parameter is decimal)
			{
				return (double)((decimal)parameter);
			}
			return parameter.ToString();
		}

		internal Hashtable extensionObjects;

		internal Hashtable parameters;
	}
}
