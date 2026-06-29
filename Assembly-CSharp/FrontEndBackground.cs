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

	[NonSerialized]
	public Camera baseCamera;
}
