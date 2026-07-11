using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
	private void Awake()
	{
		this.LoadLocks();
	}

	public bool IsLocked(string lockID)
	{
		return this.locked.ContainsKey(lockID) && this.locked[lockID];
	}

	public void Unlock(string lockID)
	{
		if (!this.locked.ContainsKey(lockID))
		{
			return;
		}
		this.locked[lockID] = false;
		Unlocks.SaveUnlocks(this.locked);
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

	public Dictionary<string, bool> locked = new Dictionary<string, bool>();

	public Dictionary<string, bool> defaultLocked = new Dictionary<string, bool>
	{
		{ "puft_lore", true },
		{ "hatch_lore", true },
		{ "drecko_lore", true },
		{ "shinebug_lore", true },
		{ "glom_lore", true },
		{ "pacu_lore", true },
		{ "poi_surface_facillity_1", true },
		{ "poi_surface_facillity_2", true },
		{ "poi_surface_facillity_3", true },
		{ "poi_surface_facillity_4", true }
	};
}
