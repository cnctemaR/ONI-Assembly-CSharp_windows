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
		int num = 0;
		while (num < this.targets.Length && num <= this.properties.maxHits - 1)
		{
			if (this.targets[num] != null)
			{
				new Hit(this.properties, this.targets[num]);
			}
			num++;
		}
	}

	private AttackProperties properties;

	private GameObject[] targets;

	public List<Hit> Hits;
}
