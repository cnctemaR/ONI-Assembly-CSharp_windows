using System;
using UnityEngine;
using UnityEngine.UI;

public class MotdBox : KMonoBehaviour
{
	public void Config(MotdBox.PageData[] data)
	{
		this.pageDatas = data;
		if (this.pageButtons != null)
		{
			for (int i = this.pageButtons.Length - 1; i >= 0; i--)
			{
				global::UnityEngine.Object.Destroy(this.pageButtons[i]);
			}
			this.pageButtons = null;
		}
		this.pageButtons = new GameObject[data.Length];
		for (int j = 0; j < this.pageButtons.Length; j++)
		{
			int idx = j;
			GameObject gameObject = Util.KInstantiateUI(this.pageCarouselButtonPrefab, this.pageCarouselContainer, false);
			gameObject.SetActive(true);
			this.pageButtons[j] = gameObject;
			MultiToggle component = gameObject.GetComponent<MultiToggle>();
			component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
			{
				this.SwitchPage(idx);
			}));
		}
		this.SwitchPage(0);
	}

	private void SwitchPage(int newPage)
	{
		this.selectedPage = newPage;
		for (int i = 0; i < this.pageButtons.Length; i++)
		{
			this.pageButtons[i].GetComponent<MultiToggle>().ChangeState((i == this.selectedPage) ? 1 : 0);
		}
		this.image.sprite = this.pageDatas[newPage].Sprite;
		this.headerLabel.SetText(this.pageDatas[newPage].HeaderText);
		this.urlOpener.SetURL(this.pageDatas[newPage].URL);
		if (string.IsNullOrEmpty(this.pageDatas[newPage].ImageText))
		{
			this.imageLabel.gameObject.SetActive(false);
			this.imageLabel.SetText("");
			return;
		}
		this.imageLabel.gameObject.SetActive(true);
		this.imageLabel.SetText(this.pageDatas[newPage].ImageText);
	}

	[SerializeField]
	private GameObject pageCarouselContainer;

	[SerializeField]
	private GameObject pageCarouselButtonPrefab;

	[SerializeField]
	private Image image;

	[SerializeField]
	private LocText headerLabel;

	[SerializeField]
	private LocText imageLabel;

	[SerializeField]
	private URLOpenFunction urlOpener;

	private int selectedPage;

	private GameObject[] pageButtons;

	private MotdBox.PageData[] pageDatas;

	public class PageData
	{
		public Sprite Sprite { get; set; }

		public string HeaderText { get; set; }

		public string ImageText { get; set; }

		public string URL { get; set; }
	}
}
