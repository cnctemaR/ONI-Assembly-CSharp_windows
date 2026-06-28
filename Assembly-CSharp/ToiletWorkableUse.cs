using System;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(Workable.DefaultWorkAnims, KAnim.PlayMode.Loop);
		Room roomOfBuilding = Game.Instance.roomProber.GetRoomOfBuilding(base.gameObject);
		if (roomOfBuilding != null)
		{
			RoomType roomType = Db.Get().RoomTypes.GetRoomType(roomOfBuilding);
			if (roomType.category == Db.Get().RoomTypeCategories.Bathroom)
			{
				worker.GetComponent<Effects>().Add("ProperBathroom", true);
			}
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.onStop.Signal(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		amountInstance.SetValue(0f);
		this.timesUsed++;
		base.OnCompleteWork(worker);
	}

	public Action<Worker> onStop;

	[Serialize]
	public int timesUsed;
}
