using System;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<KAnimControllerBase>().Play(Workable.DefaultWorkAnims, KAnim.PlayMode.Loop);
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			RoomType roomType = roomOfGameObject.roomType;
			roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		amountInstance.SetValue(0f);
		this.timesUsed++;
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesUsed;
}
