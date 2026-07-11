using System;
using System.IO;

namespace System.Xml.Xsl
{
	internal struct XmlQueryCardinality
	{
		private XmlQueryCardinality(int value)
		{
			this.value = value;
		}

		public static XmlQueryCardinality None
		{
			get
			{
				return new XmlQueryCardinality(0);
			}
		}

		public static XmlQueryCardinality Zero
		{
			get
			{
				return new XmlQueryCardinality(1);
			}
		}

		public static XmlQueryCardinality One
		{
			get
			{
				return new XmlQueryCardinality(2);
			}
		}

		public static XmlQueryCardinality ZeroOrOne
		{
			get
			{
				return new XmlQueryCardinality(3);
			}
		}

		public static XmlQueryCardinality More
		{
			get
			{
				return new XmlQueryCardinality(4);
			}
		}

		public static XmlQueryCardinality NotOne
		{
			get
			{
				return new XmlQueryCardinality(5);
			}
		}

		public static XmlQueryCardinality OneOrMore
		{
			get
			{
				return new XmlQueryCardinality(6);
			}
		}

		public static XmlQueryCardinality ZeroOrMore
		{
			get
			{
				return new XmlQueryCardinality(7);
			}
		}

		public bool Equals(XmlQueryCardinality other)
		{
			return this.value == other.value;
		}

		public static bool operator ==(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return left.value == right.value;
		}

		public static bool operator !=(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return left.value != right.value;
		}

		public override bool Equals(object other)
		{
			return other is XmlQueryCardinality && this.Equals((XmlQueryCardinality)other);
		}

		public override int GetHashCode()
		{
			return this.value;
		}

		public static XmlQueryCardinality operator |(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return new XmlQueryCardinality(left.value | right.value);
		}

		public static XmlQueryCardinality operator &(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return new XmlQueryCardinality(left.value & right.value);
		}

		public static XmlQueryCardinality operator *(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return XmlQueryCardinality.cardinalityProduct[left.value, right.value];
		}

		public static XmlQueryCardinality operator +(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return XmlQueryCardinality.cardinalitySum[left.value, right.value];
		}

		public static bool operator <=(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return (left.value & ~right.value) == 0;
		}

		public static bool operator >=(XmlQueryCardinality left, XmlQueryCardinality right)
		{
			return (right.value & ~left.value) == 0;
		}

		public XmlQueryCardinality AtMost()
		{
			return new XmlQueryCardinality(this.value | (this.value >> 1) | (this.value >> 2));
		}

		public bool NeverSubset(XmlQueryCardinality other)
		{
			return this.value != 0 && (this.value & other.value) == 0;
		}

		public string ToString(string format)
		{
			if (format == "S")
			{
				return XmlQueryCardinality.serialized[this.value];
			}
			return this.ToString();
		}

		public override string ToString()
		{
			return XmlQueryCardinality.toString[this.value];
		}

		public XmlQueryCardinality(string s)
		{
			this.value = 0;
			for (int i = 0; i < XmlQueryCardinality.serialized.Length; i++)
			{
				if (s == XmlQueryCardinality.serialized[i])
				{
					this.value = i;
					return;
				}
			}
		}

		public void GetObjectData(BinaryWriter writer)
		{
			writer.Write((byte)this.value);
		}

		public XmlQueryCardinality(BinaryReader reader)
		{
			this = new XmlQueryCardinality((int)reader.ReadByte());
		}

