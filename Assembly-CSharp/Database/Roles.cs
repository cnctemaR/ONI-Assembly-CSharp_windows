using System;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class Roles : ResourceSet<Role>
	{
		public Roles(ResourceSet parent)
			: base("Roles", parent)
		{
			this.Botanist = base.Add(new Role("Botanist", DUPLICANTS.ROLES.BOTANIST.NAME, DUPLICANTS.ROLES.BOTANIST.DESCRIPTION, "Botanist"));
			this.Botanist.AddEntitlement(new DecorEntitlement(2f));
		}

		public Role Botanist;
	}
}
