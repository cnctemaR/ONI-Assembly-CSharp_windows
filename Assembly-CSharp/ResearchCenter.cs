using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchCenter : Fabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Research;
		this.inStorage.choreType = Db.Get().ChoreTypes.FabricateFetch;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Researching;
		this.attributeConverter = Db.Get().AttributeConverters.ResearchPoints;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.Subscribe(-1503271301, new EventSystem.EventHandler(this.OnSelectObject));
		Research.Instance.Subscribe(-1914338957, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		Research.Instance.Subscribe(-125623018, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		this.Subscribe(187661686, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		this.CheckValidResearchSelected(null);
		Components.ResearchCenters.Add(this);
		this.CheckValidResearchSelected(null);
	}

	private void CheckValidResearchSelected(object data)
	{
		bool flag = false;
		bool flag2 = false;
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null)
		{
			flag = true;
			string text = string.Empty;
			foreach (Recipe recipe in base.GetRecipes())
			{
				ResearchPointObject component = recipe.Result.GetComponent<ResearchPointObject>();
				if (component != null)
				{
					if (text != string.Empty)
					{
						Debug.LogError("Research station " + base.gameObject.name + " has more than one research point recipe in fabricator recipes. It should be limited to one type at this time.");
					}
					text = component.TypeID;
				}
			}
			if (activeResearch.tech.costsByResearchTypeID.ContainsKey(text) && Research.Instance.Get(activeResearch.tech).progressInventory.PointsByTypeID[text] < activeResearch.tech.costsByResearchTypeID[text])
			{
				flag2 = true;
			}
		}
		if (this.operational.GetFlag(EnergyConsumer.PoweredFlag))
		{
			if (flag)
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
				if (!flag2)
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected, null);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
				}
			}
			else
			{
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, null);
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
			}
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
		}
		this.operational.SetFlag(ResearchCenter.ResearchSelectedFlag, flag && flag2);
		if ((!flag || !flag2) && base.worker)
		{
			base.StopWork(base.worker);
		}
	}

	protected override void CompleteOrder(Fabricator.UserOrder completed_order)
	{
		base.CompleteOrder(completed_order);
		this.CheckValidResearchSelected(null);
	}

	private void ClearResearchScreen()
	{
		if (this.researchScreen != null)
		{
			this.researchScreen.Deactivate();
			this.researchScreen = null;
		}
	}

	private void OnSelectResearchClick()
	{
		DetailsScreen.Instance.Show(false);
		if (this.researchScreen == null)
		{
			ManagementMenu.Instance.ToggleResearch();
		}
		else
		{
			this.ClearResearchScreen();
		}
	}

	private void OnSelectObject(object data)
	{
		this.ClearResearchScreen();
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.SELECTRESEARCH.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_select_research", UI.USERMENUACTIONS.SELECTRESEARCH.NAME, new global::System.Action(this.OnSelectResearchClick), global::Action.NumActions, null, null, null, null, text));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		Research.Instance.Unsubscribe(-125623018, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		this.Unsubscribe(-1852328367, new EventSystem.EventHandler(this.CheckValidResearchSelected));
		Components.ResearchCenters.Remove(this);
		this.ClearResearchScreen();
	}

	public string GetStatusString()
	{
		string text = Strings.Get("STRINGS.RESEARCH.MESSAGING.NORESEARCHSELECTED");
		if (Research.Instance.GetActiveResearch() != null)
		{
			text = "<b>" + Research.Instance.GetActiveResearch().tech.Name + "</b>";
			int num = 0;
			foreach (KeyValuePair<string, float> keyValuePair in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair.Key] != 0f)
				{
					num++;
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair2 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair2.Key] != 0f)
				{
					bool flag = false;
					foreach (Recipe recipe in base.GetRecipes())
					{
						if (recipe.Result.GetComponent<ResearchPointObject>().TypeID == keyValuePair2.Key)
						{
							flag = true;
						}
					}
					if (flag)
					{
						text = text + "\n   - " + Research.Instance.researchTypes.GetResearchType(keyValuePair2.Key).name;
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							": ",
							keyValuePair2.Value,
							"/",
							Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair2.Key]
						});
					}
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair3 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair3.Key] != 0f)
				{
					bool flag2 = false;
					foreach (Recipe recipe2 in base.GetRecipes())
					{
						if (recipe2.Result.GetComponent<ResearchPointObject>().TypeID == keyValuePair3.Key)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						if (num > 1)
						{
							text = text + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEALSOREQUIRED, Research.Instance.researchTypes.GetResearchType(keyValuePair3.Key).name);
						}
						else
						{
							text = text + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEREQUIRED, Research.Instance.researchTypes.GetResearchType(keyValuePair3.Key).name);
						}
					}
				}
			}
		}
		return text;
	}

	private ResearchScreen researchScreen;

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpAdd]
	private Notifier notifier;

	public static Operational.Flag ResearchSelectedFlag = new Operational.Flag("researchSelected", Operational.Flag.Type.Requirement);
}
