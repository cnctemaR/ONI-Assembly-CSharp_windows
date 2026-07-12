using System;

public class LimitValveTuning
{
	public static NonLinearSlider.Range[] GetDefaultSlider()
	{
		return new NonLinearSlider.Range[]
		{
			new NonLinearSlider.Range(70f, 100f),
			new NonLinearSlider.Range(30f, 500f)
		};
	}

	public const float MAX_LIMIT = 500f;

	public const float DEFAULT_LIMIT = 100f;
}
