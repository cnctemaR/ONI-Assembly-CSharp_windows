using System;

public class NotCapturable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (base.GetComponent<Capturable>() != null)
		{
			Output.LogErrorWithObj(this, new object[] { "Entity has both Capturable and NotCapturable!" });
		}
		Components.NotCapturables.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.NotCapturables.Remove(this);
		base.OnCleanUp();
	}
}
