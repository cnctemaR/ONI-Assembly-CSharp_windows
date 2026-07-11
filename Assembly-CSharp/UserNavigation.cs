using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

public class UserNavigation : KMonoBehaviour
{
	public UserNavigation()
	{
		for (global::Action action = global::Action.SetUserNav1; action <= global::Action.SetUserNav10; action++)
		{
			this.hotkeyNavPoints.Add(UserNavigation.NavPoint.Invalid);
		}
	}

	private static int GetIndex(global::Action action)
	{
		int num = -1;
		if (global::Action.SetUserNav1 <= action && action <= global::Action.SetUserNav10)
		{
			num = action - global::Action.SetUserNav1;
		}
		else if (global::Action.GotoUserNav1 <= action && action <= global::Action.GotoUserNav10)
		{
			num = action - global::Action.GotoUserNav1;
		}
		return num;
	}

	private void SetHotkeyNavPoint(global::Action action, Vector3 pos, float ortho_size)
	{
		int index = UserNavigation.GetIndex(action);
		if (index < 0)
		{
			return;
		}
		this.hotkeyNavPoints[index] = new UserNavigation.NavPoint
		{
			pos = pos,
			orthoSize = ortho_size
		};
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_set", false), CameraController.Instance.GetVerticallyScaledPosition(pos), 1f);
		eventInstance.setParameterValue("userNavPoint_ID", (float)index);
		KFMOD.EndOneShot(eventInstance);
	}

	private void GoToHotkeyNavPoint(global::Action action)
	{
		int index = UserNavigation.GetIndex(action);
		if (index < 0)
		{
			return;
		}
		UserNavigation.NavPoint navPoint = this.hotkeyNavPoints[index];
		if (navPoint.IsValid())
		{
			CameraController instance = CameraController.Instance;
			instance.SetTargetPos(navPoint.pos, navPoint.orthoSize, true);
			EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_recall", false), instance.GetVerticallyScaledPosition(instance.transform.GetPosition()), 1f);
			eventInstance.setParameterValue("userNavPoint_ID", (float)index);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public bool Handle(KButtonEvent e)
	{
		bool flag = false;
		for (global::Action action = global::Action.GotoUserNav1; action <= global::Action.GotoUserNav10; action++)
		{
			if (e.TryConsume(action))
			{
				this.GoToHotkeyNavPoint(action);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (global::Action action2 = global::Action.SetUserNav1; action2 <= global::Action.SetUserNav10; action2++)
			{
				if (e.TryConsume(action2))
				{
					Camera baseCamera = CameraController.Instance.baseCamera;
					Vector3 position = baseCamera.transform.GetPosition();
					this.SetHotkeyNavPoint(action2, position, baseCamera.orthographicSize);
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	[Serialize]
	private List<UserNavigation.NavPoint> hotkeyNavPoints = new List<UserNavigation.NavPoint>();

	[Serializable]
	private struct NavPoint
	{
		public bool IsValid()
		{
			return this.orthoSize != 0f;
		}

		public Vector3 pos;

		public float orthoSize;

		public static readonly UserNavigation.NavPoint Invalid = new UserNavigation.NavPoint
		{
			pos = Vector3.zero,
			orthoSize = 0f
		};
	}
}
