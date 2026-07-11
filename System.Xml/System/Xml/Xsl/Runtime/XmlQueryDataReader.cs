using System;
using System.IO;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlQueryDataReader : BinaryReader
	{
		public XmlQueryDataReader(Stream input)
			: base(input)
		{
		}

		public int ReadInt32Encoded()
		{
			return base.Read7BitEncodedInt();
		}

		public string ReadStringQ()
		{
			if (!this.ReadBoolean())
			{
				return null;
			}
			return this.ReadString();
		}

		public sbyte ReadSByte(sbyte minValue, sbyte maxValue)
		{
			sbyte b = this.ReadSByte();
			if (b < minValue)
			{
				throw new ArgumentOutOfRangeException("minValue");
			}
			if (maxValue < b)
			{
				throw new ArgumentOutOfRangeException("maxValue");
			}
			return b;
		}
	}
}
