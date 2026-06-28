using System;

[SkipSaveFileSerialization]
public class InfoDescription : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (!string.IsNullOrEmpty(this.nameLocString))
		{
			this.displayName = Strings.Get(this.nameLocString);
		}
		if (!string.IsNullOrEmpty(this.descriptionLocString))
		{
			this.description = Strings.Get(this.descriptionLocString);
		}
	}

	public string nameLocString = "";

	public string descriptionLocString = "";

	public string description;

	public string displayName;
}
