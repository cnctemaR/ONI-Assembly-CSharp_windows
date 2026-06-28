using System;
using KSerialization;

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
		base.SetWorkTime(10f);
		this.shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.diseasePerSecond = (float)base.GetComponent<PrimaryElement>().DiseaseCount / 10f;
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
			base.SetWorkTime(10f);
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
	}

	private void OnCancel(object data)
	{
		this.CancelDisinfection();
	}

	private const float MAX_WORK_TIME = 10f;

	[MyCmpGet]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private bool isMarkedForDisinfect;

	private float diseasePerSecond;
}
