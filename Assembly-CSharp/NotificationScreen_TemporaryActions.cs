using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class NotificationScreen_TemporaryActions : KMonoBehaviour
{
	public static NotificationScreen_TemporaryActions Instance { get; private set; }

	public static void DestroyInstance()
	{
		NotificationScreen_TemporaryActions.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NotificationScreen_TemporaryActions.Instance = this;
		this.originalRow.gameObject.SetActive(false);
	}

	private TemporaryActionRow CreateActionRow()
	{
		TemporaryActionRow temporaryActionRow = Util.KInstantiateUI<TemporaryActionRow>(this.originalRow.gameObject, this.originalRow.transform.parent.gameObject, false);
		temporaryActionRow.gameObject.SetActive(true);
		temporaryActionRow.transform.SetAsLastSibling();
		this.rows.Add(temporaryActionRow);
		return temporaryActionRow;
	}

	private void RemoveRow(TemporaryActionRow row)
	{
		if (this.rows.Contains(row))
		{
			this.rows.Remove(row);
		}
		row.OnRowHidden = null;
		row.gameObject.DeleteObject();
	}

	protected override void OnCleanUp()
	{
		if (this.rows != null)
		{
			foreach (TemporaryActionRow temporaryActionRow in this.rows.ToArray())
			{
				if (temporaryActionRow != null)
				{
					this.RemoveRow(temporaryActionRow);
				}
			}
			this.rows.Clear();
		}
		base.OnCleanUp();
	}

	public void CreateCameraReturnActionButton(Vector3 positionToReturnTo)
	{
		if (this.cameraReturnRow == null)
		{
			this.cameraReturnRow = this.CreateActionRow();
			this.cameraReturnRow.Setup(UI.TEMPORARY_ACTIONS.CAMERA_RETURN.NAME, UI.TEMPORARY_ACTIONS.CAMERA_RETURN.TOOLTIP, Assets.GetSprite("action_follow_cam"));
			this.cameraReturnRow.gameObject.name = "TemporaryActionRow_CameraReturn";
			this.cameraPositionToReturnTo = positionToReturnTo;
			this.cameraReturnRow.OnRowHidden = new Action<TemporaryActionRow>(this.RemoveRow);
			this.cameraReturnRow.OnRowClicked = new Action<TemporaryActionRow>(this.OnCameraReturnActionButtonClicked);
		}
		this.cameraReturnRow.SetLifetime(10f);
	}

	private void OnCameraReturnActionButtonClicked(TemporaryActionRow row)
	{
		if (this.cameraPositionToReturnTo != Vector3.zero)
		{
			GameUtil.FocusCamera(this.cameraPositionToReturnTo, 2f, true, false);
		}
	}

	public TemporaryActionRow originalRow;

	private List<TemporaryActionRow> rows = new List<TemporaryActionRow>();

	private TemporaryActionRow cameraReturnRow;

	private Vector3 cameraPositionToReturnTo = Vector3.zero;

	private const float CAMERA_RETURN_BUTTON_LIFETIME = 10f;
}
