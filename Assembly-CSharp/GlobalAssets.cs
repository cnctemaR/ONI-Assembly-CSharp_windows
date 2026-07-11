using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using UnityEngine;

public class GlobalAssets : MonoBehaviour
{
	private void Awake()
	{
		if (GlobalAssets.SoundTable.Count == 0)
		{
			Bank[] array = null;
			try
			{
				RESULT bankList = RuntimeManager.StudioSystem.getBankList(out array);
				if (bankList != RESULT.OK)
				{
					array = null;
				}
			}
			catch
			{
				array = null;
			}
			if (array != null)
			{
				foreach (Bank bank in array)
				{
					EventDescription[] array3;
					RESULT eventList = bank.getEventList(out array3);
					if (eventList != RESULT.OK)
					{
						string text;
						bank.getPath(out text);
						Output.LogError(new object[] { string.Format("ERROR [{0}] loading FMOD events for bank [{1}]", eventList, text) });
					}
					else
					{
						foreach (EventDescription eventDescription in array3)
						{
							string text;
							eventDescription.getPath(out text);
							string text2 = Assets.GetSimpleSoundEventName(text);
							text2 = text2.ToLowerInvariant();
							if (text2.Length > 0 && !GlobalAssets.SoundTable.ContainsKey(text2))
							{
								GlobalAssets.SoundTable[text2] = text;
								if (text.ToLower().Contains("lowpriority") || text2.ToLower().Contains("lowpriority"))
								{
									GlobalAssets.LowPrioritySounds.Add(text);
								}
								else if (text.ToLower().Contains("highpriority") || text2.ToLower().Contains("highpriority"))
								{
									GlobalAssets.HighPrioritySounds.Add(text);
								}
							}
						}
					}
				}
			}
		}
		SetDefaults.Initialize();
		LocString.CreateLocStringKeys(typeof(DUPLICANTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(MISC), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(UI), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ELEMENTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(CREATURES), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(SETITEMS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(RESEARCH), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ITEMS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(INPUT), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(INPUT_BINDINGS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDING.STATUSITEMS), "STRINGS.BUILDING.");
		LocString.CreateLocStringKeys(typeof(BUILDING.DETAILS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(LORE), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(CODEX), "STRINGS.");
	}

	public static string GetSound(string name, bool force_no_warning = false)
	{
		if (name == null)
		{
			return null;
		}
		name = name.ToLowerInvariant();
		string text = null;
		GlobalAssets.SoundTable.TryGetValue(name, out text);
		return text;
	}

	public static bool IsLowPriority(string path)
	{
		return GlobalAssets.LowPrioritySounds.Contains(path);
	}

	public static bool IsHighPriority(string path)
	{
		return GlobalAssets.HighPrioritySounds.Contains(path);
	}

	private static Dictionary<string, string> SoundTable = new Dictionary<string, string>();

	private static HashSet<string> LowPrioritySounds = new HashSet<string>();

	private static HashSet<string> HighPrioritySounds = new HashSet<string>();
}
