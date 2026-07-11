using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class GacMembershipCondition : ISecurityEncodable, ISecurityPolicyEncodable, IConstantMembershipCondition, IMembershipCondition
	{
		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is GacInstalled)
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new GacMembershipCondition();
		}

		public override bool Equals(object o)
		{
			return o != null && o is GacMembershipCondition;
		}

		public void FromXml(SecurityElement e)
		{
			this.FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", this.version, this.version);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "GAC";
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			return MembershipConditionHelper.Element(typeof(GacMembershipCondition), this.version);
		}

		private readonly int version = 1;
	}
}
