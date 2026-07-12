using System;
using System.Collections;
using UnityEngine;

public class StampToolPreview
{
	public StampToolPreview(InterfaceTool tool, params IStampToolPreviewPlugin[] plugins)
	{
		this.context = new StampToolPreviewContext();
		this.context.previewParent = new GameObject("StampToolPreview::Preview").transform;
		this.context.tool = tool;
		this.plugins = plugins;
	}

	public IEnumerator Setup(TemplateContainer stampTemplate)
	{
		this.Cleanup();
		this.context.stampTemplate = stampTemplate;
		if (this.plugins != null)
		{
			IStampToolPreviewPlugin[] array = this.plugins;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Setup(this.context);
			}
		}
		yield return null;
		if (this.context.frameAfterSetupFn != null)
		{
			this.context.frameAfterSetupFn();
		}
		yield break;
	}

	public void Refresh(int originCell)
	{
		if (this.context.stampTemplate == null)
		{
			return;
		}
		if (originCell == this.prevOriginCell)
		{
			return;
		}
		this.prevOriginCell = originCell;
		if (!Grid.IsValidCell(originCell))
		{
			return;
		}
		if (this.context.refreshFn != null)
		{
			this.context.refreshFn(originCell);
		}
		this.context.previewParent.transform.SetPosition(Grid.CellToPosCBC(originCell, this.context.tool.visualizerLayer));
		this.context.previewParent.gameObject.SetActive(true);
	}

	public void OnErrorChange(string error)
	{
		if (this.context.onErrorChangeFn != null)
		{
			this.context.onErrorChangeFn(error);
		}
	}

	public void OnPlace()
	{
		if (this.context.onPlaceFn != null)
		{
			this.context.onPlaceFn();
		}
	}

	public void Cleanup()
	{
		if (this.context.cleanupFn != null)
		{
			this.context.cleanupFn();
		}
		this.prevOriginCell = Grid.InvalidCell;
		this.context.stampTemplate = null;
		this.context.frameAfterSetupFn = null;
		this.context.refreshFn = null;
		this.context.onPlaceFn = null;
		this.context.cleanupFn = null;
	}

	private IStampToolPreviewPlugin[] plugins;

	private StampToolPreviewContext context;

	private int prevOriginCell;
}
