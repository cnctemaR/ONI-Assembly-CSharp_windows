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
		Camera camera = base.GetComponent<Camera>();
		GameObject new_camera_go = new GameObject();
		Camera new_camera = new_camera_go.AddComponent<Camera>();
		new_camera.CopyFrom(camera);
		this.renderProxy = new_camera.gameObject.AddComponent<MultipleRenderTargetProxy>();
		new_camera.name = camera.name + " MRT";
		new_camera.transform.parent = camera.transform;
		new_camera.transform.localPosition = Vector3.zero;
		new_camera.depth = camera.depth - 1f;
		camera.cullingMask = 0;
		camera.clearFlags = CameraClearFlags.Color;
		this.quad = new FullScreenQuad("MultipleRenderTarget", camera, true);
		if (this.onSetupComplete != null)
		{
			this.onSetupComplete(new_camera);
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
		this.renderProxy.ToggleColouredOverlayView(enabled);
	}

	private MultipleRenderTargetProxy renderProxy;

	private FullScreenQuad quad;

	public bool isFrontEnd;
}
