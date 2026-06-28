using System;
using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Diseases()
		{
			this.SawCorpsosis = base.Add(new SawCorpsosis());
			this.Dweebcephaly = base.Add(new Dweebcephaly());
			this.Lazibonitis = base.Add(new Lazibonitis());
			this.Diarrhea = base.Add(new Diarrhea());
			this.FoodPoisoning = base.Add(new FoodPoisoning());
			this.PutridOdour = base.Add(new PutridOdour());
			this.Spores = base.Add(new Spores());
			this.FierySkin = base.Add(new FierySkin());
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

		public Disease SawCorpsosis;

		public Disease Dweebcephaly;

		public Disease Lazibonitis;

		public Disease Diarrhea;

		public Disease FoodPoisoning;

		public Disease PutridOdour;

		public Disease Spores;

		public Disease FierySkin;
	}
}
