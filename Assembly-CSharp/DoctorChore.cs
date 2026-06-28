using System;

public class DoctorChore : Workable
{
	private DoctorChore()
	{
		this.synchronizeAnims = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.HealingSpeed;
	}
}
