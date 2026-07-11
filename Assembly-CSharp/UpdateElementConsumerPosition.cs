using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UpdateElementConsumerPosition")]
public class UpdateElementConsumerPosition : KMonoBehaviour, ISim200ms
{
	public void Sim200ms(float dt)
	{
		base.GetComponent<ElementConsumer>().RefreshConsumptionRate();
	}
}
