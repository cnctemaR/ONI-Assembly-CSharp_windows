using System;
using Klei.AI;

public class ToiletWorkableUse : BuildingWorkable
{
	protected override void OnSpawn()
	{
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(this.attributeConverter.multiplier * 8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(Workable.DefaultWorkAnims, KAnim.PlayMode.Loop);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.onComplete.Signal(worker);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		amountInstance.SetValue(0f);
		base.OnCompleteWork(worker);
	}

	public Action<Worker> onComplete;

	public Action<Worker> onAbort;
}
