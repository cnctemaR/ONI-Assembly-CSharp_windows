using System;
using System.Collections;
using UnityEngine;

public class MultipleRenderTarget : MonoBehaviour
{
	public event Action<Camera> onSetupComplete;

	private void Start()
	{
		base.StartCoroutine(this.SetupProxy());
	}

	private IEnumerator SetupProxy()
	{
		yield return null;
		Camera component = base.GetComponent<Camera>();
		Camera camera = new GameObject().AddComponent<Camera>();
		camera.CopyFrom(component);
		this.renderProxy = camera.gameObject.AddComponent<MultipleRenderTargetProxy>();
		camera.name = component.name + " MRT";
		camera.transform.parent = component.transform;
		camera.transform.SetLocalPosition(Vector3.zero);
		camera.depth = component.depth - 1f;
		component.cullingMask = 0;
		component.clearFlags = CameraClearFlags.Color;
		this.quad = new FullScreenQuad("MultipleRenderTarget", component, true);
		if (this.onSetupComplete != null)
		{
			this.onSetupComplete(camera);
		}
		yield break;
	}

	private void OnPreCull()
	{
		if (this.renderProxy != null)
		{
			this.quad.Draw(this.renderProxy.Textures[0]);
		}
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		if (this.renderProxy != null)
		{
			this.renderProxy.ToggleColouredOverlayView(enabled);
		}
	}

	private MultipleRenderTargetProxy renderProxy;

	private FullScreenQuad quad;

	public bool isFrontEnd;
}
