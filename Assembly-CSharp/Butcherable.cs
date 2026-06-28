using System;
using STRINGS;
using UnityEngine;

public class Butcherable : Workable, ISaveLoadable
{
	public void SetDrops(string[] drops)
	{
		this.Drops = drops;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(1272413801, new Action<object>(this.SetReadyToButcher));
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.workTime = 3f;
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
		this.chore = new WorkChore<Butcherable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, int.MaxValue);
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
		}
		else
		{
			this.ActivateChore(null);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.readyToButcher)
		{
			return;
		}
		if (this.chore != null)
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_harvest", "Cancel Meatify", new global::System.Action(this.OnClickCancel), global::Action.NumActions, null, null, null, string.Empty, true), 1f);
		}
		else
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_harvest", "Meatify", new global::System.Action(this.OnClickButcher), global::Action.NumActions, null, null, null, string.Empty, true), 1f);
		}
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
		for (int i = 0; i < this.Drops.Length; i++)
		{
			GameObject gameObject = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, this.Drops[i], Grid.SceneLayer.Use, Folder.Entities);
			gameObject.SetActive(true);
			Edible component2 = gameObject.GetComponent<Edible>();
			if (component2)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, string.Format(UI.ENDOFDAYREPORT.NOTES.BUTCHERED, gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.BUTCHERED_CONTEXT);
			}
		}
		this.chore = null;
		this.butchered = true;
		this.readyToButcher = false;
		this.userMenu.Refresh();
		this.Trigger(395373363, null);
	}

	private int GetDropSpawnLocation()
	{
		int num = Grid.PosToCell(base.gameObject);
		int num2 = Grid.CellAbove(num);
		if (!Grid.Solid[num2])
		{
			return num2;
		}
		return num;
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "harvest", EffectPrefabs.Instance.HarvestEffect);
		return anim;
	}

	[MyCmpGet]
	private KAnimControllerBase controller;

	[MyCmpGet]
	private Harvestable harvestable;

	[MyCmpAdd]
	private UserMenu userMenu;

	private bool readyToButcher;

	private bool butchered;

	public string[] Drops;

	private Chore chore;
}
