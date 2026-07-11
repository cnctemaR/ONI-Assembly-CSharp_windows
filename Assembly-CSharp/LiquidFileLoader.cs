using System;

internal class LiquidFileLoader : AsyncCsvLoader<LiquidFileLoader, ElementLoader.LiquidEntry>
{
	public LiquidFileLoader()
		: base(Assets.instance.simElementsLiquidsFile)
	{
	}

	public override void Run()
	{
		base.Run();
	}
}
