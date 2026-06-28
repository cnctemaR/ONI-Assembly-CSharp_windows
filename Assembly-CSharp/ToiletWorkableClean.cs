using System;

public class ToiletWorkableClean : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		this.WorkAnims = ToiletWorkableClean.CleanAnims;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play(this.WorkAnims, KAnim.PlayMode.Loop);
	}

	protected override void OnStopWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Queue("unclog_pst", KAnim.PlayMode.Once, 1f, 0f);
		base.OnStopWork(worker);
	}

	private static readonly string[] CleanAnims = new string[] { "unclog_pre", "unclog_loop" };
}
