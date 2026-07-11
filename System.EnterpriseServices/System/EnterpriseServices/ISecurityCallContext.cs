using System;
using System.Collections;

namespace System.EnterpriseServices
{
	internal interface ISecurityCallContext
	{
		int Count { get; }

		void GetEnumerator(ref IEnumerator enumerator);

		object GetItem(string user);

		bool IsCallerInRole(string role);

		bool IsSecurityEnabled();

		bool IsUserInRole(ref object user, string role);
	}
}
