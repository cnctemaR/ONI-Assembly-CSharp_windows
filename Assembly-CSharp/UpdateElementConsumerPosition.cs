using System;

public class UpdateElementConsumerPosition : KMonoBehaviour, ISim200ms
{
	public void Sim200ms(float dt)
	{
		base.GetComponent<ElementConsumer>().RefreshConsumptionRate();
	}
}
