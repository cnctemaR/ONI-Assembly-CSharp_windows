using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EquippableWorkable : Workable, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_clothing_kanim") };
		this.forcePlayPst = true;
	}

	public QualityLevel GetQuality()
	{
		return this.quality;
	}

	public void SetQuality(QualityLevel level)
	{
		this.quality = level;
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(1.5f);
		this.equippable.OnAssign += this.RefreshChore;
	}

	private void CreateChore()
	{
		this.chore = new WorkChore<EquippableWorkable>(Db.Get().ChoreTypes.Equip, this, this.equippable.assignee.GetSoleOwner().GetComponent<ChoreProvider>(), null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
	}

	public void CancelChore()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Manual equip");
			this.chore = null;
		}
	}

	private void RefreshChore(IAssignableIdentity target)
	{
		if (this.chore != null)
		{
			this.chore.Cancel("Equipment Reassigned");
			this.chore = null;
		}
		if (target != null && !target.GetSoleOwner().GetComponent<Equipment>().IsEquipped(this.equippable))
		{
			this.CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (this.equippable.assignee != null)
		{
			this.equippable.assignee.GetSoleOwner().GetComponent<Equipment>().Equip(this.equippable);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		this.workTimeRemaining = this.GetWorkTime();
		base.OnStopWork(worker);
	}

	[MyCmpReq]
	private Equippable equippable;

	private Chore chore;

	private QualityLevel quality;
}
