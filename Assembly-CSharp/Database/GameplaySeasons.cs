using System;
using Klei.AI;

namespace Database
{
	public class GameplaySeasons : ResourceSet<GameplaySeason>
	{
		public GameplaySeasons(ResourceSet parent)
			: base("GameplaySeasons", parent)
		{
			this.MeteorShowers = base.Add(new GameplaySeason("MeteorShowers", GameplaySeason.Type.World, "", 14f, false, -1f, true, -1, 0f, float.PositiveInfinity, 1).AddEvent(Db.Get().GameplayEvents.MeteorShowerIronEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerGoldEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerCopperEvent));
			this.RegolithMoonMeteorShowers = base.Add(new GameplaySeason("RegolithMoonMeteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 20f, false, -1f, true, -1, 0f, float.PositiveInfinity, 1).AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent));
			this.TemporalTearMeteorShowers = base.Add(new GameplaySeason("TemporalTearMeteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 1f, false, 0f, false, -1, 0f, float.PositiveInfinity, 1).AddEvent(Db.Get().GameplayEvents.MeteorShowerFullereneEvent));
			this.GassyMooteorShowers = base.Add(new GameplaySeason("GassyMooteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 20f, false, -1f, true, -1, 0f, float.PositiveInfinity, 1).AddEvent(Db.Get().GameplayEvents.GassyMooteorEvent));
		}

		public GameplaySeason MeteorShowers;

		public GameplaySeason GassyMooteorShowers;

		public GameplaySeason TemporalTearMeteorShowers;

		public GameplaySeason NaturalRandomEvents;

		public GameplaySeason DupeRandomEvents;

		public GameplaySeason PrickleCropSeason;

		public GameplaySeason BonusEvents;

		public GameplaySeason RegolithMoonMeteorShowers;
	}
}
