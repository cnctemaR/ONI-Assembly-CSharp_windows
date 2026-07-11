using System;
using System.IO;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Xsl
{
	internal abstract class XmlQueryType : ListBase<XmlQueryType>
	{
		static XmlQueryType()
		{
			for (int i = 0; i < XmlQueryType.BaseTypeCodes.Length; i++)
			{
				int num = i;
				for (;;)
				{
					XmlQueryType.TypeCodeDerivation[i, num] = true;
					if (XmlQueryType.BaseTypeCodes[num] == (XmlTypeCode)num)
					{
						break;
					}
					num = (int)XmlQueryType.BaseTypeCodes[num];
				}
			}
		}

		public abstract XmlTypeCode TypeCode { get; }

		public abstract XmlQualifiedNameTest NameTest { get; }

		public abstract XmlSchemaType SchemaType { get; }

		public abstract bool IsNillable { get; }

		public abstract XmlNodeKindFlags NodeKinds { get; }

		public abstract bool IsStrict { get; }

		public abstract XmlQueryCardinality Cardinality { get; }

		public abstract XmlQueryType Prime { get; }

		public abstract bool IsNotRtf { get; }

		public abstract bool IsDod { get; }

		public abstract XmlValueConverter ClrMapping { get; }

		public bool IsSubtypeOf(XmlQueryType baseType)
		{
			if (!(this.Cardinality <= baseType.Cardinality) || (!this.IsDod && baseType.IsDod))
			{
				return false;
			}
			if (!this.IsDod && baseType.IsDod)
			{
				return false;
			}
			XmlQueryType prime = this.Prime;
			XmlQueryType prime2 = baseType.Prime;
			if (prime == prime2)
			{
				return true;
			}
			if (prime.Count == 1 && prime2.Count == 1)
			{
				return prime.IsSubtypeOfItemType(prime2);
			}
			foreach (XmlQueryType xmlQueryType in prime)
			{
				bool flag = false;
				foreach (XmlQueryType xmlQueryType2 in prime2)
				{
					if (xmlQueryType.IsSubtypeOfItemType(xmlQueryType2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public bool NeverSubtypeOf(XmlQueryType baseType)
		{
			if (this.Cardinality.NeverSubset(baseType.Cardinality))
			{
				return true;
			}
			if (this.MaybeEmpty && baseType.MaybeEmpty)
			{
				return false;
			}
			if (this.Count == 0)
			{
				return false;
			}
			foreach (XmlQueryType xmlQueryType in this)
			{
				foreach (XmlQueryType xmlQueryType2 in baseType)
				{
					if (xmlQueryType.HasIntersectionItemType(xmlQueryType2))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool Equals(XmlQueryType that)
		{
			if (that == null)
			{
				return false;
			}
			if (this.Cardinality != that.Cardinality || this.IsDod != that.IsDod)
			{
				return false;
			}
			XmlQueryType prime = this.Prime;
			XmlQueryType prime2 = that.Prime;
			if (prime == prime2)
			{
				return true;
			}
			if (prime.Count != prime2.Count)
			{
				return false;
			}
			if (prime.Count == 1)
			{
				return prime.TypeCode == prime2.TypeCode && prime.NameTest == prime2.NameTest && prime.SchemaType == prime2.SchemaType && prime.IsStrict == prime2.IsStrict && prime.IsNotRtf == prime2.IsNotRtf;
			}
			foreach (XmlQueryType xmlQueryType in this)
			{
				bool flag = false;
				foreach (XmlQueryType xmlQueryType2 in that)
				{
					if (xmlQueryType.TypeCode == xmlQueryType2.TypeCode && xmlQueryType.NameTest == xmlQueryType2.NameTest && xmlQueryType.SchemaType == xmlQueryType2.SchemaType && xmlQueryType.IsStrict == xmlQueryType2.IsStrict && xmlQueryType.IsNotRtf == xmlQueryType2.IsNotRtf)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator ==(XmlQueryType left, XmlQueryType right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(XmlQueryType left, XmlQueryType right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		public bool IsEmpty
		{
			get
			{
				return this.Cardinality <= XmlQueryCardinality.Zero;
			}
		}

		public bool IsSingleton
		{
			get
			{
				return this.Cardinality <= XmlQueryCardinality.One;
			}
		}

		public bool MaybeEmpty
		{
			get
			{
				return XmlQueryCardinality.Zero <= this.Cardinality;
			}
		}

		public bool MaybeMany
		{
			get
			{
				return XmlQueryCardinality.More <= this.Cardinality;
			}
		}

		public bool IsNode
		{
			get
			{
				return (XmlQueryType.TypeCodeToFlags[(int)this.TypeCode] & XmlQueryType.TypeFlags.IsNode) > XmlQueryType.TypeFlags.None;
			}
		}

		public bool IsAtomicValue
		{
			get
			{
				return (XmlQueryType.TypeCodeToFlags[(int)this.TypeCode] & XmlQueryType.TypeFlags.IsAtomicValue) > XmlQueryType.TypeFlags.None;
			}
		}

		public bool IsNumeric
		{
			get
			{
				return (XmlQueryType.TypeCodeToFlags[(int)this.TypeCode] & XmlQueryType.TypeFlags.IsNumeric) > XmlQueryType.TypeFlags.None;
			}
		}

		public override bool Equals(object obj)
		{
			XmlQueryType xmlQueryType = obj as XmlQueryType;
			return !(xmlQueryType == null) && this.Equals(xmlQueryType);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				int num = (int)this.TypeCode;
				XmlSchemaType schemaType = this.SchemaType;
				if (schemaType != null)
				{
					num += (num << 7) ^ schemaType.GetHashCode();
				}
				num += (num << 7) ^ (int)this.NodeKinds;
				num += (num << 7) ^ this.Cardinality.GetHashCode();
				num += (num << 7) ^ (this.IsStrict ? 1 : 0);
				num -= num >> 17;
				num -= num >> 11;
				num -= num >> 5;
				this.hashCode = ((num == 0) ? 1 : num);
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.ToString("G");
		}

		public string ToString(string format)
		{
			StringBuilder stringBuilder;
			if (format == "S")
			{
				stringBuilder = new StringBuilder();
				stringBuilder.Append(this.Cardinality.ToString(format));
				stringBuilder.Append(';');
				for (int i = 0; i < this.Count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append("|");
					}
					stringBuilder.Append(this[i].TypeCode.ToString());
				}
				stringBuilder.Append(';');
				stringBuilder.Append(this.IsStrict);
				return stringBuilder.ToString();
			}
			bool flag = format == "X";
			if (this.Cardinality == XmlQueryCardinality.None)
			{
				return "none";
			}
			if (this.Cardinality == XmlQueryCardinality.Zero)
			{
				return "empty";
			}
			stringBuilder = new StringBuilder();
			int count = this.Count;
			if (count != 0)
			{
				if (count != 1)
				{
					string[] array = new string[this.Count];
					for (int j = 0; j < this.Count; j++)
					{
						array[j] = this[j].ItemTypeToString(flag);
					}
					Array.Sort<string>(array);
					stringBuilder = new StringBuilder();
					stringBuilder.Append('(');
					stringBuilder.Append(array[0]);
					for (int k = 1; k < array.Length; k++)
					{
						stringBuilder.Append(" | ");
						stringBuilder.Append(array[k]);
					}
					stringBuilder.Append(')');
				}
				else
				{
					stringBuilder.Append(this[0].ItemTypeToString(flag));
				}
			}
			else
			{
				stringBuilder.Append("none");
			}
			stringBuilder.Append(this.Cardinality.ToString());
			if (!flag && this.IsDod)
			{
				stringBuilder.Append('#');
			}
			return stringBuilder.ToString();
		}

		public abstract void GetObjectData(BinaryWriter writer);

		private bool IsSubtypeOfItemType(XmlQueryType baseType)
		{
			XmlSchemaType schemaType = baseType.SchemaType;
			if (this.TypeCode != baseType.TypeCode)
			{
				if (baseType.IsStrict)
				{
					return false;
				}
				XmlSchemaType builtInSimpleType = XmlSchemaType.GetBuiltInSimpleType(baseType.TypeCode);
				return (builtInSimpleType == null || schemaType == builtInSimpleType) && XmlQueryType.TypeCodeDerivation[this.TypeCode, baseType.TypeCode];
			}
			else
			{
				if (baseType.IsStrict)
				{
					return this.IsStrict && this.SchemaType == schemaType;
				}
				return (this.IsNotRtf || !baseType.IsNotRtf) && this.NameTest.IsSubsetOf(baseType.NameTest) && (schemaType == XmlSchemaComplexType.AnyType || XmlSchemaType.IsDerivedFrom(this.SchemaType, schemaType, XmlSchemaDerivationMethod.Empty)) && (!this.IsNillable || baseType.IsNillable);
			}
		}

		private bool HasIntersectionItemType(XmlQueryType other)
		{
			if (this.TypeCode == other.TypeCode && (this.NodeKinds & (XmlNodeKindFlags.Document | XmlNodeKindFlags.Element | XmlNodeKindFlags.Attribute)) != XmlNodeKindFlags.None)
			{
				return this.TypeCode == XmlTypeCode.Node || (this.NameTest.HasIntersection(other.NameTest) && (XmlSchemaType.IsDerivedFrom(this.SchemaType, other.SchemaType, XmlSchemaDerivationMethod.Empty) || XmlSchemaType.IsDerivedFrom(other.SchemaType, this.SchemaType, XmlSchemaDerivationMethod.Empty)));
			}
			return this.IsSubtypeOf(other) || other.IsSubtypeOf(this);
		}

		private string ItemTypeToString(bool isXQ)
		{
			string text;
			if (this.IsNode)
			{
				text = XmlQueryType.TypeNames[(int)this.TypeCode];
				XmlTypeCode typeCode = this.TypeCode;
				if (typeCode != XmlTypeCode.Document)
				{
					if (typeCode - XmlTypeCode.Element > 1)
					{
						goto IL_00B0;
					}
				}
				else if (isXQ)
				{
					text = text + "{(element" + this.NameAndType(true) + "?&text?&comment?&processing-instruction?)*}";
					goto IL_00B0;
				}
				text += this.NameAndType(isXQ);
			}
			else if (this.SchemaType != XmlSchemaComplexType.AnyType)
			{
				if (this.SchemaType.QualifiedName.IsEmpty)
				{
					text = "<:" + XmlQueryType.TypeNames[(int)this.TypeCode];
				}
				else
				{
					text = XmlQueryType.QNameToString(this.SchemaType.QualifiedName);
				}
			}
			else
			{
				text = XmlQueryType.TypeNames[(int)this.TypeCode];
			}
			IL_00B0:
			if (!isXQ && this.IsStrict)
			{
				text += "=";
			}
			return text;
		}

		private string NameAndType(bool isXQ)
		{
			string text = this.NameTest.ToString();
			string text2 = "*";
			if (this.SchemaType.QualifiedName.IsEmpty)
			{
				text2 = "typeof(" + text + ")";
			}
			else if (isXQ || (this.SchemaType != XmlSchemaComplexType.AnyType && this.SchemaType != DatatypeImplementation.AnySimpleType))
			{
				text2 = XmlQueryType.QNameToString(this.SchemaType.QualifiedName);
			}
			if (this.IsNillable)
			{
				text2 += " nillable";
			}
			if (text == "*" && text2 == "*")
			{
				return "";
			}
			return string.Concat(new string[] { "(", text, ", ", text2, ")" });
		}

		private static string QNameToString(XmlQualifiedName name)
		{
			if (name.IsEmpty)
			{
				return "*";
			}
			if (name.Namespace.Length == 0)
			{
				return name.Name;
			}
			if (name.Namespace == "http://www.w3.org/2001/XMLSchema")
			{
				return "xs:" + name.Name;
			}
			if (name.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
			{
				return "xdt:" + name.Name;
			}
			return "{" + name.Namespace + "}" + name.Name;
		}

		private static readonly XmlQueryType.BitMatrix TypeCodeDerivation = new XmlQueryType.BitMatrix(XmlQueryType.BaseTypeCodes.Length);

		private int hashCode;

		private static readonly XmlQueryType.TypeFlags[] TypeCodeToFlags = new XmlQueryType.TypeFlags[]
		{
			(XmlQueryType.TypeFlags)7,
			XmlQueryType.TypeFlags.None,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsNode,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			(XmlQueryType.TypeFlags)6,
			XmlQueryType.TypeFlags.IsAtomicValue,
			XmlQueryType.TypeFlags.IsAtomicValue
		};

		private static readonly XmlTypeCode[] BaseTypeCodes = new XmlTypeCode[]
		{
			XmlTypeCode.None,
			XmlTypeCode.Item,
			XmlTypeCode.Item,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Node,
			XmlTypeCode.Item,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.AnyAtomicType,
			XmlTypeCode.String,
			XmlTypeCode.NormalizedString,
			XmlTypeCode.Token,
			XmlTypeCode.Token,
			XmlTypeCode.Token,
			XmlTypeCode.Name,
			XmlTypeCode.NCName,
			XmlTypeCode.NCName,
			XmlTypeCode.NCName,
			XmlTypeCode.Decimal,
			XmlTypeCode.Integer,
			XmlTypeCode.NonPositiveInteger,
			XmlTypeCode.Integer,
			XmlTypeCode.Long,
			XmlTypeCode.Int,
			XmlTypeCode.Short,
			XmlTypeCode.Integer,
			XmlTypeCode.NonNegativeInteger,
			XmlTypeCode.UnsignedLong,
			XmlTypeCode.UnsignedInt,
			XmlTypeCode.UnsignedShort,
			XmlTypeCode.NonNegativeInteger,
			XmlTypeCode.Duration,
			XmlTypeCode.Duration
		};

		private static readonly string[] TypeNames = new string[]
		{
			"none", "item", "node", "document", "element", "attribute", "namespace", "processing-instruction", "comment", "text",
			"xdt:anyAtomicType", "xdt:untypedAtomic", "xs:string", "xs:boolean", "xs:decimal", "xs:float", "xs:double", "xs:duration", "xs:dateTime", "xs:time",
			"xs:date", "xs:gYearMonth", "xs:gYear", "xs:gMonthDay", "xs:gDay", "xs:gMonth", "xs:hexBinary", "xs:base64Binary", "xs:anyUri", "xs:QName",
			"xs:NOTATION", "xs:normalizedString", "xs:token", "xs:language", "xs:NMTOKEN", "xs:Name", "xs:NCName", "xs:ID", "xs:IDREF", "xs:ENTITY",
			"xs:integer", "xs:nonPositiveInteger", "xs:negativeInteger", "xs:long", "xs:int", "xs:short", "xs:byte", "xs:nonNegativeInteger", "xs:unsignedLong", "xs:unsignedInt",
			"xs:unsignedShort", "xs:unsignedByte", "xs:positiveInteger", "xdt:yearMonthDuration", "xdt:dayTimeDuration"
		};

		private enum TypeFlags
		{
			None,
			IsNode,
			IsAtomicValue,
			IsNumeric = 4
		}

		private sealed class BitMatrix
		{
			public BitMatrix(int count)
			{
				this.bits = new ulong[count];
			}

			public bool this[int index1, int index2]
			{
				get
				{
					return (this.bits[index1] & (1UL << index2)) > 0UL;
				}
				set
				{
					if (value)
					{
						this.bits[index1] |= 1UL << index2;
						return;
					}
					this.bits[index1] &= ~(1UL << index2);
				}
			}

			public bool this[XmlTypeCode index1, XmlTypeCode index2]
			{
				get
				{
					return this[(int)index1, (int)index2];
				}
			}

			private ulong[] bits;
		}
	}
}