		// Note: this type is marked as 'beforefieldinit'.
		static XmlQueryCardinality()
		{
			XmlQueryCardinality[,] array = new XmlQueryCardinality[8, 8];
			array[0, 0] = XmlQueryCardinality.None;
			array[0, 1] = XmlQueryCardinality.Zero;
			array[0, 2] = XmlQueryCardinality.None;
			array[0, 3] = XmlQueryCardinality.Zero;
			array[0, 4] = XmlQueryCardinality.None;
			array[0, 5] = XmlQueryCardinality.Zero;
			array[0, 6] = XmlQueryCardinality.None;
			array[0, 7] = XmlQueryCardinality.Zero;
			array[1, 0] = XmlQueryCardinality.Zero;
			array[1, 1] = XmlQueryCardinality.Zero;
			array[1, 2] = XmlQueryCardinality.Zero;
			array[1, 3] = XmlQueryCardinality.Zero;
			array[1, 4] = XmlQueryCardinality.Zero;
			array[1, 5] = XmlQueryCardinality.Zero;
			array[1, 6] = XmlQueryCardinality.Zero;
			array[1, 7] = XmlQueryCardinality.Zero;
			array[2, 0] = XmlQueryCardinality.None;
			array[2, 1] = XmlQueryCardinality.Zero;
			array[2, 2] = XmlQueryCardinality.One;
			array[2, 3] = XmlQueryCardinality.ZeroOrOne;
			array[2, 4] = XmlQueryCardinality.More;
			array[2, 5] = XmlQueryCardinality.NotOne;
			array[2, 6] = XmlQueryCardinality.OneOrMore;
			array[2, 7] = XmlQueryCardinality.ZeroOrMore;
			array[3, 0] = XmlQueryCardinality.Zero;
			array[3, 1] = XmlQueryCardinality.Zero;
			array[3, 2] = XmlQueryCardinality.ZeroOrOne;
			array[3, 3] = XmlQueryCardinality.ZeroOrOne;
			array[3, 4] = XmlQueryCardinality.NotOne;
			array[3, 5] = XmlQueryCardinality.NotOne;
			array[3, 6] = XmlQueryCardinality.ZeroOrMore;
			array[3, 7] = XmlQueryCardinality.ZeroOrMore;
			array[4, 0] = XmlQueryCardinality.None;
			array[4, 1] = XmlQueryCardinality.Zero;
			array[4, 2] = XmlQueryCardinality.More;
			array[4, 3] = XmlQueryCardinality.NotOne;
			array[4, 4] = XmlQueryCardinality.More;
			array[4, 5] = XmlQueryCardinality.NotOne;
			array[4, 6] = XmlQueryCardinality.More;
			array[4, 7] = XmlQueryCardinality.NotOne;
			array[5, 0] = XmlQueryCardinality.Zero;
			array[5, 1] = XmlQueryCardinality.Zero;
			array[5, 2] = XmlQueryCardinality.NotOne;
			array[5, 3] = XmlQueryCardinality.NotOne;
			array[5, 4] = XmlQueryCardinality.NotOne;
			array[5, 5] = XmlQueryCardinality.NotOne;
			array[5, 6] = XmlQueryCardinality.NotOne;
			array[5, 7] = XmlQueryCardinality.NotOne;
			array[6, 0] = XmlQueryCardinality.None;
			array[6, 1] = XmlQueryCardinality.Zero;
			array[6, 2] = XmlQueryCardinality.OneOrMore;
			array[6, 3] = XmlQueryCardinality.ZeroOrMore;
			array[6, 4] = XmlQueryCardinality.More;
			array[6, 5] = XmlQueryCardinality.NotOne;
			array[6, 6] = XmlQueryCardinality.OneOrMore;
			array[6, 7] = XmlQueryCardinality.ZeroOrMore;
			array[7, 0] = XmlQueryCardinality.Zero;
			array[7, 1] = XmlQueryCardinality.Zero;
			array[7, 2] = XmlQueryCardinality.ZeroOrMore;
			array[7, 3] = XmlQueryCardinality.ZeroOrMore;
			array[7, 4] = XmlQueryCardinality.NotOne;
			array[7, 5] = XmlQueryCardinality.NotOne;
			array[7, 6] = XmlQueryCardinality.ZeroOrMore;
			array[7, 7] = XmlQueryCardinality.ZeroOrMore;
			XmlQueryCardinality.cardinalityProduct = array;
			XmlQueryCardinality[,] array2 = new XmlQueryCardinality[8, 8];
			array2[0, 0] = XmlQueryCardinality.None;
			array2[0, 1] = XmlQueryCardinality.Zero;
			array2[0, 2] = XmlQueryCardinality.One;
			array2[0, 3] = XmlQueryCardinality.ZeroOrOne;
			array2[0, 4] = XmlQueryCardinality.More;
			array2[0, 5] = XmlQueryCardinality.NotOne;
			array2[0, 6] = XmlQueryCardinality.OneOrMore;
			array2[0, 7] = XmlQueryCardinality.ZeroOrMore;
			array2[1, 0] = XmlQueryCardinality.Zero;
			array2[1, 1] = XmlQueryCardinality.Zero;
			array2[1, 2] = XmlQueryCardinality.One;
			array2[1, 3] = XmlQueryCardinality.ZeroOrOne;
			array2[1, 4] = XmlQueryCardinality.More;
			array2[1, 5] = XmlQueryCardinality.NotOne;
			array2[1, 6] = XmlQueryCardinality.OneOrMore;
			array2[1, 7] = XmlQueryCardinality.ZeroOrMore;
			array2[2, 0] = XmlQueryCardinality.One;
			array2[2, 1] = XmlQueryCardinality.One;
			array2[2, 2] = XmlQueryCardinality.More;
			array2[2, 3] = XmlQueryCardinality.OneOrMore;
			array2[2, 4] = XmlQueryCardinality.More;
			array2[2, 5] = XmlQueryCardinality.OneOrMore;
			array2[2, 6] = XmlQueryCardinality.More;
			array2[2, 7] = XmlQueryCardinality.OneOrMore;
			array2[3, 0] = XmlQueryCardinality.ZeroOrOne;
			array2[3, 1] = XmlQueryCardinality.ZeroOrOne;
			array2[3, 2] = XmlQueryCardinality.OneOrMore;
			array2[3, 3] = XmlQueryCardinality.ZeroOrMore;
			array2[3, 4] = XmlQueryCardinality.More;
			array2[3, 5] = XmlQueryCardinality.ZeroOrMore;
			array2[3, 6] = XmlQueryCardinality.OneOrMore;
			array2[3, 7] = XmlQueryCardinality.ZeroOrMore;
			array2[4, 0] = XmlQueryCardinality.More;
			array2[4, 1] = XmlQueryCardinality.More;
			array2[4, 2] = XmlQueryCardinality.More;
			array2[4, 3] = XmlQueryCardinality.More;
			array2[4, 4] = XmlQueryCardinality.More;
			array2[4, 5] = XmlQueryCardinality.More;
			array2[4, 6] = XmlQueryCardinality.More;
			array2[4, 7] = XmlQueryCardinality.More;
			array2[5, 0] = XmlQueryCardinality.NotOne;
			array2[5, 1] = XmlQueryCardinality.NotOne;
			array2[5, 2] = XmlQueryCardinality.OneOrMore;
			array2[5, 3] = XmlQueryCardinality.ZeroOrMore;
			array2[5, 4] = XmlQueryCardinality.More;
			array2[5, 5] = XmlQueryCardinality.NotOne;
			array2[5, 6] = XmlQueryCardinality.OneOrMore;
			array2[5, 7] = XmlQueryCardinality.ZeroOrMore;
			array2[6, 0] = XmlQueryCardinality.OneOrMore;
			array2[6, 1] = XmlQueryCardinality.OneOrMore;
			array2[6, 2] = XmlQueryCardinality.More;
			array2[6, 3] = XmlQueryCardinality.OneOrMore;
			array2[6, 4] = XmlQueryCardinality.More;
			array2[6, 5] = XmlQueryCardinality.OneOrMore;
			array2[6, 6] = XmlQueryCardinality.More;
			array2[6, 7] = XmlQueryCardinality.OneOrMore;
			array2[7, 0] = XmlQueryCardinality.ZeroOrMore;
			array2[7, 1] = XmlQueryCardinality.ZeroOrMore;
			array2[7, 2] = XmlQueryCardinality.OneOrMore;
			array2[7, 3] = XmlQueryCardinality.ZeroOrMore;
			array2[7, 4] = XmlQueryCardinality.More;
			array2[7, 5] = XmlQueryCardinality.ZeroOrMore;
			array2[7, 6] = XmlQueryCardinality.OneOrMore;
			array2[7, 7] = XmlQueryCardinality.ZeroOrMore;
			XmlQueryCardinality.cardinalitySum = array2;
			XmlQueryCardinality.toString = new string[] { "", "?", "", "?", "+", "*", "+", "*" };
			XmlQueryCardinality.serialized = new string[] { "None", "Zero", "One", "ZeroOrOne", "More", "NotOne", "OneOrMore", "ZeroOrMore" };
		}

		private int value;

		private static readonly XmlQueryCardinality[,] cardinalityProduct;

		private static readonly XmlQueryCardinality[,] cardinalitySum;

		private static readonly string[] toString;

		private static readonly string[] serialized;
	}
}
