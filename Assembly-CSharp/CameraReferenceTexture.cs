using System;
using UnityEngine;

public class CameraReferenceTexture : MonoBehaviour
{
	private void OnPreCull()
	{
		if (this.quad == null)
		{
			this.quad = new FullScreenQuad("CameraReferenceTexture", base.GetComponent<Camera>(), false);
		}
		if (this.referenceCamera != null)
		{
			this.quad.Draw(this.referenceCamera.GetComponent<CameraRenderTexture>().resultTexture);
		}
	}

	public Camera referenceCamera;

	private FullScreenQuad quad;
}
