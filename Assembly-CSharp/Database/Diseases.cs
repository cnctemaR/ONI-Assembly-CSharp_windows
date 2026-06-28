using System;
using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Diseases(ResourceSet parent)
			: base("Diseases", parent)
		{
			this.FoodPoisoning = base.Add(new FoodPoisoning());
			this.PutridOdour = base.Add(new PutridOdour());
			this.Spores = base.Add(new Spores());
			this.ColdBrain = base.Add(new ColdBrain());
			this.HeatRash = base.Add(new HeatRash());
			this.SlimeLung = base.Add(new SlimeLung());
		}

		public static bool IsValidDiseaseID(string id)
		{
			bool flag = false;
			foreach (Disease disease in Db.Get().Diseases)
			{
				if (disease.Id == id)
				{
					flag = true;
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

		public Disease Dweebcephaly;

		public Disease Lazibonitis;

		public Disease FoodPoisoning;

		public Disease PutridOdour;

		public Disease Spores;

		public Disease ColdBrain;

		public Disease HeatRash;

		public Disease SlimeLung;
	}
}
