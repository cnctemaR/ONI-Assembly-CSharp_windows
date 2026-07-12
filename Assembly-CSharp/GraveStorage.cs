using System;
using System.Collections.Generic;

public class GraveStorage : Storage
{
	public override Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		KAnimFile[] array = null;
		if (this.workerTypeOverrideAnims.TryGetValue(worker.PrefabID(), out array))
		{
			this.overrideAnims = array;
		}
		return base.GetAnim(worker);
	}

	public Dictionary<Tag, KAnimFile[]> workerTypeOverrideAnims = new Dictionary<Tag, KAnimFile[]>();
}
