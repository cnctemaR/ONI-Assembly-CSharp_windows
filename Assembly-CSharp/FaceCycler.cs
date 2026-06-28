using System;

public class FaceCycler : Cycler
{
	private void Start()
	{
		this.controller = base.GetComponent<KBatchedAnimController>();
		this.Next();
	}

	public void ReBuild(KCompBuilder.BodyData body)
	{
		this.build = new KCompBuildInstance(body, this.controller);
	}

	private void SetNewAnim()
	{
		this.build.SetAnimOverride(this.hash);
	}

	protected override void Next()
	{
	}

	private KBatchedAnimController controller;

	private KCompBuildInstance build;

	private KAnimFileData buildData;

	public HashedString hash;
}
