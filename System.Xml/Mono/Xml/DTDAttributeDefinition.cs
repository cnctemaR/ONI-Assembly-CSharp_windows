using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml
{
	internal class DTDAttributeDefinition : DTDNode
	{
		internal DTDAttributeDefinition(DTDObjectModel root)
		{
			base.SetRoot(root);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public XmlSchemaDatatype Datatype
		{
			get
			{
				return this.datatype;
			}
			set
			{
				this.datatype = value;
			}
		}

		public DTDAttributeOccurenceType OccurenceType
		{
			get
			{
				return this.occurenceType;
			}
			set
			{
				this.occurenceType = value;
			}
		}

		public ArrayList EnumeratedAttributeDeclaration
		{
			get
			{
				if (this.enumeratedLiterals == null)
				{
					this.enumeratedLiterals = new ArrayList();
				}
				return this.enumeratedLiterals;
			}
		}

		public ArrayList EnumeratedNotations
		{
			get
			{
				if (this.enumeratedNotations == null)
				{
					this.enumeratedNotations = new ArrayList();
				}
				return this.enumeratedNotations;
			}
		}

		public string DefaultValue
		{
			get
			{
				if (this.resolvedDefaultValue == null)
				{
					this.resolvedDefaultValue = this.ComputeDefaultValue();
				}
				return this.resolvedDefaultValue;
			}
		}

		public string NormalizedDefaultValue
		{
			get
			{
				if (this.resolvedNormalizedDefaultValue == null)
				{
					string text = this.ComputeDefaultValue();
					try
					{
						object obj = this.Datatype.ParseValue(text, null, null);
						this.resolvedNormalizedDefaultValue = ((!(obj is string[])) ? ((!(obj is IFormattable)) ? obj.ToString() : ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture)) : string.Join(" ", (string[])obj));
					}
					catch (Exception)
					{
						this.resolvedNormalizedDefaultValue = this.Datatype.Normalize(text);
					}
				}
				return this.resolvedNormalizedDefaultValue;
			}
		}

		public string UnresolvedDefaultValue
		{
			get
			{
				return this.unresolvedDefault;
			}
			set
			{
				this.unresolvedDefault = value;
			}
		}

		public char QuoteChar
		{
			get
			{
				return (this.UnresolvedDefaultValue.Length <= 0) ? '"' : this.UnresolvedDefaultValue[0];
			}
		}

		internal XmlSchemaAttribute CreateXsdAttribute()
		{
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			base.SetLineInfo(xmlSchemaAttribute);
			xmlSchemaAttribute.Name = this.Name;
			xmlSchemaAttribute.DefaultValue = this.resolvedNormalizedDefaultValue;
			if (this.OccurenceType != DTDAttributeOccurenceType.Required)
			{
				xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
			}
			XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Empty;
			ArrayList arrayList = null;
			if (this.enumeratedNotations != null && this.enumeratedNotations.Count > 0)
			{
				xmlQualifiedName = new XmlQualifiedName("NOTATION", "http://www.w3.org/2001/XMLSchema");
				arrayList = this.enumeratedNotations;
			}
			else if (this.enumeratedLiterals != null)
			{
				arrayList = this.enumeratedLiterals;
			}
			else
			{
				switch (this.Datatype.TokenizedType)
				{
				case XmlTokenizedType.ID:
					xmlQualifiedName = new XmlQualifiedName("ID", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.IDREF:
					xmlQualifiedName = new XmlQualifiedName("IDREF", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.IDREFS:
					xmlQualifiedName = new XmlQualifiedName("IDREFS", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.ENTITY:
					xmlQualifiedName = new XmlQualifiedName("ENTITY", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.ENTITIES:
					xmlQualifiedName = new XmlQualifiedName("ENTITIES", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.NMTOKEN:
					xmlQualifiedName = new XmlQualifiedName("NMTOKEN", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.NMTOKENS:
					xmlQualifiedName = new XmlQualifiedName("NMTOKENS", "http://www.w3.org/2001/XMLSchema");
					break;
				case XmlTokenizedType.NOTATION:
					xmlQualifiedName = new XmlQualifiedName("NOTATION", "http://www.w3.org/2001/XMLSchema");
					break;
				}
			}
			if (arrayList != null)
			{
				XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
				base.SetLineInfo(xmlSchemaSimpleType);
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
				base.SetLineInfo(xmlSchemaSimpleTypeRestriction);
				xmlSchemaSimpleTypeRestriction.BaseTypeName = xmlQualifiedName;
				if (this.enumeratedNotations != null)
				{
					foreach (object obj in this.enumeratedNotations)
					{
						string text = (string)obj;
						XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = new XmlSchemaEnumerationFacet();
						base.SetLineInfo(xmlSchemaEnumerationFacet);
						xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaEnumerationFacet);
						xmlSchemaEnumerationFacet.Value = text;
					}
				}
				xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeRestriction;
			}
			else if (xmlQualifiedName != XmlQualifiedName.Empty)
			{
				xmlSchemaAttribute.SchemaTypeName = xmlQualifiedName;
			}
			return xmlSchemaAttribute;
		}

		internal string ComputeDefaultValue()
		{
			if (this.UnresolvedDefaultValue == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			string unresolvedDefaultValue = this.UnresolvedDefaultValue;
			int num2;
			while ((num2 = unresolvedDefaultValue.IndexOf('&', num)) >= 0)
			{
				int num3 = unresolvedDefaultValue.IndexOf(';', num2);
				if (unresolvedDefaultValue[num2 + 1] == '#')
				{
					char c = unresolvedDefaultValue[num2 + 2];
					NumberStyles numberStyles = NumberStyles.Integer;
					string text;
					if (c == 'x' || c == 'X')
					{
						text = unresolvedDefaultValue.Substring(num2 + 3, num3 - num2 - 3);
						numberStyles |= NumberStyles.HexNumber;
					}
					else
					{
						text = unresolvedDefaultValue.Substring(num2 + 2, num3 - num2 - 2);
					}
					stringBuilder.Append((char)int.Parse(text, numberStyles, CultureInfo.InvariantCulture));
				}
				else
				{
					stringBuilder.Append(unresolvedDefaultValue.Substring(num, num2 - 1));
					string text2 = unresolvedDefaultValue.Substring(num2 + 1, num3 - 2);
					int predefinedEntity = XmlChar.GetPredefinedEntity(text2);
					if (predefinedEntity >= 0)
					{
						stringBuilder.Append(predefinedEntity);
					}
					else
					{
						stringBuilder.Append(base.Root.ResolveEntity(text2));
					}
				}
				num = num3 + 1;
			}
			stringBuilder.Append(unresolvedDefaultValue.Substring(num));
			string text3 = stringBuilder.ToString(1, stringBuilder.Length - 2);
			stringBuilder.Length = 0;
			return text3;
		}

		private string name;

		private XmlSchemaDatatype datatype;

		private ArrayList enumeratedLiterals;

		private string unresolvedDefault;

		private ArrayList enumeratedNotations;

		private DTDAttributeOccurenceType occurenceType;

		private string resolvedDefaultValue;

		private string resolvedNormalizedDefaultValue;
	}
}
