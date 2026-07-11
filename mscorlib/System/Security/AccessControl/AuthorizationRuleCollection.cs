using System;
using System.Collections;

namespace System.Security.AccessControl
{
	public sealed class AuthorizationRuleCollection : ReadOnlyCollectionBase
	{
		private AuthorizationRuleCollection(AuthorizationRule[] rules)
		{
			base.InnerList.AddRange(rules);
		}

		public AuthorizationRule this[int index]
		{
			get
			{
				return (AuthorizationRule)base.InnerList[index];
			}
		}

		public void CopyTo(AuthorizationRule[] rules, int index)
		{
			base.InnerList.CopyTo(rules, index);
		}
	}
}
