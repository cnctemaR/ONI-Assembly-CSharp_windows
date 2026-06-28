using System;
using UnityEngine;

public struct FallerComponent
{
	public FallerComponent(Transform transform, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.isFalling = false;
		this.initialVelocity = initial_velocity;
		this.partitionerEntry = null;
		this.solidChangedCB = null;
		CircleCollider2D component = transform.GetComponent<CircleCollider2D>();
		if (component != null)
		{
			this.offset = component.radius;
		}
		else
		{
			Collider2D component2 = transform.GetComponent<Collider2D>();
			if (component2 != null)
			{
				this.offset = transform.position.y - component2.bounds.min.y;
			}
			else
			{
				this.offset = 0f;
			}
		}
	}

	public Transform transform;

	public bool isFalling;

	public float offset;

	public Vector2 initialVelocity;

	public ScenePartitionerEntry partitionerEntry;

	public Action<object> solidChangedCB;
}
