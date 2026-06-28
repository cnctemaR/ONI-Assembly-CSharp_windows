using System;

public class Death : Resource
{
	public Death(string id, ResourceSet parent, string name, string description, string pre_anim, string loop_anim)
		: base(id, parent, name)
	{
		this.preAnim = pre_anim;
		this.loopAnim = loop_anim;
		this.description = description;
	}

	public string preAnim;

	public string loopAnim;

	public string sound;

	public string description;
}
