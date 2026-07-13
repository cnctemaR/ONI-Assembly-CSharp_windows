using System;
using System.Xml;

namespace Unity.VectorGraphics
{
	internal class SVGFormatException : Exception
	{
		public SVGFormatException()
		{
		}

		public SVGFormatException(string message)
			: base(SVGFormatException.ComposeMessage(null, message))
		{
		}

		public SVGFormatException(XmlReader reader, string message)
			: base(SVGFormatException.ComposeMessage(reader, message))
		{
		}

		public static SVGFormatException StackError
		{
			get
			{
				return new SVGFormatException("Vector scene construction mismatch");
			}
		}

		private static string ComposeMessage(XmlReader reader, string message)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			bool flag = xmlLineInfo != null;
			string text;
			if (flag)
			{
				text = string.Concat(new string[]
				{
					"SVG Error (line ",
					xmlLineInfo.LineNumber.ToString(),
					", character ",
					xmlLineInfo.LinePosition.ToString(),
					"): ",
					message
				});
			}
			else
			{
				text = "SVG Error: " + message;
			}
			return text;
		}
	}
}
