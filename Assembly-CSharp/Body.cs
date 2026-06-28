using System;

[Serializable]
public class Body : Resource
{
	public BodyType bodyType;

	public KAnimFile[] buildFiles;
}
