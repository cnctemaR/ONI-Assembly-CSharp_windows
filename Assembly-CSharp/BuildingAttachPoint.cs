using System;

public class BuildingAttachPoint : KMonoBehaviour
{
	public bool AcceptsAttachment(Tag type)
	{
		return type == this.allowedAttachType;
	}

	public Tag allowedAttachType;
}
