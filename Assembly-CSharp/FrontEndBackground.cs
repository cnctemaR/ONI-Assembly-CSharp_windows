using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class FrontEndBackground : MonoBehaviour
{
	private void Start()
	{
		this.SetupCameras();
		this.slots = new AccessorySlots(null, this.head_default_anim, this.head_swap_anim, this.body_swap_anim);
		for (int i = 0; i < this.anims.Length; i++)
		{
			int minionIndex = i;
			KBatchedAnimController minon = this.anims[i].minon;
			this.anims[i].curBody = null;
			this.anims[i].overrideSet = false;
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
		this.GetNewBody(minonIdx, base.name);
		this.anims[minonIdx].minon.ClearQueue();
		this.anims[minonIdx].minon.Play(this.anims[minonIdx].anim_name, KAnim.PlayMode.Once, 1f, 0f);
		yield break;
	}

	private void GetNewBody(int minonIdx, HashedString name)
	{
		this.Apply(this.anims[minonIdx].minon, ref this.anims[minonIdx]);
	}

	private void Apply(KBatchedAnimController minon, ref FrontEndBackground.AnimChoice anim)
	{
		if (anim.curHair.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHair);
		}
		anim.curHair = this.AddRandomAccessory(minon, this.slots.Hair.accessories);
		if (anim.curEyes.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curEyes);
		}
		anim.curEyes = this.AddRandomAccessory(minon, this.slots.Eyes.accessories);
		if (anim.curHeadShape.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHeadShape);
		}
		anim.curHeadShape = this.AddRandomAccessory(minon, this.slots.HeadShape.accessories);
		if (anim.curMouth.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curMouth);
		}
		anim.curMouth = this.AddRandomAccessory(minon, this.slots.Mouth.accessories);
		if (anim.curTorso.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curTorso);
			minon.RemoveSymbolOverride(anim.curArm);
		}
		int num = global::UnityEngine.Random.Range(1, this.slots.Body.accessories.Count);
		anim.curTorso = FrontEndBackground.AddAccessory(minon, this.slots.Body.accessories[num]);
		anim.curArm = FrontEndBackground.AddAccessory(minon, this.slots.Arm.accessories[num]);
		if (!anim.overrideSet)
		{
			minon.AddAnimOverrides(anim.target_minion_anim, 0f);
			anim.overrideSet = true;
		}
		minon.UpdateSymbolLookups();
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minon, Accessory accessory)
	{
		minon.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol.build.batchTag, accessory.symbol, false);
		minon.ShowSymbol(accessory.slot.targetSymbolId);
		return accessory.slot.targetSymbolId;
	}

	public KAnimHashedString AddRandomAccessory(KBatchedAnimController minon, List<Accessory> choices)
	{
		Accessory accessory = choices[global::UnityEngine.Random.Range(1, choices.Count)];
		return FrontEndBackground.AddAccessory(minon, accessory);
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

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public FrontEndBackground.AnimChoice[] anims;

	private AccessorySlots slots;

	[NonSerialized]
	public Camera baseCamera;

	[NonSerialized]
	public Camera waterCamera;

	[NonSerialized]
	public Camera overlayCamera;

	[NonSerialized]
	public Camera overlayNoDepthCamera;

	[Serializable]
	public struct AnimChoice
	{
		public string anim_name;

		public KBatchedAnimController minon;

		public float minSecondsBetweenAction;

		public float maxSecondsBetweenAction;

		public float lastWaitTime;

		public KAnimFile curBody;

		public KAnimFile target_minion_anim;

		public bool overrideSet;

		public KAnimHashedString curHair;

		public KAnimHashedString curEyes;

		public KAnimHashedString curHeadShape;

		public KAnimHashedString curMouth;

		public KAnimHashedString curTorso;

		public KAnimHashedString curArm;
	}
}
