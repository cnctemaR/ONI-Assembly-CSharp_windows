using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[Guid("6619a740-8154-43be-a186-0319578e02db")]
	public interface IRemoteDispatch
	{
		[AutoComplete]
		string RemoteDispatchAutoDone(string s);

		[AutoComplete(false)]
		string RemoteDispatchNotAutoDone(string s);
	}
}
