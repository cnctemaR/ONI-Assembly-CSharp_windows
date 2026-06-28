using System;

public class SeededRandom
{
	public SeededRandom(int seed)
	{
		this.seed = seed;
		this.rnd = new Random(seed);
	}

	public int seed { get; private set; }

	public Random RandomSource()
	{
		return this.rnd;
	}

	public float RandomValue()
	{
		return (float)this.rnd.NextDouble();
	}

	public double NextDouble()
	{
		return this.rnd.NextDouble();
	}

	public float RandomRange(float rangeLow, float rangeHigh)
	{
		float num = rangeHigh - rangeLow;
		return rangeLow + (float)(this.rnd.NextDouble() * (double)num);
	}

	public int RandomRange(int rangeLow, int rangeHigh)
	{
		int num = rangeHigh - rangeLow;
		return rangeLow + (int)(this.rnd.NextDouble() * (double)num);
	}

	private Random rnd;
}
