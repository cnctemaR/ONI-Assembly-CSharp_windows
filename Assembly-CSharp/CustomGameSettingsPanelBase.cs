using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomGameSettingsPanelBase : MonoBehaviour
{
	public virtual void Init()
	{
	}

	public virtual void Uninit()
	{
	}

	private void OnEnable()
	{
		this.isDirty = true;
	}

	private void Update()
	{
		if (this.isDirty)
		{
			this.isDirty = false;
			this.Refresh();
		}
	}

	protected void AddWidget(CustomGameSettingWidget widget)
	{
		widget.onSettingChanged += this.OnWidgetChanged;
		this.widgets.Add(widget);
	}

	private void OnWidgetChanged(CustomGameSettingWidget widget)
	{
		this.isDirty = true;
	}

	public virtual void Refresh()
	{
		foreach (CustomGameSettingWidget customGameSettingWidget in this.widgets)
		{
			customGameSettingWidget.Refresh();
		}
	}

	protected List<CustomGameSettingWidget> widgets = new List<CustomGameSettingWidget>();

	private bool isDirty;
}
