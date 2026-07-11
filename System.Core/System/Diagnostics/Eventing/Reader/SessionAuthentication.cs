using System;

namespace System.Diagnostics.Eventing.Reader
{
	public enum SessionAuthentication
	{
		Default,
		Kerberos = 2,
		Negotiate = 1,
		Ntlm = 3
	}
}
