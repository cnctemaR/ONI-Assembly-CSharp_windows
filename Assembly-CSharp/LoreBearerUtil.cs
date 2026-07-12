using System;
using STRINGS;
using UnityEngine;

public static class LoreBearerUtil
{
	public static void AddLoreTo(GameObject prefabOrGameObject)
	{
		prefabOrGameObject.AddOrGet<LoreBearer>();
	}

	public static void AddLoreTo(GameObject prefabOrGameObject, LoreBearerAction unlockLoreFn)
	{
		KPrefabID component = prefabOrGameObject.GetComponent<KPrefabID>();
		if (component.IsInitialized())
		{
			prefabOrGameObject.AddOrGet<LoreBearer>().Internal_SetContent(unlockLoreFn);
			return;
		}
		prefabOrGameObject.AddComponent<LoreBearer>();
		component.prefabInitFn += delegate(GameObject gameObject)
		{
			gameObject.GetComponent<LoreBearer>().Internal_SetContent(unlockLoreFn);
		};
	}

	public static LoreBearerAction UnlockSpecificEntry(string unlockId, string searchDisplayText)
	{
		return delegate(InfoDialogScreen screen)
		{
			Game.Instance.unlocks.Unlock(unlockId, true);
			screen.AddPlainText(searchDisplayText);
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(unlockId, false), false);
		};
	}

	public static void UnlockNextEmail(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("emails");
		if (text != null)
		{
			string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text2));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text, false), false);
			return;
		}
		string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 8).ToString();
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text3));
	}

	public static void UnlockNextResearchNote(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("researchnotes");
		if (text != null)
		{
			string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 3).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + text2));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text, false), false);
			return;
		}
		string text3 = "SEARCH1";
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text3));
	}

	public static void UnlockNextJournalEntry(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("journals");
		if (text != null)
		{
			string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text2));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text, false), false);
			return;
		}
		string text3 = "SEARCH1";
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text3));
	}

	public static void UnlockNextDimensionalLore(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("dimensionallore");
		if (text != null)
		{
			string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text2));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text, false), false);
			return;
		}
		string text3 = "SEARCH1";
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text3));
	}

	public static void UnlockNextSpaceEntry(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("space");
		if (text != null)
		{
			string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 7).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_SUCCESS." + text2));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text, false), false);
			return;
		}
		string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 4).ToString();
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_FAIL." + text3));
	}

	public static void UnlockNextDeskPodiumEntry(InfoDialogScreen screen)
	{
		if (!Game.Instance.unlocks.IsUnlocked("story_trait_critter_manipulator_parking"))
		{
			Game.Instance.unlocks.Unlock("story_trait_critter_manipulator_parking", true);
			string text = "SEARCH" + global::UnityEngine.Random.Range(1, 1).ToString();
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_PODIUM." + text));
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID("story_trait_critter_manipulator_parking", false), false);
			return;
		}
		string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 8).ToString();
		screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text2));
	}

	public static void NerualVacillator(InfoDialogScreen screen)
	{
		Game.Instance.unlocks.Unlock("neuralvacillator", true);
		LoreBearerUtil.UnlockNextResearchNote(screen);
	}

	public static Action<InfoDialogScreen> OpenCodexByLockKeyID(string key, bool focusContent = false)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			string entryForLock = CodexCache.GetEntryForLock(key);
			if (entryForLock == null)
			{
				DebugUtil.DevLogError("Missing codex entry for lock: " + key);
				return;
			}
			ContentContainer contentContainer = null;
			if (focusContent)
			{
				CodexEntry codexEntry = CodexCache.FindEntry(entryForLock);
				int num = 0;
				while (contentContainer == null && num < codexEntry.contentContainers.Count)
				{
					if (!(codexEntry.contentContainers[num].lockID != key))
					{
						contentContainer = codexEntry.contentContainers[num];
					}
					num++;
				}
			}
			ManagementMenu.Instance.OpenCodexToEntry(entryForLock, contentContainer);
		};
	}

	public static Action<InfoDialogScreen> OpenCodexByEntryID(string id)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			ManagementMenu.Instance.OpenCodexToEntry(id, null);
		};
	}
}
