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

	public bool UnlockNext(string collectionID)
	{
		string text = string.Empty;
		foreach (string text2 in this.lockCollections[collectionID])
		{
			bool flag = this.locked[text2];
			if (flag)
			{
				text = text2;
				break;
			}
		}
		if (text != string.Empty)
		{
			this.Unlock(text);
			return true;
		}
		return false;
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
			new string[] { "critter_Puft_studied", "critter_Hatch_studied" }
		},
		{
			"emails",
			new string[]
			{
				"email_researchgiant", "email_preliminarycalculations", "email_atomiconrecruitment", "email_security2", "email_dinner1", "email_newemployee", "email_security3", "email_pens", "email_pens2", "email_memorychip",
				"email_arthistoryrequest", "email_AIcontrol", "email_AIcontrol2", "email_AIcontrol3", "email_AIcontrol4"
			}
		},
		{
			"journals",
			new string[] { "journal_cleanup", "journal_employeeprocessing", "journal_sunflowerseeds", "journal_magazine", "journal_pipedream", "journal_spittingimage" }
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
}
