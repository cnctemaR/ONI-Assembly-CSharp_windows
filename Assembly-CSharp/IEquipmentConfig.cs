using System;
using UnityEngine;

public interface IEquipmentConfig
{
	EquipmentDef CreateEquipmentDef();

	void DoPostConfigure(GameObject go);

	[Obsolete("Use IHasDlcRestrictions instead")]
	string[] GetDlcIds()
	{
		return null;
	}
}
