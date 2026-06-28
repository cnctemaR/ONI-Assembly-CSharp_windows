using System;

public class ToiletWorkableClean : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(ToiletWorkableClean.CleanAnims, KAnim.PlayMode.Loop);
	}

	protected override void OnStopWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Queue("unclog_pst", KAnim.PlayMode.Once, 1f, 0f);
		base.OnStopWork(worker);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		return ToiletWorkableClean.CleanAnims;
	}

	private static readonly HashedString[] CleanAnims = new HashedString[] { "unclog_pre", "unclog_loop" };
}
