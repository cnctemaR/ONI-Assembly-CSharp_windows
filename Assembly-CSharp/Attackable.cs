using System;
using UnityEngine;

public class Attackable : Workable
{
	public bool Slaughtered
	{
		get
		{
			return this.slaughtered;
		}
		protected set
		{
			this.slaughtered = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Attackables.Add(this);
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.workTime = 4f;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_break_kanim") };
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.SlaughterLocation = this.transform.position;
	}

	private void Update()
	{
		if (!this.slaughtered)
		{
			this.CheckDead();
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		this.transform.SetPosition(Vector3.Lerp(this.transform.position, this.SlaughterLocation, dt * 20f));
		return base.OnWorkTick(worker, dt);
	}

	public void ActivateChore()
	{
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		if (this.chore != null)
		{
			return;
		}
		this.chore = new WorkChore<Attackable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
	}

	public void DeactivateChore(string reason)
	{
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel(reason);
		this.chore = null;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.Slaughtered = true;
		this.health.Kill(Db.Get().Deaths.Generic);
		this.chore = null;
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		if (this.OnSlaughtered != null)
		{
			this.OnSlaughtered();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Attackables.Remove(this);
	}

	private void CheckDead()
	{
		if (this.health.IsDead() && !this.slaughtered)
		{
			this.Slaughtered = true;
			this.DeactivateChore("Creature died");
			if (this.OnSlaughtered != null)
			{
				this.OnSlaughtered();
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		if (!this.slaughtered && this.chore == null)
		{
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_deconstruct", "Attack", new global::System.Action(this.ActivateChore), global::Action.NumActions, null, null, null, string.Empty, true);
		}
		else if (this.chore != null)
		{
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_deconstruct", "Cancel Attack", delegate
			{
				this.DeactivateChore("Attack cancelled");
			}, global::Action.NumActions, null, null, null, string.Empty, true);
		}
		if (buttonInfo != null)
		{
			this.userMenu.AddButton(buttonInfo, 1f);
		}
	}

	[MyCmpAdd]
	private Health health;

	[MyCmpReq]
	private UserMenu userMenu;

	private Vector3 SlaughterLocation;

	private Chore chore;

	private bool slaughtered;

	public Attackable.onSlaughtered OnSlaughtered;

	public delegate void onSlaughtered();
}
