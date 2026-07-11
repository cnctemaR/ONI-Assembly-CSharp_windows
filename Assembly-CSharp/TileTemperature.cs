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
		if (component != null && component.IsReady())
		{
			int num = Grid.PosToCell(primary_element.transform.GetPosition());
			return Grid.Temperature[num];
		}
		return primary_element.InternalTemperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		SimCellOccupier component = primary_element.GetComponent<SimCellOccupier>();
		if (component != null && component.IsReady())
		{
			Debug.LogWarning("Only set a tile's temperature during initialization. Otherwise you should be modifying the cell via the sim!");
		}
		else
		{
			primary_element.InternalTemperature = temperature;
		}
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;
}
