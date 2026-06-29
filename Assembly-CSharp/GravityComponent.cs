using System;
using UnityEngine;

public struct GravityComponent
{
	public GravityComponent(Transform transform, global::System.Action on_landed)
	{
		this = new GravityComponent(transform, on_landed, Vector2.zero);
	}

	public GravityComponent(Transform transform, global::System.Action on_landed, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.elapsedTime = 0f;
		this.velocity = initial_velocity;
		this.onLanded = on_landed;
		this.radius = GravityComponent.GetRadius(transform);
	}

	public static float GetRadius(Transform transform)
	{
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if (component != null)
		{
			return component.radius;
		}
		KCollider2D component2 = transform.GetComponent<KCollider2D>();
		if (component2 != null)
		{
			return transform.GetPosition().y - component2.bounds.min.y;
		}
		return 0f;
	}

	public Transform transform;

	public Vector2 velocity;

	public float radius;

	public float elapsedTime;

	public global::System.Action onLanded;
}
