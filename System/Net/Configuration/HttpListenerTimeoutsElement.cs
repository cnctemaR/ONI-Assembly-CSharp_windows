using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	public sealed class HttpListenerTimeoutsElement : ConfigurationElement
	{
		public HttpListenerTimeoutsElement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public TimeSpan DrainEntityBody
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
		}

		public TimeSpan EntityBody
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
		}

		public TimeSpan HeaderWait
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
		}

		public TimeSpan IdleConnection
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
		}

		public long MinSendBytesPerSecond
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		public TimeSpan RequestQueue
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
		}
	}
}
