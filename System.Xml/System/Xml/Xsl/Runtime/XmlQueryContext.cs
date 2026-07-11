using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlQueryContext
	{
		internal XmlQueryContext(XmlQueryRuntime runtime, object defaultDataSource, XmlResolver dataSources, XsltArgumentList argList, WhitespaceRuleLookup wsRules)
		{
			this.runtime = runtime;
			this.dataSources = dataSources;
			this.dataSourceCache = new Hashtable();
			this.argList = argList;
			this.wsRules = wsRules;
			if (defaultDataSource is XmlReader)
			{
				this.readerSettings = new QueryReaderSettings((XmlReader)defaultDataSource);
			}
			else
			{
				this.readerSettings = new QueryReaderSettings(new NameTable());
			}
			if (defaultDataSource is string)
			{
				this.defaultDataSource = this.GetDataSource(defaultDataSource as string, null);
				if (this.defaultDataSource == null)
				{
					throw new XslTransformException("Data source '{0}' cannot be located.", new string[] { defaultDataSource as string });
				}
			}
			else if (defaultDataSource != null)
			{
				this.defaultDataSource = this.ConstructDocument(defaultDataSource, null, null);
			}
		}

		public XmlNameTable QueryNameTable
		{
			get
			{
				return this.readerSettings.NameTable;
			}
		}

		public XmlNameTable DefaultNameTable
		{
			get
			{
				if (this.defaultDataSource == null)
				{
					return null;
				}
				return this.defaultDataSource.NameTable;
			}
		}

		public XPathNavigator DefaultDataSource
		{
			get
			{
				if (this.defaultDataSource == null)
				{
					throw new XslTransformException("Query requires a default data source, but no default was supplied to the query engine.", new string[] { string.Empty });
				}
				return this.defaultDataSource;
			}
		}

		public XPathNavigator GetDataSource(string uriRelative, string uriBase)
		{
			XPathNavigator xpathNavigator = null;
			try
			{
				Uri uri = ((uriBase != null) ? this.dataSources.ResolveUri(null, uriBase) : null);
				Uri uri2 = this.dataSources.ResolveUri(uri, uriRelative);
				if (uri2 != null)
				{
					xpathNavigator = this.dataSourceCache[uri2] as XPathNavigator;
				}
				if (xpathNavigator == null)
				{
					object entity = this.dataSources.GetEntity(uri2, null, null);
					if (entity != null)
					{
						xpathNavigator = this.ConstructDocument(entity, uriRelative, uri2);
						this.dataSourceCache.Add(uri2, xpathNavigator);
					}
				}
			}
			catch (XslTransformException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (!XmlException.IsCatchableException(ex))
				{
					throw;
				}
				throw new XslTransformException(ex, "An error occurred while loading document '{0}'. See InnerException for a complete description of the error.", new string[] { uriRelative });
			}
			return xpathNavigator;
		}

		private XPathNavigator ConstructDocument(object dataSource, string uriRelative, Uri uriResolved)
		{
			Stream stream = dataSource as Stream;
			if (stream != null)
			{
				XmlReader xmlReader = this.readerSettings.CreateReader(stream, (uriResolved != null) ? uriResolved.ToString() : null);
				try
				{
					return new XPathDocument(WhitespaceRuleReader.CreateReader(xmlReader, this.wsRules), XmlSpace.Preserve).CreateNavigator();
				}
				finally
				{
					xmlReader.Close();
				}
			}
			if (dataSource is XmlReader)
			{
				return new XPathDocument(WhitespaceRuleReader.CreateReader(dataSource as XmlReader, this.wsRules), XmlSpace.Preserve).CreateNavigator();
			}
			if (!(dataSource is IXPathNavigable))
			{
				throw new XslTransformException("Cannot query the data source object referenced by URI '{0}', because the provided XmlResolver returned an object of type '{1}'. Only Stream, XmlReader, and IXPathNavigable data source objects are currently supported.", new string[]
				{
					uriRelative,
					dataSource.GetType().ToString()
				});
			}
			if (this.wsRules != null)
			{
				throw new XslTransformException("White space cannot be stripped from input documents that have already been loaded. Provide the input document as an XmlReader instead.", new string[] { string.Empty });
			}
			return (dataSource as IXPathNavigable).CreateNavigator();
		}

		public object GetParameter(string localName, string namespaceUri)
		{
			if (this.argList == null)
			{
				return null;
			}
			return this.argList.GetParam(localName, namespaceUri);
		}

		public object GetLateBoundObject(string namespaceUri)
		{
			if (this.argList == null)
			{
				return null;
			}
			return this.argList.GetExtensionObject(namespaceUri);
		}

		public bool LateBoundFunctionExists(string name, string namespaceUri)
		{
			if (this.argList == null)
			{
				return false;
			}
			object extensionObject = this.argList.GetExtensionObject(namespaceUri);
			return extensionObject != null && new XmlExtensionFunction(name, namespaceUri, -1, extensionObject.GetType(), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).CanBind();
		}

		public IList<XPathItem> InvokeXsltLateBoundFunction(string name, string namespaceUri, IList<XPathItem>[] args)
		{
			object obj = ((this.argList != null) ? this.argList.GetExtensionObject(namespaceUri) : null);
			if (obj == null)
			{
				throw new XslTransformException("Cannot find a script or an extension object associated with namespace '{0}'.", new string[] { namespaceUri });
			}
			if (this.extFuncsLate == null)
			{
				this.extFuncsLate = new XmlExtensionFunctionTable();
			}
			XmlExtensionFunction xmlExtensionFunction = this.extFuncsLate.Bind(name, namespaceUri, args.Length, obj.GetType(), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			object[] array = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				XmlQueryType xmlArgumentType = xmlExtensionFunction.GetXmlArgumentType(i);
				XmlTypeCode typeCode = xmlArgumentType.TypeCode;
				if (typeCode != XmlTypeCode.Item)
				{
					if (typeCode != XmlTypeCode.Node)
					{
						switch (typeCode)
						{
						case XmlTypeCode.String:
							array[i] = XsltConvert.ToString(args[i]);
							break;
						case XmlTypeCode.Boolean:
							array[i] = XsltConvert.ToBoolean(args[i]);
							break;
						case XmlTypeCode.Double:
							array[i] = XsltConvert.ToDouble(args[i]);
							break;
						}
					}
					else if (xmlArgumentType.IsSingleton)
					{
						array[i] = XsltConvert.ToNode(args[i]);
					}
					else
					{
						array[i] = XsltConvert.ToNodeSet(args[i]);
					}
				}
				else
				{
					array[i] = args[i];
				}
				Type clrArgumentType = xmlExtensionFunction.GetClrArgumentType(i);
				if (xmlArgumentType.TypeCode == XmlTypeCode.Item || !clrArgumentType.IsAssignableFrom(array[i].GetType()))
				{
					array[i] = this.runtime.ChangeTypeXsltArgument(xmlArgumentType, array[i], clrArgumentType);
				}
			}
			object obj2 = xmlExtensionFunction.Invoke(obj, array);
			if (obj2 == null && xmlExtensionFunction.ClrReturnType == XsltConvert.VoidType)
			{
				return XmlQueryNodeSequence.Empty;
			}
			return (IList<XPathItem>)this.runtime.ChangeTypeXsltResult(XmlQueryTypeFactory.ItemS, obj2);
		}

		public void OnXsltMessageEncountered(string message)
		{
			XsltMessageEncounteredEventHandler xsltMessageEncounteredEventHandler = ((this.argList != null) ? this.argList.xsltMessageEncountered : null);
			if (xsltMessageEncounteredEventHandler != null)
			{
				xsltMessageEncounteredEventHandler(this, new XmlILQueryEventArgs(message));
				return;
			}
			Console.WriteLine(message);
		}

		private XmlQueryRuntime runtime;

		private XPathNavigator defaultDataSource;

		private XmlResolver dataSources;

		private Hashtable dataSourceCache;

		private XsltArgumentList argList;

		private XmlExtensionFunctionTable extFuncsLate;

		private WhitespaceRuleLookup wsRules;

		private QueryReaderSettings readerSettings;
	}
}
