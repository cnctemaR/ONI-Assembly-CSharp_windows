using System;
using System.Collections;
using UnityEngine;

public class FrontEndBackground : UIDupeRandomizer
{
	protected override void Start()
	{
		this.SetupCameras();
		base.Start();
		for (int i = 0; i < this.anims.Length; i++)
		{
			int minionIndex = i;
			KBatchedAnimController minon = this.anims[i].minon;
			minon.onAnimComplete += delegate(HashedString name)
			{
				this.WaitForABit(minionIndex, name);
			};
			this.WaitForABit(i, HashedString.Invalid);
		}
	}

	private void WaitForABit(int minonIdx, HashedString name)
	{
		Time.timeScale = 1f;
		base.StartCoroutine(this.WaitForTime(minonIdx));
	}

	private IEnumerator WaitForTime(int minonIdx)
	{
		this.anims[minonIdx].lastWaitTime = global::UnityEngine.Random.Range(this.anims[minonIdx].minSecondsBetweenAction, this.anims[minonIdx].maxSecondsBetweenAction);
		yield return new WaitForSeconds(this.anims[minonIdx].lastWaitTime);
		base.GetNewBody(minonIdx);
		this.anims[minonIdx].minon.ClearQueue();
		this.anims[minonIdx].minon.Play(this.anims[minonIdx].anim_name, KAnim.PlayMode.Once, 1f, 0f);
		yield break;
	}

	private void SetupCameras()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Cameras";
		gameObject.transform.parent = base.transform.parent;
		Util.Reset(gameObject.transform);
		this.baseCamera = base.GetComponentInChildren<Camera>();
		this.baseCamera.name = "BaseCamera";
		this.baseCamera.transform.SetParent(gameObject.transform);
		this.waterCamera = CameraController.CloneCamera(this.baseCamera, "WaterCamera");
		this.waterCamera.transform.SetParent(gameObject.transform);
		this.overlayCamera = Camera.main;
		this.overlayCamera.name = "OverlayCamera";
		this.overlayCamera.transform.SetParent(gameObject.transform);
		this.overlayNoDepthCamera = CameraController.CloneCamera(this.baseCamera, "OverlayNoDepthCamera");
		this.overlayNoDepthCamera.transform.SetParent(gameObject.transform);
		this.baseCamera.gameObject.AddComponent<MultipleRenderTarget>();
		this.baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		int mask = LayerMask.GetMask(new string[] { "PlaceWithDepth", "Overlay" });
		this.baseCamera.cullingMask &= ~mask;
		this.baseCamera.tag = "Untagged";
		this.baseCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_LitTex";
		this.waterCamera.cullingMask = 0;
		this.waterCamera.clearFlags = CameraClearFlags.Depth;
		this.waterCamera.depth = this.baseCamera.depth + 2f;
		CameraReferenceTexture cameraReferenceTexture = this.waterCamera.gameObject.AddComponent<CameraReferenceTexture>();
		cameraReferenceTexture.referenceCamera = this.baseCamera;
		this.waterCamera.tag = "Untagged";
		this.overlayCamera.cullingMask = mask;
		this.overlayCamera.clearFlags = CameraClearFlags.Nothing;
		this.overlayCamera.depth = this.baseCamera.depth + 3f;
		this.overlayCamera.transform.localPosition = Vector3.zero;
		this.overlayCamera.transform.localRotation = Quaternion.identity;
		this.overlayCamera.renderingPath = RenderingPath.Forward;
		this.overlayCamera.hdr = false;
		this.overlayCamera.tag = "Untagged";
		int mask2 = LayerMask.GetMask(new string[] { "Overlay", "Place" });
		this.baseCamera.cullingMask &= ~mask2;
		this.overlayNoDepthCamera.clearFlags = CameraClearFlags.Depth;
		this.overlayNoDepthCamera.cullingMask = mask2;
		this.overlayNoDepthCamera.transform.localPosition = Vector3.zero;
		this.overlayNoDepthCamera.depth = this.baseCamera.depth + 4f;
		this.overlayNoDepthCamera.tag = "MainCamera";
	}

	[NonSerialized]
	public Camera baseCamera;

	[NonSerialized]
	public Camera waterCamera;

	[NonSerialized]
	public Camera overlayCamera;

	[NonSerialized]
	public Camera overlayNoDepthCamera;
}
