using System;

public class DigMonitor : KMonoBehaviour
{
	public static DigMonitor Get()
	{
		return DigMonitor.Instance;
	}

	protected override void OnPrefabInit()
	{
		DigMonitor.Instance = this;
	}

	protected override void OnSpawn()
	{
		Components.Diggables.Register(new Action<Diggable>(this.OnAddDiggable), new Action<Diggable>(this.OnRemoveDiggable));
	}

	private void Update()
	{
		bool flag = false;
		foreach (MinionIdentity minionIdentity in Components.MinionIdentities)
		{
			EquipmentSlot equipmentSlot = EquipmentSet.Get().slotSet.Get("Multitool");
			Equipment component = minionIdentity.GetComponent<Equipment>();
			if (component != null && component.IsSlotOccupied(equipmentSlot))
			{
				flag = true;
				break;
			}
		}
		if (flag != this.hasBoringMachines)
		{
			this.hasBoringMachines = flag;
			foreach (Diggable diggable in Components.Diggables)
			{
				this.Refresh(diggable);
			}
		}
	}

	private void Refresh(Diggable diggable)
	{
		Element element = Grid.Element[Grid.PosToCell(diggable)];
		bool flag = Diggable.RequiresTool(element) && !this.hasBoringMachines;
		diggable.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NeedBoringMachine, flag, this);
		if (!flag)
		{
			flag = Diggable.Undiggable(element);
			diggable.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NeutroniumUnminable, flag, this);
		}
	}

	private void OnAddDiggable(Diggable diggable)
	{
		this.Refresh(diggable);
	}

	private void OnRemoveDiggable(Diggable diggable)
	{
	}

	private static DigMonitor Instance;

	private bool hasBoringMachines;
}
