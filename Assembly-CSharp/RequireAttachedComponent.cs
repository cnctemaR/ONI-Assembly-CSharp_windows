using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RequireAttachedComponent : RocketLaunchCondition
{
	public Type RequiredType
	{
		get
		{
			return this.requiredType;
		}
		set
		{
			this.requiredType = value;
			this.typeNameString = this.requiredType.Name;
		}
	}

	public RequireAttachedComponent(AttachableBuilding myAttachable, Type required_type, string type_name_string)
	{
		this.myAttachable = myAttachable;
		this.requiredType = required_type;
		this.typeNameString = type_name_string;
	}

	public override RocketLaunchCondition.LaunchStatus EvaluateLaunchCondition()
	{
		if (this.myAttachable != null)
		{
			using (List<GameObject>.Enumerator enumerator = AttachableBuilding.GetAttachedNetwork(this.myAttachable).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetComponent(this.requiredType))
					{
						return RocketLaunchCondition.LaunchStatus.Ready;
					}
				}
			}
			return RocketLaunchCondition.LaunchStatus.Failure;
		}
		return RocketLaunchCondition.LaunchStatus.Failure;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return this.typeNameString + " " + UI.STARMAP.LAUNCHCHECKLIST.REQUIRED;
		}
		return this.typeNameString + " " + UI.STARMAP.LAUNCHCHECKLIST.INSTALLED;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.INSTALLED_TOOLTIP, this.typeNameString);
		}
		return string.Format(UI.STARMAP.LAUNCHCHECKLIST.REQUIRED_TOOLTIP, this.typeNameString);
	}

	private string typeNameString;

	private Type requiredType;

	private AttachableBuilding myAttachable;
}
