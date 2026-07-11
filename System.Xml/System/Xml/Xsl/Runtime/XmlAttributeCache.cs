using System;
using System.Xml.Schema;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class XmlAttributeCache : XmlRawWriter, IRemovableWriter
	{
		public void Init(XmlRawWriter wrapped)
		{
			this.SetWrappedWriter(wrapped);
			this.numEntries = 0;
			this.idxLastName = 0;
			this.hashCodeUnion = 0;
		}

		public int Count
		{
			get
			{
				return this.numEntries;
			}
		}

		public OnRemoveWriter OnRemoveWriterEvent
		{
			get
			{
				return this.onRemove;
			}
			set
			{
				this.onRemove = value;
			}
		}

		private void SetWrappedWriter(XmlRawWriter writer)
		{
			IRemovableWriter removableWriter = writer as IRemovableWriter;
			if (removableWriter != null)
			{
				removableWriter.OnRemoveWriterEvent = new OnRemoveWriter(this.SetWrappedWriter);
			}
			this.wrapped = writer;
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			int num = 0;
			int num2 = 1 << (int)localName[0];
			if ((this.hashCodeUnion & num2) != 0)
			{
				while (!this.arrAttrs[num].IsDuplicate(localName, ns, num2))
				{
					num = this.arrAttrs[num].NextNameIndex;
					if (num == 0)
					{
						break;
					}
				}
			}
			else
			{
				this.hashCodeUnion |= num2;
			}
			this.EnsureAttributeCache();
			if (this.numEntries != 0)
			{
				this.arrAttrs[this.idxLastName].NextNameIndex = this.numEntries;
			}
			int num3 = this.numEntries;
			this.numEntries = num3 + 1;
			this.idxLastName = num3;
			this.arrAttrs[this.idxLastName].Init(prefix, localName, ns, num2);
		}

		public override void WriteEndAttribute()
		{
		}

		internal override void WriteNamespaceDeclaration(string prefix, string ns)
		{
			this.FlushAttributes();
			this.wrapped.WriteNamespaceDeclaration(prefix, ns);
		}

		public override void WriteString(string text)
		{
			this.EnsureAttributeCache();
			XmlAttributeCache.AttrNameVal[] array = this.arrAttrs;
			int num = this.numEntries;
			this.numEntries = num + 1;
			array[num].Init(text);
		}

		public override void WriteValue(object value)
		{
			this.EnsureAttributeCache();
			XmlAttributeCache.AttrNameVal[] array = this.arrAttrs;
			int num = this.numEntries;
			this.numEntries = num + 1;
			array[num].Init((XmlAtomicValue)value);
		}

		public override void WriteValue(string value)
		{
			this.WriteValue(value);
		}

		internal override void StartElementContent()
		{
			this.FlushAttributes();
			this.wrapped.StartElementContent();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
		}

		public override void WriteComment(string text)
		{
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
		}

		public override void WriteEntityRef(string name)
		{
		}

		public override void Close()
		{
			this.wrapped.Close();
		}

		public override void Flush()
		{
			this.wrapped.Flush();
		}

		private void FlushAttributes()
		{
			int num = 0;
			while (num != this.numEntries)
			{
				int nextNameIndex = this.arrAttrs[num].NextNameIndex;
				if (nextNameIndex == 0)
				{
					nextNameIndex = this.numEntries;
				}
				string localName = this.arrAttrs[num].LocalName;
				if (localName != null)
				{
					string prefix = this.arrAttrs[num].Prefix;
					string @namespace = this.arrAttrs[num].Namespace;
					this.wrapped.WriteStartAttribute(prefix, localName, @namespace);
					while (++num != nextNameIndex)
					{
						string text = this.arrAttrs[num].Text;
						if (text != null)
						{
							this.wrapped.WriteString(text);
						}
						else
						{
							this.wrapped.WriteValue(this.arrAttrs[num].Value);
						}
					}
					this.wrapped.WriteEndAttribute();
				}
				else
				{
					num = nextNameIndex;
				}
			}
			if (this.onRemove != null)
			{
				this.onRemove(this.wrapped);
			}
		}

		private void EnsureAttributeCache()
		{
			if (this.arrAttrs == null)
			{
				this.arrAttrs = new XmlAttributeCache.AttrNameVal[32];
				return;
			}
			if (this.numEntries >= this.arrAttrs.Length)
			{
				XmlAttributeCache.AttrNameVal[] array = new XmlAttributeCache.AttrNameVal[this.numEntries * 2];
				Array.Copy(this.arrAttrs, array, this.numEntries);
				this.arrAttrs = array;
			}
		}

		private XmlRawWriter wrapped;

		private OnRemoveWriter onRemove;

		private XmlAttributeCache.AttrNameVal[] arrAttrs;

		private int numEntries;

		private int idxLastName;

		private int hashCodeUnion;

		private const int DefaultCacheSize = 32;

		private struct AttrNameVal
		{
			public string LocalName
			{
				get
				{
					return this.localName;
				}
			}

			public string Prefix
			{
				get
				{
					return this.prefix;
				}
			}

			public string Namespace
			{
				get
				{
					return this.namespaceName;
				}
			}

			public string Text
			{
				get
				{
					return this.text;
				}
			}

			public XmlAtomicValue Value
			{
				get
				{
					return this.value;
				}
			}

			public int NextNameIndex
			{
				get
				{
					return this.nextNameIndex;
				}
				set
				{
					this.nextNameIndex = value;
				}
			}

			public void Init(string prefix, string localName, string ns, int hashCode)
			{
				this.localName = localName;
				this.prefix = prefix;
				this.namespaceName = ns;
				this.hashCode = hashCode;
				this.nextNameIndex = 0;
			}

			public void Init(string text)
			{
				this.text = text;
				this.value = null;
			}

			public void Init(XmlAtomicValue value)
			{
				this.text = null;
				this.value = value;
			}

			public bool IsDuplicate(string localName, string ns, int hashCode)
			{
				if (this.localName != null && this.hashCode == hashCode && this.localName.Equals(localName) && this.namespaceName.Equals(ns))
				{
					this.localName = null;
					return true;
				}
				return false;
			}

			private string localName;

			private string prefix;

			private string namespaceName;

			private string text;

			private XmlAtomicValue value;

			private int hashCode;

			private int nextNameIndex;
		}
	}
}
