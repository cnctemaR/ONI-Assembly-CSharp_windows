using System;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
	public Attack(AttackProperties properties, GameObject[] targets)
	{
		this.properties = properties;
		this.targets = targets;
		this.RollHits();
	}

	private void RollHits()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			if (i > this.properties.maxHits - 1)
			{
				break;
			}
			if (this.targets[i] != null)
			{
				new Hit(this.properties, this.targets[i]);
			}
		}
	}

	private AttackProperties properties;

	private GameObject[] targets;

	public List<Hit> Hits;
}
