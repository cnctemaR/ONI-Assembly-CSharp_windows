using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public static class LoreBearerUtil
{
	public static Action<InfoDialogScreen> GetUnlockActionForCollection(string collectionId)
	{
		Action<InfoDialogScreen> action2;
		if (LoreBearerUtil.CollectionUnlockMethods.TryGetValue(collectionId, out action2))
		{
			return action2;
		}
		LoreBearerAction action = LoreBearerUtil.UnlockNextInCollections(new string[] { collectionId });
		return delegate(InfoDialogScreen screen)
		{
			action(screen);
		};
	}

	public static void AddPOILoreSupport(GameObject prefabOrGameObject)
	{
		prefabOrGameObject.AddOrGet<LoreBearer>().useDefaultLore = false;
	}

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

	public static void AddLoreTo(GameObject prefabOrGameObject, string[] collectionsToUnlockFrom)
	{
		KPrefabID component = prefabOrGameObject.GetComponent<KPrefabID>();
		if (component.IsInitialized())
		{
			prefabOrGameObject.AddOrGet<LoreBearer>().Internal_SetContent(LoreBearerUtil.UnlockNextInCollections(collectionsToUnlockFrom));
			return;
		}
		prefabOrGameObject.AddComponent<LoreBearer>();
		component.prefabInitFn += delegate(GameObject gameObject)
		{
			gameObject.GetComponent<LoreBearer>().Internal_SetContent(LoreBearerUtil.UnlockNextInCollections(collectionsToUnlockFrom));
		};
	}

	public static LoreBearerAction UnlockSpecificEntry(string unlockId, string searchDisplayText, bool focus = false)
	{
		return delegate(InfoDialogScreen screen)
		{
			Game.Instance.unlocks.Unlock(unlockId, true);
			screen.AddPlainText(searchDisplayText);
			screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(unlockId, focus), false);
		};
	}

	public static LoreBearerAction UnlockSpecificEntryThenNext(string unlockId, string searchDisplayText, Action<InfoDialogScreen> next, bool focus = false)
	{
		return delegate(InfoDialogScreen screen)
		{
			if (!Game.Instance.unlocks.IsUnlocked(unlockId))
			{
				Game.Instance.unlocks.Unlock(unlockId, true);
				screen.AddPlainText(searchDisplayText);
				screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(unlockId, focus), false);
				return;
			}
			next(screen);
		};
	}

	public static void UnlockNextEmail(InfoDialogScreen screen)
	{
		string text = Game.Instance.unlocks.UnlockNext("emails", false);
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
		string text = Game.Instance.unlocks.UnlockNext("researchnotes", false);
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
		string text = Game.Instance.unlocks.UnlockNext("journals", false);
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
		string text = Game.Instance.unlocks.UnlockNext("dimensionallore", true);
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
		string text = Game.Instance.unlocks.UnlockNext("space", false);
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

	public static LoreBearerAction UnlockNextInCollections(string[] collectionsToUnlockFrom)
	{
		return delegate(InfoDialogScreen screen)
		{
			foreach (string text in collectionsToUnlockFrom)
			{
				string text2 = Game.Instance.unlocks.UnlockNext(text, false);
				if (text2 != null)
				{
					screen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS.SEARCH1);
					screen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearerUtil.OpenCodexByLockKeyID(text2, false), false);
					return;
				}
			}
			string text3 = "SEARCH1";
			screen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text3));
		};
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
			ManagementMenu.Instance.OpenCodexToLockId(key, focusContent);
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

	public static readonly Dictionary<string, Action<InfoDialogScreen>> CollectionUnlockMethods = new Dictionary<string, Action<InfoDialogScreen>>
	{
		{
			"emails",
			new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextEmail)
		},
		{
			"researchnotes",
			new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextResearchNote)
		},
		{
			"journals",
			new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextJournalEntry)
		},
		{
			"dimensionallore",
			new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextDimensionalLore)
		},
		{
			"space",
			new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextSpaceEntry)
		}
	};
}
