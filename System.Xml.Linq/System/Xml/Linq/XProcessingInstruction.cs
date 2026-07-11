using System;

namespace System.Xml.Linq
{
	public class XProcessingInstruction : XNode
	{
		public XProcessingInstruction(string target, string data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			XProcessingInstruction.ValidateName(target);
			this.target = target;
			this.data = data;
		}

		public XProcessingInstruction(XProcessingInstruction other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.target = other.target;
			this.data = other.data;
		}

		internal XProcessingInstruction(XmlReader r)
		{
			this.target = r.Name;
			this.data = r.Value;
			r.Read();
		}

		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.data = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		public string Target
		{
			get
			{
				return this.target;
			}
			set
			{
				XProcessingInstruction.ValidateName(value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this.target = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteProcessingInstruction(this.target, this.data);
		}

		internal override XNode CloneNode()
		{
			return new XProcessingInstruction(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XProcessingInstruction xprocessingInstruction = node as XProcessingInstruction;
			return xprocessingInstruction != null && this.target == xprocessingInstruction.target && this.data == xprocessingInstruction.data;
		}

		internal override int GetDeepHashCode()
		{
			return this.target.GetHashCode() ^ this.data.GetHashCode();
		}

		private static void ValidateName(string name)
		{
			XmlConvert.VerifyNCName(name);
			if (string.Compare(name, "xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				throw new ArgumentException(Res.GetString("Argument_InvalidPIName", new object[] { name }));
			}
		}

		internal string target;

		internal string data;
	}
}
