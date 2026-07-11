using System;
using System.Collections;

namespace System.Xml.Xsl
{
	public class XsltArgumentList
	{
		public object GetParam(string name, string namespaceUri)
		{
			return this.parameters[new XmlQualifiedName(name, namespaceUri)];
		}

		public object GetExtensionObject(string namespaceUri)
		{
			return this.extensions[namespaceUri];
		}

		public void AddParam(string name, string namespaceUri, object parameter)
		{
			XsltArgumentList.CheckArgumentNull(name, "name");
			XsltArgumentList.CheckArgumentNull(namespaceUri, "namespaceUri");
			XsltArgumentList.CheckArgumentNull(parameter, "parameter");
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			xmlQualifiedName.Verify();
			this.parameters.Add(xmlQualifiedName, parameter);
		}

		public void AddExtensionObject(string namespaceUri, object extension)
		{
			XsltArgumentList.CheckArgumentNull(namespaceUri, "namespaceUri");
			XsltArgumentList.CheckArgumentNull(extension, "extension");
			this.extensions.Add(namespaceUri, extension);
		}

		public object RemoveParam(string name, string namespaceUri)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, namespaceUri);
			object obj = this.parameters[xmlQualifiedName];
			this.parameters.Remove(xmlQualifiedName);
			return obj;
		}

		public object RemoveExtensionObject(string namespaceUri)
		{
			object obj = this.extensions[namespaceUri];
			this.extensions.Remove(namespaceUri);
			return obj;
		}

		public event XsltMessageEncounteredEventHandler XsltMessageEncountered
		{
			add
			{
				this.xsltMessageEncountered = (XsltMessageEncounteredEventHandler)Delegate.Combine(this.xsltMessageEncountered, value);
			}
			remove
			{
				this.xsltMessageEncountered = (XsltMessageEncounteredEventHandler)Delegate.Remove(this.xsltMessageEncountered, value);
			}
		}

		public void Clear()
		{
			this.parameters.Clear();
			this.extensions.Clear();
			this.xsltMessageEncountered = null;
		}

		private static void CheckArgumentNull(object param, string paramName)
		{
			if (param == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		private Hashtable parameters = new Hashtable();

		private Hashtable extensions = new Hashtable();

		internal XsltMessageEncounteredEventHandler xsltMessageEncountered;
	}
}
