using System;

public class FactionManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		FactionManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Faction GetFaction(FactionManager.FactionID faction)
	{
		Faction faction2;
		switch (faction)
		{
		case FactionManager.FactionID.Duplicant:
			faction2 = this.Duplicant;
			break;
		case FactionManager.FactionID.Friendly:
			faction2 = this.Friendly;
			break;
		case FactionManager.FactionID.Hostile:
			faction2 = this.Hostile;
			break;
		case FactionManager.FactionID.Prey:
			faction2 = this.Prey;
			break;
		case FactionManager.FactionID.Predator:
			faction2 = this.Predator;
			break;
		case FactionManager.FactionID.Pest:
			faction2 = this.Pest;
			break;
		default:
			faction2 = null;
			break;
		}
		return faction2;
	}

	public FactionManager.Disposition GetDisposition(FactionManager.FactionID of_faction, FactionManager.FactionID to_faction)
	{
		FactionManager.Disposition disposition;
		if (FactionManager.Instance.GetFaction(of_faction).Dispositions.ContainsKey(to_faction))
		{
			disposition = FactionManager.Instance.GetFaction(of_faction).Dispositions[to_faction];
		}
		else
		{
			disposition = FactionManager.Disposition.Neutral;
		}
		return disposition;
	}

	public static FactionManager Instance;

	public Faction Duplicant = new Faction(FactionManager.FactionID.Duplicant);

	public Faction Friendly = new Faction(FactionManager.FactionID.Friendly);

	public Faction Hostile = new Faction(FactionManager.FactionID.Hostile);

	public Faction Predator = new Faction(FactionManager.FactionID.Predator);

	public Faction Prey = new Faction(FactionManager.FactionID.Prey);

	public Faction Pest = new Faction(FactionManager.FactionID.Pest);

	public enum FactionID
	{
		Duplicant,
		Friendly,
		Hostile,
		Prey,
		Predator,
		Pest,
		NumberOfFactions
	}

	public enum Disposition
	{
		Assist,
		Neutral,
		Attack
	}
}
