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
				return this._local;
			}
			set
			{
				this._local = value;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this._uri;
			}
			set
			{
				this._uri = value;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
			set
			{
				this._prefix = value;
			}
		}

		public string QualifiedName
		{
			get
			{
				if (this._prefix.Length == 0)
				{
					return this._local;
				}
				return this._prefix + ":" + this._local;
			}
		}

		public override int GetHashCode()
		{
			return this._local.GetHashCode();
		}

		public override bool Equals(object other)
		{
			QilName qilName = other as QilName;
			return !(qilName == null) && this._local == qilName._local && this._uri == qilName._uri;
		}

		public static bool operator ==(QilName a, QilName b)
		{
			return a == b || (a != null && b != null && a._local == b._local && a._uri == b._uri);
		}

		public static bool operator !=(QilName a, QilName b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			if (this._prefix.Length != 0)
			{
				return string.Concat(new string[] { "{", this._uri, "}", this._prefix, ":", this._local });
			}
			if (this._uri.Length == 0)
			{
				return this._local;
			}
			return "{" + this._uri + "}" + this._local;
		}

		private string _local;

		private string _uri;

		private string _prefix;
	}
}
