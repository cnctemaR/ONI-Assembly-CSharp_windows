using System;

namespace Database
{
	public class Faces : ResourceSet<Face>
	{
		public Faces()
		{
			this.Neutral = base.Add(new Face("Neutral"));
			this.Happy = base.Add(new Face("Happy"));
			this.Uncomfortable = base.Add(new Face("Uncomfortable"));
			this.Cold = base.Add(new Face("Cold"));
			this.Hot = base.Add(new Face("Hot"));
			this.Tired = base.Add(new Face("Tired"));
			this.Sleep = base.Add(new Face("Sleep"));
			this.Hungry = base.Add(new Face("Hungry"));
			this.Angry = base.Add(new Face("Angry"));
			this.Suffocate = base.Add(new Face("Suffocate"));
			this.Sick = base.Add(new Face("Sick"));
			this.SickSpores = base.Add(new Face("Spores"));
			this.SickFierySkin = base.Add(new Face("Fiery"));
			this.Dead = base.Add(new Face("Death"));
		}

		public Face Neutral;

		public Face Happy;

		public Face Uncomfortable;

		public Face Cold;

		public Face Hot;

		public Face Tired;

		public Face Sleep;

		public Face Hungry;

		public Face Angry;

		public Face Suffocate;

		public Face Dead;

		public Face Sick;

		public Face SickSpores;

		public Face SickFierySkin;
	}
}
