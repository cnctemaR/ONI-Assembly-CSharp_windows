using System;
using System.Security.Principal;
using System.Text;

namespace System.Net
{
	public sealed class HttpListenerContext
	{
		internal HttpListenerContext(HttpConnection cnc)
		{
			this.cnc = cnc;
			this.request = new HttpListenerRequest(this);
			this.response = new HttpListenerResponse(this);
		}

		internal int ErrorStatus
		{
			get
			{
				return this.err_status;
			}
			set
			{
				this.err_status = value;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		internal bool HaveError
		{
			get
			{
				return this.error != null;
			}
		}

		internal HttpConnection Connection
		{
			get
			{
				return this.cnc;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return this.request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return this.response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return this.user;
			}
		}

		internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
		{
			if (expectedSchemes == AuthenticationSchemes.Anonymous)
			{
				return;
			}
			string text = this.request.Headers["Authorization"];
			if (text == null || text.Length < 2)
			{
				return;
			}
			string[] array = text.Split(new char[] { ' ' }, 2);
			if (string.Compare(array[0], "basic", true) == 0)
			{
				this.user = this.ParseBasicAuthentication(array[1]);
			}
		}

		internal IPrincipal ParseBasicAuthentication(string authData)
		{
			IPrincipal principal;
			try
			{
				string text = Encoding.Default.GetString(Convert.FromBase64String(authData));
				int num = text.IndexOf(':');
				string text2 = text.Substring(num + 1);
				text = text.Substring(0, num);
				num = text.IndexOf('\\');
				string text3;
				if (num > 0)
				{
					text3 = text.Substring(num);
				}
				else
				{
					text3 = text;
				}
				HttpListenerBasicIdentity httpListenerBasicIdentity = new HttpListenerBasicIdentity(text3, text2);
				principal = new GenericPrincipal(httpListenerBasicIdentity, new string[0]);
			}
			catch (Exception)
			{
				principal = null;
			}
			return principal;
		}

		private HttpListenerRequest request;

		private HttpListenerResponse response;

		private IPrincipal user;

		private HttpConnection cnc;

		private string error;

		private int err_status = 400;

		internal HttpListener Listener;
	}
}
