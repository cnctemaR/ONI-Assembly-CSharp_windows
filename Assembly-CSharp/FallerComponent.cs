using System;
using UnityEngine;

public struct FallerComponent
{
	public FallerComponent(Transform transform, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.transformInstanceId = transform.GetInstanceID();
		this.isFalling = false;
		this.initialVelocity = initial_velocity;
		this.partitionerEntry = default(HandleVector<int>.Handle);
		this.solidChangedCB = null;
		this.cellChangedCB = null;
		this.cellChangedHandlerID = 0UL;
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if (component != null)
		{
			this.offset = component.radius;
			return;
		}
		KCollider2D component2 = transform.GetComponent<KCollider2D>();
		if (component2 != null)
		{
			this.offset = transform.GetPosition().y - component2.bounds.min.y;
			return;
		}
		this.offset = 0f;
	}

	public Transform transform;

	public int transformInstanceId;

	public bool isFalling;

	public float offset;

	public Vector2 initialVelocity;

	public HandleVector<int>.Handle partitionerEntry;

	public Action<object> solidChangedCB;

	public Action<object> cellChangedCB;

	public ulong cellChangedHandlerID;
}
