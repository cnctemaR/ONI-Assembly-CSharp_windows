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

	public bool isReserved
	{
		get
		{
			return !(this.reservedBy == null);
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

	public void ClearReservation(GameObject reserver)
	{
		if (this.reservedBy == reserver)
		{
			this.reservedBy = null;
		}
	}

	private GameObject reservedBy;
}
