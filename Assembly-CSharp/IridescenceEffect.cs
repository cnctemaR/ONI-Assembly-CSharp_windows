using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/IridescenceEffect")]
public class IridescenceEffect : KMonoBehaviour
{
	private void Update()
	{
		if (World.Instance == null)
		{
			return;
		}
		GroundRenderer groundRenderer = World.Instance.groundRenderer;
		if (groundRenderer != null)
		{
			Vector3 position = Camera.main.transform.position;
			float zoomFactor = CameraController.Instance.zoomFactor;
			float num = (Mathf.Cos(position.x / zoomFactor) + Mathf.Sin(position.y / zoomFactor)) / 4f + 0.5f;
			this.UpdatePearl(groundRenderer, num);
		}
	}

	private void UpdatePearl(GroundRenderer renderer, float t)
	{
		Color color = Color.Lerp(this.pearl1, this.pearl2, t);
		Color color2 = Color.Lerp(this.pearl2, this.pearl1, t);
		renderer.SetShineColors(SimHashes.Pearl, color2, color);
	}

	public Color pearl1;

	public Color pearl2;
}
