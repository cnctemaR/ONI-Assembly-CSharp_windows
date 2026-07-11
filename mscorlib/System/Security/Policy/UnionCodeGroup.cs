using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class UnionCodeGroup : CodeGroup
	{
		public UnionCodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy)
			: base(membershipCondition, policy)
		{
		}

		internal UnionCodeGroup(SecurityElement e, PolicyLevel level)
			: base(e, level)
		{
		}

		public override CodeGroup Copy()
		{
			return this.Copy(true);
		}

		internal CodeGroup Copy(bool childs)
		{
			UnionCodeGroup unionCodeGroup = new UnionCodeGroup(base.MembershipCondition, base.PolicyStatement);
			unionCodeGroup.Name = base.Name;
			unionCodeGroup.Description = base.Description;
			if (childs)
			{
				foreach (object obj in base.Children)
				{
					CodeGroup codeGroup = (CodeGroup)obj;
					unionCodeGroup.AddChild(codeGroup.Copy());
				}
			}
			return unionCodeGroup;
		}

		public override PolicyStatement Resolve(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			if (!base.MembershipCondition.Check(evidence))
			{
				return null;
			}
			PermissionSet permissionSet = base.PolicyStatement.PermissionSet.Copy();
			if (base.Children.Count > 0)
			{
				foreach (object obj in base.Children)
				{
					PolicyStatement policyStatement = ((CodeGroup)obj).Resolve(evidence);
					if (policyStatement != null)
					{
						permissionSet = permissionSet.Union(policyStatement.PermissionSet);
					}
				}
			}
			PolicyStatement policyStatement2 = base.PolicyStatement.Copy();
			policyStatement2.PermissionSet = permissionSet;
			return policyStatement2;
		}

		public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			if (!base.MembershipCondition.Check(evidence))
			{
				return null;
			}
			CodeGroup codeGroup = this.Copy(false);
			if (base.Children.Count > 0)
			{
				foreach (object obj in base.Children)
				{
					CodeGroup codeGroup2 = ((CodeGroup)obj).ResolveMatchingCodeGroups(evidence);
					if (codeGroup2 != null)
					{
						codeGroup.AddChild(codeGroup2);
					}
				}
			}
			return codeGroup;
		}

		public override string MergeLogic
		{
			get
			{
				return "Union";
			}
		}
	}
}
