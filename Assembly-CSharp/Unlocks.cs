using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
	private void Awake()
	{
		foreach (KeyValuePair<string, string[]> keyValuePair in this.lockCollections)
		{
			foreach (string text in keyValuePair.Value)
			{
				this.defaultLocked.Add(text, true);
			}
		}
		this.LoadLocks();
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
			return Path.Combine(Util.RootFolder(), "unlocks.json");
		}
	}

	public static void SaveUnlocks(Dictionary<string, bool> locks)
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		if (File.Exists(Unlocks.UnlocksFilename))
		{
			File.Delete(Unlocks.UnlocksFilename);
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

	public Dictionary<string, bool> locked = new Dictionary<string, bool>();

	public Dictionary<string, bool> defaultLocked = new Dictionary<string, bool>
	{
		{ "poi_surface_facillity_1", true },
		{ "poi_surface_facillity_2", true },
		{ "poi_surface_facillity_3", true },
		{ "poi_surface_facillity_4", true }
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
				"journal_spittingimage", "journal_A046_1", "journal_A046_2", "journal_A046_3", "journal_A046_4", "journal_ants", "journal_debrief", "journal_movedrats", "journal_revisitednumbers"
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
			new string[0]
		},
		{
			"main_log",
			new string[] { "log2", "log3" }
		}
	};
}
