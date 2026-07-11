using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightSymbolTracker")]
public class LightSymbolTracker : KMonoBehaviour, IRenderEveryTick
{
	public void RenderEveryTick(float dt)
	{
		Vector3 vector = Vector3.zero;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		bool flag;
		vector = (component.GetTransformMatrix() * component.GetSymbolLocalTransform(this.targetSymbol, out flag)).MultiplyPoint(Vector3.zero) - base.transform.GetPosition();
		base.GetComponent<Light2D>().Offset = vector;
	}

	public HashedString targetSymbol;
}
