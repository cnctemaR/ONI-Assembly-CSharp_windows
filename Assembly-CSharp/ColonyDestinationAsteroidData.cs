using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;

public class ColonyDestinationAsteroidData
{
	public ColonyDestinationAsteroidData(string worldName, int seed)
	{
		this.Scale = 1f;
		this.TargetScale = 1f;
		this.world = SettingsCache.worlds.GetWorldData(worldName);
		this.ReInitialize(seed);
	}

	public float TargetScale { get; set; }

	public float Scale { get; set; }

	public int seed { get; private set; }

	public string worldPath
	{
		get
		{
			return this.world.filePath;
		}
	}

	public string sprite { get; private set; }

	public int difficulty { get; private set; }

	public void ReInitialize(int seed)
	{
		this.seed = seed;
		this.paramDescriptors.Clear();
		this.traitDescriptors.Clear();
		this.sprite = this.world.spriteName;
		this.difficulty = this.world.difficulty;
	}

	public List<AsteroidDescriptor> GetParamDescriptors()
	{
		if (this.paramDescriptors.Count == 0)
		{
			this.paramDescriptors = this.GenerateParamDescriptors();
		}
		return this.paramDescriptors;
	}

	public List<AsteroidDescriptor> GetTraitDescriptors()
	{
		if (this.traitDescriptors.Count == 0)
		{
			this.traitDescriptors = this.GenerateTraitDescriptors();
		}
		return this.traitDescriptors;
	}

	private List<AsteroidDescriptor> GenerateParamDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.PLANETNAME, Strings.Get(this.world.name)), null, null));
		list.Add(new AsteroidDescriptor(Strings.Get(this.world.description), null, null));
		Tuple<string, string, string> tuple = ColonyDestinationAsteroidData.survivalOptions[this.difficulty];
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third), null, null));
		return list;
	}

	private List<AsteroidDescriptor> GenerateTraitDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		if (this.world.disableWorldTraits)
		{
			list.Add(new AsteroidDescriptor(WORLD_TRAITS.NO_TRAITS.NAME, WORLD_TRAITS.NO_TRAITS.DESCRIPTION, null));
		}
		else
		{
			List<string> randomTraits = SettingsCache.GetRandomTraits(this.seed);
			foreach (string text in randomTraits)
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(text);
				list.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", Strings.Get(cachedTrait.name), cachedTrait.colorHex), Strings.Get(cachedTrait.description), null));
			}
		}
		return list;
	}

	private global::ProcGen.World world;

	private List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();

	private List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();

	private static List<Tuple<string, string, string>> survivalOptions = new List<Tuple<string, string, string>>
	{
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.MOSTHOSPITABLE, string.Empty, "D2F40C"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYHIGH, string.Empty, "7DE419"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.HIGH, string.Empty, "36D246"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.NEUTRAL, string.Empty, "63C2B7"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LOW, string.Empty, "6A8EB1"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYLOW, string.Empty, "937890"),
		new Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LEASTHOSPITABLE, string.Empty, "9636DF")
	};
}
