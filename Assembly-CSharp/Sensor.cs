using System;
using UnityEngine;

public class Sensor
{
	public bool IsEnabled { get; private set; } = true;

	public string Name { get; private set; }

	public Sensor(Sensors sensors, bool active)
	{
		this.sensors = sensors;
		this.SetActive(active);
		this.Name = base.GetType().Name;
	}

	public Sensor(Sensors sensors)
	{
		this.sensors = sensors;
		this.Name = base.GetType().Name;
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return this.sensors.GetComponent<ComponentType>();
	}

	public GameObject gameObject
	{
		get
		{
			return this.sensors.gameObject;
		}
	}

	public Transform transform
	{
		get
		{
			return this.gameObject.transform;
		}
	}

	public virtual void SetActive(bool enabled)
	{
		this.IsEnabled = enabled;
	}

	public void Trigger(int hash, object data = null)
	{
		this.sensors.Trigger(hash, data);
	}

	public virtual void Update()
	{
	}

	public virtual void ShowEditor()
	{
	}

	protected Sensors sensors;
}
