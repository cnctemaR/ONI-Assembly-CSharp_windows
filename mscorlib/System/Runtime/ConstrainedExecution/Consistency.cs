using System;

namespace System.Runtime.ConstrainedExecution
{
	[Serializable]
	public enum Consistency
	{
		MayCorruptAppDomain = 1,
		MayCorruptInstance,
		MayCorruptProcess = 0,
		WillNotCorruptState = 3
	}
}
