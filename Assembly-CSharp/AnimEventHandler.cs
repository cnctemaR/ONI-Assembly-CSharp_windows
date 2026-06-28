using System;
using System.Diagnostics;
using UnityEngine;

public class AnimEventHandler : KMonoBehaviour
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event AnimEventHandler.SetPos onWorkTargetSet;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimTracker[] componentsInChildren = base.GetComponentsInChildren<KBatchedAnimTracker>(true);
		foreach (KBatchedAnimTracker kbatchedAnimTracker in componentsInChildren)
		{
			if (kbatchedAnimTracker.useTargetPoint)
			{
				this.onWorkTargetSet += kbatchedAnimTracker.SetTarget;
			}
		}
		this.controller = base.GetComponent<KBatchedAnimController>();
		this.animCollider = base.GetComponent<BoxCollider2D>();
		this.baseOffset = this.animCollider.offset;
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
		Vector3 pivotSymbolPosition = this.controller.GetPivotSymbolPosition();
		this.animCollider.offset = new Vector2(this.baseOffset.x + pivotSymbolPosition.x - base.transform.position.x, this.baseOffset.y + pivotSymbolPosition.y - base.transform.position.y);
	}

	private KBatchedAnimController controller;

	private BoxCollider2D animCollider;

	private Vector3 targetPos;

	private Vector2 baseOffset;

	private HashedString context;

	private delegate void SetPos(Vector3 pos);
}
