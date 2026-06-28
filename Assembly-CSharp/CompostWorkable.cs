using System;

public class CompostWorkable : BuildingWorkable
{
	protected override void OnStartWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	protected override void OnStopWork(Worker worker)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
	}
}
