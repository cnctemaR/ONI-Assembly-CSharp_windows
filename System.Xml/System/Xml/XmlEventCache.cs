using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml.Xsl.Runtime;

namespace System.Xml
{
	internal sealed class XmlEventCache : XmlRawWriter
	{
		public XmlEventCache(string baseUri, bool hasRootNode)
		{
			this.baseUri = baseUri;
			this.hasRootNode = hasRootNode;
		}

		public void EndEvents()
		{
			if (this.singleText.Count == 0)
			{
				this.AddEvent(XmlEventCache.XmlEventType.Unknown);
			}
		}

		public string BaseUri
		{
			get
			{
				return this.baseUri;
			}
		}

		public bool HasRootNode
		{
			get
			{
				return this.hasRootNode;
			}
		}

		public void EventsToWriter(XmlWriter writer)
		{
			if (this.singleText.Count != 0)
			{
				writer.WriteString(this.singleText.GetResult());
				return;
			}
			XmlRawWriter xmlRawWriter = writer as XmlRawWriter;
			for (int i = 0; i < this.pages.Count; i++)
			{
				XmlEventCache.XmlEvent[] array = this.pages[i];
				for (int j = 0; j < array.Length; j++)
				{
					switch (array[j].EventType)
					{
					case XmlEventCache.XmlEventType.Unknown:
						return;
					case XmlEventCache.XmlEventType.DocType:
						writer.WriteDocType(array[j].String1, array[j].String2, array[j].String3, (string)array[j].Object);
						break;
					case XmlEventCache.XmlEventType.StartElem:
						writer.WriteStartElement(array[j].String1, array[j].String2, array[j].String3);
						break;
					case XmlEventCache.XmlEventType.StartAttr:
						writer.WriteStartAttribute(array[j].String1, array[j].String2, array[j].String3);
						break;
					case XmlEventCache.XmlEventType.EndAttr:
						writer.WriteEndAttribute();
						break;
					case XmlEventCache.XmlEventType.CData:
						writer.WriteCData(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.Comment:
						writer.WriteComment(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.PI:
						writer.WriteProcessingInstruction(array[j].String1, array[j].String2);
						break;
					case XmlEventCache.XmlEventType.Whitespace:
						writer.WriteWhitespace(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.String:
						writer.WriteString(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.Raw:
						writer.WriteRaw(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.EntRef:
						writer.WriteEntityRef(array[j].String1);
						break;
					case XmlEventCache.XmlEventType.CharEnt:
						writer.WriteCharEntity((char)array[j].Object);
						break;
					case XmlEventCache.XmlEventType.SurrCharEnt:
					{
						char[] array2 = (char[])array[j].Object;
						writer.WriteSurrogateCharEntity(array2[0], array2[1]);
						break;
					}
					case XmlEventCache.XmlEventType.Base64:
					{
						byte[] array3 = (byte[])array[j].Object;
						writer.WriteBase64(array3, 0, array3.Length);
						break;
					}
					case XmlEventCache.XmlEventType.BinHex:
					{
						byte[] array3 = (byte[])array[j].Object;
						writer.WriteBinHex(array3, 0, array3.Length);
						break;
					}
					case XmlEventCache.XmlEventType.XmlDecl1:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteXmlDeclaration((XmlStandalone)array[j].Object);
						}
						break;
					case XmlEventCache.XmlEventType.XmlDecl2:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteXmlDeclaration(array[j].String1);
						}
						break;
					case XmlEventCache.XmlEventType.StartContent:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.StartElementContent();
						}
						break;
					case XmlEventCache.XmlEventType.EndElem:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteEndElement(array[j].String1, array[j].String2, array[j].String3);
						}
						else
						{
							writer.WriteEndElement();
						}
						break;
					case XmlEventCache.XmlEventType.FullEndElem:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteFullEndElement(array[j].String1, array[j].String2, array[j].String3);
						}
						else
						{
							writer.WriteFullEndElement();
						}
						break;
					case XmlEventCache.XmlEventType.Nmsp:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteNamespaceDeclaration(array[j].String1, array[j].String2);
						}
						else
						{
							writer.WriteAttributeString("xmlns", array[j].String1, "http://www.w3.org/2000/xmlns/", array[j].String2);
						}
						break;
					case XmlEventCache.XmlEventType.EndBase64:
						if (xmlRawWriter != null)
						{
							xmlRawWriter.WriteEndBase64();
						}
						break;
					case XmlEventCache.XmlEventType.Close:
						writer.Close();
						break;
					case XmlEventCache.XmlEventType.Flush:
						writer.Flush();
						break;
					case XmlEventCache.XmlEventType.Dispose:
						((IDisposable)writer).Dispose();
						break;
					}
				}
			}
		}

