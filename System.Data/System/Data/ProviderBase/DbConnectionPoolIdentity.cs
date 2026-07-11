using System;

namespace System.Data.ProviderBase
{
	[Serializable]
	internal sealed class DbConnectionPoolIdentity
	{
		internal static DbConnectionPoolIdentity GetCurrent()
		{
			return DbConnectionPoolIdentity.GetCurrentManaged();
		}

		private DbConnectionPoolIdentity(string sidString, bool isRestricted, bool isNetwork)
		{
			this._sidString = sidString;
			this._isRestricted = isRestricted;
			this._isNetwork = isNetwork;
			this._hashCode = ((sidString == null) ? 0 : sidString.GetHashCode());
		}

		internal bool IsRestricted
		{
			get
			{
				return this._isRestricted;
			}
		}

		public override bool Equals(object value)
		{
			bool flag = this == DbConnectionPoolIdentity.NoIdentity || this == value;
			if (!flag && value != null)
			{
				DbConnectionPoolIdentity dbConnectionPoolIdentity = (DbConnectionPoolIdentity)value;
				flag = this._sidString == dbConnectionPoolIdentity._sidString && this._isRestricted == dbConnectionPoolIdentity._isRestricted && this._isNetwork == dbConnectionPoolIdentity._isNetwork;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this._hashCode;
		}

		internal static DbConnectionPoolIdentity GetCurrentManaged()
		{
			string text = ((!string.IsNullOrWhiteSpace(Environment.UserDomainName)) ? (Environment.UserDomainName + "\\") : "") + Environment.UserName;
			bool flag = false;
			bool flag2 = false;
			return new DbConnectionPoolIdentity(text, flag2, flag);
		}

		public static readonly DbConnectionPoolIdentity NoIdentity = new DbConnectionPoolIdentity(string.Empty, false, true);

		private readonly string _sidString;

		private readonly bool _isRestricted;

		private readonly bool _isNetwork;

		private readonly int _hashCode;
	}
}
