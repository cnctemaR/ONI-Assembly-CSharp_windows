using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VirtualCursorOverlayFix : MonoBehaviour
{
	private void Awake()
	{
		int width = Screen.currentResolution.width;
		int height = Screen.currentResolution.height;
		this.cursorRendTex = new RenderTexture(width, height, 0);
		this.screenSpaceCamera.enabled = true;
		this.screenSpaceCamera.targetTexture = this.cursorRendTex;
		this.screenSpaceOverlayImage.material.SetTexture("_MainTex", this.cursorRendTex);
		base.StartCoroutine(this.RenderVirtualCursor());
	}

	private IEnumerator RenderVirtualCursor()
	{
		bool ShowCursor = KInputManager.currentControllerIsGamepad;
		while (Application.isPlaying)
		{
			ShowCursor = KInputManager.currentControllerIsGamepad;
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.C))
			{
				ShowCursor = true;
			}
			this.screenSpaceCamera.enabled = true;
			if (!this.screenSpaceOverlayImage.enabled && ShowCursor)
			{
				yield return SequenceUtil.WaitForSecondsRealtime(0.1f);
			}
			this.actualCursor.enabled = ShowCursor;
			this.screenSpaceOverlayImage.enabled = ShowCursor;
			this.screenSpaceOverlayImage.material.SetTexture("_MainTex", this.cursorRendTex);
			yield return null;
		}
		yield break;
	}

	private RenderTexture cursorRendTex;

	public Camera screenSpaceCamera;

	public Image screenSpaceOverlayImage;

	public RawImage actualCursor;
}
