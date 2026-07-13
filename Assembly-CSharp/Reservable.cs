using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Reservable")]
public class Reservable : KMonoBehaviour
{
	public GameObject ReservedBy
	{
		get
		{
			return this.reservedBy;
		}
	}

	public bool IsReserved
	{
		get
		{
			return this.reservedBy != null;
		}
	}

	public bool Reserve(GameObject reserver)
	{
		if (this.reservedBy == null)
		{
			this.reservedBy = reserver;
			return true;
		}
		return false;
	}

	public void ClearReservation()
	{
		this.reservedBy = null;
	}

	public bool IsReservableBy(GameObject reserver)
	{
		return this.reservedBy == null || this.reservedBy == reserver;
	}

	private GameObject reservedBy;
}
