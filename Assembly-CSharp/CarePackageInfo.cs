using System;
using UnityEngine;

public class CarePackageInfo : ITelepadDeliverable
{
	public CarePackageInfo(string ID, float amount, Func<bool> requirement)
	{
		this.id = ID;
		this.quantity = amount;
		this.requirement = requirement;
	}

	public CarePackageInfo(string ID, float amount, Func<bool> requirement, string facadeID)
	{
		this.id = ID;
		this.quantity = amount;
		this.requirement = requirement;
		this.facadeID = facadeID;
	}

	public GameObject Deliver(Vector3 location)
	{
		location += Vector3.right / 2f;
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(CarePackageConfig.ID), location);
		gameObject.SetActive(true);
		gameObject.GetComponent<CarePackage>().SetInfo(this);
		return gameObject;
	}

	public readonly string id;

	public readonly float quantity;

	public readonly Func<bool> requirement;

	public readonly string facadeID;
}
