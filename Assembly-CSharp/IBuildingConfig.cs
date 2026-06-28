using System;
using UnityEngine;

public abstract class IBuildingConfig
{
	public abstract BuildingDef CreateBuildingDef();

	public abstract void ConfigureBuildingTemplate(GameObject go);

	public abstract void DoPostConfigure(GameObject go);

	public virtual void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}
}
