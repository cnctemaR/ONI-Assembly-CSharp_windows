using System;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampToolPreview_Placers : IStampToolPreviewPlugin
{
	public StampToolPreview_Placers(GameObject placerPrefab)
	{
		StampToolPreview_Placers <>4__this = this;
		this.pool = new GameObjectPool(delegate
		{
			if (<>4__this.poolParent == null)
			{
				<>4__this.poolParent = new GameObject("StampToolPreview::PlacerPool").transform;
			}
			GameObject gameObject = Util.KInstantiate(placerPrefab, <>4__this.poolParent.gameObject, null);
			gameObject.SetActive(false);
			return gameObject;
		}, delegate(GameObject _)
		{
		}, 0);
	}

	public void Setup(StampToolPreviewContext context)
	{
		for (int i = 0; i < context.stampTemplate.cells.Count; i++)
		{
			Cell cell = context.stampTemplate.cells[i];
			GameObject instance = this.pool.GetInstance();
			instance.transform.SetParent(context.previewParent.transform, false);
			instance.transform.localPosition = new Vector3((float)cell.location_x, (float)cell.location_y);
			instance.SetActive(true);
			this.inUse.Add(instance);
		}
		context.onErrorChangeFn = (Action<string>)Delegate.Combine(context.onErrorChangeFn, new Action<string>(delegate(string error)
		{
			foreach (GameObject gameObject in this.inUse)
			{
				if (!gameObject.IsNullOrDestroyed())
				{
					gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = ((error != null) ? StampToolPreviewUtil.COLOR_ERROR : StampToolPreviewUtil.COLOR_OK);
				}
			}
		}));
		context.cleanupFn = (global::System.Action)Delegate.Combine(context.cleanupFn, new global::System.Action(delegate
		{
			foreach (GameObject gameObject2 in this.inUse)
			{
				if (!gameObject2.IsNullOrDestroyed())
				{
					gameObject2.SetActive(false);
					gameObject2.transform.SetParent(this.poolParent);
					this.pool.ReleaseInstance(gameObject2);
				}
			}
			this.inUse.Clear();
		}));
	}

	private List<GameObject> inUse = new List<GameObject>();

	private GameObjectPool pool;

	private Transform poolParent;
}
