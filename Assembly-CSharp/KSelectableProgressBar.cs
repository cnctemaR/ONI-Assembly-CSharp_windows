using System;

public class KSelectableProgressBar : KSelectable
{
	public override string GetName()
	{
		int num = (int)(this.progressBar.PercentFull * (float)this.scaleAmount);
		return string.Format("{0} {1}/{2}", this.entityName, num, this.scaleAmount);
	}

	[MyCmpGet]
	private ProgressBar progressBar;

	private int scaleAmount = 100;
}
