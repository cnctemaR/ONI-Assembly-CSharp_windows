using System;

public class TuningData<TuningType>
{
	public static TuningType Get()
	{
		TuningSystem.Init();
		return TuningData<TuningType>._TuningData;
	}

	public static TuningType _TuningData;
}
