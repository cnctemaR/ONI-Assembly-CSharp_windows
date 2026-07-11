using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenHeader")]
public class ReportScreenHeader : KMonoBehaviour
{
	public void SetMainEntry(ReportManager.ReportGroup reportGroup)
	{
		if (this.mainRow == null)
		{
			this.mainRow = Util.KInstantiateUI(this.rowTemplate.gameObject, base.gameObject, true).GetComponent<ReportScreenHeaderRow>();
		}
		this.mainRow.SetLine(reportGroup);
	}

	[SerializeField]
	private ReportScreenHeaderRow rowTemplate;

	private ReportScreenHeaderRow mainRow;
}
