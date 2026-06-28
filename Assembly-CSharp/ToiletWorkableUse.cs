using System;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
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
		this.timesUsed++;
		base.OnCompleteWork(worker);
	}

	public Action<Worker> onComplete;

	public Action<Worker> onAbort;

	[Serialize]
	public int timesUsed;
}
