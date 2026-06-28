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
		this.initialVelocity = initial_velocity;
		this.onLanded = on_landed;
		this.offset = 0f;
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
		}
	}

	public Vector2 Velocity
	{
		get
		{
			Vector2 vector = new Vector2(this.initialVelocity.x, this.initialVelocity.y + -9.8f * Mathf.Max(0f, this.elapsedTime));
			return vector;
		}
	}

	private const float Acceleration = -9.8f;

	public Transform transform;

	public Vector2 initialVelocity;

	public float offset;

	public float elapsedTime;

	public global::System.Action onLanded;
}
