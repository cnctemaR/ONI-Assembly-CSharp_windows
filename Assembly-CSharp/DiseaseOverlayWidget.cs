using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DiseaseOverlayWidget : KMonoBehaviour
{
	public void Refresh(AmountInstance value_src)
	{
		GameObject gameObject = value_src.gameObject;
		if (!(gameObject == null))
		{
			base.transform.position = gameObject.transform.position + this.offset;
			DiseaseMonitor.Instance smi = gameObject.GetSMI<DiseaseMonitor.Instance>();
			if (smi != null && smi.IsSick())
			{
				Diseases diseases = gameObject.GetComponent<MinionModifiers>().diseases;
				Disease modifier = diseases[0].modifier;
				this.diseasedImage.color = modifier.overlayColour;
				if (!this.diseasedImage.enabled)
				{
					this.diseasedImage.gameObject.GetComponent<ToolTip>().toolTip = modifier.Name;
				}
				this.diseasedImage.enabled = true;
				this.progressFill.transform.parent.gameObject.SetActive(false);
				this.germsImage.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				if (value_src != null)
				{
					this.progressFill.transform.parent.gameObject.SetActive(true);
					float num = value_src.value / value_src.GetMax();
					Vector3 localScale = this.progressFill.rectTransform.localScale;
					localScale.y = num;
					this.progressFill.rectTransform.localScale = localScale;
					this.progressToolTip.toolTip = DUPLICANTS.ATTRIBUTES.IMMUNITY.NAME + " " + GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
				}
				else
				{
					this.progressFill.transform.parent.gameObject.SetActive(false);
				}
				int num2 = 0;
				Amounts amounts = gameObject.GetComponent<Modifiers>().GetAmounts();
				foreach (Disease disease in Db.Get().Diseases)
				{
					float value = amounts.Get(disease.amount).value;
					if (value > 0f)
					{
						Image image;
						if (num2 < this.displayedDiseases.Count)
						{
							image = this.displayedDiseases[num2];
						}
						else
						{
							GameObject gameObject2 = Util.KInstantiateUI(this.germsImage.gameObject, this.germsImage.transform.parent.gameObject, true);
							image = gameObject2.GetComponent<Image>();
							this.displayedDiseases.Add(image);
						}
						image.color = disease.overlayColour;
						ToolTip component = image.GetComponent<ToolTip>();
						component.toolTip = disease.Name + " " + GameUtil.GetFormattedDiseaseAmount((int)value);
						num2++;
					}
				}
				for (int i = this.displayedDiseases.Count - 1; i >= num2; i--)
				{
					Util.KDestroyGameObject(this.displayedDiseases[i].gameObject);
					this.displayedDiseases.RemoveAt(i);
				}
				this.diseasedImage.enabled = false;
				this.progressFill.transform.parent.gameObject.SetActive(this.displayedDiseases.Count > 0);
				this.germsImage.transform.parent.gameObject.SetActive(this.displayedDiseases.Count > 0);
			}
		}
	}

	[SerializeField]
	private Image progressFill;

	[SerializeField]
	private ToolTip progressToolTip;

	[SerializeField]
	private Image germsImage;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private Image diseasedImage;

	private List<Image> displayedDiseases = new List<Image>();
}
