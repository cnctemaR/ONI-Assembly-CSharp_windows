using System;
using UnityEngine;
using UnityEngine.UI;

public class KScrollbarVisibility : MonoBehaviour
{
	private void Start()
	{
		this.Update();
	}

	private void Update()
	{
		if (this.content.content == null)
		{
			return;
		}
		bool flag = false;
		Vector2 vector = new Vector2(this.parent.rect.width, this.parent.rect.height);
		Vector2 sizeDelta = this.content.content.GetComponent<RectTransform>().sizeDelta;
		if ((sizeDelta.x >= vector.x && this.checkWidth) || (sizeDelta.y >= vector.y && this.checkHeight))
		{
			flag = true;
		}
		if (this.scrollbar.gameObject.activeSelf != flag)
		{
			this.scrollbar.gameObject.SetActive(flag);
			if (this.others != null)
			{
				GameObject[] array = this.others;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(flag);
				}
			}
		}
	}

	[SerializeField]
	private ScrollRect content;

	[SerializeField]
	private RectTransform parent;

	[SerializeField]
	private bool checkWidth = true;

	[SerializeField]
	private bool checkHeight = true;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private GameObject[] others;
}
