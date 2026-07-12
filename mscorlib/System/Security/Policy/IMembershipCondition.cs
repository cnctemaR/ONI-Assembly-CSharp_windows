using System;

namespace System.Security.Policy
{
	public interface IMembershipCondition : ISecurityEncodable, ISecurityPolicyEncodable
	{
		bool Check(Evidence evidence);

		IMembershipCondition Copy();

		bool Equals(object obj);

		string ToString();
	}
}
