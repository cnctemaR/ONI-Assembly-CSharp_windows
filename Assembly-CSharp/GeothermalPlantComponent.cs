using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;

public class GeothermalPlantComponent : KMonoBehaviour, ICheckboxListGroupControl, IRelatedEntities
{
	string ICheckboxListGroupControl.Title
	{
		get
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.SIDESCREENS.BRING_ONLINE_TITLE;
		}
	}

	string ICheckboxListGroupControl.Description
	{
		get
		{
			return COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.SIDESCREENS.BRING_ONLINE_DESC;
		}
	}

	public ICheckboxListGroupControl.ListGroup[] GetData()
	{
		ColonyAchievement activateGeothermalPlant = Db.Get().ColonyAchievements.ActivateGeothermalPlant;
		ICheckboxListGroupControl.CheckboxItem[] array = new ICheckboxListGroupControl.CheckboxItem[activateGeothermalPlant.requirementChecklist.Count];
		for (int i = 0; i < array.Length; i++)
		{
			ICheckboxListGroupControl.CheckboxItem checkboxItem = default(ICheckboxListGroupControl.CheckboxItem);
			bool flag = activateGeothermalPlant.requirementChecklist[i].Success();
			checkboxItem.isOn = flag;
			checkboxItem.text = (activateGeothermalPlant.requirementChecklist[i] as VictoryColonyAchievementRequirement).Name();
			checkboxItem.tooltip = activateGeothermalPlant.requirementChecklist[i].GetProgress(flag);
			array[i] = checkboxItem;
		}
		return new ICheckboxListGroupControl.ListGroup[]
		{
			new ICheckboxListGroupControl.ListGroup(activateGeothermalPlant.Name, array, null, null)
		};
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public int CheckboxSideScreenSortOrder()
	{
		return 100;
	}

	public static bool GeothermalControllerRepaired()
	{
		return SaveGame.Instance.ColonyAchievementTracker.GeothermalControllerRepaired;
	}

	public static bool GeothermalFacilityDiscovered()
	{
		return SaveGame.Instance.ColonyAchievementTracker.GeothermalFacilityDiscovered;
	}

	protected override void OnSpawn()
	{
		base.Subscribe(-1503271301, new Action<object>(this.OnObjectSelect));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public static void DisplayPopup(string title, string desc, HashedString anim, global::System.Action onDismissCallback, Transform clickFocus = null)
	{
		EventInfoData eventInfoData = new EventInfoData(title, desc, anim);
		if (Components.LiveMinionIdentities.Count >= 2)
		{
			int num = global::UnityEngine.Random.Range(0, Components.LiveMinionIdentities.Count);
			int num2 = global::UnityEngine.Random.Range(1, Components.LiveMinionIdentities.Count);
			eventInfoData.minions = new GameObject[]
			{
				Components.LiveMinionIdentities[num].gameObject,
				Components.LiveMinionIdentities[(num + num2) % Components.LiveMinionIdentities.Count].gameObject
			};
		}
		else if (Components.LiveMinionIdentities.Count == 1)
		{
			eventInfoData.minions = new GameObject[] { Components.LiveMinionIdentities[0].gameObject };
		}
		eventInfoData.AddDefaultOption(onDismissCallback);
		eventInfoData.clickFocus = clickFocus;
		EventInfoScreen.ShowPopup(eventInfoData);
	}

	protected void RevealAllVentsAndController()
	{
		foreach (WorldGenSpawner.Spawnable spawnable in SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag(true, new Tag[] { "GeothermalVentEntity" }))
		{
			int num;
			int num2;
			Grid.CellToXY(spawnable.cell, out num, out num2);
			GridVisibility.Reveal(num, num2 + 2, 5, 5f);
		}
		foreach (WorldGenSpawner.Spawnable spawnable2 in SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag(true, new Tag[] { "GeothermalControllerEntity" }))
		{
			int num3;
			int num4;
			Grid.CellToXY(spawnable2.cell, out num3, out num4);
			GridVisibility.Reveal(num3, num4 + 3, 7, 7f);
		}
		SelectTool.Instance.Select(null, true);
	}

	protected void OnObjectSelect(object clicked)
	{
		base.Unsubscribe(-1503271301, new Action<object>(this.OnObjectSelect));
		if (SaveGame.Instance.ColonyAchievementTracker.GeothermalFacilityDiscovered)
		{
			return;
		}
		SaveGame.Instance.ColonyAchievementTracker.GeothermalFacilityDiscovered = true;
		GeothermalPlantComponent.DisplayPopup(COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.POPUPS.GEOTHERMAL_DISCOVERED_TITLE, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.POPUPS.GEOTHERMAL_DISOCVERED_DESC, "geothermalplantintro_kanim", new global::System.Action(this.RevealAllVentsAndController), null);
	}

	public static void OnVentingHotMaterial(int worldid)
	{
		foreach (GeothermalVent geothermalVent in Components.GeothermalVents.GetItems(worldid))
		{
			if (geothermalVent.IsQuestEntombed())
			{
				geothermalVent.SetQuestComplete();
				if (!SaveGame.Instance.ColonyAchievementTracker.GeothermalClearedEntombedVent)
				{
					GeothermalVictorySequence.VictoryVent = geothermalVent;
					GeothermalPlantComponent.DisplayPopup(COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.POPUPS.GEOPLANT_ERRUPTED_TITLE, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.POPUPS.GEOPLANT_ERRUPTED_DESC, "geothermalplantachievement_kanim", delegate
					{
						SaveGame.Instance.ColonyAchievementTracker.GeothermalClearedEntombedVent = true;
					}, null);
					break;
				}
			}
		}
	}

	public List<KSelectable> GetRelatedEntities()
	{
		List<KSelectable> list = new List<KSelectable>();
		int myWorldId = this.GetMyWorldId();
		foreach (GeothermalController geothermalController in Components.GeothermalControllers.GetItems(myWorldId))
		{
			list.Add(geothermalController.GetComponent<KSelectable>());
		}
		foreach (GeothermalVent geothermalVent in Components.GeothermalVents.GetItems(myWorldId))
		{
			list.Add(geothermalVent.GetComponent<KSelectable>());
		}
		return list;
	}

	public const string POPUP_DISCOVERED_KANIM = "geothermalplantintro_kanim";

	public const string POPUP_PROGRESS_KANIM = "geothermalplantonline_kanim";

	public const string POPUP_COMPLETE_KANIM = "geothermalplantachievement_kanim";
}
