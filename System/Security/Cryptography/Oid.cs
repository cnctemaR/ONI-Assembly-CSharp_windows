using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography
{
	public sealed class Oid
	{
		public Oid()
		{
		}

		public Oid(string oid)
			: this(oid, OidGroup.All, true)
		{
		}

		internal Oid(string oid, OidGroup group, bool lookupFriendlyName)
		{
			if (lookupFriendlyName)
			{
				string text = X509Utils.FindOidInfoWithFallback(2U, oid, group);
				if (text == null)
				{
					text = oid;
				}
				this.Value = text;
			}
			else
			{
				this.Value = oid;
			}
			this.m_group = group;
		}

		public Oid(string value, string friendlyName)
		{
			this.m_value = value;
			this.m_friendlyName = friendlyName;
		}

		public Oid(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this.m_value = oid.m_value;
			this.m_friendlyName = oid.m_friendlyName;
			this.m_group = oid.m_group;
		}

		private Oid(string value, string friendlyName, OidGroup group)
		{
			this.m_value = value;
			this.m_friendlyName = friendlyName;
			this.m_group = group;
		}

		public static Oid FromFriendlyName(string friendlyName, OidGroup group)
		{
			if (friendlyName == null)
			{
				throw new ArgumentNullException("friendlyName");
			}
			string text = X509Utils.FindOidInfo(2U, friendlyName, group);
			if (text == null)
			{
				throw new CryptographicException(global::SR.GetString("The OID value is invalid."));
			}
			return new Oid(text, friendlyName, group);
		}

		public static Oid FromOidValue(string oidValue, OidGroup group)
		{
			if (oidValue == null)
			{
				throw new ArgumentNullException("oidValue");
			}
			string text = X509Utils.FindOidInfo(1U, oidValue, group);
			if (text == null)
			{
				throw new CryptographicException(global::SR.GetString("The OID value is invalid."));
			}
			return new Oid(oidValue, text, group);
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				if (this.m_friendlyName == null && this.m_value != null)
				{
					this.m_friendlyName = X509Utils.FindOidInfoWithFallback(1U, this.m_value, this.m_group);
				}
				return this.m_friendlyName;
			}
			set
			{
				this.m_friendlyName = value;
				if (this.m_friendlyName != null)
				{
					string text = X509Utils.FindOidInfoWithFallback(2U, this.m_friendlyName, this.m_group);
					if (text != null)
					{
						this.m_value = text;
					}
				}
			}
		}

		private string m_value;

		private string m_friendlyName;

		private OidGroup m_group;
	}
}
