using System;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	private ToiletWorkableUse()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

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
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			roomOfGameObject.roomType.TriggerRoomEffects(base.GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Db.Get().Amounts.Bladder.Lookup(worker).SetValue(0f);
		this.timesUsed++;
		base.Trigger(-350347868, worker);
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesUsed;
}
