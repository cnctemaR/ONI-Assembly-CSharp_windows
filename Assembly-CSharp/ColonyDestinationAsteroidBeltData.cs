using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

public class ColonyDestinationAsteroidBeltData
{
	public float TargetScale { get; set; }

	public float Scale { get; set; }

	public int seed { get; private set; }

	public string startWorldPath
	{
		get
		{
			return this.startWorld.filePath;
		}
	}

	public Sprite sprite { get; private set; }

	public int difficulty { get; private set; }

	public string startWorldName
	{
		get
		{
			return Strings.Get(this.startWorld.name);
		}
	}

	public string properName
	{
		get
		{
			if (this.cluster == null)
			{
				return "";
			}
			return this.cluster.name;
		}
	}

	public string beltPath
	{
		get
		{
			if (this.cluster == null)
			{
				return WorldGenSettings.ClusterDefaultName;
			}
			return this.cluster.filePath;
		}
	}

	public List<global::ProcGen.World> worlds { get; private set; }

	public ClusterLayout Layout
	{
		get
		{
			return this.cluster;
		}
	}

	public global::ProcGen.World GetStartWorld
	{
		get
		{
			return this.startWorld;
		}
	}

	public ColonyDestinationAsteroidBeltData(string staringWorldName, int seed, string clusterPath)
	{
		this.startWorld = SettingsCache.worlds.GetWorldData(staringWorldName);
		this.Scale = (this.TargetScale = this.startWorld.iconScale);
		this.worlds = new List<global::ProcGen.World>();
		if (clusterPath != null)
		{
			this.cluster = SettingsCache.clusterLayouts.GetClusterData(clusterPath);
			for (int i = 0; i < this.cluster.worldPlacements.Count; i++)
			{
				if (i != this.cluster.startWorldIndex)
				{
					this.worlds.Add(SettingsCache.worlds.GetWorldData(this.cluster.worldPlacements[i].world));
				}
			}
		}
		this.ReInitialize(seed);
	}

	public static Sprite GetUISprite(string filename)
	{
		if (filename.IsNullOrWhiteSpace())
		{
			filename = (DlcManager.FeatureClusterSpaceEnabled() ? "asteroid_sandstone_start_kanim" : "Asteroid_sandstone");
		}
		KAnimFile kanimFile;
		Assets.TryGetAnim(filename, out kanimFile);
		if (kanimFile != null)
		{
			return Def.GetUISpriteFromMultiObjectAnim(kanimFile, "ui", false, "");
		}
		return Assets.GetSprite(filename);
	}

	public void ReInitialize(int seed)
	{
		this.seed = seed;
		this.paramDescriptors.Clear();
		this.traitDescriptors.Clear();
		this.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(this.startWorld.asteroidIcon);
		this.difficulty = this.cluster.difficulty;
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
		if (this.cluster != null && DlcManager.FeatureClusterSpaceEnabled())
		{
			list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, Strings.Get(this.cluster.name)), Strings.Get(this.cluster.description), Color.white, null, null));
		}
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.PLANETNAME, this.startWorldName), null, Color.white, null, null));
		list.Add(new AsteroidDescriptor(Strings.Get(this.startWorld.description), null, Color.white, null, null));
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.MOONNAMES, Array.Empty<object>()), null, Color.white, null, null));
			foreach (global::ProcGen.World world in this.worlds)
			{
				list.Add(new AsteroidDescriptor(string.Format("{0}", Strings.Get(world.name)), Strings.Get(world.description), Color.white, null, null));
			}
		}
		int num = Mathf.Clamp(this.difficulty, 0, ColonyDestinationAsteroidBeltData.survivalOptions.Count - 1);
		global::Tuple<string, string, string> tuple = ColonyDestinationAsteroidBeltData.survivalOptions[num];
		list.Add(new AsteroidDescriptor(string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third), null, Color.white, null, null));
		return list;
	}

	private List<AsteroidDescriptor> GenerateTraitDescriptors()
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		List<global::ProcGen.World> list2 = new List<global::ProcGen.World>();
		list2.Add(this.startWorld);
		list2.AddRange(this.worlds);
		for (int i = 0; i < list2.Count; i++)
		{
			global::ProcGen.World world = list2[i];
			if (DlcManager.IsExpansion1Active())
			{
				list.Add(new AsteroidDescriptor("", null, Color.white, null, null));
				list.Add(new AsteroidDescriptor(string.Format("<b>{0}</b>", Strings.Get(world.name)), null, Color.white, null, null));
			}
			List<WorldTrait> worldTraits = this.GetWorldTraits(world);
			foreach (WorldTrait worldTrait in worldTraits)
			{
				string text = worldTrait.filePath.Substring(worldTrait.filePath.LastIndexOf("/") + 1);
				list.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", Strings.Get(worldTrait.name), worldTrait.colorHex), Strings.Get(worldTrait.description), global::Util.ColorFromHex(worldTrait.colorHex), null, text));
			}
			if (worldTraits.Count == 0)
			{
				list.Add(new AsteroidDescriptor(WORLD_TRAITS.NO_TRAITS.NAME, WORLD_TRAITS.NO_TRAITS.DESCRIPTION, Color.white, null, "NoTraits"));
			}
		}
		return list;
	}

	public List<AsteroidDescriptor> GenerateTraitDescriptors(global::ProcGen.World singleWorld, bool includeDefaultTrait = true)
	{
		List<AsteroidDescriptor> list = new List<AsteroidDescriptor>();
		List<global::ProcGen.World> list2 = new List<global::ProcGen.World>();
		list2.Add(this.startWorld);
		list2.AddRange(this.worlds);
		for (int i = 0; i < list2.Count; i++)
		{
			if (list2[i] == singleWorld)
			{
				global::ProcGen.World world = list2[i];
				List<WorldTrait> worldTraits = this.GetWorldTraits(world);
				foreach (WorldTrait worldTrait in worldTraits)
				{
					string text = worldTrait.filePath.Substring(worldTrait.filePath.LastIndexOf("/") + 1);
					list.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", Strings.Get(worldTrait.name), worldTrait.colorHex), Strings.Get(worldTrait.description), global::Util.ColorFromHex(worldTrait.colorHex), null, text));
				}
				if (worldTraits.Count == 0 && includeDefaultTrait)
				{
					list.Add(new AsteroidDescriptor(WORLD_TRAITS.NO_TRAITS.NAME, WORLD_TRAITS.NO_TRAITS.DESCRIPTION, Color.white, null, "NoTraits"));
				}
			}
		}
		return list;
	}

	public List<WorldTrait> GetWorldTraits(global::ProcGen.World singleWorld)
	{
		List<WorldTrait> list = new List<WorldTrait>();
		List<global::ProcGen.World> list2 = new List<global::ProcGen.World>();
		list2.Add(this.startWorld);
		list2.AddRange(this.worlds);
		for (int i = 0; i < list2.Count; i++)
		{
			if (list2[i] == singleWorld)
			{
				global::ProcGen.World world = list2[i];
				int num = this.seed;
				if (num > 0)
				{
					num += this.cluster.worldPlacements.FindIndex((WorldPlacement x) => x.world == world.filePath);
				}
				foreach (string text in SettingsCache.GetRandomTraits(num, world))
				{
					WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(text, true);
					list.Add(cachedWorldTrait);
				}
			}
		}
		return list;
	}

	private global::ProcGen.World startWorld;

	private ClusterLayout cluster;

	private List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();

	private List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();

	public static List<global::Tuple<string, string, string>> survivalOptions = new List<global::Tuple<string, string, string>>
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
