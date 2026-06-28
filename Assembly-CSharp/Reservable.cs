using System;
using UnityEngine;

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
		bool flag;
		if (this.reservedBy == null)
		{
			this.reservedBy = reserver;
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
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
