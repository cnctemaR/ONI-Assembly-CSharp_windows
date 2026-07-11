using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Xml.Linq
{
	[KnownType(typeof(NameSerializer))]
	[Serializable]
	public sealed class XName : IEquatable<XName>, ISerializable
	{
		internal XName(XNamespace ns, string localName)
		{
			this.ns = ns;
			this.localName = XmlConvert.VerifyNCName(localName);
			this.hashCode = ns.GetHashCode() ^ localName.GetHashCode();
		}

		public string LocalName
		{
			get
			{
				return this.localName;
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

		public override string ToString()
		{
			if (this.ns.NamespaceName.Length == 0)
			{
				return this.localName;
			}
			return "{" + this.ns.NamespaceName + "}" + this.localName;
		}

		public static XName Get(string expandedName)
		{
			if (expandedName == null)
			{
				throw new ArgumentNullException("expandedName");
			}
			if (expandedName.Length == 0)
			{
				throw new ArgumentException(Res.GetString("Argument_InvalidExpandedName", new object[] { expandedName }));
			}
			if (expandedName[0] != '{')
			{
				return XNamespace.None.GetName(expandedName);
			}
			int num = expandedName.LastIndexOf('}');
			if (num <= 1 || num == expandedName.Length - 1)
			{
				throw new ArgumentException(Res.GetString("Argument_InvalidExpandedName", new object[] { expandedName }));
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
			return this.hashCode;
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

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("name", this.ToString());
			info.SetType(typeof(NameSerializer));
		}

		internal XName()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private XNamespace ns;

		private string localName;

		private int hashCode;
	}
}
