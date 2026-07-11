using System;

namespace Database
{
	public class Expressions : ResourceSet<Expression>
	{
		public Expressions(ResourceSet parent)
			: base("Expressions", parent)
		{
			Faces faces = Db.Get().Faces;
			this.Angry = new Expression("Angry", this, faces.Angry);
			this.Suffocate = new Expression("Suffocate", this, faces.Suffocate);
			this.RecoverBreath = new Expression("RecoverBreath", this, faces.Uncomfortable);
			this.RedAlert = new Expression("RedAlert", this, faces.Hot);
			this.Hungry = new Expression("Hungry", this, faces.Hungry);
			this.SickSpores = new Expression("SickSpores", this, faces.SickSpores);
			this.Zombie = new Expression("Zombie", this, faces.Zombie);
			this.SickFierySkin = new Expression("SickFierySkin", this, faces.SickFierySkin);
			this.SickCold = new Expression("SickCold", this, faces.SickCold);
			this.Sick = new Expression("Sick", this, faces.Sick);
			this.Cold = new Expression("Cold", this, faces.Cold);
			this.Hot = new Expression("Hot", this, faces.Hot);
			this.FullBladder = new Expression("FullBladder", this, faces.Uncomfortable);
			this.Tired = new Expression("Tired", this, faces.Tired);
			this.Unhappy = new Expression("Unhappy", this, faces.Uncomfortable);
			this.Uncomfortable = new Expression("Uncomfortable", this, faces.Uncomfortable);
			this.Productive = new Expression("Productive", this, faces.Productive);
			this.Determined = new Expression("Determined", this, faces.Determined);
			this.Sticker = new Expression("Sticker", this, faces.Sticker);
			this.Balloon = new Expression("Sticker", this, faces.Balloon);
			this.Sparkle = new Expression("Sticker", this, faces.Sparkle);
			this.Tickled = new Expression("Tickled", this, faces.Tickled);
			this.Happy = new Expression("Happy", this, faces.Happy);
			this.Relief = new Expression("Relief", this, faces.Happy);
			this.Neutral = new Expression("Neutral", this, faces.Neutral);
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.resources[i].priority = 100 * (this.Count - i);
			}
		}

		public Expression Neutral;

		public Expression Happy;

		public Expression Uncomfortable;

		public Expression Cold;

		public Expression Hot;

		public Expression FullBladder;

		public Expression Tired;

		public Expression Hungry;

		public Expression Angry;

		public Expression Unhappy;

		public Expression RedAlert;

		public Expression Suffocate;

		public Expression RecoverBreath;

		public Expression Sick;

		public Expression SickSpores;

		public Expression Zombie;

		public Expression SickFierySkin;

		public Expression SickCold;

		public Expression Relief;

		public Expression Productive;

		public Expression Determined;

		public Expression Sticker;

		public Expression Balloon;

		public Expression Sparkle;

		public Expression Tickled;
	}
}
