using System;
using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Diseases(ResourceSet parent, bool statsOnly = false)
			: base("Diseases", parent)
		{
			this.FoodGerms = base.Add(new FoodGerms(statsOnly));
			this.SlimeGerms = base.Add(new SlimeGerms(statsOnly));
			this.PollenGerms = base.Add(new PollenGerms(statsOnly));
			this.ZombieSpores = base.Add(new ZombieSpores(statsOnly));
			if (DlcManager.FeatureRadiationEnabled())
			{
				this.RadiationPoisoning = base.Add(new RadiationPoisoning(statsOnly));
			}
		}

		public bool IsValidID(string id)
		{
			bool flag = false;
			using (List<Disease>.Enumerator enumerator = this.resources.GetEnumerator())
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

		public byte GetIndex(int hash)
		{
			byte b = 0;
			while ((int)b < this.resources.Count)
			{
				Disease disease = this.resources[(int)b];
				if (hash == disease.id.GetHashCode())
				{
					return b;
				}
				b += 1;
			}
			return byte.MaxValue;
		}

		public byte GetIndex(HashedString id)
		{
			return this.GetIndex(id.GetHashCode());
		}

		public Disease FoodGerms;

		public Disease SlimeGerms;

		public Disease PollenGerms;

		public Disease ZombieSpores;

		public Disease RadiationPoisoning;
	}
}
