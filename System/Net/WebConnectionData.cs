using System;
using System.IO;

namespace System.Net
{
	internal class WebConnectionData
	{
		public WebConnectionData()
		{
			this._readState = ReadState.None;
		}

		public WebConnectionData(HttpWebRequest request)
		{
			this._request = request;
		}

		public HttpWebRequest request
		{
			get
			{
				return this._request;
			}
			set
			{
				this._request = value;
			}
		}

		public ReadState ReadState
		{
			get
			{
				return this._readState;
			}
			set
			{
				lock (this)
				{
					if (this._readState == ReadState.Aborted && value != ReadState.Aborted)
					{
						throw new WebException("Aborted", WebExceptionStatus.RequestCanceled);
					}
					this._readState = value;
				}
			}
		}

		private HttpWebRequest _request;

		public int StatusCode;

		public string StatusDescription;

		public WebHeaderCollection Headers;

		public Version Version;

		public Version ProxyVersion;

		public Stream stream;

		public string[] Challenge;

		private ReadState _readState;
	}
}
