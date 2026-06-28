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
		GlobalAssets.ignoreMissing = false;
		if (GlobalAssets.SoundTable.Count == 0)
		{
			Bank[] array;
			RESULT result = RuntimeManager.StudioSystem.getBankList(out array);
			if (result != RESULT.OK)
			{
				Output.LogError(new object[] { string.Format("ERROR [{0}] loading FMOD banks", result) });
			}
			foreach (Bank bank in array)
			{
				EventDescription[] array3;
				result = bank.getEventList(out array3);
				if (result != RESULT.OK)
				{
					string text;
					bank.getPath(out text);
					Output.LogError(new object[] { string.Format("ERROR [{0}] loading FMOD events for bank [{1}]", result, text) });
				}
				else
				{
					foreach (EventDescription eventDescription in array3)
					{
						string text;
						eventDescription.getPath(out text);
						string text2 = Assets.GetSimpleSoundEventName(text);
						text2 = text2.ToLowerInvariant();
						if (text2.Length > 0)
						{
							GlobalAssets.SoundTable[text2] = text;
						}
					}
				}
			}
		}
		SetDefaults.Initialize();
		Strings.Add(Assets.StringTables);
		LocString.CreateLocStringKeys(typeof(DUPLICANTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(MISC), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(UI), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ELEMENTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(CREATURES), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(REGIONS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(TAGS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(RESEARCH), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ITEMS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDING.STATUSITEMS), "STRINGS.BUILDING.");
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
		if (!GlobalAssets.ignoreMissing && text == null && !force_no_warning)
		{
			Output.LogWarning(new object[] { "SoundTable does not contain sound named [" + name + "]" });
		}
		return text;
	}

	private static Dictionary<string, string> SoundTable = new Dictionary<string, string>();

	private static bool ignoreMissing = true;
}
