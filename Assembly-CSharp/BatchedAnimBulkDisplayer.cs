using System;
using System.Collections.Generic;
using UnityEngine;

public class BatchedAnimBulkDisplayer : MonoBehaviour
{
	private void Awake()
	{
		KBatchedAnimUpdater.CreateInstance();
	}

	private void Start()
	{
		DebugHandler.FreeCameraMode = true;
	}

	private void LateUpdate()
	{
		if (Time.frameCount % 30 == 0)
		{
		}
		KBatchedAnimUpdater.instance.LateUpdate();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Display all Animations", new GUILayoutOption[0]))
		{
			this.Generate();
		}
	}

	private void Generate()
	{
		List<KAnimGroupFile.Group> data = KAnimGroupFile.GetGroupFile().GetData();
		for (int i = 0; i < data.Count; i++)
		{
			this.CreateOneGroup(data[i]);
		}
	}

	private void CreateOneGroup(KAnimGroupFile.Group group)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = group.id.ToString();
		gameObject.transform.parent = base.transform;
		for (int i = 0; i < group.files.Count; i++)
		{
			this.CreateOneAnimCycler(gameObject.transform, group, group.files[i]);
			this.offsetx += 4;
			if (this.itemCount % 20 == 19)
			{
				this.offsety += 4;
				this.offsetx = 0;
			}
			this.itemCount++;
		}
	}

	private void CreateOneAnimCycler(Transform parent, KAnimGroupFile.Group group, KAnimFile file)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = parent;
		gameObject.transform.SetPosition(new Vector3((float)this.offsetx, (float)this.offsety, 0f));
		gameObject.SetActive(false);
		gameObject.name = file.name;
		gameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(file.name);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kbatchedAnimController.doHideSnapTo = false;
		List<KAnimFile> list = new List<KAnimFile>();
		if (group.renderType == KAnimBatchGroup.RendererType.AnimOnly && group.animTarget.HashValue != 0)
		{
			KAnimGroupFile.Group group2 = KAnimGroupFile.GetGroup(group.animTarget);
			list.AddRange(group2.files);
		}
		list.Add(file);
		kbatchedAnimController.AddAnims(list.ToArray());
		AnimCycler animCycler = gameObject.AddComponent<AnimCycler>();
		animCycler.file = file.GetData();
		animCycler.controller = kbatchedAnimController;
		gameObject.SetActive(true);
	}

	public int offsetx;

	public int offsety;

	public int maxx = 2000;

	public int itemCount;
}
