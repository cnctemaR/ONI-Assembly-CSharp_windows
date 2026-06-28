using System;
using KSerialization;
using UnityEngine;

public class SimpleMover : KMonoBehaviour, ISim33ms
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
		return base.transform.GetPosition() + this.cellPositionOffset;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		this.previousTransformPosition = base.transform.GetPosition();
		this.targetTransformPosition = base.transform.GetPosition();
		this.TryStartUpdating();
	}

	private void TryStartUpdating()
	{
		if (this.teleportDelay > 0f || this.moveTimeRemaining > 0f)
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}
	}

	private void TryStopUpdating()
	{
		if (this.teleportDelay <= 0f && this.moveTimeRemaining <= 0f)
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void Sim33ms(float dt)
	{
		if (this.moveTimeRemaining != 0f)
		{
			this.moveTimeRemaining = Mathf.Clamp(this.moveTimeRemaining - dt, 0f, this.moveTimeRemaining);
			if (this.moveTimeRemaining == 0f)
			{
				base.Trigger(1027377649, GameHashes.ObjectMovementSleep);
				base.Trigger(-1436222551, null);
			}
		}
		if (base.transform.GetPosition() != this.targetTransformPosition && this.moveTimeRemaining != 0f)
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
			this.teleportDelay -= dt;
			if (this.teleportDelay <= 0f)
			{
				base.transform.SetPosition(this.targetTransformPosition);
			}
		}
		this.TryStopUpdating();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.grey;
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.GetPosition())), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.GetPosition() + Vector3.right)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.GetPosition() + Vector3.left)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.GetPosition() + Vector3.up)), Vector3.one * 0.2f);
		Gizmos.DrawCube(CreatureHelpers.CenterPositionOfCell(Grid.PosToCell(base.transform.GetPosition() + Vector3.down)), Vector3.one * 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawCube(this.CurrentNavigationPosition(), Vector3.one * 0.2f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(this.targetTransformPosition, Vector3.one * 0.2f);
	}

	public void StopMovement()
	{
		this.moveTimeRemaining = 0f;
		this.targetTransformPosition = base.transform.GetPosition();
		base.Trigger(1027377649, GameHashes.ObjectMovementSleep);
		this.TryStopUpdating();
	}

	public void Land(Vector3 landPosition)
	{
		this.StopMovement();
		float num = 0.1f;
		this.MoveToTarget(landPosition, num);
		this.TryStopUpdating();
	}

	public void MoveToTarget(Vector3 target, float moveOverTime)
	{
		this.previousTransformPosition = base.transform.GetPosition();
		this.targetTransformPosition = target;
		this.targetTransformPosition.z = this.zPosition;
		this.moveTimeRemaining = moveOverTime;
		this.moveDuration = this.moveTimeRemaining;
		base.Trigger(1027377649, GameHashes.ObjectMovementWakeUp);
		this.TryStartUpdating();
	}

	public void TeleportToTarget(Vector3 target, float afterDelay = 0f)
	{
		target.z = this.zPosition;
		if (afterDelay > 0f)
		{
			this.teleportDelay = afterDelay;
			this.targetTransformPosition = target;
			this.previousTransformPosition = base.transform.GetPosition();
			this.moveTimeRemaining = 0f;
			this.moveDuration = 0f;
		}
		else
		{
			this.previousTransformPosition = base.transform.GetPosition();
			base.transform.SetPosition(target);
			this.teleportDelay = 0f;
			this.targetTransformPosition = base.transform.GetPosition();
			this.moveTimeRemaining = 0f;
			this.moveDuration = 0f;
		}
		this.TryStopUpdating();
	}

	[Serialize]
	private Vector3 previousTransformPosition;

	[Serialize]
	private Vector3 targetTransformPosition;

	[Serialize]
	private float moveTimeRemaining;

	private float zPosition = -2f;

	private float moveDuration;

	private float teleportDelay;

	public bool smoothStep;

	public Vector3 cellPositionOffset = Vector3.zero;
}
