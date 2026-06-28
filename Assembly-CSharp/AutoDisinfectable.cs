using System;
using KSerialization;
using STRINGS;

public class AutoDisinfectable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
		this.resetProgressOnStop = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.SetWorkTime(10f);
		this.shouldTransferDiseaseWithWorker = false;
	}

	private void Update()
	{
		if (KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		this.RefreshChore();
	}

	private void RefreshChore()
	{
		if (!this.enableAutoDisinfect || !SaveGame.Instance.enableAutoDisinfect)
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Autodisinfect Disabled");
				this.chore = null;
			}
		}
		else if (this.chore == null || !(this.chore.driver != null))
		{
			if (this.chore == null && this.primaryElement.DiseaseCount > SaveGame.Instance.minGermCountForDisinfect)
			{
				this.chore = new WorkChore<AutoDisinfectable>(Db.Get().ChoreTypes.Disinfect, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true, int.MaxValue);
			}
			else if (this.primaryElement.DiseaseCount < SaveGame.Instance.minGermCountForDisinfect && this.chore != null)
			{
				this.chore.Cancel("AutoDisinfectable.Update");
				this.chore = null;
			}
		}
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
		this.chore = null;
		this.userMenu.Refresh();
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		return new Workable.AnimInfo
		{
			smi = new MultitoolController.Instance(this, worker, "disinfect", EffectPrefabs.Instance.DisinfectEffect)
		};
	}

	private void EnableAutoDisinfect()
	{
		this.enableAutoDisinfect = true;
		this.RefreshChore();
	}

	private void DisableAutoDisinfect()
	{
		this.enableAutoDisinfect = false;
		this.RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.enableAutoDisinfect)
		{
			UserMenu userMenu = this.userMenu;
			string text = BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_disinfect", BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.NAME, new global::System.Action(this.EnableAutoDisinfect), global::Action.NumActions, null, null, null, text, true), 10f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.TOOLTIP;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_disinfect", BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.NAME, new global::System.Action(this.DisableAutoDisinfect), global::Action.NumActions, null, null, null, text, true), 10f);
		}
	}

	private const float MAX_WORK_TIME = 10f;

	[MyCmpGet]
	private UserMenu userMenu;

	private Chore chore;

	private float diseasePerSecond;

	[MyCmpGet]
	private PrimaryElement primaryElement;

	[Serialize]
	private bool enableAutoDisinfect = true;
}
