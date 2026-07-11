using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

public class ColonyDestinationAsteroidData
{
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

	public string properName
	{
		get
		{
			return Strings.Get(this.world.name);
		}
	}

	public ColonyDestinationAsteroidData(string worldName, int seed)
	{
		this.Scale = 1f;
		this.TargetScale = 1f;
		this.world = SettingsCache.worlds.GetWorldData(worldName);
		this.ReInitialize(seed);
	}

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
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.PLANETNAME, this.properName), null, null));
		list.Add(new AsteroidDescriptor(Strings.Get(this.world.description), null, null));
		int num = Mathf.Clamp(this.difficulty, 0, ColonyDestinationAsteroidData.survivalOptions.Count - 1);
		global::Tuple<string, string, string> tuple = ColonyDestinationAsteroidData.survivalOptions[num];
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
			foreach (string text in SettingsCache.GetRandomTraits(this.seed))
			{
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(text, true);
				list.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", Strings.Get(cachedTrait.name), cachedTrait.colorHex), Strings.Get(cachedTrait.description), null));
			}
		}
		return list;
	}

	private global::ProcGen.World world;

	private List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();

	private List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();

	private static List<global::Tuple<string, string, string>> survivalOptions = new List<global::Tuple<string, string, string>>
	{
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.MOSTHOSPITABLE, "", "D2F40C"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYHIGH, "", "7DE419"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.HIGH, "", "36D246"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.NEUTRAL, "", "63C2B7"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LOW, "", "6A8EB1"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.VERYLOW, "", "937890"),
		new global::Tuple<string, string, string>(WORLDS.SURVIVAL_CHANCE.LEASTHOSPITABLE, "", "9636DF")
	};
}
