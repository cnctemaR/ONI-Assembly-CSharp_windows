using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AnimEventHandler")]
public class AnimEventHandler : KMonoBehaviour
{
	private event AnimEventHandler.SetPos onWorkTargetSet;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (KBatchedAnimTracker kbatchedAnimTracker in base.GetComponentsInChildren<KBatchedAnimTracker>(true))
		{
			if (kbatchedAnimTracker.useTargetPoint)
			{
				this.onWorkTargetSet += kbatchedAnimTracker.SetTarget;
			}
		}
		this.baseOffset = this.animCollider.offset;
		this.instanceIndex = AnimEventHandler.InstanceSequence++;
	}

	public HashedString GetContext()
	{
		return this.context;
	}

	public void UpdateWorkTarget(Vector3 pos)
	{
		if (this.onWorkTargetSet != null)
		{
			this.onWorkTargetSet(pos);
		}
	}

	public void SetContext(HashedString context)
	{
		this.context = context;
	}

	public void SetTargetPos(Vector3 target_pos)
	{
		this.targetPos = target_pos;
	}

	public Vector3 GetTargetPos()
	{
		return this.targetPos;
	}

	public void ClearContext()
	{
		this.context = default(HashedString);
	}

	public void LateUpdate()
	{
		int num = Time.frameCount % 3;
		int num2 = this.instanceIndex % 3;
		if (num != num2)
		{
			return;
		}
		Vector3 pivotSymbolPosition = this.controller.GetPivotSymbolPosition();
		Vector3 vector = this.navigator.NavGrid.GetNavTypeData(this.navigator.CurrentNavType).animControllerOffset;
		this.animCollider.offset = new Vector2(this.baseOffset.x + pivotSymbolPosition.x - base.transform.GetPosition().x - vector.x, this.baseOffset.y + pivotSymbolPosition.y - base.transform.GetPosition().y + vector.y);
	}

	[MyCmpGet]
	private KBatchedAnimController controller;

	[MyCmpGet]
	private KBoxCollider2D animCollider;

	[MyCmpGet]
	private Navigator navigator;

	private Vector3 targetPos;

	public Vector2 baseOffset;

	private HashedString context;

	private int instanceIndex;

	private static int InstanceSequence;

	private delegate void SetPos(Vector3 pos);
}
