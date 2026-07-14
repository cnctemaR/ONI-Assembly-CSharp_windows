using System;
using System.Collections.Generic;
using System.Linq;
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
		this.DefineDefaultGradients();
		this.BindAnimList();
	}

	public void OnAfterDeserialize()
	{
		this.DefineDefaultGradients();
		this.BindAnimList();
	}

	private void DefineDefaultGradients()
	{
		foreach (Substance substance in this.list)
		{
			Gradient gradient = substance.Gradient;
		}
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

	public void RemoveDuplicates()
	{
		this.list = this.list.Distinct<Substance>(new SubstanceTable.SubstanceEqualityComparer()).ToList<Substance>();
	}

	[SerializeField]
	private List<Substance> list;

	public Material solidMaterial;

	public Material liquidMaterial;

	private class SubstanceEqualityComparer : IEqualityComparer<Substance>
	{
		public bool Equals(Substance x, Substance y)
		{
			return x.elementID.Equals(y.elementID);
		}

		public int GetHashCode(Substance obj)
		{
			return obj.elementID.GetHashCode();
		}
	}
}
