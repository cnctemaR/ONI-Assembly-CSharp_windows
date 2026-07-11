using System;
using System.Collections;

namespace System.EnterpriseServices
{
	public sealed class SecurityCallers : IEnumerable
	{
		internal SecurityCallers()
		{
		}

		internal SecurityCallers(ISecurityCallersColl collection)
		{
		}

		public int Count
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public SecurityIdentity this[int idx]
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
