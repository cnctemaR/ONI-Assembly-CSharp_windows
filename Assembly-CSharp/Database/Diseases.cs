using System;
using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Diseases(ResourceSet parent)
			: base("Diseases", parent)
		{
			this.FoodGerms = base.Add(new FoodGerms());
			this.SlimeGerms = base.Add(new SlimeGerms());
			this.PollenGerms = base.Add(new PollenGerms());
			this.ZombieSpores = base.Add(new ZombieSpores());
		}

		public static bool IsValidID(string id)
		{
			bool flag = false;
			using (List<Disease>.Enumerator enumerator = Db.Get().Diseases.resources.GetEnumerator())
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
			Diseases diseases = Db.Get().Diseases;
			byte b = 0;
			while ((int)b < diseases.Count)
			{
				Disease disease = diseases[(int)b];
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
	}
}
