using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Xml
{
	public class XmlDeclaration : XmlLinkedNode
	{
		protected internal XmlDeclaration(string version, string encoding, string standalone, XmlDocument doc)
			: base(doc)
		{
			if (encoding == null)
			{
				encoding = string.Empty;
			}
			if (standalone == null)
			{
				standalone = string.Empty;
			}
			this.version = version;
			this.encoding = encoding;
			this.standalone = standalone;
		}

		public string Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = ((value != null) ? value : string.Empty);
			}
		}

		public override string InnerText
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.ParseInput(value);
			}
		}

		public override string LocalName
		{
			get
			{
				return "xml";
			}
		}

		public override string Name
		{
			get
			{
				return "xml";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.XmlDeclaration;
			}
		}

		public string Standalone
		{
			get
			{
				return this.standalone;
			}
			set
			{
				if (value != null)
				{
					if (string.Compare(value, "YES", true, CultureInfo.InvariantCulture) == 0)
					{
						this.standalone = "yes";
					}
					if (string.Compare(value, "NO", true, CultureInfo.InvariantCulture) == 0)
					{
						this.standalone = "no";
					}
				}
				else
				{
					this.standalone = string.Empty;
				}
			}
		}

		public override string Value
		{
			get
			{
				string text = string.Empty;
				string text2 = string.Empty;
				if (this.encoding != string.Empty)
				{
					text = string.Format(" encoding=\"{0}\"", this.encoding);
				}
				if (this.standalone != string.Empty)
				{
					text2 = string.Format(" standalone=\"{0}\"", this.standalone);
				}
				return string.Format("version=\"{0}\"{1}{2}", this.Version, text, text2);
			}
			set
			{
				this.ParseInput(value);
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlDeclaration(this.Version, this.Encoding, this.standalone, this.OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteRaw(string.Format("<?xml {0}?>", this.Value));
		}

		private int SkipWhitespace(string input, int index)
		{
			while (index < input.Length)
			{
				if (!XmlChar.IsWhitespace((int)input[index]))
				{
					break;
				}
				index++;
			}
			return index;
		}

		private void ParseInput(string input)
		{
			int num = this.SkipWhitespace(input, 0);
			if (num + 7 > input.Length || input.IndexOf("version", num, 7) != num)
			{
				throw new XmlException("Missing 'version' specification.");
			}
			num = this.SkipWhitespace(input, num + 7);
			char c = input[num];
			if (c != '=')
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num++;
			num = this.SkipWhitespace(input, num);
			c = input[num];
			if (c != '"' && c != '\'')
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num++;
			int num2 = input.IndexOf(c, num);
			if (num2 < 0 || input.IndexOf("1.0", num, 3) != num)
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num += 4;
			if (num == input.Length)
			{
				return;
			}
			if (!XmlChar.IsWhitespace((int)input[num]))
			{
				throw new XmlException("Invalid XML declaration.");
			}
			num = this.SkipWhitespace(input, num + 1);
			if (num == input.Length)
			{
				return;
			}
			if (input.Length > num + 8 && input.IndexOf("encoding", num, 8) > 0)
			{
				num = this.SkipWhitespace(input, num + 8);
				c = input[num];
				if (c != '=')
				{
					throw new XmlException("Invalid 'version' specification.");
				}
				num++;
				num = this.SkipWhitespace(input, num);
				c = input[num];
				if (c != '"' && c != '\'')
				{
					throw new XmlException("Invalid 'encoding' specification.");
				}
				num2 = input.IndexOf(c, num + 1);
				if (num2 < 0)
				{
					throw new XmlException("Invalid 'encoding' specification.");
				}
				this.Encoding = input.Substring(num + 1, num2 - num - 1);
				num = num2 + 1;
				if (num == input.Length)
				{
					return;
				}
				if (!XmlChar.IsWhitespace((int)input[num]))
				{
					throw new XmlException("Invalid XML declaration.");
				}
				num = this.SkipWhitespace(input, num + 1);
			}
			if (input.Length > num + 10 && input.IndexOf("standalone", num, 10) > 0)
			{
				num = this.SkipWhitespace(input, num + 10);
				c = input[num];
				if (c != '=')
				{
					throw new XmlException("Invalid 'version' specification.");
				}
				num++;
				num = this.SkipWhitespace(input, num);
				c = input[num];
				if (c != '"' && c != '\'')
				{
					throw new XmlException("Invalid 'standalone' specification.");
				}
				num2 = input.IndexOf(c, num + 1);
				if (num2 < 0)
				{
					throw new XmlException("Invalid 'standalone' specification.");
				}
				string text = input.Substring(num + 1, num2 - num - 1);
				string text2 = text;
				if (text2 != null)
				{
					if (XmlDeclaration.<>f__switch$map30 == null)
					{
						XmlDeclaration.<>f__switch$map30 = new Dictionary<string, int>(2)
						{
							{ "yes", 0 },
							{ "no", 0 }
						};
					}
					int num3;
					if (XmlDeclaration.<>f__switch$map30.TryGetValue(text2, out num3))
					{
						if (num3 == 0)
						{
							this.Standalone = text;
							num = num2 + 1;
							num = this.SkipWhitespace(input, num);
							goto IL_0308;
						}
					}
				}
				throw new XmlException("Invalid standalone specification.");
			}
			IL_0308:
			if (num != input.Length)
			{
				throw new XmlException("Invalid XML declaration.");
			}
		}

		private string encoding = "UTF-8";

		private string standalone;

		private string version;
	}
}
