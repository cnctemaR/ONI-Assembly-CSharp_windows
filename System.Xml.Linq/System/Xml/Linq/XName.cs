using System;
using System.Runtime.Serialization;
using Unity;

namespace System.Xml.Linq
{
	[Serializable]
	public sealed class XName : IEquatable<XName>, ISerializable
	{
		internal XName(XNamespace ns, string localName)
		{
			this._ns = ns;
			this._localName = XmlConvert.VerifyNCName(localName);
			this._hashCode = ns.GetHashCode() ^ localName.GetHashCode();
		}

		public string LocalName
		{
			get
			{
				return this._localName;
			}
		}

		public XNamespace Namespace
		{
			get
			{
				return this._ns;
			}
		}

		public string NamespaceName
		{
			get
			{
				return this._ns.NamespaceName;
			}
		}

		public override string ToString()
		{
			if (this._ns.NamespaceName.Length == 0)
			{
				return this._localName;
			}
			return "{" + this._ns.NamespaceName + "}" + this._localName;
		}

		public static XName Get(string expandedName)
		{
			if (expandedName == null)
			{
				throw new ArgumentNullException("expandedName");
			}
			if (expandedName.Length == 0)
			{
				throw new ArgumentException(global::SR.Format("'{0}' is an invalid expanded name.", expandedName));
			}
			if (expandedName[0] != '{')
			{
				return XNamespace.None.GetName(expandedName);
			}
			int num = expandedName.LastIndexOf('}');
			if (num <= 1 || num == expandedName.Length - 1)
			{
				throw new ArgumentException(global::SR.Format("'{0}' is an invalid expanded name.", expandedName));
			}
			return XNamespace.Get(expandedName, 1, num - 1).GetName(expandedName, num + 1, expandedName.Length - num - 1);
		}

		public static XName Get(string localName, string namespaceName)
		{
			return XNamespace.Get(namespaceName).GetName(localName);
		}

		[CLSCompliant(false)]
		public static implicit operator XName(string expandedName)
		{
			if (expandedName == null)
			{
				return null;
			}
			return XName.Get(expandedName);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return this._hashCode;
		}

		public static bool operator ==(XName left, XName right)
		{
			return left == right;
		}

		public static bool operator !=(XName left, XName right)
		{
			return left != right;
		}

		bool IEquatable<XName>.Equals(XName other)
		{
			return this == other;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}

		internal XName()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private XNamespace _ns;

		private string _localName;

		private int _hashCode;
	}
}
