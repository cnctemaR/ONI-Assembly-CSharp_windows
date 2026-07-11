using System;
using System.Collections.Generic;

public class Sensors : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<Brain>().onPreUpdate += this.OnBrainPreUpdate;
	}

	public SensorType GetSensor<SensorType>() where SensorType : Sensor
	{
		foreach (Sensor sensor in this.sensors)
		{
			if (typeof(SensorType).IsAssignableFrom(sensor.GetType()))
			{
				return (SensorType)((object)sensor);
			}
		}
		Debug.LogError("Missing sensor of type: " + typeof(SensorType).Name);
		return (SensorType)((object)null);
	}

	public void Add(Sensor sensor)
	{
		this.sensors.Add(sensor);
		sensor.Update();
	}

	public void UpdateSensors()
	{
		foreach (Sensor sensor in this.sensors)
		{
			sensor.Update();
		}
	}

	private void OnBrainPreUpdate()
	{
		this.UpdateSensors();
	}

	public void ShowEditor()
	{
		foreach (Sensor sensor in this.sensors)
		{
			sensor.ShowEditor();
		}
	}

	public List<Sensor> sensors = new List<Sensor>();
}
