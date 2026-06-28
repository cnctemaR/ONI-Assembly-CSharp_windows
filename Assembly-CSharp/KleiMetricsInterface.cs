using System;
using System.Collections.Generic;

public interface KleiMetricsInterface
{
	void StartSession();

	void EndSession(bool crashed = false);

	void StartNewGame();

	void EndGame();

	void SendEvent(Dictionary<string, object> eventData);
}
