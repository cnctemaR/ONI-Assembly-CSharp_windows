using System;
using Database;
using UnityEngine;

public interface IKleiPermitDioramaVisTarget
{
	GameObject GetGameObject();

	void ConfigureSetup();

	void ConfigureWith(PermitResource permit);
}
