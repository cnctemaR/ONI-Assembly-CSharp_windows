using System;
using UnityEngine;

public class UnderwaterBreathingLocation : KMonoBehaviour
{
	public int breathableCell { get; private set; }

	public float GetAvailableBreathableMass()
	{
		PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Breathable, 0f);
		if (!(primaryElement != null))
		{
			return 0f;
		}
		return primaryElement.Mass;
	}

	public void MarkCells()
	{
		Components.UnderwaterBreathingLocations.Add(this);
		this.breathableCell = Grid.PosToCell(this);
	}

	public void UnmarkCells()
	{
		this.breathableCell = -1;
		Components.UnderwaterBreathingLocations.Remove(this);
	}

	public bool ReserveLocation(GameObject reserver, bool reserve)
	{
		bool flag = false;
		if (reserve)
		{
			flag = this.reservable.Reserve(reserver);
		}
		else if (this.reservable.IsReservableBy(reserver))
		{
			this.reservable.ClearReservation();
			flag = true;
		}
		return flag;
	}

	public bool CanReserve(GameObject reserver)
	{
		return !this.reservable.IsReserved || this.reservable.IsReservableBy(reserver);
	}

	[MyCmpGet]
	public Storage storage;

	[MyCmpAdd]
	private Reservable reservable;
}
