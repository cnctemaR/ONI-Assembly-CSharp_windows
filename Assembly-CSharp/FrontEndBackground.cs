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
		base.StartCoroutine(this.WaitForTime(minonIdx));
	}

	private IEnumerator WaitForTime(int minonIdx)
	{
		this.anims[minonIdx].lastWaitTime = global::UnityEngine.Random.Range(this.anims[minonIdx].minSecondsBetweenAction, this.anims[minonIdx].maxSecondsBetweenAction);
		yield return new WaitForSecondsRealtime(this.anims[minonIdx].lastWaitTime);
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
		this.baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		this.baseCamera.tag = "Untagged";
	}

	[NonSerialized]
	public Camera baseCamera;
}
