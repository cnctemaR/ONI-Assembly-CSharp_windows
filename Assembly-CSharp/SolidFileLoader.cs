using System;

internal class SolidFileLoader : AsyncCsvLoader<SolidFileLoader, ElementLoader.SolidEntry>
{
	public SolidFileLoader()
		: base(Assets.instance.simElementsSolidsFile)
	{
	}

	public override void Run()
	{
		base.Run();
	}
}
