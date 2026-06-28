using System;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownables : Assignables
{
	[OnSerializing]
	protected void OnSerializing()
	{
		base.Save<OwnableSlotInstance.SaveData>(ref this.saveData);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Load(this.saveData);
	}

	[Serialize]
	private OwnableSlotInstance.SaveData[] saveData = new OwnableSlotInstance.SaveData[0];
}
