using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ProcGen;

public class Unlocks : KMonoBehaviour, ISim4000ms
{
	protected override void OnPrefabInit()
	{
		foreach (KeyValuePair<string, string[]> keyValuePair in this.lockCollections)
		{
			foreach (string text in keyValuePair.Value)
			{
				this.defaultLocked.Add(text, true);
			}
		}
		foreach (KeyValuePair<int, string> keyValuePair2 in this.cycleLocked)
		{
			this.defaultLocked.Add(keyValuePair2.Value, true);
		}
		this.LoadLocks();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
		base.Subscribe<Unlocks>(-1056989049, Unlocks.OnLaunchRocketDelegate);
		base.Subscribe<Unlocks>(282337316, Unlocks.OnDuplicantDiedDelegate);
		base.Subscribe<Unlocks>(-107300940, Unlocks.OnResearchCompleteDelegate);
		base.Subscribe<Unlocks>(-818188514, Unlocks.OnDiscoveredSpaceDelegate);
		this.UnlockCycleCodexes();
		Components.LiveMinionIdentities.OnAdd += this.OnNewDupe;
	}

	public bool IsLocked(string lockID)
	{
		return !string.IsNullOrEmpty(lockID) && this.locked.ContainsKey(lockID) && this.locked[lockID];
	}

	public void Unlock(string lockID)
	{
		if (!this.locked.ContainsKey(lockID))
		{
			return;
		}
		this.locked[lockID] = false;
		Unlocks.SaveUnlocks(this.locked);
		Game.Instance.Trigger(1594320620, lockID);
	}

	public void UnlockOne(string[] lockIDs)
	{
		List<string> list = new List<string>();
		foreach (string text in lockIDs)
		{
			if (this.IsLocked(text))
			{
				list.Add(text);
			}
		}
		this.Unlock(list.GetRandom<string>());
	}

	private static string UnlocksFilename
	{
		get
		{
			return global::System.IO.Path.Combine(global::Util.RootFolder(), "unlocks.json");
		}
	}

	public static void SaveUnlocks(Dictionary<string, bool> locks)
	{
		if (!Directory.Exists(global::Util.RootFolder()))
		{
			Directory.CreateDirectory(global::Util.RootFolder());
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> keyValuePair in locks)
		{
			if (!keyValuePair.Value)
			{
				list.Add(keyValuePair.Key);
			}
		}
		string text = JsonConvert.SerializeObject(list);
		File.WriteAllText(Unlocks.UnlocksFilename, text);
	}

	private void SetToDefaultLocks()
	{
		this.locked.Clear();
		foreach (KeyValuePair<string, bool> keyValuePair in this.defaultLocked)
		{
			this.locked.Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	public void LoadLocks()
	{
		this.SetToDefaultLocks();
		if (!File.Exists(Unlocks.UnlocksFilename))
		{
			return;
		}
		string text = File.ReadAllText(Unlocks.UnlocksFilename);
		if (text == null || text == string.Empty)
		{
			return;
		}
		try
		{
			string[] array = JsonConvert.DeserializeObject<string[]>(text);
			foreach (string text2 in array)
			{
				if (this.locked.ContainsKey(text2))
				{
					this.locked[text2] = false;
				}
				else
				{
					this.locked.Add(text2, false);
				}
			}
		}
		catch
		{
			Output.LogError(new object[]
			{
				"Error parsing",
				Unlocks.UnlocksFilename
			});
		}
		if (this.locked == null || this.locked.Count == 0)
		{
			return;
		}
	}

	public string UnlockNext(string collectionID)
	{
		foreach (string text in this.lockCollections[collectionID])
		{
			if (this.locked[text])
			{
				this.Unlock(text);
				return text;
			}
		}
		return null;
	}

	private void UnlockCycleCodexes()
	{
		foreach (KeyValuePair<int, string> keyValuePair in this.cycleLocked)
		{
			if (GameClock.Instance.GetCycle() + 1 >= keyValuePair.Key)
			{
				this.Unlock(keyValuePair.Value);
			}
		}
	}

	private void OnNewDay(object data)
	{
		this.UnlockCycleCodexes();
	}

	private void OnLaunchRocket(object data)
	{
		this.Unlock("firstrocketlaunch");
	}

	private void OnDuplicantDied(object data)
	{
		this.Unlock("duplicantdeath");
		if (Components.LiveMinionIdentities.Count == 1)
		{
			this.Unlock("onedupeleft");
		}
	}

	private void OnNewDupe(MinionIdentity minion_identity)
	{
		if (Components.LiveMinionIdentities.Count >= 20)
		{
			this.Unlock("twentydupecolony");
		}
		else if (Components.LiveMinionIdentities.Count >= 35)
		{
			this.Unlock("fulldupecolony");
		}
	}

	private void OnResearchComplete(object data)
	{
		Tech tech = (Tech)data;
		this.Unlock("firstresearch");
		if (tech.Id == "BasicRocketry")
		{
			this.Unlock("rocketryresearch");
		}
	}

	private void OnDiscoveredSpace(object data)
	{
		this.Unlock("surfacebreach");
	}

	public void Sim4000ms(float dt)
	{
		int num = int.MinValue;
		int num2 = int.MinValue;
		int num3 = int.MaxValue;
		int num4 = int.MaxValue;
		foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
		{
			if (!(minionIdentity == null))
			{
				int num5 = Grid.PosToCell(minionIdentity);
				if (Grid.IsValidCell(num5))
				{
					int num6;
					int num7;
					Grid.CellToXY(num5, out num6, out num7);
					if (num7 > num2)
					{
						num2 = num7;
						num = num6;
					}
					if (num7 < num4)
					{
						num3 = num6;
						num4 = num7;
					}
				}
			}
		}
		if (num2 != -2147483648)
		{
			int num8 = num2;
			for (int i = 0; i < 30; i++)
			{
				num8++;
				int num9 = Grid.XYToCell(num, num8);
				if (!Grid.IsValidCell(num9))
				{
					break;
				}
				SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num9);
				if (subWorldZoneType == SubWorld.ZoneType.Space)
				{
					this.Unlock("nearingsurface");
					break;
				}
			}
		}
		if (num4 != 2147483647)
		{
			int num10 = num4;
			for (int j = 0; j < 30; j++)
			{
				num10--;
				int num11 = Grid.XYToCell(num3, num10);
				if (!Grid.IsValidCell(num11))
				{
					break;
				}
				SubWorld.ZoneType subWorldZoneType2 = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num11);
				if (subWorldZoneType2 == SubWorld.ZoneType.ToxicJungle && Grid.Element[num11].id == SimHashes.Magma)
				{
					this.Unlock("nearingmagma");
					break;
				}
			}
		}
	}

