using System;

namespace System.Net
{
	public class Authorization
	{
		public Authorization(string token)
			: this(token, true)
		{
		}

		public Authorization(string token, bool complete)
			: this(token, complete, null)
		{
		}

		public Authorization(string token, bool complete, string connectionGroupId)
		{
			this.token = token;
			this.complete = complete;
			this.connectionGroupId = connectionGroupId;
		}

		public string Message
		{
			get
			{
				return this.token;
			}
		}

		public bool Complete
		{
			get
			{
				return this.complete;
			}
		}

		public string ConnectionGroupId
		{
			get
			{
				return this.connectionGroupId;
			}
		}

		public string[] ProtectionRealm
		{
			get
			{
				return this.protectionRealm;
			}
			set
			{
				this.protectionRealm = value;
			}
		}

		internal IAuthenticationModule Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public bool MutuallyAuthenticated
		{
			get
			{
				throw Authorization.GetMustImplement();
			}
			set
			{
				throw Authorization.GetMustImplement();
			}
		}

		private string token;

		private bool complete;

		private string connectionGroupId;

		private string[] protectionRealm;

		private IAuthenticationModule module;
	}
}
