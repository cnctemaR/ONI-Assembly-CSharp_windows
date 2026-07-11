using System;
using System.Collections;

namespace System.EnterpriseServices
{
	internal interface ISecurityCallersColl
	{
		int Count { get; }

		void GetEnumerator(out IEnumerator enumerator);

		ISecurityIdentityColl GetItem(int idx);
	}
}
