using System;
using System.CodeDom.Compiler;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlMemberMapping
	{
		internal XmlMemberMapping(string memberName, string defaultNamespace, XmlTypeMapMember mapMem, bool encodedFormat)
		{
			this._mapMember = mapMem;
			this._memberName = memberName;
			if (mapMem is XmlTypeMapMemberAnyElement)
			{
				XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = (XmlTypeMapMemberAnyElement)mapMem;
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)xmlTypeMapMemberAnyElement.ElementInfo[xmlTypeMapMemberAnyElement.ElementInfo.Count - 1];
				this._elementName = xmlTypeMapElementInfo.ElementName;
				this._namespace = xmlTypeMapElementInfo.Namespace;
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					this._typeNamespace = xmlTypeMapElementInfo.MappedType.Namespace;
				}
				else
				{
					this._typeNamespace = string.Empty;
				}
			}
			else if (mapMem is XmlTypeMapMemberElement)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo2 = (XmlTypeMapElementInfo)((XmlTypeMapMemberElement)mapMem).ElementInfo[0];
				this._elementName = xmlTypeMapElementInfo2.ElementName;
				if (encodedFormat)
				{
					this._namespace = defaultNamespace;
					if (xmlTypeMapElementInfo2.MappedType != null)
					{
						this._typeNamespace = string.Empty;
					}
					else
					{
						this._typeNamespace = xmlTypeMapElementInfo2.DataTypeNamespace;
					}
				}
				else
				{
					this._namespace = xmlTypeMapElementInfo2.Namespace;
					if (xmlTypeMapElementInfo2.MappedType != null)
					{
						this._typeNamespace = xmlTypeMapElementInfo2.MappedType.Namespace;
					}
					else
					{
						this._typeNamespace = string.Empty;
					}
					this._form = xmlTypeMapElementInfo2.Form;
				}
			}
			else
			{
				this._elementName = this._memberName;
				this._namespace = string.Empty;
			}
			if (this._form == XmlSchemaForm.None)
			{
				this._form = XmlSchemaForm.Qualified;
			}
		}

		public bool Any
		{
			get
			{
				return this._mapMember is XmlTypeMapMemberAnyElement;
			}
		}

		public string ElementName
		{
			get
			{
				return this._elementName;
			}
		}

		public string MemberName
		{
			get
			{
				return this._memberName;
			}
		}

		public string Namespace
		{
			get
			{
				return this._namespace;
			}
		}

		public string TypeFullName
		{
			get
			{
				return this._mapMember.TypeData.FullTypeName;
			}
		}

		public string TypeName
		{
			get
			{
				return this._mapMember.TypeData.XmlType;
			}
		}

		public string TypeNamespace
		{
			get
			{
				return this._typeNamespace;
			}
		}

		internal XmlTypeMapMember TypeMapMember
		{
			get
			{
				return this._mapMember;
			}
		}

		internal XmlSchemaForm Form
		{
			get
			{
				return this._form;
			}
		}

		public string XsdElementName
		{
			get
			{
				return this._mapMember.Name;
			}
		}

		public string GenerateTypeName(CodeDomProvider codeProvider)
		{
			string text = codeProvider.CreateValidIdentifier(this._mapMember.TypeData.FullTypeName);
			return (!this._mapMember.TypeData.IsValueType || !this._mapMember.TypeData.IsNullable) ? text : ("System.Nullable`1[" + text + "]");
		}

		public bool CheckSpecified
		{
			get
			{
				return this._mapMember.IsOptionalValueType;
			}
		}

		private XmlTypeMapMember _mapMember;

		private string _elementName;

		private string _memberName;

		private string _namespace;

		private string _typeNamespace;

		private XmlSchemaForm _form;
	}
}
