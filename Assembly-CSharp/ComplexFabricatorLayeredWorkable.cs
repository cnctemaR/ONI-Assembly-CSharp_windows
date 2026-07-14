using System;

public class ComplexFabricatorLayeredWorkable : ComplexFabricatorWorkable
{
	public override Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		if (this.foregroundLayer != Grid.SceneLayer.NoLayer)
		{
			anim.smi = new SimpleLayeredAnimWork.Instance(base.gameObject, worker, this.foregroundLayer, this.synchronizeAnims, anim.overrideAnims);
		}
		return anim;
	}

	public Grid.SceneLayer foregroundLayer = Grid.SceneLayer.NoLayer;
}
