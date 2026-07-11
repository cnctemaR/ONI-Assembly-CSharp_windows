using System;
using UnityEngine;

public struct GravityComponent
{
	public GravityComponent(Transform transform, global::System.Action on_landed, Vector2 initial_velocity, bool land_on_fake_floors)
	{
		this.transform = transform;
		this.elapsedTime = 0f;
		this.velocity = initial_velocity;
		this.onLanded = on_landed;
		this.landOnFakeFloors = land_on_fake_floors;
		KCollider2D component = transform.GetComponent<KCollider2D>();
		this.extents = GravityComponent.GetExtents(component);
		this.yOffset = GravityComponent.GetOffset(component).y;
	}

	public static float GetGroundOffset(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.bounds.extents.y - collider.offset.y;
		}
		return 0f;
	}

	public static Vector2 GetExtents(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.bounds.extents;
		}
		return Vector2.zero;
	}

	public static Vector2 GetOffset(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.offset;
		}
		return Vector2.zero;
	}

	public Transform transform;

	public Vector2 velocity;

	public float elapsedTime;

	public global::System.Action onLanded;

	public bool landOnFakeFloors;

	public Vector2 extents;

	public float yOffset;
}