		public string EventsToString()
		{
			if (this.singleText.Count != 0)
			{
				return this.singleText.GetResult();
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int i = 0; i < this.pages.Count; i++)
			{
				XmlEventCache.XmlEvent[] array = this.pages[i];
				for (int j = 0; j < array.Length; j++)
				{
					switch (array[j].EventType)
					{
					case XmlEventCache.XmlEventType.Unknown:
						return stringBuilder.ToString();
					case XmlEventCache.XmlEventType.StartAttr:
						flag = true;
						break;
					case XmlEventCache.XmlEventType.EndAttr:
						flag = false;
						break;
					case XmlEventCache.XmlEventType.CData:
					case XmlEventCache.XmlEventType.Whitespace:
					case XmlEventCache.XmlEventType.String:
					case XmlEventCache.XmlEventType.Raw:
						if (!flag)
						{
							stringBuilder.Append(array[j].String1);
						}
						break;
					}
				}
			}
			return string.Empty;
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return null;
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.AddEvent(XmlEventCache.XmlEventType.DocType, name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.AddEvent(XmlEventCache.XmlEventType.StartElem, prefix, localName, ns);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.AddEvent(XmlEventCache.XmlEventType.StartAttr, prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.AddEvent(XmlEventCache.XmlEventType.EndAttr);
		}

		public override void WriteCData(string text)
		{
			this.AddEvent(XmlEventCache.XmlEventType.CData, text);
		}

		public override void WriteComment(string text)
		{
			this.AddEvent(XmlEventCache.XmlEventType.Comment, text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.AddEvent(XmlEventCache.XmlEventType.PI, name, text);
		}

		public override void WriteWhitespace(string ws)
		{
			this.AddEvent(XmlEventCache.XmlEventType.Whitespace, ws);
		}

		public override void WriteString(string text)
		{
			if (this.pages == null)
			{
				this.singleText.ConcatNoDelimiter(text);
				return;
			}
			this.AddEvent(XmlEventCache.XmlEventType.String, text);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteRaw(new string(buffer, index, count));
		}

		public override void WriteRaw(string data)
		{
			this.AddEvent(XmlEventCache.XmlEventType.Raw, data);
		}

		public override void WriteEntityRef(string name)
		{
			this.AddEvent(XmlEventCache.XmlEventType.EntRef, name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.AddEvent(XmlEventCache.XmlEventType.CharEnt, ch);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			char[] array = new char[] { lowChar, highChar };
			this.AddEvent(XmlEventCache.XmlEventType.SurrCharEnt, array);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.AddEvent(XmlEventCache.XmlEventType.Base64, XmlEventCache.ToBytes(buffer, index, count));
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.AddEvent(XmlEventCache.XmlEventType.BinHex, XmlEventCache.ToBytes(buffer, index, count));
		}

		public override void Close()
		{
			this.AddEvent(XmlEventCache.XmlEventType.Close);
		}

		public override void Flush()
		{
			this.AddEvent(XmlEventCache.XmlEventType.Flush);
		}

		public override void WriteValue(object value)
		{
			this.WriteString(XmlUntypedConverter.Untyped.ToString(value, this.resolver));
		}

		public override void WriteValue(string value)
		{
			this.WriteString(value);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.AddEvent(XmlEventCache.XmlEventType.Dispose);
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
			this.AddEvent(XmlEventCache.XmlEventType.XmlDecl1, standalone);
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
			this.AddEvent(XmlEventCache.XmlEventType.XmlDecl2, xmldecl);
		}

		internal override void StartElementContent()
		{
			this.AddEvent(XmlEventCache.XmlEventType.StartContent);
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			this.AddEvent(XmlEventCache.XmlEventType.EndElem, prefix, localName, ns);
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			this.AddEvent(XmlEventCache.XmlEventType.FullEndElem, prefix, localName, ns);
		}

		internal override void WriteNamespaceDeclaration(string prefix, string ns)
		{
			this.AddEvent(XmlEventCache.XmlEventType.Nmsp, prefix, ns);
		}

		internal override void WriteEndBase64()
		{
			this.AddEvent(XmlEventCache.XmlEventType.EndBase64);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType, string s1)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType, s1);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType, string s1, string s2)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType, s1, s2);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType, string s1, string s2, string s3)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType, s1, s2, s3);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType, string s1, string s2, string s3, object o)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType, s1, s2, s3, o);
		}

		private void AddEvent(XmlEventCache.XmlEventType eventType, object o)
		{
			int num = this.NewEvent();
			this.pageCurr[num].InitEvent(eventType, o);
		}

		private int NewEvent()
		{
			if (this.pages == null)
			{
				this.pages = new List<XmlEventCache.XmlEvent[]>();
				this.pageCurr = new XmlEventCache.XmlEvent[32];
				this.pages.Add(this.pageCurr);
				if (this.singleText.Count != 0)
				{
					this.pageCurr[0].InitEvent(XmlEventCache.XmlEventType.String, this.singleText.GetResult());
					this.pageSize++;
					this.singleText.Clear();
				}
			}
			else if (this.pageSize >= this.pageCurr.Length)
			{
				this.pageCurr = new XmlEventCache.XmlEvent[this.pageSize * 2];
				this.pages.Add(this.pageCurr);
				this.pageSize = 0;
			}
			int num = this.pageSize;
			this.pageSize = num + 1;
			return num;
		}

		private static byte[] ToBytes(byte[] buffer, int index, int count)
		{
			if (index != 0 || count != buffer.Length)
			{
				if (buffer.Length - index > count)
				{
					count = buffer.Length - index;
				}
				byte[] array = new byte[count];
				Array.Copy(buffer, index, array, 0, count);
				return array;
			}
			return buffer;
		}

		private List<XmlEventCache.XmlEvent[]> pages;

		private XmlEventCache.XmlEvent[] pageCurr;

		private int pageSize;

		private bool hasRootNode;

		private StringConcat singleText;

		private string baseUri;

		private const int InitialPageSize = 32;

		private enum XmlEventType
		{
			Unknown,
			DocType,
			StartElem,
			StartAttr,
			EndAttr,
			CData,
			Comment,
			PI,
			Whitespace,
			String,
			Raw,
			EntRef,
			CharEnt,
			SurrCharEnt,
			Base64,
			BinHex,
			XmlDecl1,
			XmlDecl2,
			StartContent,
			EndElem,
			FullEndElem,
			Nmsp,
			EndBase64,
			Close,
			Flush,
			Dispose
		}

		private struct XmlEvent
		{
			public void InitEvent(XmlEventCache.XmlEventType eventType)
			{
				this.eventType = eventType;
			}

			public void InitEvent(XmlEventCache.XmlEventType eventType, string s1)
			{
				this.eventType = eventType;
				this.s1 = s1;
			}

			public void InitEvent(XmlEventCache.XmlEventType eventType, string s1, string s2)
			{
				this.eventType = eventType;
				this.s1 = s1;
				this.s2 = s2;
			}

			public void InitEvent(XmlEventCache.XmlEventType eventType, string s1, string s2, string s3)
			{
				this.eventType = eventType;
				this.s1 = s1;
				this.s2 = s2;
				this.s3 = s3;
			}

			public void InitEvent(XmlEventCache.XmlEventType eventType, string s1, string s2, string s3, object o)
			{
				this.eventType = eventType;
				this.s1 = s1;
				this.s2 = s2;
				this.s3 = s3;
				this.o = o;
			}

			public void InitEvent(XmlEventCache.XmlEventType eventType, object o)
			{
				this.eventType = eventType;
				this.o = o;
			}

			public XmlEventCache.XmlEventType EventType
			{
				get
				{
					return this.eventType;
				}
			}

			public string String1
			{
				get
				{
					return this.s1;
				}
			}

			public string String2
			{
				get
				{
					return this.s2;
				}
			}

			public string String3
			{
				get
				{
					return this.s3;
				}
			}

			public object Object
			{
				get
				{
					return this.o;
				}
			}

			private XmlEventCache.XmlEventType eventType;

			private string s1;

			private string s2;

			private string s3;

			private object o;
		}
	}
}
