using System;
using UnityEngine;

public abstract class IBuildingConfig : IHasDlcRestrictions
{
	public abstract BuildingDef CreateBuildingDef();

	public virtual void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	public abstract void DoPostConfigureComplete(GameObject go);

	public virtual void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public virtual void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public virtual void ConfigurePost(BuildingDef def)
	{
	}

	[Obsolete("Implement GetRequiredDlcIds and/or GetForbiddenDlcIds instead")]
	public virtual string[] GetDlcIds()
	{
		return null;
	}

	public virtual string[] GetRequiredDlcIds()
	{
		return null;
	}

	public virtual string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public virtual bool ForbidFromLoading()
	{
		return false;
	}
}
