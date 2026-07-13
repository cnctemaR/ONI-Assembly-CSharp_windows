using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightSymbolTracker")]
public class LightSymbolTracker : KMonoBehaviour, IRenderEveryTick
{
	protected override void OnSpawn()
	{
		this.animController = base.GetComponent<KBatchedAnimController>();
		this.light2D = base.GetComponent<Light2D>();
		this.pickupable = base.GetComponent<Pickupable>();
	}

	public bool IsEnableAndVisible()
	{
		return CameraController.Instance.VisibleArea.CurrentAreaExtended.Contains(this.pickupable.cachedCell) && base.enabled;
	}

	public void RenderEveryTick(float dt)
	{
		if (!this.IsEnableAndVisible())
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		bool flag;
		vector = (this.animController.GetTransformMatrix() * this.animController.GetSymbolLocalTransform(this.targetSymbol, out flag)).MultiplyPoint(Vector3.zero) - base.transform.position;
		this.light2D.Offset = vector;
	}

	public HashedString targetSymbol;

	private KBatchedAnimController animController;

	private Light2D light2D;

	private Pickupable pickupable;
}
