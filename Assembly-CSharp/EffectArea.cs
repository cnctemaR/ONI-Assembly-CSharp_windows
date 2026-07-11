using System;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EffectArea")]
public class EffectArea : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Effect = Db.Get().effects.Get(this.EffectName);
	}

	private void Update()
	{
		int num = 0;
		int num2 = 0;
		Grid.PosToXY(base.transform.GetPosition(), out num, out num2);
		foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
		{
			int num3 = 0;
			int num4 = 0;
			Grid.PosToXY(minionIdentity.transform.GetPosition(), out num3, out num4);
			if (Math.Abs(num3 - num) <= this.Area && Math.Abs(num4 - num2) <= this.Area)
			{
				minionIdentity.GetComponent<Effects>().Add(this.Effect, true);
			}
		}
	}

	public string EffectName;

	public int Area;

	private Effect Effect;
}
