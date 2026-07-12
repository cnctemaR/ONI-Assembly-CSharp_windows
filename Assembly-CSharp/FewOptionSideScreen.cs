using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FewOptionSideScreen : SideScreenContent
{
	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.RefreshOptions();
		}
	}

	private void RefreshOptions()
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.rows)
		{
			keyValuePair.Value.GetComponent<MultiToggle>().ChangeState((keyValuePair.Key == this.targetFewOptions.GetSelectedOption()) ? 1 : 0);
		}
	}

	private void ClearRows()
	{
		for (int i = this.rowContainer.childCount - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.rowContainer.GetChild(i));
		}
		this.rows.Clear();
	}

	private void SpawnRows()
	{
		FewOptionSideScreen.IFewOptionSideScreen.Option[] options = this.targetFewOptions.GetOptions();
		for (int i = 0; i < options.Length; i++)
		{
			FewOptionSideScreen.IFewOptionSideScreen.Option option = options[i];
			GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer.gameObject, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("label").SetText(option.labelText);
			component.GetReference<Image>("icon").sprite = option.iconSpriteColorTuple.first;
			component.GetReference<Image>("icon").color = option.iconSpriteColorTuple.second;
			gameObject.GetComponent<ToolTip>().toolTip = option.tooltipText;
			gameObject.GetComponent<MultiToggle>().onClick = delegate
			{
				this.targetFewOptions.OnOptionSelected(option);
				this.RefreshOptions();
			};
			this.rows.Add(option.tag, gameObject);
		}
		this.RefreshOptions();
	}

	public override void SetTarget(GameObject target)
	{
		this.ClearRows();
		this.targetFewOptions = target.GetComponent<FewOptionSideScreen.IFewOptionSideScreen>();
		this.SpawnRows();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<FewOptionSideScreen.IFewOptionSideScreen>() != null;
	}

	public GameObject rowPrefab;

	public RectTransform rowContainer;

	public Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

	private FewOptionSideScreen.IFewOptionSideScreen targetFewOptions;

	public interface IFewOptionSideScreen
	{
		FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions();

		void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option);

		Tag GetSelectedOption();

		public struct Option
		{
			public Option(Tag tag, string labelText, global::Tuple<Sprite, Color> iconSpriteColorTuple, string tooltipText = "")
			{
				this.tag = tag;
				this.labelText = labelText;
				this.iconSpriteColorTuple = iconSpriteColorTuple;
				this.tooltipText = tooltipText;
			}

			public Tag tag;

			public string labelText;

			public string tooltipText;

			public global::Tuple<Sprite, Color> iconSpriteColorTuple;
		}
	}
}
