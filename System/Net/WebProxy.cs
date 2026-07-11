using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace System.Net
{
	[Serializable]
	public class WebProxy : ISerializable, IWebProxy
	{
		public WebProxy()
			: this(null, false, null, null)
		{
		}

		public WebProxy(string address)
			: this(WebProxy.ToUri(address), false, null, null)
		{
		}

		public WebProxy(global::System.Uri address)
			: this(address, false, null, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal)
			: this(WebProxy.ToUri(address), bypassOnLocal, null, null)
		{
		}

		public WebProxy(string host, int port)
			: this(new global::System.Uri(string.Concat(new object[] { "http://", host, ":", port })))
		{
		}

		public WebProxy(global::System.Uri address, bool bypassOnLocal)
			: this(address, bypassOnLocal, null, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal, string[] bypassList)
			: this(WebProxy.ToUri(address), bypassOnLocal, bypassList, null)
		{
		}

		public WebProxy(global::System.Uri address, bool bypassOnLocal, string[] bypassList)
			: this(address, bypassOnLocal, bypassList, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal, string[] bypassList, ICredentials credentials)
			: this(WebProxy.ToUri(address), bypassOnLocal, bypassList, credentials)
		{
		}

		public WebProxy(global::System.Uri address, bool bypassOnLocal, string[] bypassList, ICredentials credentials)
		{
			this.address = address;
			this.bypassOnLocal = bypassOnLocal;
			if (bypassList != null)
			{
				this.bypassList = new ArrayList(bypassList);
			}
			this.credentials = credentials;
			this.CheckBypassList();
		}

		protected WebProxy(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.address = (global::System.Uri)serializationInfo.GetValue("_ProxyAddress", typeof(global::System.Uri));
			this.bypassOnLocal = serializationInfo.GetBoolean("_BypassOnLocal");
			this.bypassList = (ArrayList)serializationInfo.GetValue("_BypassList", typeof(ArrayList));
			this.useDefaultCredentials = serializationInfo.GetBoolean("_UseDefaultCredentials");
			this.credentials = null;
			this.CheckBypassList();
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		public global::System.Uri Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public ArrayList BypassArrayList
		{
			get
			{
				if (this.bypassList == null)
				{
					this.bypassList = new ArrayList();
				}
				return this.bypassList;
			}
		}

		public string[] BypassList
		{
			get
			{
				return (string[])this.BypassArrayList.ToArray(typeof(string));
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.bypassList = new ArrayList(value);
				this.CheckBypassList();
			}
		}

		public bool BypassProxyOnLocal
		{
			get
			{
				return this.bypassOnLocal;
			}
			set
			{
				this.bypassOnLocal = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.credentials = value;
			}
		}

		[global::System.MonoTODO("Does not affect Credentials, since CredentialCache.DefaultCredentials is not implemented.")]
		public bool UseDefaultCredentials
		{
			get
			{
				return this.useDefaultCredentials;
			}
			set
			{
				this.useDefaultCredentials = value;
			}
		}

		[Obsolete("This method has been deprecated", false)]
		[global::System.MonoTODO("Can we get this info under windows from the system?")]
		public static WebProxy GetDefaultProxy()
		{
			IWebProxy select = GlobalProxySelection.Select;
			if (select is WebProxy)
			{
				return (WebProxy)select;
			}
			return new WebProxy();
		}

		public global::System.Uri GetProxy(global::System.Uri destination)
		{
			if (this.IsBypassed(destination))
			{
				return destination;
			}
			return this.address;
		}

		public bool IsBypassed(global::System.Uri host)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (host.IsLoopback && this.bypassOnLocal)
			{
				return true;
			}
			if (this.address == null)
			{
				return true;
			}
			string host2 = host.Host;
			if (this.bypassOnLocal && host2.IndexOf('.') == -1)
			{
				return true;
			}
			if (!this.bypassOnLocal)
			{
				if (string.Compare(host2, "localhost", true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				if (string.Compare(host2, "loopback", true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				IPAddress ipaddress = null;
				if (IPAddress.TryParse(host2, out ipaddress) && IPAddress.IsLoopback(ipaddress))
				{
					return true;
				}
			}
			if (this.bypassList == null || this.bypassList.Count == 0)
			{
				return false;
			}
			bool flag;
			try
			{
				string text = host.Scheme + "://" + host.Authority;
				int i;
				for (i = 0; i < this.bypassList.Count; i++)
				{
					global::System.Text.RegularExpressions.Regex regex = new global::System.Text.RegularExpressions.Regex((string)this.bypassList[i], global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | global::System.Text.RegularExpressions.RegexOptions.Singleline);
					if (regex.IsMatch(text))
					{
						break;
					}
				}
				if (i == this.bypassList.Count)
				{
					flag = false;
				}
				else
				{
					while (i < this.bypassList.Count)
					{
						new global::System.Text.RegularExpressions.Regex((string)this.bypassList[i]);
						i++;
					}
					flag = true;
				}
			}
			catch (ArgumentException)
			{
				flag = false;
			}
			return flag;
		}

		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("_BypassOnLocal", this.bypassOnLocal);
			serializationInfo.AddValue("_ProxyAddress", this.address);
			serializationInfo.AddValue("_BypassList", this.bypassList);
			serializationInfo.AddValue("_UseDefaultCredentials", this.UseDefaultCredentials);
		}

		private void CheckBypassList()
		{
			if (this.bypassList == null)
			{
				return;
			}
			for (int i = 0; i < this.bypassList.Count; i++)
			{
				new global::System.Text.RegularExpressions.Regex((string)this.bypassList[i]);
			}
		}

		private static global::System.Uri ToUri(string address)
		{
			if (address == null)
			{
				return null;
			}
			if (address.IndexOf("://") == -1)
			{
				address = "http://" + address;
			}
			return new global::System.Uri(address);
		}

		private global::System.Uri address;

		private bool bypassOnLocal;

		private ArrayList bypassList;

		private ICredentials credentials;

		private bool useDefaultCredentials;
	}
}
