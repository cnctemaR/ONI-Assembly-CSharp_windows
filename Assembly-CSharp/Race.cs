using System;

[Serializable]
public class Race : Resource
{
	public KCompBuild GetHeadComp()
	{
		return this.headComp;
	}

	public Body GetBody(BodyType body_type)
	{
		foreach (Body body in this.bodies)
		{
			if (body.bodyType == body_type)
			{
				return body;
			}
		}
		return this.bodies[0];
	}

	public override void Initialize()
	{
		this.headComp = new KCompBuild(Assets.GetAnim("body_comp_default_kanim"));
	}

	public string StringKey;

	public KAnimFile headCompFile;

	private KCompBuild headComp;

	public Race.Bodies bodies;

	[Serializable]
	public class Bodies : ResourceSet<Body>
	{
	}
}
