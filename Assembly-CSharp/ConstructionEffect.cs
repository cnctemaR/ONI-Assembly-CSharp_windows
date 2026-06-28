using System;

public class ConstructionEffect : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.GetBatchInstanceData().SetClipParams(this.ClipDepth, true);
		}
	}

	public float ClipDepth;
}
