using System;
using UnityEngine;

public class KleiPermitBuildingAnimateIn : MonoBehaviour
{
	private void Awake()
	{
		this.placeAnimController.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 1);
		this.colorAnimController.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 1);
		this.updater = Updater.Parallel(new Updater[]
		{
			KleiPermitBuildingAnimateIn.MakeAnimInUpdater(this.sourceAnimController, this.placeAnimController, this.colorAnimController),
			this.extraUpdater
		});
	}

	private void Update()
	{
		this.sourceAnimController.gameObject.SetActive(false);
		this.updater.Internal_Update(Time.unscaledDeltaTime);
	}

	private void OnDisable()
	{
		this.sourceAnimController.gameObject.SetActive(true);
		global::UnityEngine.Object.Destroy(this.placeAnimController.gameObject);
		global::UnityEngine.Object.Destroy(this.colorAnimController.gameObject);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public static KleiPermitBuildingAnimateIn MakeFor(KBatchedAnimController sourceAnimController, Updater extraUpdater = default(Updater), string place_anim = "place")
	{
		sourceAnimController.gameObject.SetActive(false);
		KBatchedAnimController kbatchedAnimController = global::UnityEngine.Object.Instantiate<KBatchedAnimController>(sourceAnimController, sourceAnimController.transform.parent, false);
		kbatchedAnimController.gameObject.name = "KleiPermitBuildingAnimateIn.placeAnimController";
		kbatchedAnimController.initialAnim = place_anim;
		KBatchedAnimController kbatchedAnimController2 = global::UnityEngine.Object.Instantiate<KBatchedAnimController>(sourceAnimController, sourceAnimController.transform.parent, false);
		kbatchedAnimController2.gameObject.name = "KleiPermitBuildingAnimateIn.colorAnimController";
		KAnimFileData data = sourceAnimController.AnimFiles[0].GetData();
		KAnim.Anim anim = data.GetAnim("idle");
		if (anim == null)
		{
			anim = data.GetAnim("off");
			if (anim == null)
			{
				anim = data.GetAnim(0);
			}
		}
		kbatchedAnimController2.initialAnim = anim.name;
		GameObject gameObject = new GameObject("KleiPermitBuildingAnimateIn");
		gameObject.SetActive(false);
		gameObject.transform.SetParent(sourceAnimController.transform.parent, false);
		KleiPermitBuildingAnimateIn kleiPermitBuildingAnimateIn = gameObject.AddComponent<KleiPermitBuildingAnimateIn>();
		kleiPermitBuildingAnimateIn.sourceAnimController = sourceAnimController;
		kleiPermitBuildingAnimateIn.placeAnimController = kbatchedAnimController;
		kleiPermitBuildingAnimateIn.colorAnimController = kbatchedAnimController2;
		kleiPermitBuildingAnimateIn.extraUpdater = ((extraUpdater.fn == null) ? Updater.None() : extraUpdater);
		kbatchedAnimController.gameObject.SetActive(true);
		kbatchedAnimController2.gameObject.SetActive(true);
		gameObject.SetActive(true);
		return kleiPermitBuildingAnimateIn;
	}

	public static Updater MakeAnimInUpdater(KBatchedAnimController sourceAnimController, KBatchedAnimController placeAnimController, KBatchedAnimController colorAnimController)
	{
		return Updater.Parallel(new Updater[]
		{
			Updater.Series(new Updater[]
			{
				Updater.Ease(delegate(float alpha)
				{
					placeAnimController.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Clamp(alpha, 1f, 255f));
				}, 1f, 255f, 0.1f, Easing.CubicOut, -1f),
				Updater.Ease(delegate(float alpha)
				{
					placeAnimController.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Clamp(255f - alpha, 1f, 255f));
					colorAnimController.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Clamp(alpha, 1f, 255f));
				}, 1f, 255f, 0.3f, Easing.CubicIn, -1f)
			}),
			Updater.Series(new Updater[]
			{
				Updater.Ease(delegate(float scale)
				{
					scale = sourceAnimController.transform.localScale.x * scale;
					placeAnimController.transform.localScale = Vector3.one * scale;
					colorAnimController.transform.localScale = Vector3.one * scale;
				}, 1f, 1.012f, 0.2f, Easing.CubicOut, -1f),
				Updater.Ease(delegate(float scale)
				{
					scale = sourceAnimController.transform.localScale.x * scale;
					placeAnimController.transform.localScale = Vector3.one * scale;
					colorAnimController.transform.localScale = Vector3.one * scale;
				}, 1.012f, 1f, 0.1f, Easing.CubicIn, -1f)
			})
		});
	}

	private KBatchedAnimController sourceAnimController;

	private KBatchedAnimController placeAnimController;

	private KBatchedAnimController colorAnimController;

	private Updater updater;

	private Updater extraUpdater;
}
