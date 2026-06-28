using System;
using UnityEngine;

public class AnimEventHandler : KMonoBehaviour
{
	private event AnimEventHandler.SetPos onWorkTargetSet;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimTracker[] componentsInChildren = base.GetComponentsInChildren<KBatchedAnimTracker>(true);
		foreach (KBatchedAnimTracker kbatchedAnimTracker in componentsInChildren)
		{
			if (kbatchedAnimTracker.useTargetPoint)
			{
				this.onWorkTargetSet = (AnimEventHandler.SetPos)Delegate.Combine(this.onWorkTargetSet, new AnimEventHandler.SetPos(kbatchedAnimTracker.SetTarget));
			}
		}
		this.baseOffset = base.GetComponent<BoxCollider2D>().offset;
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
		Vector3 pivotSymbolPosition = base.GetComponent<KBatchedAnimController>().GetPivotSymbolPosition();
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		component.offset = new Vector2(this.baseOffset.x + pivotSymbolPosition.x - this.transform.position.x, this.baseOffset.y + pivotSymbolPosition.y - this.transform.position.y);
	}

	private Vector3 targetPos;

	private Vector2 baseOffset;

	private HashedString context;

	private delegate void SetPos(Vector3 pos);
}
