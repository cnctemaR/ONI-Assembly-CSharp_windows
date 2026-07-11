using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/OffscreenIndicator")]
public class OffscreenIndicator : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		OffscreenIndicator.Instance = this;
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in this.targets)
		{
			this.UpdateArrow(keyValuePair.Value, keyValuePair.Key);
		}
	}

	public void ActivateIndicator(GameObject target)
	{
		if (!this.targets.ContainsKey(target))
		{
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(target, "ui", false);
			if (uisprite != null)
			{
				this.ActivateIndicator(target, uisprite);
			}
		}
	}

	public void ActivateIndicator(GameObject target, GameObject iconSource)
	{
		if (!this.targets.ContainsKey(target))
		{
			MinionIdentity component = iconSource.GetComponent<MinionIdentity>();
			if (component != null)
			{
				GameObject gameObject = Util.KInstantiateUI(this.IndicatorPrefab, this.IndicatorContainer, true);
				gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon").gameObject.SetActive(false);
				CrewPortrait reference = gameObject.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait");
				reference.gameObject.SetActive(true);
				reference.SetIdentityObject(component, true);
				this.targets.Add(target, gameObject);
			}
		}
	}

	public void ActivateIndicator(GameObject target, global::Tuple<Sprite, Color> icon)
	{
		if (!this.targets.ContainsKey(target))
		{
			GameObject gameObject = Util.KInstantiateUI(this.IndicatorPrefab, this.IndicatorContainer, true);
			Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon");
			if (icon != null)
			{
				reference.sprite = icon.first;
				reference.color = icon.second;
				this.targets.Add(target, gameObject);
			}
		}
	}

	public void DeactivateIndicator(GameObject target)
	{
		if (this.targets.ContainsKey(target))
		{
			global::UnityEngine.Object.Destroy(this.targets[target]);
			this.targets.Remove(target);
		}
	}

	private void UpdateArrow(GameObject arrow, GameObject target)
	{
		if (target == null)
		{
			global::UnityEngine.Object.Destroy(arrow);
			this.targets.Remove(target);
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(target.transform.position);
		if ((double)vector.x > 0.3 && (double)vector.x < 0.7 && (double)vector.y > 0.3 && (double)vector.y < 0.7)
		{
			arrow.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").SetIdentityObject(null, true);
			arrow.SetActive(false);
			return;
		}
		arrow.SetActive(true);
		arrow.rectTransform().SetLocalPosition(Vector3.zero);
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
		vector2.z = target.transform.position.z;
		Vector3 normalized = (target.transform.position - vector2).normalized;
		arrow.transform.up = normalized;
		this.UpdateTargetIconPosition(target, arrow);
	}

	private void UpdateTargetIconPosition(GameObject goTarget, GameObject indicator)
	{
		Vector3 vector = goTarget.transform.position;
		vector = Camera.main.WorldToViewportPoint(vector);
		if (vector.z < 0f)
		{
			vector.x = 1f - vector.x;
			vector.y = 1f - vector.y;
			vector.z = 0f;
			vector = this.Vector3Maxamize(vector);
		}
		vector = Camera.main.ViewportToScreenPoint(vector);
		vector.x = Mathf.Clamp(vector.x, this.edgeInset, (float)Screen.width - this.edgeInset);
		vector.y = Mathf.Clamp(vector.y, this.edgeInset, (float)Screen.height - this.edgeInset);
		indicator.transform.position = vector;
		indicator.GetComponent<HierarchyReferences>().GetReference<Image>("icon").rectTransform.up = Vector3.up;
		indicator.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").transform.up = Vector3.up;
	}

	public Vector3 Vector3Maxamize(Vector3 vector)
	{
		float num = 0f;
		num = ((vector.x > num) ? vector.x : num);
		num = ((vector.y > num) ? vector.y : num);
		num = ((vector.z > num) ? vector.z : num);
		return vector / num;
	}

	public GameObject IndicatorPrefab;

	public GameObject IndicatorContainer;

	private Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();

	public static OffscreenIndicator Instance;

	[SerializeField]
	private float edgeInset = 25f;
}
