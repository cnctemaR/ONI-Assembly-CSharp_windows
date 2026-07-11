using System;
using System.Text;

namespace System.Xml.Linq
{
	public class XCData : XText
	{
		public XCData(string value)
			: base(value)
		{
		}

		public XCData(XCData other)
			: base(other)
		{
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.CDATA;
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			int num = 0;
			StringBuilder stringBuilder = null;
			for (int i = 0; i < base.Value.Length - 2; i++)
			{
				if (base.Value[i] == ']' && base.Value[i + 1] == ']' && base.Value[i + 2] == '>')
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(base.Value, num, i - num);
					stringBuilder.Append("]]&gt;");
					num = i + 3;
				}
			}
			if (num != 0 && num != base.Value.Length)
			{
				stringBuilder.Append(base.Value, num, base.Value.Length - num);
			}
			w.WriteCData((stringBuilder != null) ? stringBuilder.ToString() : base.Value);
		}
	}
}
