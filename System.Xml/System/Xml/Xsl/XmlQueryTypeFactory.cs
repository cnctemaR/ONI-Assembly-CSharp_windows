using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	internal static class XmlQueryTypeFactory
	{
		public static XmlQueryType Type(XmlTypeCode code, bool isStrict)
		{
			return XmlQueryTypeFactory.ItemType.Create(code, isStrict);
		}

		public static XmlQueryType Type(XmlSchemaSimpleType schemaType, bool isStrict)
		{
			if (schemaType.Datatype.Variety == XmlSchemaDatatypeVariety.Atomic)
			{
				if (schemaType == DatatypeImplementation.AnySimpleType)
				{
					return XmlQueryTypeFactory.AnyAtomicTypeS;
				}
				return XmlQueryTypeFactory.ItemType.Create(schemaType, isStrict);
			}
			else
			{
				while (schemaType.DerivedBy == XmlSchemaDerivationMethod.Restriction)
				{
					schemaType = (XmlSchemaSimpleType)schemaType.BaseXmlSchemaType;
				}
				if (schemaType.DerivedBy == XmlSchemaDerivationMethod.List)
				{
					return XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Type(((XmlSchemaSimpleTypeList)schemaType.Content).BaseItemType, isStrict), XmlQueryCardinality.ZeroOrMore);
				}
				XmlSchemaSimpleType[] baseMemberTypes = ((XmlSchemaSimpleTypeUnion)schemaType.Content).BaseMemberTypes;
				XmlQueryType[] array = new XmlQueryType[baseMemberTypes.Length];
				for (int i = 0; i < baseMemberTypes.Length; i++)
				{
					array[i] = XmlQueryTypeFactory.Type(baseMemberTypes[i], isStrict);
				}
				return XmlQueryTypeFactory.Choice(array);
			}
		}

		public static XmlQueryType Choice(XmlQueryType left, XmlQueryType right)
		{
			return XmlQueryTypeFactory.SequenceType.Create(XmlQueryTypeFactory.ChoiceType.Create(XmlQueryTypeFactory.PrimeChoice(new List<XmlQueryType>(left), right)), left.Cardinality | right.Cardinality);
		}

		public static XmlQueryType Choice(params XmlQueryType[] types)
		{
			if (types.Length == 0)
			{
				return XmlQueryTypeFactory.None;
			}
			if (types.Length == 1)
			{
				return types[0];
			}
			List<XmlQueryType> list = new List<XmlQueryType>(types[0]);
			XmlQueryCardinality xmlQueryCardinality = types[0].Cardinality;
			for (int i = 1; i < types.Length; i++)
			{
				XmlQueryTypeFactory.PrimeChoice(list, types[i]);
				xmlQueryCardinality |= types[i].Cardinality;
			}
			return XmlQueryTypeFactory.SequenceType.Create(XmlQueryTypeFactory.ChoiceType.Create(list), xmlQueryCardinality);
		}

		public static XmlQueryType NodeChoice(XmlNodeKindFlags kinds)
		{
			return XmlQueryTypeFactory.ChoiceType.Create(kinds);
		}

		public static XmlQueryType Sequence(XmlQueryType left, XmlQueryType right)
		{
			return XmlQueryTypeFactory.SequenceType.Create(XmlQueryTypeFactory.ChoiceType.Create(XmlQueryTypeFactory.PrimeChoice(new List<XmlQueryType>(left), right)), left.Cardinality + right.Cardinality);
		}

		public static XmlQueryType PrimeProduct(XmlQueryType t, XmlQueryCardinality c)
		{
			if (t.Cardinality == c && !t.IsDod)
			{
				return t;
			}
			return XmlQueryTypeFactory.SequenceType.Create(t.Prime, c);
		}

		public static XmlQueryType Product(XmlQueryType t, XmlQueryCardinality c)
		{
			return XmlQueryTypeFactory.PrimeProduct(t, t.Cardinality * c);
		}

		public static XmlQueryType AtMost(XmlQueryType t, XmlQueryCardinality c)
		{
			return XmlQueryTypeFactory.PrimeProduct(t, c.AtMost());
		}

		private static List<XmlQueryType> PrimeChoice(List<XmlQueryType> accumulator, IList<XmlQueryType> types)
		{
			foreach (XmlQueryType xmlQueryType in types)
			{
				XmlQueryTypeFactory.AddItemToChoice(accumulator, xmlQueryType);
			}
			return accumulator;
		}

		private static void AddItemToChoice(List<XmlQueryType> accumulator, XmlQueryType itemType)
		{
			bool flag = true;
			for (int i = 0; i < accumulator.Count; i++)
			{
				if (itemType.IsSubtypeOf(accumulator[i]))
				{
					return;
				}
				if (accumulator[i].IsSubtypeOf(itemType))
				{
					if (flag)
					{
						flag = false;
						accumulator[i] = itemType;
					}
					else
					{
						accumulator.RemoveAt(i);
						i--;
					}
				}
			}
			if (flag)
			{
				accumulator.Add(itemType);
			}
		}

		public static XmlQueryType Type(XPathNodeType kind, XmlQualifiedNameTest nameTest, XmlSchemaType contentType, bool isNillable)
		{
			return XmlQueryTypeFactory.ItemType.Create(XmlQueryTypeFactory.NodeKindToTypeCode[(int)kind], nameTest, contentType, isNillable);
		}

		[Conditional("DEBUG")]
		public static void CheckSerializability(XmlQueryType type)
		{
			type.GetObjectData(new BinaryWriter(Stream.Null));
		}

		public static void Serialize(BinaryWriter writer, XmlQueryType type)
		{
			sbyte b;
			if (type.GetType() == typeof(XmlQueryTypeFactory.ItemType))
			{
				b = 0;
			}
			else if (type.GetType() == typeof(XmlQueryTypeFactory.ChoiceType))
			{
				b = 1;
			}
			else if (type.GetType() == typeof(XmlQueryTypeFactory.SequenceType))
			{
				b = 2;
			}
			else
			{
				b = -1;
			}
			writer.Write(b);
			type.GetObjectData(writer);
		}

		public static XmlQueryType Deserialize(BinaryReader reader)
		{
			switch (reader.ReadByte())
			{
			case 0:
				return XmlQueryTypeFactory.ItemType.Create(reader);
			case 1:
				return XmlQueryTypeFactory.ChoiceType.Create(reader);
			case 2:
				return XmlQueryTypeFactory.SequenceType.Create(reader);
			default:
				return null;
			}
		}

		public static readonly XmlQueryType None = XmlQueryTypeFactory.ChoiceType.None;

		public static readonly XmlQueryType Empty = XmlQueryTypeFactory.SequenceType.Zero;

		public static readonly XmlQueryType Item = XmlQueryTypeFactory.Type(XmlTypeCode.Item, false);

		public static readonly XmlQueryType ItemS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Item, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Node = XmlQueryTypeFactory.Type(XmlTypeCode.Node, false);

		public static readonly XmlQueryType NodeS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Node, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Element = XmlQueryTypeFactory.Type(XmlTypeCode.Element, false);

		public static readonly XmlQueryType ElementS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Element, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Document = XmlQueryTypeFactory.Type(XmlTypeCode.Document, false);

		public static readonly XmlQueryType DocumentS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Document, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Attribute = XmlQueryTypeFactory.Type(XmlTypeCode.Attribute, false);

		public static readonly XmlQueryType AttributeQ = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Attribute, XmlQueryCardinality.ZeroOrOne);

		public static readonly XmlQueryType AttributeS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Attribute, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Namespace = XmlQueryTypeFactory.Type(XmlTypeCode.Namespace, false);

		public static readonly XmlQueryType NamespaceS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Namespace, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Text = XmlQueryTypeFactory.Type(XmlTypeCode.Text, false);

		public static readonly XmlQueryType TextS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Text, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Comment = XmlQueryTypeFactory.Type(XmlTypeCode.Comment, false);

		public static readonly XmlQueryType CommentS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Comment, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType PI = XmlQueryTypeFactory.Type(XmlTypeCode.ProcessingInstruction, false);

		public static readonly XmlQueryType PIS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.PI, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType DocumentOrElement = XmlQueryTypeFactory.Choice(XmlQueryTypeFactory.Document, XmlQueryTypeFactory.Element);

		public static readonly XmlQueryType DocumentOrElementQ = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.DocumentOrElement, XmlQueryCardinality.ZeroOrOne);

		public static readonly XmlQueryType DocumentOrElementS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.DocumentOrElement, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Content = XmlQueryTypeFactory.Choice(new XmlQueryType[]
		{
			XmlQueryTypeFactory.Element,
			XmlQueryTypeFactory.Comment,
			XmlQueryTypeFactory.PI,
			XmlQueryTypeFactory.Text
		});

		public static readonly XmlQueryType ContentS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.Content, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType DocumentOrContent = XmlQueryTypeFactory.Choice(XmlQueryTypeFactory.Document, XmlQueryTypeFactory.Content);

		public static readonly XmlQueryType DocumentOrContentS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.DocumentOrContent, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType AttributeOrContent = XmlQueryTypeFactory.Choice(XmlQueryTypeFactory.Attribute, XmlQueryTypeFactory.Content);

		public static readonly XmlQueryType AttributeOrContentS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.AttributeOrContent, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType AnyAtomicType = XmlQueryTypeFactory.Type(XmlTypeCode.AnyAtomicType, false);

		public static readonly XmlQueryType AnyAtomicTypeS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.AnyAtomicType, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType String = XmlQueryTypeFactory.Type(XmlTypeCode.String, false);

		public static readonly XmlQueryType StringX = XmlQueryTypeFactory.Type(XmlTypeCode.String, true);

		public static readonly XmlQueryType StringXS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.StringX, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType Boolean = XmlQueryTypeFactory.Type(XmlTypeCode.Boolean, false);

		public static readonly XmlQueryType BooleanX = XmlQueryTypeFactory.Type(XmlTypeCode.Boolean, true);

		public static readonly XmlQueryType Int = XmlQueryTypeFactory.Type(XmlTypeCode.Int, false);

		public static readonly XmlQueryType IntX = XmlQueryTypeFactory.Type(XmlTypeCode.Int, true);

		public static readonly XmlQueryType IntXS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.IntX, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType IntegerX = XmlQueryTypeFactory.Type(XmlTypeCode.Integer, true);

		public static readonly XmlQueryType LongX = XmlQueryTypeFactory.Type(XmlTypeCode.Long, true);

		public static readonly XmlQueryType DecimalX = XmlQueryTypeFactory.Type(XmlTypeCode.Decimal, true);

		public static readonly XmlQueryType FloatX = XmlQueryTypeFactory.Type(XmlTypeCode.Float, true);

		public static readonly XmlQueryType Double = XmlQueryTypeFactory.Type(XmlTypeCode.Double, false);

		public static readonly XmlQueryType DoubleX = XmlQueryTypeFactory.Type(XmlTypeCode.Double, true);

		public static readonly XmlQueryType DateTimeX = XmlQueryTypeFactory.Type(XmlTypeCode.DateTime, true);

		public static readonly XmlQueryType QNameX = XmlQueryTypeFactory.Type(XmlTypeCode.QName, true);

		public static readonly XmlQueryType UntypedDocument = XmlQueryTypeFactory.ItemType.UntypedDocument;

		public static readonly XmlQueryType UntypedElement = XmlQueryTypeFactory.ItemType.UntypedElement;

		public static readonly XmlQueryType UntypedAttribute = XmlQueryTypeFactory.ItemType.UntypedAttribute;

		public static readonly XmlQueryType UntypedNode = XmlQueryTypeFactory.Choice(new XmlQueryType[]
		{
			XmlQueryTypeFactory.UntypedDocument,
			XmlQueryTypeFactory.UntypedElement,
			XmlQueryTypeFactory.UntypedAttribute,
			XmlQueryTypeFactory.Namespace,
			XmlQueryTypeFactory.Text,
			XmlQueryTypeFactory.Comment,
			XmlQueryTypeFactory.PI
		});

		public static readonly XmlQueryType UntypedNodeS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.UntypedNode, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType NodeNotRtf = XmlQueryTypeFactory.ItemType.NodeNotRtf;

		public static readonly XmlQueryType NodeNotRtfQ = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtf, XmlQueryCardinality.ZeroOrOne);

		public static readonly XmlQueryType NodeNotRtfS = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtf, XmlQueryCardinality.ZeroOrMore);

		public static readonly XmlQueryType NodeSDod = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtf, XmlQueryCardinality.ZeroOrMore);

		private static readonly XmlTypeCode[] NodeKindToTypeCode = new XmlTypeCode[]
		{
			XmlTypeCode.Document,
			XmlTypeCode.Element,
			XmlTypeCode.Attribute,
			XmlTypeCode.Namespace,
			XmlTypeCode.Text,
			XmlTypeCode.Text,
			XmlTypeCode.Text,
			XmlTypeCode.ProcessingInstruction,
			XmlTypeCode.Comment,
			XmlTypeCode.Node
		};

		private sealed class ItemType : XmlQueryType
		{
			static ItemType()
			{
				int num = 55;
				XmlQueryTypeFactory.ItemType.BuiltInItemTypes = new XmlQueryType[num];
				XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict = new XmlQueryType[num];
				for (int i = 0; i < num; i++)
				{
					XmlTypeCode xmlTypeCode = (XmlTypeCode)i;
					switch (i)
					{
					case 0:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = XmlQueryTypeFactory.ChoiceType.None;
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ChoiceType.None;
						break;
					case 1:
					case 2:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, false);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i];
						break;
					case 3:
					case 4:
					case 6:
					case 7:
					case 8:
					case 9:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, true);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i];
						break;
					case 5:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.AnySimpleType, false, false, true);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i];
						break;
					case 10:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.AnyAtomicType, false, false, true);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i];
						break;
					case 11:
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.UntypedAtomicType, false, true, true);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i];
						break;
					default:
					{
						XmlSchemaType builtInSimpleType = XmlSchemaType.GetBuiltInSimpleType(xmlTypeCode);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypes[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, builtInSimpleType, false, false, true);
						XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[i] = new XmlQueryTypeFactory.ItemType(xmlTypeCode, XmlQualifiedNameTest.Wildcard, builtInSimpleType, false, true, true);
						break;
					}
					}
				}
				XmlQueryTypeFactory.ItemType.UntypedDocument = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Document, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.UntypedAnyType, false, false, true);
				XmlQueryTypeFactory.ItemType.UntypedElement = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Element, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.UntypedAnyType, false, false, true);
				XmlQueryTypeFactory.ItemType.UntypedAttribute = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Attribute, XmlQualifiedNameTest.Wildcard, DatatypeImplementation.UntypedAtomicType, false, false, true);
				XmlQueryTypeFactory.ItemType.NodeNotRtf = new XmlQueryTypeFactory.ItemType(XmlTypeCode.Node, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false, false, true);
				XmlQueryTypeFactory.ItemType.SpecialBuiltInItemTypes = new XmlQueryType[]
				{
					XmlQueryTypeFactory.ItemType.UntypedDocument,
					XmlQueryTypeFactory.ItemType.UntypedElement,
					XmlQueryTypeFactory.ItemType.UntypedAttribute,
					XmlQueryTypeFactory.ItemType.NodeNotRtf
				};
			}

			public static XmlQueryType Create(XmlTypeCode code, bool isStrict)
			{
				if (isStrict)
				{
					return XmlQueryTypeFactory.ItemType.BuiltInItemTypesStrict[(int)code];
				}
				return XmlQueryTypeFactory.ItemType.BuiltInItemTypes[(int)code];
			}

			public static XmlQueryType Create(XmlSchemaSimpleType schemaType, bool isStrict)
			{
				XmlTypeCode typeCode = schemaType.Datatype.TypeCode;
				if (schemaType == XmlSchemaType.GetBuiltInSimpleType(typeCode))
				{
					return XmlQueryTypeFactory.ItemType.Create(typeCode, isStrict);
				}
				return new XmlQueryTypeFactory.ItemType(typeCode, XmlQualifiedNameTest.Wildcard, schemaType, false, isStrict, true);
			}

			public static XmlQueryType Create(XmlTypeCode code, XmlQualifiedNameTest nameTest, XmlSchemaType contentType, bool isNillable)
			{
				if (code - XmlTypeCode.Document <= 1)
				{
					if (nameTest.IsWildcard)
					{
						if (contentType == XmlSchemaComplexType.AnyType)
						{
							return XmlQueryTypeFactory.ItemType.Create(code, false);
						}
						if (contentType == XmlSchemaComplexType.UntypedAnyType)
						{
							if (code == XmlTypeCode.Element)
							{
								return XmlQueryTypeFactory.ItemType.UntypedElement;
							}
							if (code == XmlTypeCode.Document)
							{
								return XmlQueryTypeFactory.ItemType.UntypedDocument;
							}
						}
					}
					return new XmlQueryTypeFactory.ItemType(code, nameTest, contentType, isNillable, false, true);
				}
				if (code != XmlTypeCode.Attribute)
				{
					return XmlQueryTypeFactory.ItemType.Create(code, false);
				}
				if (nameTest.IsWildcard)
				{
					if (contentType == DatatypeImplementation.AnySimpleType)
					{
						return XmlQueryTypeFactory.ItemType.Create(code, false);
					}
					if (contentType == DatatypeImplementation.UntypedAtomicType)
					{
						return XmlQueryTypeFactory.ItemType.UntypedAttribute;
					}
				}
				return new XmlQueryTypeFactory.ItemType(code, nameTest, contentType, isNillable, false, true);
			}

			private ItemType(XmlTypeCode code, XmlQualifiedNameTest nameTest, XmlSchemaType schemaType, bool isNillable, bool isStrict, bool isNotRtf)
			{
				this.code = code;
				this.nameTest = nameTest;
				this.schemaType = schemaType;
				this.isNillable = isNillable;
				this.isStrict = isStrict;
				this.isNotRtf = isNotRtf;
				switch (code)
				{
				case XmlTypeCode.Item:
					this.nodeKinds = XmlNodeKindFlags.Any;
					return;
				case XmlTypeCode.Node:
					this.nodeKinds = XmlNodeKindFlags.Any;
					return;
				case XmlTypeCode.Document:
					this.nodeKinds = XmlNodeKindFlags.Document;
					return;
				case XmlTypeCode.Element:
					this.nodeKinds = XmlNodeKindFlags.Element;
					return;
				case XmlTypeCode.Attribute:
					this.nodeKinds = XmlNodeKindFlags.Attribute;
					return;
				case XmlTypeCode.Namespace:
					this.nodeKinds = XmlNodeKindFlags.Namespace;
					return;
				case XmlTypeCode.ProcessingInstruction:
					this.nodeKinds = XmlNodeKindFlags.PI;
					return;
				case XmlTypeCode.Comment:
					this.nodeKinds = XmlNodeKindFlags.Comment;
					return;
				case XmlTypeCode.Text:
					this.nodeKinds = XmlNodeKindFlags.Text;
					return;
				default:
					this.nodeKinds = XmlNodeKindFlags.None;
					return;
				}
			}

			public override void GetObjectData(BinaryWriter writer)
			{
				sbyte b = (sbyte)this.code;
				for (int i = 0; i < XmlQueryTypeFactory.ItemType.SpecialBuiltInItemTypes.Length; i++)
				{
					if (this == XmlQueryTypeFactory.ItemType.SpecialBuiltInItemTypes[i])
					{
						b = (sbyte)(~(sbyte)i);
						break;
					}
				}
				writer.Write(b);
				if (0 <= b)
				{
					writer.Write(this.isStrict);
				}
			}

			public static XmlQueryType Create(BinaryReader reader)
			{
				sbyte b = reader.ReadSByte();
				if (0 <= b)
				{
					return XmlQueryTypeFactory.ItemType.Create((XmlTypeCode)b, reader.ReadBoolean());
				}
				return XmlQueryTypeFactory.ItemType.SpecialBuiltInItemTypes[(int)(~(int)b)];
			}

			public override XmlTypeCode TypeCode
			{
				get
				{
					return this.code;
				}
			}

			public override XmlQualifiedNameTest NameTest
			{
				get
				{
					return this.nameTest;
				}
			}

			public override XmlSchemaType SchemaType
			{
				get
				{
					return this.schemaType;
				}
			}

			public override bool IsNillable
			{
				get
				{
					return this.isNillable;
				}
			}

			public override XmlNodeKindFlags NodeKinds
			{
				get
				{
					return this.nodeKinds;
				}
			}

			public override bool IsStrict
			{
				get
				{
					return this.isStrict;
				}
			}

			public override bool IsNotRtf
			{
				get
				{
					return this.isNotRtf;
				}
			}

			public override bool IsDod
			{
				get
				{
					return false;
				}
			}

			public override XmlQueryCardinality Cardinality
			{
				get
				{
					return XmlQueryCardinality.One;
				}
			}

			public override XmlQueryType Prime
			{
				get
				{
					return this;
				}
			}

			public override XmlValueConverter ClrMapping
			{
				get
				{
					if (base.IsAtomicValue)
					{
						return this.SchemaType.ValueConverter;
					}
					if (base.IsNode)
					{
						return XmlNodeConverter.Node;
					}
					return XmlAnyConverter.Item;
				}
			}

			public override int Count
			{
				get
				{
					return 1;
				}
			}

			public override XmlQueryType this[int index]
			{
				get
				{
					if (index != 0)
					{
						throw new IndexOutOfRangeException();
					}
					return this;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public static readonly XmlQueryType UntypedDocument;

			public static readonly XmlQueryType UntypedElement;

			public static readonly XmlQueryType UntypedAttribute;

			public static readonly XmlQueryType NodeNotRtf;

			private static XmlQueryType[] BuiltInItemTypes;

			private static XmlQueryType[] BuiltInItemTypesStrict;

			private static XmlQueryType[] SpecialBuiltInItemTypes;

			private XmlTypeCode code;

			private XmlQualifiedNameTest nameTest;

			private XmlSchemaType schemaType;

			private bool isNillable;

			private XmlNodeKindFlags nodeKinds;

			private bool isStrict;

			private bool isNotRtf;
		}

		private sealed class ChoiceType : XmlQueryType
		{
			public static XmlQueryType Create(XmlNodeKindFlags nodeKinds)
			{
				if (Bits.ExactlyOne((uint)nodeKinds))
				{
					return XmlQueryTypeFactory.ItemType.Create(XmlQueryTypeFactory.ChoiceType.NodeKindToTypeCode[Bits.LeastPosition((uint)nodeKinds)], false);
				}
				List<XmlQueryType> list = new List<XmlQueryType>();
				while (nodeKinds != XmlNodeKindFlags.None)
				{
					list.Add(XmlQueryTypeFactory.ItemType.Create(XmlQueryTypeFactory.ChoiceType.NodeKindToTypeCode[Bits.LeastPosition((uint)nodeKinds)], false));
					nodeKinds = (XmlNodeKindFlags)Bits.ClearLeast((uint)nodeKinds);
				}
				return XmlQueryTypeFactory.ChoiceType.Create(list);
			}

			public static XmlQueryType Create(List<XmlQueryType> members)
			{
				if (members.Count == 0)
				{
					return XmlQueryTypeFactory.ChoiceType.None;
				}
				if (members.Count == 1)
				{
					return members[0];
				}
				return new XmlQueryTypeFactory.ChoiceType(members);
			}

			private ChoiceType(List<XmlQueryType> members)
			{
				this.members = members;
				for (int i = 0; i < members.Count; i++)
				{
					XmlQueryType xmlQueryType = members[i];
					if (this.code == XmlTypeCode.None)
					{
						this.code = xmlQueryType.TypeCode;
						this.schemaType = xmlQueryType.SchemaType;
					}
					else if (base.IsNode && xmlQueryType.IsNode)
					{
						if (this.code == xmlQueryType.TypeCode)
						{
							if (this.code == XmlTypeCode.Element)
							{
								this.schemaType = XmlSchemaComplexType.AnyType;
							}
							else if (this.code == XmlTypeCode.Attribute)
							{
								this.schemaType = DatatypeImplementation.AnySimpleType;
							}
						}
						else
						{
							this.code = XmlTypeCode.Node;
							this.schemaType = null;
						}
					}
					else if (base.IsAtomicValue && xmlQueryType.IsAtomicValue)
					{
						this.code = XmlTypeCode.AnyAtomicType;
						this.schemaType = DatatypeImplementation.AnyAtomicType;
					}
					else
					{
						this.code = XmlTypeCode.Item;
						this.schemaType = null;
					}
					this.nodeKinds |= xmlQueryType.NodeKinds;
				}
			}

			public override void GetObjectData(BinaryWriter writer)
			{
				writer.Write(this.members.Count);
				for (int i = 0; i < this.members.Count; i++)
				{
					XmlQueryTypeFactory.Serialize(writer, this.members[i]);
				}
			}

			public static XmlQueryType Create(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				List<XmlQueryType> list = new List<XmlQueryType>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add(XmlQueryTypeFactory.Deserialize(reader));
				}
				return XmlQueryTypeFactory.ChoiceType.Create(list);
			}

			public override XmlTypeCode TypeCode
			{
				get
				{
					return this.code;
				}
			}

			public override XmlQualifiedNameTest NameTest
			{
				get
				{
					return XmlQualifiedNameTest.Wildcard;
				}
			}

			public override XmlSchemaType SchemaType
			{
				get
				{
					return this.schemaType;
				}
			}

			public override bool IsNillable
			{
				get
				{
					return false;
				}
			}

			public override XmlNodeKindFlags NodeKinds
			{
				get
				{
					return this.nodeKinds;
				}
			}

			public override bool IsStrict
			{
				get
				{
					return this.members.Count == 0;
				}
			}

			public override bool IsNotRtf
			{
				get
				{
					for (int i = 0; i < this.members.Count; i++)
					{
						if (!this.members[i].IsNotRtf)
						{
							return false;
						}
					}
					return true;
				}
			}

			public override bool IsDod
			{
				get
				{
					return false;
				}
			}

			public override XmlQueryCardinality Cardinality
			{
				get
				{
					if (this.TypeCode != XmlTypeCode.None)
					{
						return XmlQueryCardinality.One;
					}
					return XmlQueryCardinality.None;
				}
			}

			public override XmlQueryType Prime
			{
				get
				{
					return this;
				}
			}

			public override XmlValueConverter ClrMapping
			{
				get
				{
					if (this.code == XmlTypeCode.None || this.code == XmlTypeCode.Item)
					{
						return XmlAnyConverter.Item;
					}
					if (base.IsAtomicValue)
					{
						return this.SchemaType.ValueConverter;
					}
					return XmlNodeConverter.Node;
				}
			}

			public override int Count
			{
				get
				{
					return this.members.Count;
				}
			}

			public override XmlQueryType this[int index]
			{
				get
				{
					return this.members[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public static readonly XmlQueryType None = new XmlQueryTypeFactory.ChoiceType(new List<XmlQueryType>());

			private XmlTypeCode code;

			private XmlSchemaType schemaType;

			private XmlNodeKindFlags nodeKinds;

			private List<XmlQueryType> members;

			private static readonly XmlTypeCode[] NodeKindToTypeCode = new XmlTypeCode[]
			{
				XmlTypeCode.None,
				XmlTypeCode.Document,
				XmlTypeCode.Element,
				XmlTypeCode.Attribute,
				XmlTypeCode.Text,
				XmlTypeCode.Comment,
				XmlTypeCode.ProcessingInstruction,
				XmlTypeCode.Namespace
			};
		}

		private sealed class SequenceType : XmlQueryType
		{
			public static XmlQueryType Create(XmlQueryType prime, XmlQueryCardinality card)
			{
				if (prime.TypeCode == XmlTypeCode.None)
				{
					if (!(XmlQueryCardinality.Zero <= card))
					{
						return XmlQueryTypeFactory.None;
					}
					return XmlQueryTypeFactory.SequenceType.Zero;
				}
				else
				{
					if (card == XmlQueryCardinality.None)
					{
						return XmlQueryTypeFactory.None;
					}
					if (card == XmlQueryCardinality.Zero)
					{
						return XmlQueryTypeFactory.SequenceType.Zero;
					}
					if (card == XmlQueryCardinality.One)
					{
						return prime;
					}
					return new XmlQueryTypeFactory.SequenceType(prime, card);
				}
			}

			private SequenceType(XmlQueryType prime, XmlQueryCardinality card)
			{
				this.prime = prime;
				this.card = card;
			}

			public override void GetObjectData(BinaryWriter writer)
			{
				writer.Write(this.IsDod);
				if (this.IsDod)
				{
					return;
				}
				XmlQueryTypeFactory.Serialize(writer, this.prime);
				this.card.GetObjectData(writer);
			}

			public static XmlQueryType Create(BinaryReader reader)
			{
				if (reader.ReadBoolean())
				{
					return XmlQueryTypeFactory.NodeSDod;
				}
				XmlQueryType xmlQueryType = XmlQueryTypeFactory.Deserialize(reader);
				XmlQueryCardinality xmlQueryCardinality = new XmlQueryCardinality(reader);
				return XmlQueryTypeFactory.SequenceType.Create(xmlQueryType, xmlQueryCardinality);
			}

			public override XmlTypeCode TypeCode
			{
				get
				{
					return this.prime.TypeCode;
				}
			}

			public override XmlQualifiedNameTest NameTest
			{
				get
				{
					return this.prime.NameTest;
				}
			}

			public override XmlSchemaType SchemaType
			{
				get
				{
					return this.prime.SchemaType;
				}
			}

			public override bool IsNillable
			{
				get
				{
					return this.prime.IsNillable;
				}
			}

			public override XmlNodeKindFlags NodeKinds
			{
				get
				{
					return this.prime.NodeKinds;
				}
			}

			public override bool IsStrict
			{
				get
				{
					return this.prime.IsStrict;
				}
			}

			public override bool IsNotRtf
			{
				get
				{
					return this.prime.IsNotRtf;
				}
			}

			public override bool IsDod
			{
				get
				{
					return this == XmlQueryTypeFactory.NodeSDod;
				}
			}

			public override XmlQueryCardinality Cardinality
			{
				get
				{
					return this.card;
				}
			}

			public override XmlQueryType Prime
			{
				get
				{
					return this.prime;
				}
			}

			public override XmlValueConverter ClrMapping
			{
				get
				{
					if (this.converter == null)
					{
						this.converter = XmlListConverter.Create(this.prime.ClrMapping);
					}
					return this.converter;
				}
			}

			public override int Count
			{
				get
				{
					return this.prime.Count;
				}
			}

			public override XmlQueryType this[int index]
			{
				get
				{
					return this.prime[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public static readonly XmlQueryType Zero = new XmlQueryTypeFactory.SequenceType(XmlQueryTypeFactory.ChoiceType.None, XmlQueryCardinality.Zero);

			private XmlQueryType prime;

			private XmlQueryCardinality card;

			private XmlValueConverter converter;
		}
	}
}
