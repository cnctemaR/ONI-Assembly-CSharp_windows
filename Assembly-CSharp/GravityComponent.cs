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
		Collider2D component = transform.GetComponent<Collider2D>();
		if (component != null)
		{
			this.offset = transform.position.y - component.bounds.min.y;
		}
		else
		{
			this.offset = 0f;
		}
	}

	public Vector2 Velocity
	{
		get
		{
			Vector2 vector = new Vector2(this.initialVelocity.x, this.initialVelocity.y + -9.8f * this.elapsedTime);
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
