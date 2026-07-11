using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class AllMembershipCondition : ISecurityEncodable, ISecurityPolicyEncodable, IConstantMembershipCondition, IMembershipCondition
	{
		public bool Check(Evidence evidence)
		{
			return true;
		}

		public IMembershipCondition Copy()
		{
			return new AllMembershipCondition();
		}

		public override bool Equals(object o)
		{
			return o is AllMembershipCondition;
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
			return typeof(AllMembershipCondition).GetHashCode();
		}

		public override string ToString()
		{
			return "All code";
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			return MembershipConditionHelper.Element(typeof(AllMembershipCondition), this.version);
		}

		private readonly int version = 1;
	}
}
