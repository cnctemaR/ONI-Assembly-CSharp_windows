using System;
using Klei.AI;

namespace Database
{
	public class Sicknesses : ResourceSet<Sickness>
	{
		public Sicknesses(ResourceSet parent)
			: base("Sicknesses", parent)
		{
			this.FoodSickness = base.Add(new FoodSickness());
			this.SlimeSickness = base.Add(new SlimeSickness());
			this.ZombieSickness = base.Add(new ZombieSickness());
			this.Allergies = base.Add(new Allergies());
			this.ColdBrain = base.Add(new ColdBrain());
			this.HeatRash = base.Add(new HeatRash());
			this.Sunburn = base.Add(new Sunburn());
		}

		public static bool IsValidID(string id)
		{
			bool flag = false;
			foreach (Sickness sickness in Db.Get().Sicknesses.resources)
			{
				if (sickness.Id == id)
				{
					flag = true;
				}
			}
			return flag;
		}

		public Sickness FoodSickness;

		public Sickness SlimeSickness;

		public Sickness ZombieSickness;

		public Sickness Allergies;

		public Sickness ColdBrain;

		public Sickness HeatRash;

		public Sickness Sunburn;
	}
}
