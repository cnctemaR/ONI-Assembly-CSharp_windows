using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilName : QilLiteral
	{
		public QilName(QilNodeType nodeType, string local, string uri, string prefix)
			: base(nodeType, null)
		{
			this.LocalName = local;
			this.NamespaceUri = uri;
			this.Prefix = prefix;
			base.Value = this;
		}

		public string LocalName
		{
			get
			{
				return this.local;
			}
			set
			{
				this.local = value;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				this.prefix = value;
			}
		}

		public string QualifiedName
		{
			get
			{
				if (this.prefix.Length == 0)
				{
					return this.local;
				}
				return this.prefix + ":" + this.local;
			}
		}

		public override int GetHashCode()
		{
			return this.local.GetHashCode();
		}

		public override bool Equals(object other)
		{
			QilName qilName = other as QilName;
			return !(qilName == null) && this.local == qilName.local && this.uri == qilName.uri;
		}

		public static bool operator ==(QilName a, QilName b)
		{
			return a == b || (a != null && b != null && a.local == b.local && a.uri == b.uri);
		}

		public static bool operator !=(QilName a, QilName b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			if (this.prefix.Length != 0)
			{
				return string.Concat(new string[] { "{", this.uri, "}", this.prefix, ":", this.local });
			}
			if (this.uri.Length == 0)
			{
				return this.local;
			}
			return "{" + this.uri + "}" + this.local;
		}

		private string local;

		private string uri;

		private string prefix;
	}
}
