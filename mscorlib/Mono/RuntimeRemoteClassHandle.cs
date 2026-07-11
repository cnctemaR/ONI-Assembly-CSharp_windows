using System;

namespace Mono
{
	internal struct RuntimeRemoteClassHandle
	{
		internal unsafe RuntimeRemoteClassHandle(RuntimeStructs.RemoteClass* value)
		{
			this.value = value;
		}

		internal unsafe RuntimeClassHandle ProxyClass
		{
			get
			{
				return new RuntimeClassHandle(this.value->proxy_class);
			}
		}

		private unsafe RuntimeStructs.RemoteClass* value;
	}
}
