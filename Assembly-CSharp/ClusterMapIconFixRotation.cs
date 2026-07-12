using System;

public class ClusterMapIconFixRotation : KMonoBehaviour
{
	private void Update()
	{
		if (base.transform.parent != null)
		{
			float z = base.transform.parent.rotation.eulerAngles.z;
			this.rotation = -z;
			this.animController.Rotation = this.rotation;
		}
	}

	[MyCmpGet]
	private KBatchedAnimController animController;

	private float rotation;
}
