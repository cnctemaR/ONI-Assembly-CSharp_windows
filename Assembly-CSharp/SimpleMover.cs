using System;
using KSerialization;
using UnityEngine;

public class SimpleMover : KMonoBehaviour
{
	public Vector3 TargetTransformPosition
	{
		get
		{
			return this.targetTransformPosition;
		}
	}

	public Vector3 CurrentNavigationPosition()
	{
		return base.transform.position + this.cellPositionOffset;
	}

	protected override void OnSpawn()
	{
		this.previousTransformPosition = base.transform.position;
		this.targetTransformPosition = base.transform.position;
	}

	private void Update()
	{
		if (this.moveTimeRemaining != 0f)
		{
			this.moveTimeRemaining = Mathf.Clamp(this.moveTimeRemaining - Time.deltaTime, 0f, this.moveTimeRemaining);
			if (this.moveTimeRemaining == 0f)
			{
				base.Trigger(1027377649, GameHashes.ObjectMovementSleep);
				base.Trigger(-1436222551, null);
			}
		}
		if (base.transform.position != this.targetTransformPosition && this.moveTimeRemaining != 0f)
		{
			float num = (this.moveDuration - this.moveTimeRemaining) / this.moveDuration;
			if (this.smoothStep)
			{
				num = num * num * num * (num * (6f * num - 15f) + 10f);
			}
			base.transform.SetPosition(Vector3.Lerp(this.previousTransformPosition, this.targetTransformPosition, num));
		}
		if (this.teleportDelay > 0f)
		{
			this.teleportDelay -= Time.deltaTime;
			if (this.teleportDelay <= 0f)
			{
				base.transform.SetPosition(this.targetTransformPosition);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.grey;
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.position)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.position + Vector3.right)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.position + Vector3.left)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.position + Vector3.up)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.position + Vector3.down)), Vector3.one * 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawCube(this.CurrentNavigationPosition(), Vector3.one * 0.2f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(this.targetTransformPosition, Vector3.one * 0.2f);
	}

	public void StopMovement()
	{
		this.moveTimeRemaining = 0f;
		this.targetTransformPosition = base.transform.position;
		base.Trigger(1027377649, GameHashes.ObjectMovementSleep);
	}

	public void Land(Vector3 landPosition)
	{
		this.StopMovement();
		float num = 0.1f;
		this.MoveToTarget(landPosition, num);
	}

	public void MoveToTarget(Vector3 target, float moveOverTime)
	{
		this.previousTransformPosition = base.transform.position;
		this.targetTransformPosition = target;
		this.targetTransformPosition.z = this.zPosition;
		this.moveTimeRemaining = moveOverTime;
		this.moveDuration = this.moveTimeRemaining;
		base.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
	}

	public void TeleportToTarget(Vector3 target, float afterDelay = 0f)
	{
		target.z = this.zPosition;
		if (afterDelay > 0f)
		{
			this.teleportDelay = afterDelay;
			this.targetTransformPosition = target;
			this.previousTransformPosition = base.transform.position;
			this.moveTimeRemaining = 0f;
			this.moveDuration = 0f;
		}
		else
		{
			this.previousTransformPosition = base.transform.position;
			base.transform.SetPosition(target);
			this.teleportDelay = 0f;
			this.targetTransformPosition = base.transform.position;
			this.moveTimeRemaining = 0f;
			this.moveDuration = 0f;
		}
	}

	[Serialize]
	private Vector3 previousTransformPosition;

	[Serialize]
	private Vector3 targetTransformPosition;

	[Serialize]
	private float moveTimeRemaining = 0f;

	private float zPosition = -2f;

	private float moveDuration = 0f;

	private float teleportDelay = 0f;

	public bool smoothStep = false;

	public Vector3 cellPositionOffset = Vector3.zero;
}
