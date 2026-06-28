using System;

[SkipSaveFileSerialization]
public class PreserveOnEntomb : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-1089732772, new Action<object>(this.OnEntombedChanged));
		this.OnEntombedChanged(null);
	}

	private void OnEntombedChanged(object data = null)
	{
		this.Trigger(751746776, Grid.Element[Grid.PosToCell(base.gameObject)].IsSolid);
	}
}
