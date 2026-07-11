using System;
using System.Collections.Generic;
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
			using (List<Sickness>.Enumerator enumerator = Db.Get().Sicknesses.resources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == id)
					{
						flag = true;
					}
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
