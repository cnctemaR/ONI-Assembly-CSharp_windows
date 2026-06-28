using System;
using System.Collections.Generic;
using UnityEngine;

public class SubstanceTable : ScriptableObject, ISerializationCallbackReceiver
{
	public List<Substance> GetList()
	{
		return this.list;
	}

	public Substance GetSubstance(SimHashes substance)
	{
		int count = this.list.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.list[i].elementID == substance)
			{
				return this.list[i];
			}
		}
		return null;
	}

	public void OnBeforeSerialize()
	{
		this.BindAnimList();
	}

	public void OnAfterDeserialize()
	{
		this.BindAnimList();
	}

	private void BindAnimList()
	{
		foreach (Substance substance in this.list)
		{
			if (substance.anim != null && (substance.anims == null || substance.anims.Length == 0))
			{
				substance.anims = new KAnimFile[1];
				substance.anims[0] = substance.anim;
			}
		}
	}

	[SerializeField]
	private List<Substance> list;
}
