using System;
using UnityEngine;

public class NativeAnimBatchLoader : MonoBehaviour
{
	private void Awake()
	{
		KAnimBatchManager.Destroy();
		KAnimGroupFile.Destroy();
		KGlobalAnimParser.Destroy();
		KGlobalAnimParser.ClearMissingSymbols();
		KAnimGroupFile.GetGroupFile().LoadAll();
		if (this.dumpMissingSymbols)
		{
			KGlobalAnimParser.DumpMissingSymbols();
		}
		KAnimBatchManager.Instance().CompleteInit();
	}

	private void Start()
	{
		if (this.generateObjects)
		{
			for (int i = 0; i < this.enableObjects.Length; i++)
			{
				if (this.enableObjects[i] != null)
				{
					this.enableObjects[i].GetComponent<KBatchedAnimController>().visibilityType = KAnimControllerBase.VisibilityType.Always;
					this.enableObjects[i].SetActive(true);
				}
			}
		}
		if (this.setTimeScale)
		{
			Time.timeScale = 1f;
		}
		if (this.destroySelf)
		{
			global::UnityEngine.Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		if (this.destroySelf)
		{
			return;
		}
		if (this.performUpdate)
		{
			KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
			KAnimBatchManager.Instance().UpdateDirty();
		}
		if (this.performRender)
		{
			KAnimBatchManager.Instance().Render();
		}
	}

	public bool performTimeUpdate;

	public bool performUpdate;

	public bool performRender;

	public bool setTimeScale;

	public bool destroySelf;

	public bool generateObjects;

	public bool dumpMissingSymbols;

	public GameObject[] enableObjects;
}
