using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Butcherable")]
public class Butcherable : Workable, ISaveLoadable
{
	public void SetDrops(string[] drops)
	{
		this.drops = drops;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Butcherable>(1272413801, Butcherable.SetReadyToButcherDelegate);
		base.Subscribe<Butcherable>(493375141, Butcherable.OnRefreshUserMenuDelegate);
		this.workTime = 3f;
		this.multitoolContext = "harvest";
		this.multitoolHitEffectTag = "fx_harvest_splash";
	}

	public void SetReadyToButcher(object param)
	{
		this.readyToButcher = true;
	}

	public void SetReadyToButcher(bool ready)
	{
		this.readyToButcher = ready;
	}

	public void ActivateChore(object param)
	{
		if (this.chore != null)
		{
			return;
		}
		this.chore = new WorkChore<Butcherable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		this.OnRefreshUserMenu(null);
	}

	public void CancelChore(object param)
	{
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel("User cancelled");
		this.chore = null;
	}

	private void OnClickCancel()
	{
		this.CancelChore(null);
	}

	private void OnClickButcher()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnButcherComplete();
			return;
		}
		this.ActivateChore(null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.readyToButcher)
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((this.chore != null) ? new KIconButtonMenu.ButtonInfo("action_harvest", "Cancel Meatify", new global::System.Action(this.OnClickCancel), global::Action.NumActions, null, null, null, "", true) : new KIconButtonMenu.ButtonInfo("action_harvest", "Meatify", new global::System.Action(this.OnClickButcher), global::Action.NumActions, null, null, null, "", true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.OnButcherComplete();
	}

	public void OnButcherComplete()
	{
		if (this.butchered)
		{
			return;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (component && component.IsSelected)
		{
			SelectTool.Instance.Select(null, false);
		}
		for (int i = 0; i < this.drops.Length; i++)
		{
			GameObject gameObject = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, this.drops[i], Grid.SceneLayer.Ore);
			gameObject.SetActive(true);
			Edible component2 = gameObject.GetComponent<Edible>();
			if (component2)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.BUTCHERED, "{0}", gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.BUTCHERED_CONTEXT);
			}
		}
		this.chore = null;
		this.butchered = true;
		this.readyToButcher = false;
		Game.Instance.userMenu.Refresh(base.gameObject);
		base.Trigger(395373363, null);
	}

	private int GetDropSpawnLocation()
	{
		int num = Grid.PosToCell(base.gameObject);
		int num2 = Grid.CellAbove(num);
		if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
		{
			return num2;
		}
		return num;
	}

	[MyCmpGet]
	private KAnimControllerBase controller;

	[MyCmpGet]
	private Harvestable harvestable;

	private bool readyToButcher;

	private bool butchered;

	public string[] drops;

	private Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Butcherable> SetReadyToButcherDelegate = new EventSystem.IntraObjectHandler<Butcherable>(delegate(Butcherable component, object data)
	{
		component.SetReadyToButcher(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Butcherable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Butcherable>(delegate(Butcherable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
