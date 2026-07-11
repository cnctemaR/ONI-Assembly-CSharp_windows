using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;

namespace System.Net.WebSockets
{
	public class HttpListenerWebSocketContext : WebSocketContext
	{
		internal HttpListenerWebSocketContext(Uri requestUri, NameValueCollection headers, CookieCollection cookieCollection, IPrincipal user, bool isAuthenticated, bool isLocal, bool isSecureConnection, string origin, IEnumerable<string> secWebSocketProtocols, string secWebSocketVersion, string secWebSocketKey, WebSocket webSocket)
		{
			this._cookieCollection = new CookieCollection();
			this._cookieCollection.Add(cookieCollection);
			this._headers = new NameValueCollection(headers);
			this._user = HttpListenerWebSocketContext.CopyPrincipal(user);
			this._requestUri = requestUri;
			this._isAuthenticated = isAuthenticated;
			this._isLocal = isLocal;
			this._isSecureConnection = isSecureConnection;
			this._origin = origin;
			this._secWebSocketProtocols = secWebSocketProtocols;
			this._secWebSocketVersion = secWebSocketVersion;
			this._secWebSocketKey = secWebSocketKey;
			this._webSocket = webSocket;
		}

		public override Uri RequestUri
		{
			get
			{
				return this._requestUri;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public override string Origin
		{
			get
			{
				return this._origin;
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				return this._secWebSocketProtocols;
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return this._secWebSocketVersion;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return this._secWebSocketKey;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				return this._cookieCollection;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._user;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._isAuthenticated;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return this._isLocal;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return this._isSecureConnection;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return this._webSocket;
			}
		}

		private static IPrincipal CopyPrincipal(IPrincipal user)
		{
			if (user != null)
			{
				if (user is WindowsPrincipal)
				{
					throw new PlatformNotSupportedException();
				}
				HttpListenerBasicIdentity httpListenerBasicIdentity;
				if ((httpListenerBasicIdentity = user.Identity as HttpListenerBasicIdentity) != null)
				{
					return new GenericPrincipal(new HttpListenerBasicIdentity(httpListenerBasicIdentity.Name, httpListenerBasicIdentity.Password), null);
				}
			}
			return null;
		}

		private readonly Uri _requestUri;

		private readonly NameValueCollection _headers;

		private readonly CookieCollection _cookieCollection;

		private readonly IPrincipal _user;

		private readonly bool _isAuthenticated;

		private readonly bool _isLocal;

		private readonly bool _isSecureConnection;

		private readonly string _origin;

		private readonly IEnumerable<string> _secWebSocketProtocols;

		private readonly string _secWebSocketVersion;

		private readonly string _secWebSocketKey;

		private readonly WebSocket _webSocket;
	}
}
