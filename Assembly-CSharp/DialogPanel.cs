using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		if (this.destroyOnDeselect)
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					Util.KDestroyGameObject(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
		base.gameObject.SetActive(false);
	}

	public bool destroyOnDeselect = true;
}
