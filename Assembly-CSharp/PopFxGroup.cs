using System;
using System.Collections.Generic;
using UnityEngine;

public class PopFxGroup : KMonoBehaviour
{
	public void WakeUp(int key)
	{
		if (!this.isLive)
		{
			this.isLive = true;
			this.lastSpawnTimeStamp = float.MinValue;
			this.lastKeyUsed = key;
		}
	}

	public void Enqueue(PopFX effect)
	{
		this.spawnQueue.Enqueue(effect);
	}

	public void Update()
	{
		if (!this.isLive)
		{
			return;
		}
		if (!PopFXManager.Instance.Ready())
		{
			return;
		}
		if (Time.unscaledTime - this.lastSpawnTimeStamp >= 0.1f)
		{
			this.padding = ((this.padding == -1f) ? ((float)(Mathf.Min(this.spawnQueue.Count, 3) - 1) * 1f) : Mathf.Max(this.padding - 1f, 0f));
			PopFX popFX = ((this.spawnQueue.Count > 0) ? this.spawnQueue.Dequeue() : null);
			if (popFX != null)
			{
				if (this.spawnPosition == PopFxGroup.INVALID_SPAWN_POSITION)
				{
					this.spawnPosition = popFX.StartPos;
				}
				popFX.Run(this.spawnPosition, Vector3.up * this.padding);
				this.lastSpawnTimeStamp = Time.unscaledTime;
				return;
			}
			this.Recycle();
		}
	}

	public void Recycle()
	{
		this.isLive = false;
		this.lastSpawnTimeStamp = float.MinValue;
		this.spawnPosition = PopFxGroup.INVALID_SPAWN_POSITION;
		this.padding = -1f;
		while (this.spawnQueue.Count > 0)
		{
			this.spawnQueue.Dequeue().Recycle();
		}
		PopFXManager.Instance.RecycleFxGroup(this.lastKeyUsed, this);
		this.lastKeyUsed = -1;
		base.gameObject.SetActive(false);
	}

	public static readonly Vector3 INVALID_SPAWN_POSITION = Vector3.one * -1f;

	public const float INVALID_PADDING = -1f;

	public const float SPAWN_COOLDOWN = 0.1f;

	public const float MAX_PADDING_MULTIPLIER = 2f;

	public const int MAX_ITEM_COUNT_PADDING = 3;

	public const float INDIVIDUAL_PADDING = 1f;

	public Queue<PopFX> spawnQueue = new Queue<PopFX>();

	private float padding = -1f;

	private int lastKeyUsed = -1;

	private bool isLive;

	private float lastSpawnTimeStamp = float.MinValue;

	private Vector3 spawnPosition = PopFxGroup.INVALID_SPAWN_POSITION;
}
