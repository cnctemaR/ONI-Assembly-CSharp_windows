using System;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SafeFreeNegoCredentials : SafeFreeCredentials
	{
		public SafeGssCredHandle GssCredential
		{
			get
			{
				return this._credential;
			}
		}

		public bool IsNtlmOnly
		{
			get
			{
				return this._isNtlmOnly;
			}
		}

		public string UserName
		{
			get
			{
				return this._userName;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
		}

		public SafeFreeNegoCredentials(bool isNtlmOnly, string username, string password, string domain)
			: base(IntPtr.Zero, true)
		{
			int num = username.IndexOf('\\');
			if (num > 0 && username.IndexOf('\\', num + 1) < 0 && string.IsNullOrEmpty(domain))
			{
				domain = username.Substring(0, num);
				username = username.Substring(num + 1);
			}
			if (domain != null)
			{
				domain = domain.Trim();
			}
			username = username.Trim();
			if (username.IndexOf('@') < 0 && !string.IsNullOrEmpty(domain))
			{
				username = username + "@" + domain;
			}
			bool flag = false;
			this._isNtlmOnly = isNtlmOnly;
			this._userName = username;
			this._isDefault = string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password);
			this._credential = SafeGssCredHandle.Create(username, password, isNtlmOnly);
			this._credential.DangerousAddRef(ref flag);
		}

		public override bool IsInvalid
		{
			get
			{
				return this._credential == null;
			}
		}

		protected override bool ReleaseHandle()
		{
			this._credential.DangerousRelease();
			this._credential = null;
			return true;
		}

		private SafeGssCredHandle _credential;

		private readonly bool _isNtlmOnly;

		private readonly string _userName;

		private readonly bool _isDefault;
	}
}
