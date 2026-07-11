using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml.Linq
{
	public class XAttribute : XObject
	{
		public XAttribute(XAttribute other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.value = other.value;
		}

		public XAttribute(XName name, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.SetValue(value);
		}

		public static IEnumerable<XAttribute> EmptySequence
		{
			get
			{
				return XAttribute.empty_array;
			}
		}

		public bool IsNamespaceDeclaration
		{
			get
			{
				return this.name.Namespace == XNamespace.Xmlns || (this.name.LocalName == "xmlns" && this.name.Namespace == XNamespace.None);
			}
		}

		public XName Name
		{
			get
			{
				return this.name;
			}
		}

		public XAttribute NextAttribute
		{
			get
			{
				return this.next;
			}
			internal set
			{
				this.next = value;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Attribute;
			}
		}

		public XAttribute PreviousAttribute
		{
			get
			{
				return this.previous;
			}
			internal set
			{
				this.previous = value;
			}
		}

		public string Value
		{
			get
			{
				return XUtil.ToString(this.value);
			}
			set
			{
				this.value = value;
			}
		}

		public void Remove()
		{
			if (base.Parent != null)
			{
				if (this.next != null)
				{
					this.next.previous = this.previous;
				}
				if (this.previous != null)
				{
					this.previous.next = this.next;
				}
				if (base.Parent.FirstAttribute == this)
				{
					base.Parent.FirstAttribute = this.next;
				}
				if (base.Parent.LastAttribute == this)
				{
					base.Parent.LastAttribute = this.previous;
				}
				base.SetOwner(null);
			}
			this.next = null;
			this.previous = null;
		}

		public void SetValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.value = XUtil.ToString(value);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.name.ToString());
			stringBuilder.Append("=\"");
			int num = 0;
			for (;;)
			{
				int num2 = this.value.IndexOfAny(XAttribute.escapeChars, num);
				if (num2 < 0)
				{
					break;
				}
				stringBuilder.Append(this.value, num, num2 - num);
				char c = this.value[num2];
				switch (c)
				{
				case '\t':
					stringBuilder.Append("&#x9;");
					break;
				case '\n':
					stringBuilder.Append("&#xA;");
					break;
				default:
					switch (c)
					{
					case '<':
						stringBuilder.Append("&lt;");
						break;
					default:
						if (c != '"')
						{
							if (c == '&')
							{
								stringBuilder.Append("&amp;");
							}
						}
						else
						{
							stringBuilder.Append("&quot;");
						}
						break;
					case '>':
						stringBuilder.Append("&gt;");
						break;
					}
					break;
				case '\r':
					stringBuilder.Append("&#xD;");
					break;
				}
				num = num2 + 1;
			}
			if (num > 0)
			{
				stringBuilder.Append(this.value, num, this.value.Length - num);
			}
			else
			{
				stringBuilder.Append(this.value);
			}
			stringBuilder.Append("\"");
			return stringBuilder.ToString();
		}

		public static explicit operator bool(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XUtil.ConvertToBoolean(attribute.value);
		}

		public static explicit operator bool?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new bool?(XUtil.ConvertToBoolean(attribute.value)) : null;
		}

		public static explicit operator DateTime(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XUtil.ToDateTime(attribute.value);
		}

		public static explicit operator DateTime?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new DateTime?(XUtil.ToDateTime(attribute.value)) : null;
		}

		public static explicit operator DateTimeOffset(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDateTimeOffset(attribute.value);
		}

		public static explicit operator DateTimeOffset?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new DateTimeOffset?(XmlConvert.ToDateTimeOffset(attribute.value)) : null;
		}

		public static explicit operator decimal(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDecimal(attribute.value);
		}

		public static explicit operator decimal?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new decimal?(XmlConvert.ToDecimal(attribute.value)) : null;
		}

		public static explicit operator double(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToDouble(attribute.value);
		}

		public static explicit operator double?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new double?(XmlConvert.ToDouble(attribute.value)) : null;
		}

		public static explicit operator float(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToSingle(attribute.value);
		}

		public static explicit operator float?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new float?(XmlConvert.ToSingle(attribute.value)) : null;
		}

		public static explicit operator Guid(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToGuid(attribute.value);
		}

		public static explicit operator Guid?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new Guid?(XmlConvert.ToGuid(attribute.value)) : null;
		}

		public static explicit operator int(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToInt32(attribute.value);
		}

		public static explicit operator int?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new int?(XmlConvert.ToInt32(attribute.value)) : null;
		}

		public static explicit operator long(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToInt64(attribute.value);
		}

		public static explicit operator long?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new long?(XmlConvert.ToInt64(attribute.value)) : null;
		}

		[CLSCompliant(false)]
		public static explicit operator uint(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToUInt32(attribute.value);
		}

		[CLSCompliant(false)]
		public static explicit operator uint?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new uint?(XmlConvert.ToUInt32(attribute.value)) : null;
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToUInt64(attribute.value);
		}

		[CLSCompliant(false)]
		public static explicit operator ulong?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new ulong?(XmlConvert.ToUInt64(attribute.value)) : null;
		}

		public static explicit operator TimeSpan(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			return XmlConvert.ToTimeSpan(attribute.value);
		}

		public static explicit operator TimeSpan?(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return (attribute.value != null) ? new TimeSpan?(XmlConvert.ToTimeSpan(attribute.value)) : null;
		}

		public static explicit operator string(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return attribute.value;
		}

		private static readonly XAttribute[] empty_array = new XAttribute[0];

		private XName name;

		private string value;

		private XAttribute next;

		private XAttribute previous;

		private static readonly char[] escapeChars = new char[] { '<', '>', '&', '"', '\r', '\n', '\t' };
	}
}
