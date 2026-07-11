using System;

internal class GasFileLoader : AsyncCsvLoader<GasFileLoader, ElementLoader.GasEntry>
{
	public GasFileLoader()
		: base(Assets.instance.simElementsGasesFile)
	{
	}

	public override void Run()
	{
		base.Run();
	}
}
