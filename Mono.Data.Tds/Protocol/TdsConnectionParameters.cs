using System;
using System.Net;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionParameters
	{
		public TdsConnectionParameters()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.ApplicationName = "Mono";
			this.Database = string.Empty;
			this.Charset = string.Empty;
			this.Hostname = Dns.GetHostName();
			this.Language = string.Empty;
			this.LibraryName = "Mono";
			this.Password = string.Empty;
			this.ProgName = "Mono";
			this.User = string.Empty;
			this.DomainLogin = false;
			this.DefaultDomain = string.Empty;
			this.AttachDBFileName = string.Empty;
		}

		public string ApplicationName;

		public string Database;

		public string Charset;

		public string Hostname;

		public string Language;

		public string LibraryName;

		public string Password;

		public string ProgName;

		public string User;

		public bool DomainLogin;

		public string DefaultDomain;

		public string AttachDBFileName;
	}
}
