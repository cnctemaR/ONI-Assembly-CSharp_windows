using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Role : Resource
	{
		public Role(string id, string name, string desc, string skillId)
			: base(id, name)
		{
			this.Description = desc;
			this.SkillID = skillId;
			this.Entitlements = new List<Entitlement>();
		}

		public Role AddEntitlement(Entitlement entitlement)
		{
			this.Entitlements.Add(entitlement);
			return this;
		}

		public void ApplyEntitlements(GameObject minion)
		{
			for (int i = 0; i < this.Entitlements.Count; i++)
			{
				this.Entitlements[i].Apply(minion);
			}
		}

		public void UnapplyEntitlements(GameObject minion)
		{
			for (int i = 0; i < this.Entitlements.Count; i++)
			{
				this.Entitlements[i].Unapply(minion);
			}
		}

		public string Description;

		public string SkillID;

		public List<Entitlement> Entitlements;
	}
}
