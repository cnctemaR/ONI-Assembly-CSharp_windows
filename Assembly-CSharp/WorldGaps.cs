using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using ProcGen;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public class WorldGaps
{
	public WorldGaps()
	{
		this.voidCells = new HashSet<int>();
	}

	[SerializeField]
	public Cloud[] clouds { get; private set; }

	public Cloud currentCloud { get; private set; }

	public Cloud Next()
	{
		if (this.clouds == null || this.clouds.Length == 0)
		{
			this.currentCloudIndex = -1;
			this.currentCloud = null;
			return null;
		}
		this.currentCloudIndex++;
		this.currentCloudIndex %= this.clouds.Length;
		this.currentCloud = this.clouds[this.currentCloudIndex];
		return this.currentCloud;
	}

	public void SetGasClouds(List<Cloud> newClouds)
	{
		this.clouds = newClouds.ToArray();
		if (this.currentCloudIndex != -1 && this.currentCloudIndex < this.clouds.Length)
		{
			this.currentCloud = this.clouds[this.currentCloudIndex];
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		this.voidCells = new HashSet<int>();
	}

	[Serialize]
	[SerializeField]
	public WorldGaps.State state = WorldGaps.State.EnabledOff;

	[Serialize]
	[SerializeField]
	public HashSet<int> voidCells;

	[SerializeField]
	[Serialize]
	public float currentTime;

	[SerializeField]
	public float cyclePeriod;

	[SerializeField]
	public float notifyTimeApproaching;

	[SerializeField]
	[Serialize]
	public int currentCloudIndex = -1;

	public enum State
	{
		Disabled,
		EnabledRampUp,
		EnabledFull,
		EnabledRampDown,
		EnabledOff
	}
}
