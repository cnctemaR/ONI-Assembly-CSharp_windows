using System;
using UnityEngine.Pool;

public class LogicValueChanged
{
	public HashedString portID;

	public int newValue;

	public int prevValue;

	public static ObjectPool<LogicValueChanged> Pool = new ObjectPool<LogicValueChanged>(() => new LogicValueChanged(), null, null, null, false, 1, 8);
}
