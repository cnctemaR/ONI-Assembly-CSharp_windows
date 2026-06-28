using System;
using KSerialization;
using STRINGS;

public class Disinfectable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		this.Subscribe(2127324410, new Action<object>(this.OnCancel));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		if (this.isMarkedForDisinfect)
		{
			this.MarkForDisinfect(true);
		}
		base.SetWorkTime(35f);
		this.shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.diseasePerSecond = (float)base.GetComponent<PrimaryElement>().DiseaseCount / 35f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -(int)(this.diseasePerSecond * dt + 0.5f), "Disinfectable.OnWorkTick");
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -component.DiseaseCount, "Disinfectable.OnCompleteWork");
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		this.isMarkedForDisinfect = false;
		this.chore = null;
		this.userMenu.Refresh();
		Prioritizable.RemoveRef(base.gameObject);
	}

	private void ToggleMarkForDisinfect()
	{
		if (this.isMarkedForDisinfect)
		{
			this.CancelDisinfection();
		}
		else
		{
			base.SetWorkTime(35f);
			this.MarkForDisinfect(false);
		}
	}

	private void CancelDisinfection()
	{
		if (this.isMarkedForDisinfect)
		{
			Prioritizable.RemoveRef(base.gameObject);
			base.ShowProgressBar(false);
			this.isMarkedForDisinfect = false;
			this.chore.Cancel("disinfection cancelled");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		}
	}

	public void MarkForDisinfect(bool force = false)
	{
		if (!this.isMarkedForDisinfect || force)
		{
			this.isMarkedForDisinfect = true;
			Prioritizable.AddRef(base.gameObject);
			this.chore = new WorkChore<Disinfectable>(Db.Get().ChoreTypes.Disinfect, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		return new Workable.AnimInfo
		{
			smi = new MultitoolController.Instance(this, worker, "disinfect", EffectPrefabs.Instance.DisinfectEffect)
		};
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.isMarkedForDisinfect)
		{
			bool flag = base.GetComponent<PrimaryElement>().DiseaseIdx != byte.MaxValue;
			string text = BUILDINGS.DISINFECTABLE.ENABLE_DISINFECT.TOOLTIP;
			if (!flag)
			{
				text = BUILDINGS.DISINFECTABLE.NO_DISEASE.TOOLTIP;
			}
			UserMenu userMenu = this.userMenu;
			string text2 = text;
			bool flag2 = flag;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_disinfect", BUILDINGS.DISINFECTABLE.ENABLE_DISINFECT.NAME, new global::System.Action(this.ToggleMarkForDisinfect), global::Action.NumActions, null, null, null, text2, flag2), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text2 = BUILDINGS.DISINFECTABLE.DISABLE_DISINFECT.TOOLTIP;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_disinfect", BUILDINGS.DISINFECTABLE.DISABLE_DISINFECT.NAME, new global::System.Action(this.ToggleMarkForDisinfect), global::Action.NumActions, null, null, null, text2, true), 1f);
		}
	}

	private void OnCancel(object data)
	{
		this.CancelDisinfection();
	}

	private const float MAX_WORK_TIME = 35f;

	[MyCmpGet]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private bool isMarkedForDisinfect;

	private float diseasePerSecond;
}
