using System;

[SkipSaveFileSerialization]
public class TileTemperature : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.primaryElement.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(TileTemperature.OnGetTemperature);
		this.primaryElement.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(TileTemperature.OnSetTemperature);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimCellOccupier component = primary_element.GetComponent<SimCellOccupier>();
		float num2;
		if (component != null && component.IsReady())
		{
			int num = Grid.PosToCell(primary_element.transform.position);
			num2 = Grid.Temperature[num];
		}
		else
		{
			num2 = primary_element.InternalTemperature;
		}
		return num2;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		SimCellOccupier component = primary_element.GetComponent<SimCellOccupier>();
		if (!(component != null) || !component.IsReady())
		{
			primary_element.InternalTemperature = temperature;
		}
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;
}
