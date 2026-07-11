using System;
using System.Collections;

namespace System.Security.AccessControl
{
	public sealed class AuthorizationRuleCollection : ReadOnlyCollectionBase
	{
		public AuthorizationRuleCollection()
		{
		}

		internal AuthorizationRuleCollection(AuthorizationRule[] rules)
		{
			base.InnerList.AddRange(rules);
		}

		public void AddRule(AuthorizationRule rule)
		{
			base.InnerList.Add(rule);
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
