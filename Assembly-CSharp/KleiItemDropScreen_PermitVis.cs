using System;
using System.Collections;
using Database;
using UnityEngine;

public class KleiItemDropScreen_PermitVis : KMonoBehaviour
{
	public void ConfigureWith(PermitResource permit)
	{
		PermitPresentationInfo permitPresentationInfo = PermitItems.GetPermitPresentationInfo(permit.Id);
		bool flag = permit != null;
		this.ResetState();
		this.equipmentVis.gameObject.SetActive(false);
		this.fallbackVis.gameObject.SetActive(false);
		if (!flag)
		{
			this.fallbackVis.gameObject.SetActive(true);
			this.fallbackVis.ConfigureWith(permit, permitPresentationInfo);
			return;
		}
		if (permit.PermitCategory == PermitCategory.Equipment)
		{
			this.equipmentVis.gameObject.SetActive(true);
			this.equipmentVis.ConfigureWith(permit, permitPresentationInfo);
			return;
		}
		this.fallbackVis.gameObject.SetActive(true);
		this.fallbackVis.ConfigureWith(permit, permitPresentationInfo);
	}

	public Promise AnimateIn()
	{
		return Updater.RunRoutine(this, this.AnimateInRoutine());
	}

	public Promise AnimateOut()
	{
		return Updater.RunRoutine(this, this.AnimateOutRoutine());
	}

	private IEnumerator AnimateInRoutine()
	{
		this.root.gameObject.SetActive(true);
		yield return Updater.Ease(delegate(Vector3 v3)
		{
			this.root.transform.localScale = v3;
		}, this.root.transform.localScale, Vector3.one, 0.5f, null);
		yield break;
	}

	private IEnumerator AnimateOutRoutine()
	{
		yield return Updater.Ease(delegate(Vector3 v3)
		{
			this.root.transform.localScale = v3;
		}, this.root.transform.localScale, Vector3.zero, 0.25f, null);
		this.root.gameObject.SetActive(true);
		yield break;
	}

	public void ResetState()
	{
		this.root.transform.localScale = Vector3.zero;
	}

	[SerializeField]
	private RectTransform root;

	[Header("Different Permit Visualizers")]
	[SerializeField]
	private KleiItemDropScreen_PermitVis_Fallback fallbackVis;

	[SerializeField]
	private KleiItemDropScreen_PermitVis_DupeEquipment equipmentVis;
}
