using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System.Xml.Xsl.XPath
{
	[Serializable]
	internal class XPathCompileException : XslLoadException
	{
		protected XPathCompileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.queryString = (string)info.GetValue("QueryString", typeof(string));
			this.startChar = (int)info.GetValue("StartChar", typeof(int));
			this.endChar = (int)info.GetValue("EndChar", typeof(int));
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("QueryString", this.queryString);
			info.AddValue("StartChar", this.startChar);
			info.AddValue("EndChar", this.endChar);
		}

		internal XPathCompileException(string queryString, int startChar, int endChar, string resId, params string[] args)
			: base(resId, args)
		{
			this.queryString = queryString;
			this.startChar = startChar;
			this.endChar = endChar;
		}

		internal XPathCompileException(string resId, params string[] args)
			: base(resId, args)
		{
		}

		private static void AppendTrimmed(StringBuilder sb, string value, int startIndex, int count, XPathCompileException.TrimType trimType)
		{
			if (count <= 32)
			{
				sb.Append(value, startIndex, count);
				return;
			}
			switch (trimType)
			{
			case XPathCompileException.TrimType.Left:
				sb.Append("...");
				sb.Append(value, startIndex + count - 32, 32);
				return;
			case XPathCompileException.TrimType.Right:
				sb.Append(value, startIndex, 32);
				sb.Append("...");
				return;
			case XPathCompileException.TrimType.Middle:
				sb.Append(value, startIndex, 16);
				sb.Append("...");
				sb.Append(value, startIndex + count - 16, 16);
				return;
			default:
				return;
			}
		}

		internal string MarkOutError()
		{
			if (this.queryString == null || this.queryString.Trim(new char[] { ' ' }).Length == 0)
			{
				return null;
			}
			int num = this.endChar - this.startChar;
			StringBuilder stringBuilder = new StringBuilder();
			XPathCompileException.AppendTrimmed(stringBuilder, this.queryString, 0, this.startChar, XPathCompileException.TrimType.Left);
			if (num > 0)
			{
				stringBuilder.Append(" -->");
				XPathCompileException.AppendTrimmed(stringBuilder, this.queryString, this.startChar, num, XPathCompileException.TrimType.Middle);
			}
			stringBuilder.Append("<-- ");
			XPathCompileException.AppendTrimmed(stringBuilder, this.queryString, this.endChar, this.queryString.Length - this.endChar, XPathCompileException.TrimType.Right);
			return stringBuilder.ToString();
		}

		internal override string FormatDetailedMessage()
		{
			string text = this.Message;
			string text2 = this.MarkOutError();
			if (text2 != null && text2.Length > 0)
			{
				if (text.Length > 0)
				{
					text += Environment.NewLine;
				}
				text += text2;
			}
			return text;
		}

		public string queryString;

		public int startChar;

		public int endChar;

		private enum TrimType
		{
			Left,
			Right,
			Middle
		}
	}
}
