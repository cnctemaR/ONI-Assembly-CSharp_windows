using System;

public interface ILogicEventSender : ILogicNetworkConnection
{
	int GetLogicCell();

	int GetLogicValue();
}
