using System;
using System.Collections;

namespace System.EnterpriseServices
{
	internal interface ISecurityIdentityColl
	{
		int Count { get; }

		void GetEnumerator(out IEnumerator enumerator);

		SecurityIdentity GetItem(int idx);
	}
}
