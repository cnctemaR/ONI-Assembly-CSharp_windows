using System;

namespace Database
{
	public class Expressions : ResourceSet<Expression>
	{
		public Expressions(ResourceSet parent)
			: base("Expressions", parent)
		{
			Faces faces = Db.Get().Faces;
			this.Dead = new Expression("Dead", this, faces.Dead);
			this.Sleep = new Expression("Sleep", this, faces.Sleep);
			this.Angry = new Expression("Angry", this, faces.Angry);
			this.RedAlert = new Expression("RedAlert", this, faces.Hot);
			this.Suffocate = new Expression("Suffocate", this, faces.Suffocate);
			this.RecoverBreath = new Expression("RecoverBreath", this, faces.Uncomfortable);
			this.Hungry = new Expression("Hungry", this, faces.Hungry);
			this.SickSpores = new Expression("SickSpores", this, faces.SickSpores);
			this.SickFierySkin = new Expression("SickFierySkin", this, faces.SickFierySkin);
			this.Sick = new Expression("Sick", this, faces.Sick);
			this.Cold = new Expression("Cold", this, faces.Cold);
			this.Hot = new Expression("Hot", this, faces.Hot);
			this.FullBladder = new Expression("FullBladder", this, faces.Uncomfortable);
			this.Tired = new Expression("Tired", this, faces.Tired);
			this.Unhappy = new Expression("Unhappy", this, faces.Uncomfortable);
			this.Uncomfortable = new Expression("Uncomfortable", this, faces.Uncomfortable);
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

		public Expression Sleep;

		public Expression Hungry;

		public Expression Angry;

		public Expression Unhappy;

		public Expression RedAlert;

		public Expression Suffocate;

		public Expression RecoverBreath;

		public Expression Dead;

		public Expression Sick;

		public Expression SickSpores;

		public Expression SickFierySkin;

		public Expression Relief;
	}
}