	public Dictionary<string, bool> locked = new Dictionary<string, bool>();

	public Dictionary<string, bool> defaultLocked = new Dictionary<string, bool>
	{
		{ "poi_surface_facillity_1", true },
		{ "poi_surface_facillity_2", true },
		{ "poi_surface_facillity_3", true },
		{ "poi_surface_facillity_4", true },
		{ "firstrocketlaunch", true },
		{ "duplicantdeath", true },
		{ "onedupeleft", true },
		{ "twentydupecolony", true },
		{ "fulldupecolony", true },
		{ "firstresearch", true },
		{ "rocketryresearch", true },
		{ "surfacebreach", true },
		{ "nearingsurface", true },
		{ "nearingmagma", true },
		{ "neuralvacillator", true }
	};

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"critters",
			new string[] { "critter_Hatch_studied" }
		},
		{
			"emails",
			new string[]
			{
				"email_preliminarycalculations", "email_researchgiant", "email_frankiesblog", "email_atomiconrecruitment", "email_thejanitor", "email_security2", "email_newemployee", "email_security3", "email_hollandsdog", "email_pens",
				"email_pens2", "email_memorychip", "email_arthistoryrequest", "email_AIcontrol", "email_AIcontrol2", "email_AIcontrol3", "email_AIcontrol4"
			}
		},
		{
			"journals",
			new string[]
			{
				"journal_cleanup", "journal_employeeprocessing", "journal_sunflowerseeds", "journal_B835_1", "journal_B835_2", "journal_B835_3", "journal_B835_4", "journal_B835_5", "journal_B835_6", "journal_pipedream",
				"journal_spittingimage", "journal_A046_1", "journal_A046_2", "journal_A046_3", "journal_A046_4", "journal_ants", "journal_debrief", "journal_B327_1", "journal_B327_2", "journal_B327_3",
				"journal_B327_4", "journal_movedrats", "journal_revisitednumbers"
			}
		},
		{
			"researchnotes",
			new string[] { "notes_clonedrats", "notes_hibiscus3", "notes_geneticooze", "notes_memoryimplantation" }
		},
		{
			"misc",
			new string[] { "misc_mailroometiquette", "misc_unattendedcultures", "misc_newsecurity", "misc_politerequest", "misc_casualfriday", "misc_bringyourkidtowork", "misc_dishbot" }
		},
		{
			"special_set_items",
			new string[] { "display_prop1", "display_prop2", "display_prop3", "pod_evacuation", "printingpod" }
		}
	};

	public Dictionary<int, string> cycleLocked = new Dictionary<int, string>
	{
		{ 3, "log2" },
		{ 10, "log3" },
		{ 15, "log4" },
		{ 20, "log5" },
		{ 30, "log6" },
		{ 35, "log7" }
	};

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnLaunchRocketDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks unlocks, object data)
	{
		unlocks.OnLaunchRocket(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDuplicantDiedDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks unlocks, object data)
	{
		unlocks.OnDuplicantDied(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnResearchCompleteDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks unlocks, object data)
	{
		unlocks.OnResearchComplete(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDiscoveredSpaceDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks unlocks, object data)
	{
		unlocks.OnDiscoveredSpace(data);
	});
}
