using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		if (this.destroyOnDeselect)
		{
			foreach (object obj in base.transform)
			{
				Util.KDestroyGameObject(((Transform)obj).gameObject);
			}
		}
		base.gameObject.SetActive(false);
	}

	public bool destroyOnDeselect = true;
}
