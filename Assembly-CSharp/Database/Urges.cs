using System;

namespace Database
{
	public class Urges : ResourceSet<Urge>
	{
		public Urges()
		{
			this.HealCritical = base.Add(new Urge("HealCritical"));
			this.BeIncapacitated = base.Add(new Urge("BeIncapacitated"));
			this.PacifyEat = base.Add(new Urge("PacifyEat"));
			this.PacifySleep = base.Add(new Urge("PacifySleep"));
			this.PacifyIdle = base.Add(new Urge("PacifyIdle"));
			this.EmoteHighPriority = base.Add(new Urge("EmoteHighPriority"));
			this.RecoverBreath = base.Add(new Urge("RecoverBreath"));
			this.Aggression = base.Add(new Urge("Aggression"));
			this.Relax = base.Add(new Urge("Relax"));
			this.MoveToQuarantine = base.Add(new Urge("MoveToQuarantine"));
			this.WashHands = base.Add(new Urge("WashHands"));
			this.Shower = base.Add(new Urge("Shower"));
			this.Eat = base.Add(new Urge("Eat"));
			this.Pee = base.Add(new Urge("Pee"));
			this.RestDueToDisease = base.Add(new Urge("RestDueToDisease"));
			this.Sleep = base.Add(new Urge("Sleep"));
			this.Narcolepsy = base.Add(new Urge("Narcolepsy"));
			this.Doctor = base.Add(new Urge("Doctor"));
			this.Heal = base.Add(new Urge("Heal"));
			this.Feed = base.Add(new Urge("Feed"));
			this.PacifyRelocate = base.Add(new Urge("PacifyRelocate"));
			this.Emote = base.Add(new Urge("Emote"));
			this.MoveToSafety = base.Add(new Urge("MoveToSafety"));
			this.WarmUp = base.Add(new Urge("WarmUp"));
			this.CoolDown = base.Add(new Urge("CoolDown"));
		}

		public Urge BeIncapacitated;

		public Urge Sleep;

		public Urge Narcolepsy;

		public Urge Eat;

		public Urge WashHands;

		public Urge Shower;

		public Urge Pee;

		public Urge MoveToQuarantine;

		public Urge HealCritical;

		public Urge RecoverBreath;

		public Urge Emote;

		public Urge Feed;

		public Urge Doctor;

		public Urge Flee;

		public Urge Heal;

		public Urge PacifyIdle;

		public Urge PacifyEat;

		public Urge PacifySleep;

		public Urge PacifyRelocate;

		public Urge RestDueToDisease;

		public Urge EmoteHighPriority;

		public Urge Aggression;

		public Urge Relax;

		public Urge MoveToSafety;

		public Urge WarmUp;

		public Urge CoolDown;
	}
}
