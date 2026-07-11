using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using MS.Internal.Xml.XPath;

namespace System.Xml.XPath
{
	[DebuggerDisplay("{debuggerDisplayProxy}")]
	public abstract class XPathNavigator : XPathItem, ICloneable, IXPathNavigable, IXmlNamespaceResolver
	{
		public override string ToString()
		{
			return this.Value;
		}

		public sealed override bool IsNode
		{
			get
			{
				return true;
			}
		}

		public override XmlSchemaType XmlType
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo == null || schemaInfo.Validity != XmlSchemaValidity.Valid)
				{
					return null;
				}
				XmlSchemaType memberType = schemaInfo.MemberType;
				if (memberType != null)
				{
					return memberType;
				}
				return schemaInfo.SchemaType;
			}
		}

		public virtual void SetValue(string value)
		{
			throw new NotSupportedException();
		}

		public override object TypedValue
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaType.Datatype;
							if (xmlSchemaDatatype != null)
							{
								return xmlSchemaType.ValueConverter.ChangeType(this.Value, xmlSchemaDatatype.ValueType, this);
							}
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaType.Datatype;
							if (xmlSchemaDatatype != null)
							{
								return xmlSchemaType.ValueConverter.ChangeType(xmlSchemaDatatype.ParseValue(this.Value, this.NameTable, this), xmlSchemaDatatype.ValueType, this);
							}
						}
					}
				}
				return this.Value;
			}
		}

		public virtual void SetTypedValue(object typedValue)
		{
			if (typedValue == null)
			{
				throw new ArgumentNullException("typedValue");
			}
			XPathNodeType nodeType = this.NodeType;
			if (nodeType - XPathNodeType.Element > 1)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			string text = null;
			IXmlSchemaInfo schemaInfo = this.SchemaInfo;
			if (schemaInfo != null)
			{
				XmlSchemaType schemaType = schemaInfo.SchemaType;
				if (schemaType != null)
				{
					text = schemaType.ValueConverter.ToString(typedValue, this);
					XmlSchemaDatatype datatype = schemaType.Datatype;
					if (datatype != null)
					{
						datatype.ParseValue(text, this.NameTable, this);
					}
				}
			}
			if (text == null)
			{
				text = XmlUntypedConverter.Untyped.ToString(typedValue, this);
			}
			this.SetValue(text);
		}

		public override Type ValueType
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaType.Datatype;
							if (xmlSchemaDatatype != null)
							{
								return xmlSchemaDatatype.ValueType;
							}
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaType.Datatype;
							if (xmlSchemaDatatype != null)
							{
								return xmlSchemaDatatype.ValueType;
							}
						}
					}
				}
				return typeof(string);
			}
		}

		public override bool ValueAsBoolean
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							return xmlSchemaType.ValueConverter.ToBoolean(this.Value);
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
							if (datatype != null)
							{
								return xmlSchemaType.ValueConverter.ToBoolean(datatype.ParseValue(this.Value, this.NameTable, this));
							}
						}
					}
				}
				return XmlUntypedConverter.Untyped.ToBoolean(this.Value);
			}
		}

		public override DateTime ValueAsDateTime
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							return xmlSchemaType.ValueConverter.ToDateTime(this.Value);
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
							if (datatype != null)
							{
								return xmlSchemaType.ValueConverter.ToDateTime(datatype.ParseValue(this.Value, this.NameTable, this));
							}
						}
					}
				}
				return XmlUntypedConverter.Untyped.ToDateTime(this.Value);
			}
		}

		public override double ValueAsDouble
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							return xmlSchemaType.ValueConverter.ToDouble(this.Value);
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
							if (datatype != null)
							{
								return xmlSchemaType.ValueConverter.ToDouble(datatype.ParseValue(this.Value, this.NameTable, this));
							}
						}
					}
				}
				return XmlUntypedConverter.Untyped.ToDouble(this.Value);
			}
		}

		public override int ValueAsInt
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							return xmlSchemaType.ValueConverter.ToInt32(this.Value);
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
							if (datatype != null)
							{
								return xmlSchemaType.ValueConverter.ToInt32(datatype.ParseValue(this.Value, this.NameTable, this));
							}
						}
					}
				}
				return XmlUntypedConverter.Untyped.ToInt32(this.Value);
			}
		}

		public override long ValueAsLong
		{
			get
			{
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				if (schemaInfo != null)
				{
					if (schemaInfo.Validity == XmlSchemaValidity.Valid)
					{
						XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
						if (xmlSchemaType == null)
						{
							xmlSchemaType = schemaInfo.SchemaType;
						}
						if (xmlSchemaType != null)
						{
							return xmlSchemaType.ValueConverter.ToInt64(this.Value);
						}
					}
					else
					{
						XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
						if (xmlSchemaType != null)
						{
							XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
							if (datatype != null)
							{
								return xmlSchemaType.ValueConverter.ToInt64(datatype.ParseValue(this.Value, this.NameTable, this));
							}
						}
					}
				}
				return XmlUntypedConverter.Untyped.ToInt64(this.Value);
			}
		}

		public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
		{
			if (nsResolver == null)
			{
				nsResolver = this;
			}
			IXmlSchemaInfo schemaInfo = this.SchemaInfo;
			if (schemaInfo != null)
			{
				if (schemaInfo.Validity == XmlSchemaValidity.Valid)
				{
					XmlSchemaType xmlSchemaType = schemaInfo.MemberType;
					if (xmlSchemaType == null)
					{
						xmlSchemaType = schemaInfo.SchemaType;
					}
					if (xmlSchemaType != null)
					{
						return xmlSchemaType.ValueConverter.ChangeType(this.Value, returnType, nsResolver);
					}
				}
				else
				{
					XmlSchemaType xmlSchemaType = schemaInfo.SchemaType;
					if (xmlSchemaType != null)
					{
						XmlSchemaDatatype datatype = xmlSchemaType.Datatype;
						if (datatype != null)
						{
							return xmlSchemaType.ValueConverter.ChangeType(datatype.ParseValue(this.Value, this.NameTable, nsResolver), returnType, nsResolver);
						}
					}
				}
			}
			return XmlUntypedConverter.Untyped.ChangeType(this.Value, returnType, nsResolver);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public virtual XPathNavigator CreateNavigator()
		{
			return this.Clone();
		}

		public abstract XmlNameTable NameTable { get; }

		public virtual string LookupNamespace(string prefix)
		{
			if (prefix == null)
			{
				return null;
			}
			if (this.NodeType != XPathNodeType.Element)
			{
				XPathNavigator xpathNavigator = this.Clone();
				if (xpathNavigator.MoveToParent())
				{
					return xpathNavigator.LookupNamespace(prefix);
				}
			}
			else if (this.MoveToNamespace(prefix))
			{
				string value = this.Value;
				this.MoveToParent();
				return value;
			}
			if (prefix.Length == 0)
			{
				return string.Empty;
			}
			if (prefix == "xml")
			{
				return "http://www.w3.org/XML/1998/namespace";
			}
			if (prefix == "xmlns")
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			return null;
		}

		public virtual string LookupPrefix(string namespaceURI)
		{
			if (namespaceURI == null)
			{
				return null;
			}
			XPathNavigator xpathNavigator = this.Clone();
			if (this.NodeType != XPathNodeType.Element)
			{
				if (xpathNavigator.MoveToParent())
				{
					return xpathNavigator.LookupPrefix(namespaceURI);
				}
			}
			else if (xpathNavigator.MoveToFirstNamespace(XPathNamespaceScope.All))
			{
				while (!(namespaceURI == xpathNavigator.Value))
				{
					if (!xpathNavigator.MoveToNextNamespace(XPathNamespaceScope.All))
					{
						goto IL_004C;
					}
				}
				return xpathNavigator.LocalName;
			}
			IL_004C:
			if (namespaceURI == this.LookupNamespace(string.Empty))
			{
				return string.Empty;
			}
			if (namespaceURI == "http://www.w3.org/XML/1998/namespace")
			{
				return "xml";
			}
			if (namespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			return null;
		}

		public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			XPathNodeType nodeType = this.NodeType;
			if ((nodeType != XPathNodeType.Element && scope != XmlNamespaceScope.Local) || nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				XPathNavigator xpathNavigator = this.Clone();
				if (xpathNavigator.MoveToParent())
				{
					return xpathNavigator.GetNamespacesInScope(scope);
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (scope == XmlNamespaceScope.All)
			{
				dictionary["xml"] = "http://www.w3.org/XML/1998/namespace";
			}
			if (this.MoveToFirstNamespace((XPathNamespaceScope)scope))
			{
				do
				{
					string localName = this.LocalName;
					string value = this.Value;
					if (localName.Length != 0 || value.Length != 0 || scope == XmlNamespaceScope.Local)
					{
						dictionary[localName] = value;
					}
				}
				while (this.MoveToNextNamespace((XPathNamespaceScope)scope));
				this.MoveToParent();
			}
			return dictionary;
		}

		public static IEqualityComparer NavigatorComparer
		{
			get
			{
				return XPathNavigator.comparer;
			}
		}

		public abstract XPathNavigator Clone();

		public abstract XPathNodeType NodeType { get; }

		public abstract string LocalName { get; }

		public abstract string Name { get; }

		public abstract string NamespaceURI { get; }

		public abstract string Prefix { get; }

		public abstract string BaseURI { get; }

		public abstract bool IsEmptyElement { get; }

		public virtual string XmlLang
		{
			get
			{
				XPathNavigator xpathNavigator = this.Clone();
				while (!xpathNavigator.MoveToAttribute("lang", "http://www.w3.org/XML/1998/namespace"))
				{
					if (!xpathNavigator.MoveToParent())
					{
						return string.Empty;
					}
				}
				return xpathNavigator.Value;
			}
		}

		public virtual XmlReader ReadSubtree()
		{
			XPathNodeType nodeType = this.NodeType;
			if (nodeType > XPathNodeType.Element)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			return this.CreateReader();
		}

		public virtual void WriteSubtree(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteNode(this, true);
		}

		public virtual object UnderlyingObject
		{
			get
			{
				return null;
			}
		}

		public virtual bool HasAttributes
		{
			get
			{
				if (!this.MoveToFirstAttribute())
				{
					return false;
				}
				this.MoveToParent();
				return true;
			}
		}

		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			if (!this.MoveToAttribute(localName, namespaceURI))
			{
				return "";
			}
			string value = this.Value;
			this.MoveToParent();
			return value;
		}

		public virtual bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (this.MoveToFirstAttribute())
			{
				while (!(localName == this.LocalName) || !(namespaceURI == this.NamespaceURI))
				{
					if (!this.MoveToNextAttribute())
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public abstract bool MoveToFirstAttribute();

		public abstract bool MoveToNextAttribute();

		public virtual string GetNamespace(string name)
		{
			if (this.MoveToNamespace(name))
			{
				string value = this.Value;
				this.MoveToParent();
				return value;
			}
			if (name == "xml")
			{
				return "http://www.w3.org/XML/1998/namespace";
			}
			if (name == "xmlns")
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			return string.Empty;
		}

		public virtual bool MoveToNamespace(string name)
		{
			if (this.MoveToFirstNamespace(XPathNamespaceScope.All))
			{
				while (!(name == this.LocalName))
				{
					if (!this.MoveToNextNamespace(XPathNamespaceScope.All))
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public abstract bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope);

		public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);

		public bool MoveToFirstNamespace()
		{
			return this.MoveToFirstNamespace(XPathNamespaceScope.All);
		}

		public bool MoveToNextNamespace()
		{
			return this.MoveToNextNamespace(XPathNamespaceScope.All);
		}

		public abstract bool MoveToNext();

		public abstract bool MoveToPrevious();

		public virtual bool MoveToFirst()
		{
			XPathNodeType nodeType = this.NodeType;
			return nodeType - XPathNodeType.Attribute > 1 && this.MoveToParent() && this.MoveToFirstChild();
		}

		public abstract bool MoveToFirstChild();

		public abstract bool MoveToParent();

		public virtual void MoveToRoot()
		{
			while (this.MoveToParent())
			{
			}
		}

		public abstract bool MoveTo(XPathNavigator other);

		public abstract bool MoveToId(string id);

		public virtual bool MoveToChild(string localName, string namespaceURI)
		{
			if (this.MoveToFirstChild())
			{
				while (this.NodeType != XPathNodeType.Element || !(localName == this.LocalName) || !(namespaceURI == this.NamespaceURI))
				{
					if (!this.MoveToNext())
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public virtual bool MoveToChild(XPathNodeType type)
		{
			if (this.MoveToFirstChild())
			{
				int contentKindMask = XPathNavigator.GetContentKindMask(type);
				while (((1 << (int)this.NodeType) & contentKindMask) == 0)
				{
					if (!this.MoveToNext())
					{
						this.MoveToParent();
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI)
		{
			return this.MoveToFollowing(localName, namespaceURI, null);
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			XPathNavigator xpathNavigator = this.Clone();
			XPathNodeType xpathNodeType;
			if (end != null)
			{
				xpathNodeType = end.NodeType;
				if (xpathNodeType - XPathNodeType.Attribute <= 1)
				{
					end = end.Clone();
					end.MoveToNonDescendant();
				}
			}
			xpathNodeType = this.NodeType;
			if (xpathNodeType - XPathNodeType.Attribute <= 1 && !this.MoveToParent())
			{
				return false;
			}
			for (;;)
			{
				if (!this.MoveToFirstChild())
				{
					while (!this.MoveToNext())
					{
						if (!this.MoveToParent())
						{
							goto Block_6;
						}
					}
				}
				if (end != null && this.IsSamePosition(end))
				{
					goto Block_8;
				}
				if (this.NodeType == XPathNodeType.Element && !(localName != this.LocalName) && !(namespaceURI != this.NamespaceURI))
				{
					return true;
				}
			}
			Block_6:
			this.MoveTo(xpathNavigator);
			return false;
			Block_8:
			this.MoveTo(xpathNavigator);
			return false;
		}

		public virtual bool MoveToFollowing(XPathNodeType type)
		{
			return this.MoveToFollowing(type, null);
		}

		public virtual bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			XPathNavigator xpathNavigator = this.Clone();
			int contentKindMask = XPathNavigator.GetContentKindMask(type);
			XPathNodeType xpathNodeType;
			if (end != null)
			{
				xpathNodeType = end.NodeType;
				if (xpathNodeType - XPathNodeType.Attribute <= 1)
				{
					end = end.Clone();
					end.MoveToNonDescendant();
				}
			}
			xpathNodeType = this.NodeType;
			if (xpathNodeType - XPathNodeType.Attribute <= 1 && !this.MoveToParent())
			{
				return false;
			}
			for (;;)
			{
				if (!this.MoveToFirstChild())
				{
					while (!this.MoveToNext())
					{
						if (!this.MoveToParent())
						{
							goto Block_6;
						}
					}
				}
				if (end != null && this.IsSamePosition(end))
				{
					goto Block_8;
				}
				if (((1 << (int)this.NodeType) & contentKindMask) != 0)
				{
					return true;
				}
			}
			Block_6:
			this.MoveTo(xpathNavigator);
			return false;
			Block_8:
			this.MoveTo(xpathNavigator);
			return false;
		}

		public virtual bool MoveToNext(string localName, string namespaceURI)
		{
			XPathNavigator xpathNavigator = this.Clone();
			while (this.MoveToNext())
			{
				if (this.NodeType == XPathNodeType.Element && localName == this.LocalName && namespaceURI == this.NamespaceURI)
				{
					return true;
				}
			}
			this.MoveTo(xpathNavigator);
			return false;
		}

		public virtual bool MoveToNext(XPathNodeType type)
		{
			XPathNavigator xpathNavigator = this.Clone();
			int contentKindMask = XPathNavigator.GetContentKindMask(type);
			while (this.MoveToNext())
			{
				if (((1 << (int)this.NodeType) & contentKindMask) != 0)
				{
					return true;
				}
			}
			this.MoveTo(xpathNavigator);
			return false;
		}

		public virtual bool HasChildren
		{
			get
			{
				if (this.MoveToFirstChild())
				{
					this.MoveToParent();
					return true;
				}
				return false;
			}
		}

		public abstract bool IsSamePosition(XPathNavigator other);

		public virtual bool IsDescendant(XPathNavigator nav)
		{
			if (nav != null)
			{
				nav = nav.Clone();
				while (nav.MoveToParent())
				{
					if (nav.IsSamePosition(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			if (nav == null)
			{
				return XmlNodeOrder.Unknown;
			}
			if (this.IsSamePosition(nav))
			{
				return XmlNodeOrder.Same;
			}
			XPathNavigator xpathNavigator = this.Clone();
			XPathNavigator xpathNavigator2 = nav.Clone();
			int i = XPathNavigator.GetDepth(xpathNavigator.Clone());
			int j = XPathNavigator.GetDepth(xpathNavigator2.Clone());
			if (i > j)
			{
				while (i > j)
				{
					xpathNavigator.MoveToParent();
					i--;
				}
				if (xpathNavigator.IsSamePosition(xpathNavigator2))
				{
					return XmlNodeOrder.After;
				}
			}
			if (j > i)
			{
				while (j > i)
				{
					xpathNavigator2.MoveToParent();
					j--;
				}
				if (xpathNavigator.IsSamePosition(xpathNavigator2))
				{
					return XmlNodeOrder.Before;
				}
			}
			XPathNavigator xpathNavigator3 = xpathNavigator.Clone();
			XPathNavigator xpathNavigator4 = xpathNavigator2.Clone();
			while (xpathNavigator3.MoveToParent() && xpathNavigator4.MoveToParent())
			{
				if (xpathNavigator3.IsSamePosition(xpathNavigator4))
				{
					xpathNavigator.GetType().ToString() != "Microsoft.VisualStudio.Modeling.StoreNavigator";
					return this.CompareSiblings(xpathNavigator, xpathNavigator2);
				}
				xpathNavigator.MoveToParent();
				xpathNavigator2.MoveToParent();
			}
			return XmlNodeOrder.Unknown;
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this as IXmlSchemaInfo;
			}
		}

		public virtual bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler validationEventHandler)
		{
			XmlSchemaType xmlSchemaType = null;
			XmlSchemaElement xmlSchemaElement = null;
			XmlSchemaAttribute xmlSchemaAttribute = null;
			switch (this.NodeType)
			{
			case XPathNodeType.Root:
				if (schemas == null)
				{
					throw new InvalidOperationException(Res.GetString("An XmlSchemaSet must be provided to validate the document."));
				}
				xmlSchemaType = null;
				break;
			case XPathNodeType.Element:
			{
				if (schemas == null)
				{
					throw new InvalidOperationException(Res.GetString("An XmlSchemaSet must be provided to validate the document."));
				}
				IXmlSchemaInfo xmlSchemaInfo = this.SchemaInfo;
				if (xmlSchemaInfo != null)
				{
					xmlSchemaType = xmlSchemaInfo.SchemaType;
					xmlSchemaElement = xmlSchemaInfo.SchemaElement;
				}
				if (xmlSchemaType == null && xmlSchemaElement == null)
				{
					throw new InvalidOperationException(Res.GetString("Element should have prior schema information to call this method.", null));
				}
				break;
			}
			case XPathNodeType.Attribute:
			{
				if (schemas == null)
				{
					throw new InvalidOperationException(Res.GetString("An XmlSchemaSet must be provided to validate the document."));
				}
				IXmlSchemaInfo xmlSchemaInfo = this.SchemaInfo;
				if (xmlSchemaInfo != null)
				{
					xmlSchemaType = xmlSchemaInfo.SchemaType;
					xmlSchemaAttribute = xmlSchemaInfo.SchemaAttribute;
				}
				if (xmlSchemaType == null && xmlSchemaAttribute == null)
				{
					throw new InvalidOperationException(Res.GetString("Element should have prior schema information to call this method.", null));
				}
				break;
			}
			default:
				throw new InvalidOperationException(Res.GetString("Validate and CheckValidity are only allowed on Root or Element nodes.", null));
			}
			XmlReader xmlReader = this.CreateReader();
			XPathNavigator.CheckValidityHelper checkValidityHelper = new XPathNavigator.CheckValidityHelper(validationEventHandler, xmlReader as XPathNavigatorReader);
			validationEventHandler = new ValidationEventHandler(checkValidityHelper.ValidationCallback);
			XmlReader validatingReader = this.GetValidatingReader(xmlReader, schemas, validationEventHandler, xmlSchemaType, xmlSchemaElement, xmlSchemaAttribute);
			while (validatingReader.Read())
			{
			}
			return checkValidityHelper.IsValid;
		}

		private XmlReader GetValidatingReader(XmlReader reader, XmlSchemaSet schemas, ValidationEventHandler validationEvent, XmlSchemaType schemaType, XmlSchemaElement schemaElement, XmlSchemaAttribute schemaAttribute)
		{
			if (schemaAttribute != null)
			{
				return schemaAttribute.Validate(reader, null, schemas, validationEvent);
			}
			if (schemaElement != null)
			{
				return schemaElement.Validate(reader, null, schemas, validationEvent);
			}
			if (schemaType != null)
			{
				return schemaType.Validate(reader, null, schemas, validationEvent);
			}
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			xmlReaderSettings.Schemas = schemas;
			xmlReaderSettings.ValidationEventHandler += validationEvent;
			return XmlReader.Create(reader, xmlReaderSettings);
		}

		public virtual XPathExpression Compile(string xpath)
		{
			return XPathExpression.Compile(xpath);
		}

		public virtual XPathNavigator SelectSingleNode(string xpath)
		{
			return this.SelectSingleNode(XPathExpression.Compile(xpath));
		}

		public virtual XPathNavigator SelectSingleNode(string xpath, IXmlNamespaceResolver resolver)
		{
			return this.SelectSingleNode(XPathExpression.Compile(xpath, resolver));
		}

		public virtual XPathNavigator SelectSingleNode(XPathExpression expression)
		{
			XPathNodeIterator xpathNodeIterator = this.Select(expression);
			if (xpathNodeIterator.MoveNext())
			{
				return xpathNodeIterator.Current;
			}
			return null;
		}

		public virtual XPathNodeIterator Select(string xpath)
		{
			return this.Select(XPathExpression.Compile(xpath));
		}

		public virtual XPathNodeIterator Select(string xpath, IXmlNamespaceResolver resolver)
		{
			return this.Select(XPathExpression.Compile(xpath, resolver));
		}

		public virtual XPathNodeIterator Select(XPathExpression expr)
		{
			XPathNodeIterator xpathNodeIterator = this.Evaluate(expr) as XPathNodeIterator;
			if (xpathNodeIterator == null)
			{
				throw XPathException.Create("Expression must evaluate to a node-set.");
			}
			return xpathNodeIterator;
		}

		public virtual object Evaluate(string xpath)
		{
			return this.Evaluate(XPathExpression.Compile(xpath), null);
		}

		public virtual object Evaluate(string xpath, IXmlNamespaceResolver resolver)
		{
			return this.Evaluate(XPathExpression.Compile(xpath, resolver));
		}

		public virtual object Evaluate(XPathExpression expr)
		{
			return this.Evaluate(expr, null);
		}

		public virtual object Evaluate(XPathExpression expr, XPathNodeIterator context)
		{
			CompiledXpathExpr compiledXpathExpr = expr as CompiledXpathExpr;
			if (compiledXpathExpr == null)
			{
				throw XPathException.Create("This is an invalid object. Only objects returned from Compile() can be passed as input.");
			}
			Query query = Query.Clone(compiledXpathExpr.QueryTree);
			query.Reset();
			if (context == null)
			{
				context = new XPathSingletonIterator(this.Clone(), true);
			}
			object obj = query.Evaluate(context);
			if (obj is XPathNodeIterator)
			{
				return new XPathSelectionIterator(context.Current, query);
			}
			return obj;
		}

		public virtual bool Matches(XPathExpression expr)
		{
			CompiledXpathExpr compiledXpathExpr = expr as CompiledXpathExpr;
			if (compiledXpathExpr == null)
			{
				throw XPathException.Create("This is an invalid object. Only objects returned from Compile() can be passed as input.");
			}
			Query query = Query.Clone(compiledXpathExpr.QueryTree);
			bool flag;
			try
			{
				flag = query.MatchNode(this) != null;
			}
			catch (XPathException)
			{
				throw XPathException.Create("'{0}' is an invalid XSLT pattern.", compiledXpathExpr.Expression);
			}
			return flag;
		}

		public virtual bool Matches(string xpath)
		{
			return this.Matches(XPathNavigator.CompileMatchPattern(xpath));
		}

		public virtual XPathNodeIterator SelectChildren(XPathNodeType type)
		{
			return new XPathChildIterator(this.Clone(), type);
		}

		public virtual XPathNodeIterator SelectChildren(string name, string namespaceURI)
		{
			return new XPathChildIterator(this.Clone(), name, namespaceURI);
		}

		public virtual XPathNodeIterator SelectAncestors(XPathNodeType type, bool matchSelf)
		{
			return new XPathAncestorIterator(this.Clone(), type, matchSelf);
		}

		public virtual XPathNodeIterator SelectAncestors(string name, string namespaceURI, bool matchSelf)
		{
			return new XPathAncestorIterator(this.Clone(), name, namespaceURI, matchSelf);
		}

		public virtual XPathNodeIterator SelectDescendants(XPathNodeType type, bool matchSelf)
		{
			return new XPathDescendantIterator(this.Clone(), type, matchSelf);
		}

		public virtual XPathNodeIterator SelectDescendants(string name, string namespaceURI, bool matchSelf)
		{
			return new XPathDescendantIterator(this.Clone(), name, namespaceURI, matchSelf);
		}

		public virtual bool CanEdit
		{
			get
			{
				return false;
			}
		}

		public virtual XmlWriter PrependChild()
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter AppendChild()
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter InsertAfter()
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter InsertBefore()
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter CreateAttributes()
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter ReplaceRange(XPathNavigator lastSiblingToReplace)
		{
			throw new NotSupportedException();
		}

		public virtual void ReplaceSelf(string newNode)
		{
			XmlReader xmlReader = this.CreateContextReader(newNode, false);
			this.ReplaceSelf(xmlReader);
		}

		public virtual void ReplaceSelf(XmlReader newNode)
		{
			if (newNode == null)
			{
				throw new ArgumentNullException("newNode");
			}
			XPathNodeType nodeType = this.NodeType;
			if (nodeType == XPathNodeType.Root || nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			XmlWriter xmlWriter = this.ReplaceRange(this);
			this.BuildSubtree(newNode, xmlWriter);
			xmlWriter.Close();
		}

		public virtual void ReplaceSelf(XPathNavigator newNode)
		{
			if (newNode == null)
			{
				throw new ArgumentNullException("newNode");
			}
			XmlReader xmlReader = newNode.CreateReader();
			this.ReplaceSelf(xmlReader);
		}

		public virtual string OuterXml
		{
			get
			{
				if (this.NodeType == XPathNodeType.Attribute)
				{
					return this.Name + "=\"" + this.Value + "\"";
				}
				if (this.NodeType != XPathNodeType.Namespace)
				{
					StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
					{
						Indent = true,
						OmitXmlDeclaration = true,
						ConformanceLevel = ConformanceLevel.Auto
					});
					try
					{
						xmlWriter.WriteNode(this, true);
					}
					finally
					{
						xmlWriter.Close();
					}
					return stringWriter.ToString();
				}
				if (this.LocalName.Length == 0)
				{
					return "xmlns=\"" + this.Value + "\"";
				}
				return string.Concat(new string[] { "xmlns:", this.LocalName, "=\"", this.Value, "\"" });
			}
			set
			{
				this.ReplaceSelf(value);
			}
		}

		public virtual string InnerXml
		{
			get
			{
				XPathNodeType nodeType = this.NodeType;
				if (nodeType <= XPathNodeType.Element)
				{
					StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
					{
						Indent = true,
						OmitXmlDeclaration = true,
						ConformanceLevel = ConformanceLevel.Auto
					});
					try
					{
						if (this.MoveToFirstChild())
						{
							do
							{
								xmlWriter.WriteNode(this, true);
							}
							while (this.MoveToNext());
							this.MoveToParent();
						}
					}
					finally
					{
						xmlWriter.Close();
					}
					return stringWriter.ToString();
				}
				if (nodeType - XPathNodeType.Attribute > 1)
				{
					return string.Empty;
				}
				return this.Value;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				XPathNodeType nodeType = this.NodeType;
				if (nodeType > XPathNodeType.Element)
				{
					if (nodeType != XPathNodeType.Attribute)
					{
						throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
					}
					this.SetValue(value);
					return;
				}
				else
				{
					XPathNavigator xpathNavigator = this.CreateNavigator();
					while (xpathNavigator.MoveToFirstChild())
					{
						xpathNavigator.DeleteSelf();
					}
					if (value.Length != 0)
					{
						xpathNavigator.AppendChild(value);
						return;
					}
					return;
				}
			}
		}

		public virtual void AppendChild(string newChild)
		{
			XmlReader xmlReader = this.CreateContextReader(newChild, true);
			this.AppendChild(xmlReader);
		}

		public virtual void AppendChild(XmlReader newChild)
		{
			if (newChild == null)
			{
				throw new ArgumentNullException("newChild");
			}
			XmlWriter xmlWriter = this.AppendChild();
			this.BuildSubtree(newChild, xmlWriter);
			xmlWriter.Close();
		}

		public virtual void AppendChild(XPathNavigator newChild)
		{
			if (newChild == null)
			{
				throw new ArgumentNullException("newChild");
			}
			if (!this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			XmlReader xmlReader = newChild.CreateReader();
			this.AppendChild(xmlReader);
		}

		public virtual void PrependChild(string newChild)
		{
			XmlReader xmlReader = this.CreateContextReader(newChild, true);
			this.PrependChild(xmlReader);
		}

		public virtual void PrependChild(XmlReader newChild)
		{
			if (newChild == null)
			{
				throw new ArgumentNullException("newChild");
			}
			XmlWriter xmlWriter = this.PrependChild();
			this.BuildSubtree(newChild, xmlWriter);
			xmlWriter.Close();
		}

		public virtual void PrependChild(XPathNavigator newChild)
		{
			if (newChild == null)
			{
				throw new ArgumentNullException("newChild");
			}
			if (!this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			XmlReader xmlReader = newChild.CreateReader();
			this.PrependChild(xmlReader);
		}

		public virtual void InsertBefore(string newSibling)
		{
			XmlReader xmlReader = this.CreateContextReader(newSibling, false);
			this.InsertBefore(xmlReader);
		}

		public virtual void InsertBefore(XmlReader newSibling)
		{
			if (newSibling == null)
			{
				throw new ArgumentNullException("newSibling");
			}
			XmlWriter xmlWriter = this.InsertBefore();
			this.BuildSubtree(newSibling, xmlWriter);
			xmlWriter.Close();
		}

		public virtual void InsertBefore(XPathNavigator newSibling)
		{
			if (newSibling == null)
			{
				throw new ArgumentNullException("newSibling");
			}
			if (!this.IsValidSiblingType(newSibling.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			XmlReader xmlReader = newSibling.CreateReader();
			this.InsertBefore(xmlReader);
		}

		public virtual void InsertAfter(string newSibling)
		{
			XmlReader xmlReader = this.CreateContextReader(newSibling, false);
			this.InsertAfter(xmlReader);
		}

		public virtual void InsertAfter(XmlReader newSibling)
		{
			if (newSibling == null)
			{
				throw new ArgumentNullException("newSibling");
			}
			XmlWriter xmlWriter = this.InsertAfter();
			this.BuildSubtree(newSibling, xmlWriter);
			xmlWriter.Close();
		}

		public virtual void InsertAfter(XPathNavigator newSibling)
		{
			if (newSibling == null)
			{
				throw new ArgumentNullException("newSibling");
			}
			if (!this.IsValidSiblingType(newSibling.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current position of the navigator."));
			}
			XmlReader xmlReader = newSibling.CreateReader();
			this.InsertAfter(xmlReader);
		}

		public virtual void DeleteRange(XPathNavigator lastSiblingToDelete)
		{
			throw new NotSupportedException();
		}

		public virtual void DeleteSelf()
		{
			this.DeleteRange(this);
		}

		public virtual void PrependChildElement(string prefix, string localName, string namespaceURI, string value)
		{
			XmlWriter xmlWriter = this.PrependChild();
			xmlWriter.WriteStartElement(prefix, localName, namespaceURI);
			if (value != null)
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void AppendChildElement(string prefix, string localName, string namespaceURI, string value)
		{
			XmlWriter xmlWriter = this.AppendChild();
			xmlWriter.WriteStartElement(prefix, localName, namespaceURI);
			if (value != null)
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void InsertElementBefore(string prefix, string localName, string namespaceURI, string value)
		{
			XmlWriter xmlWriter = this.InsertBefore();
			xmlWriter.WriteStartElement(prefix, localName, namespaceURI);
			if (value != null)
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void InsertElementAfter(string prefix, string localName, string namespaceURI, string value)
		{
			XmlWriter xmlWriter = this.InsertAfter();
			xmlWriter.WriteStartElement(prefix, localName, namespaceURI);
			if (value != null)
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void CreateAttribute(string prefix, string localName, string namespaceURI, string value)
		{
			XmlWriter xmlWriter = this.CreateAttributes();
			xmlWriter.WriteStartAttribute(prefix, localName, namespaceURI);
			if (value != null)
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndAttribute();
			xmlWriter.Close();
		}

		internal bool MoveToPrevious(string localName, string namespaceURI)
		{
			XPathNavigator xpathNavigator = this.Clone();
			localName = ((localName != null) ? this.NameTable.Get(localName) : null);
			while (this.MoveToPrevious())
			{
				if (this.NodeType == XPathNodeType.Element && localName == this.LocalName && namespaceURI == this.NamespaceURI)
				{
					return true;
				}
			}
			this.MoveTo(xpathNavigator);
			return false;
		}

		internal bool MoveToPrevious(XPathNodeType type)
		{
			XPathNavigator xpathNavigator = this.Clone();
			int contentKindMask = XPathNavigator.GetContentKindMask(type);
			while (this.MoveToPrevious())
			{
				if (((1 << (int)this.NodeType) & contentKindMask) != 0)
				{
					return true;
				}
			}
			this.MoveTo(xpathNavigator);
			return false;
		}

		internal bool MoveToNonDescendant()
		{
			if (this.NodeType == XPathNodeType.Root)
			{
				return false;
			}
			if (this.MoveToNext())
			{
				return true;
			}
			XPathNavigator xpathNavigator = this.Clone();
			if (!this.MoveToParent())
			{
				return false;
			}
			XPathNodeType nodeType = xpathNavigator.NodeType;
			if (nodeType - XPathNodeType.Attribute <= 1 && this.MoveToFirstChild())
			{
				return true;
			}
			while (!this.MoveToNext())
			{
				if (!this.MoveToParent())
				{
					this.MoveTo(xpathNavigator);
					return false;
				}
			}
			return true;
		}

		internal uint IndexInParent
		{
			get
			{
				XPathNavigator xpathNavigator = this.Clone();
				uint num = 0U;
				XPathNodeType nodeType = this.NodeType;
				if (nodeType != XPathNodeType.Attribute)
				{
					if (nodeType != XPathNodeType.Namespace)
					{
						while (xpathNavigator.MoveToNext())
						{
							num += 1U;
						}
					}
					else
					{
						while (xpathNavigator.MoveToNextNamespace())
						{
							num += 1U;
						}
					}
				}
				else
				{
					while (xpathNavigator.MoveToNextAttribute())
					{
						num += 1U;
					}
				}
				return num;
			}
		}

		internal virtual string UniqueId
		{
			get
			{
				XPathNavigator xpathNavigator = this.Clone();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(XPathNavigator.NodeTypeLetter[(int)this.NodeType]);
				for (;;)
				{
					uint num = xpathNavigator.IndexInParent;
					if (!xpathNavigator.MoveToParent())
					{
						break;
					}
					if (num <= 31U)
					{
						stringBuilder.Append(XPathNavigator.UniqueIdTbl[(int)num]);
					}
					else
					{
						stringBuilder.Append('0');
						do
						{
							stringBuilder.Append(XPathNavigator.UniqueIdTbl[(int)(num & 31U)]);
							num >>= 5;
						}
						while (num != 0U);
						stringBuilder.Append('0');
					}
				}
				return stringBuilder.ToString();
			}
		}

		private static XPathExpression CompileMatchPattern(string xpath)
		{
			bool flag;
			return new CompiledXpathExpr(new QueryBuilder().BuildPatternQuery(xpath, out flag), xpath, flag);
		}

		private static int GetDepth(XPathNavigator nav)
		{
			int num = 0;
			while (nav.MoveToParent())
			{
				num++;
			}
			return num;
		}

		private XmlNodeOrder CompareSiblings(XPathNavigator n1, XPathNavigator n2)
		{
			int num = 0;
			XPathNodeType xpathNodeType = n1.NodeType;
			if (xpathNodeType != XPathNodeType.Attribute)
			{
				if (xpathNodeType != XPathNodeType.Namespace)
				{
					num += 2;
				}
			}
			else
			{
				num++;
			}
			xpathNodeType = n2.NodeType;
			if (xpathNodeType != XPathNodeType.Attribute)
			{
				if (xpathNodeType == XPathNodeType.Namespace)
				{
					if (num == 0)
					{
						while (n1.MoveToNextNamespace())
						{
							if (n1.IsSamePosition(n2))
							{
								return XmlNodeOrder.Before;
							}
						}
					}
				}
				else
				{
					num -= 2;
					if (num == 0)
					{
						while (n1.MoveToNext())
						{
							if (n1.IsSamePosition(n2))
							{
								return XmlNodeOrder.Before;
							}
						}
					}
				}
			}
			else
			{
				num--;
				if (num == 0)
				{
					while (n1.MoveToNextAttribute())
					{
						if (n1.IsSamePosition(n2))
						{
							return XmlNodeOrder.Before;
						}
					}
				}
			}
			if (num >= 0)
			{
				return XmlNodeOrder.After;
			}
			return XmlNodeOrder.Before;
		}

		internal static XmlNamespaceManager GetNamespaces(IXmlNamespaceResolver resolver)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			foreach (KeyValuePair<string, string> keyValuePair in resolver.GetNamespacesInScope(XmlNamespaceScope.All))
			{
				if (keyValuePair.Key != "xmlns")
				{
					xmlNamespaceManager.AddNamespace(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return xmlNamespaceManager;
		}

		internal static int GetContentKindMask(XPathNodeType type)
		{
			return XPathNavigator.ContentKindMasks[(int)type];
		}

		internal static int GetKindMask(XPathNodeType type)
		{
			if (type == XPathNodeType.All)
			{
				return int.MaxValue;
			}
			if (type == XPathNodeType.Text)
			{
				return 112;
			}
			return 1 << (int)type;
		}

		internal static bool IsText(XPathNodeType type)
		{
			return type - XPathNodeType.Text <= 2;
		}

		private bool IsValidChildType(XPathNodeType type)
		{
			XPathNodeType nodeType = this.NodeType;
			if (nodeType != XPathNodeType.Root)
			{
				if (nodeType == XPathNodeType.Element)
				{
					if (type == XPathNodeType.Element || type - XPathNodeType.Text <= 4)
					{
						return true;
					}
				}
			}
			else if (type == XPathNodeType.Element || type - XPathNodeType.SignificantWhitespace <= 3)
			{
				return true;
			}
			return false;
		}

		private bool IsValidSiblingType(XPathNodeType type)
		{
			XPathNodeType nodeType = this.NodeType;
			return (nodeType == XPathNodeType.Element || nodeType - XPathNodeType.Text <= 4) && (type == XPathNodeType.Element || type - XPathNodeType.Text <= 4);
		}

		private XmlReader CreateReader()
		{
			return XPathNavigatorReader.Create(this);
		}

		private XmlReader CreateContextReader(string xml, bool fromCurrentNode)
		{
			if (xml == null)
			{
				throw new ArgumentNullException("xml");
			}
			XPathNavigator xpathNavigator = this.CreateNavigator();
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this.NameTable);
			if (!fromCurrentNode)
			{
				xpathNavigator.MoveToParent();
			}
			if (xpathNavigator.MoveToFirstNamespace(XPathNamespaceScope.All))
			{
				do
				{
					xmlNamespaceManager.AddNamespace(xpathNavigator.LocalName, xpathNavigator.Value);
				}
				while (xpathNavigator.MoveToNextNamespace(XPathNamespaceScope.All));
			}
			XmlParserContext xmlParserContext = new XmlParserContext(this.NameTable, xmlNamespaceManager, null, XmlSpace.Default);
			return new XmlTextReader(xml, XmlNodeType.Element, xmlParserContext)
			{
				WhitespaceHandling = WhitespaceHandling.Significant
			};
		}

		internal void BuildSubtree(XmlReader reader, XmlWriter writer)
		{
			string text = "http://www.w3.org/2000/xmlns/";
			ReadState readState = reader.ReadState;
			if (readState != ReadState.Initial && readState != ReadState.Interactive)
			{
				throw new ArgumentException(Res.GetString("Operation is not valid due to the current state of the object."), "reader");
			}
			int num = 0;
			if (readState == ReadState.Initial)
			{
				if (!reader.Read())
				{
					return;
				}
				num++;
			}
			do
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					bool isEmptyElement = reader.IsEmptyElement;
					while (reader.MoveToNextAttribute())
					{
						if (reader.NamespaceURI == text)
						{
							if (reader.Prefix.Length == 0)
							{
								writer.WriteAttributeString("", "xmlns", text, reader.Value);
							}
							else
							{
								writer.WriteAttributeString("xmlns", reader.LocalName, text, reader.Value);
							}
						}
						else
						{
							writer.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
							writer.WriteString(reader.Value);
							writer.WriteEndAttribute();
						}
					}
					reader.MoveToElement();
					if (isEmptyElement)
					{
						writer.WriteEndElement();
					}
					else
					{
						num++;
					}
					break;
				}
				case XmlNodeType.Attribute:
					if (reader.NamespaceURI == text)
					{
						if (reader.Prefix.Length == 0)
						{
							writer.WriteAttributeString("", "xmlns", text, reader.Value);
						}
						else
						{
							writer.WriteAttributeString("xmlns", reader.LocalName, text, reader.Value);
						}
					}
					else
					{
						writer.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						writer.WriteString(reader.Value);
						writer.WriteEndAttribute();
					}
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					writer.WriteString(reader.Value);
					break;
				case XmlNodeType.EntityReference:
					reader.ResolveEntity();
					break;
				case XmlNodeType.ProcessingInstruction:
					writer.WriteProcessingInstruction(reader.LocalName, reader.Value);
					break;
				case XmlNodeType.Comment:
					writer.WriteComment(reader.Value);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					writer.WriteString(reader.Value);
					break;
				case XmlNodeType.EndElement:
					writer.WriteFullEndElement();
					num--;
					break;
				}
			}
			while (reader.Read() && num > 0);
		}

		private object debuggerDisplayProxy
		{
			get
			{
				return new XPathNavigator.DebuggerDisplayProxy(this);
			}
		}

		internal static readonly XPathNavigatorKeyComparer comparer = new XPathNavigatorKeyComparer();

		internal static readonly char[] NodeTypeLetter = new char[] { 'R', 'E', 'A', 'N', 'T', 'S', 'W', 'P', 'C', 'X' };

		internal static readonly char[] UniqueIdTbl = new char[]
		{
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4',
			'5', '6'
		};

		internal const int AllMask = 2147483647;

		internal const int NoAttrNmspMask = 2147483635;

		internal const int TextMask = 112;

		internal static readonly int[] ContentKindMasks = new int[] { 1, 2, 0, 0, 112, 32, 64, 128, 256, 2147483635 };

		private class CheckValidityHelper
		{
			internal CheckValidityHelper(ValidationEventHandler nextEventHandler, XPathNavigatorReader reader)
			{
				this.isValid = true;
				this.nextEventHandler = nextEventHandler;
				this.reader = reader;
			}

			internal void ValidationCallback(object sender, ValidationEventArgs args)
			{
				if (args.Severity == XmlSeverityType.Error)
				{
					this.isValid = false;
				}
				XmlSchemaValidationException ex = args.Exception as XmlSchemaValidationException;
				if (ex != null && this.reader != null)
				{
					ex.SetSourceObject(this.reader.UnderlyingObject);
				}
				if (this.nextEventHandler != null)
				{
					this.nextEventHandler(sender, args);
					return;
				}
				if (ex != null && args.Severity == XmlSeverityType.Error)
				{
					throw ex;
				}
			}

			internal bool IsValid
			{
				get
				{
					return this.isValid;
				}
			}

			private bool isValid;

			private ValidationEventHandler nextEventHandler;

			private XPathNavigatorReader reader;
		}

		[DebuggerDisplay("{ToString()}")]
		internal struct DebuggerDisplayProxy
		{
			public DebuggerDisplayProxy(XPathNavigator nav)
			{
				this.nav = nav;
			}

			public override string ToString()
			{
				string text = this.nav.NodeType.ToString();
				switch (this.nav.NodeType)
				{
				case XPathNodeType.Element:
					text = text + ", Name=\"" + this.nav.Name + "\"";
					break;
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
				case XPathNodeType.ProcessingInstruction:
					text = text + ", Name=\"" + this.nav.Name + "\"";
					text = text + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.nav.Value) + "\"";
					break;
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.Comment:
					text = text + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.nav.Value) + "\"";
					break;
				}
				return text;
			}

			private XPathNavigator nav;
		}
	}
}
