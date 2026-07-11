using System;
using System.Collections;
using UnityEngine;

public class FrontEndBackground : UIDupeRandomizer
{
	protected override void Start()
	{
		this.tuning = TuningData<FrontEndBackground.Tuning>.Get();
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
		this.dreckoController = base.transform.GetChild(0).Find("startmenu_drecko").GetComponent<KBatchedAnimController>();
		this.dreckoController.enabled = false;
		this.nextDreckoTime = global::UnityEngine.Random.Range(this.tuning.minFirstDreckoInterval, this.tuning.maxFirstDreckoInterval) + Time.unscaledTime;
	}

	protected override void Update()
	{
		base.Update();
		this.UpdateDrecko();
	}

	private void UpdateDrecko()
	{
		if (Time.unscaledTime > this.nextDreckoTime)
		{
			this.dreckoController.enabled = true;
			this.dreckoController.Play("idle", KAnim.PlayMode.Once, 1f, 0f);
			this.nextDreckoTime = global::UnityEngine.Random.Range(this.tuning.minDreckoInterval, this.tuning.maxDreckoInterval) + Time.unscaledTime;
		}
	}

	private void WaitForABit(int minion_idx, HashedString name)
	{
		base.StartCoroutine(this.WaitForTime(minion_idx));
	}

	private IEnumerator WaitForTime(int minion_idx)
	{
		this.anims[minion_idx].lastWaitTime = global::UnityEngine.Random.Range(this.anims[minion_idx].minSecondsBetweenAction, this.anims[minion_idx].maxSecondsBetweenAction);
		yield return new WaitForSecondsRealtime(this.anims[minion_idx].lastWaitTime);
		base.GetNewBody(minion_idx);
		this.anims[minion_idx].minon.ClearQueue();
		this.anims[minion_idx].minon.Play(this.anims[minion_idx].anim_name, KAnim.PlayMode.Once, 1f, 0f);
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
		this.baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		this.baseCamera.tag = "Untagged";
	}

	private KBatchedAnimController dreckoController;

	private float nextDreckoTime;

	private FrontEndBackground.Tuning tuning;

	[NonSerialized]
	public Camera baseCamera;

	public class Tuning : TuningData<FrontEndBackground.Tuning>
	{
		public float minDreckoInterval;

		public float maxDreckoInterval;

		public float minFirstDreckoInterval;

		public float maxFirstDreckoInterval;
	}
}
