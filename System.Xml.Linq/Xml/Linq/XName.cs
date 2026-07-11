using System;
using System.Runtime.Serialization;

namespace System.Xml.Linq
{
	[Serializable]
	public sealed class XName : ISerializable, IEquatable<XName>
	{
		private XName(SerializationInfo info, StreamingContext context)
		{
			string @string = info.GetString("name");
			string text;
			string text2;
			XName.ExpandName(@string, out text, out text2);
			this.local = text;
			this.ns = XNamespace.Get(text2);
		}

		internal XName(string local, XNamespace ns)
		{
			this.local = XmlConvert.VerifyNCName(local);
			this.ns = ns;
		}

		bool IEquatable<XName>.Equals(XName other)
		{
			return this == other;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("name", this.ToString());
		}

		private static Exception ErrorInvalidExpandedName()
		{
			return new ArgumentException("Invalid expanded name.");
		}

		public string LocalName
		{
			get
			{
				return this.local;
			}
		}

		public XNamespace Namespace
		{
			get
			{
				return this.ns;
			}
		}

		public string NamespaceName
		{
			get
			{
				return this.ns.NamespaceName;
			}
		}

		public override bool Equals(object obj)
		{
			XName xname = obj as XName;
			return xname != null && this == xname;
		}

		public static XName Get(string expandedName)
		{
			string text;
			string text2;
			XName.ExpandName(expandedName, out text, out text2);
			return XName.Get(text, text2);
		}

		private static void ExpandName(string expandedName, out string local, out string ns)
		{
			if (expandedName == null)
			{
				throw new ArgumentNullException("expandedName");
			}
			ns = null;
			local = null;
			if (expandedName.Length == 0)
			{
				throw XName.ErrorInvalidExpandedName();
			}
			if (expandedName[0] == '{')
			{
				for (int i = 1; i < expandedName.Length; i++)
				{
					if (expandedName[i] == '}')
					{
						ns = expandedName.Substring(1, i - 1);
					}
				}
				if (string.IsNullOrEmpty(ns))
				{
					throw XName.ErrorInvalidExpandedName();
				}
				if (expandedName.Length == ns.Length + 2)
				{
					throw XName.ErrorInvalidExpandedName();
				}
				local = expandedName.Substring(ns.Length + 2);
			}
			else
			{
				local = expandedName;
				ns = string.Empty;
			}
		}

		public static XName Get(string localName, string namespaceName)
		{
			return XNamespace.Get(namespaceName).GetName(localName);
		}

		public override int GetHashCode()
		{
			return this.local.GetHashCode() ^ this.ns.GetHashCode();
		}

		public override string ToString()
		{
			if (this.ns == XNamespace.None)
			{
				return this.local;
			}
			return "{" + this.ns.NamespaceName + "}" + this.local;
		}

		public static bool operator ==(XName n1, XName n2)
		{
			if (n1 == null)
			{
				return n2 == null;
			}
			return n2 != null && (object.ReferenceEquals(n1, n2) || (n1.local == n2.local && n1.ns == n2.ns));
		}

		public static implicit operator XName(string s)
		{
			return (s != null) ? XName.Get(s) : null;
		}

		public static bool operator !=(XName n1, XName n2)
		{
			return !(n1 == n2);
		}

		private string local;

		private XNamespace ns;
	}
}
