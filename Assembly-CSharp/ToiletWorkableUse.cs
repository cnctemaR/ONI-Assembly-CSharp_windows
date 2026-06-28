using System;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Ownable, IGameObjectEffectDescriptor
{
	private ToiletWorkableUse()
	{
		this.showProgressBar = true;
		base.slot = Db.Get().OwnableSlots.Toilet;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(Workable.DefaultWorkAnims, KAnim.PlayMode.Loop);
		Room roomOfBuilding = Game.Instance.roomProber.GetRoomOfBuilding(base.GetComponent<BuildingComplete>());
		if (roomOfBuilding != null)
		{
			string id = RoomTypes.GetRoomType(roomOfBuilding).id;
			if (id == "Latrine" || id == "PrivateBathroom")
			{
				worker.GetComponent<Effects>().Add("ProperBathroom", true);
			}
		}
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
