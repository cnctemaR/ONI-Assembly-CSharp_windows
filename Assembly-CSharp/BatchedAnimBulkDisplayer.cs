using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class BatchedAnimBulkDisplayer : MonoBehaviour
{
	private void UpdateLeftToGenerate()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.groups.Count; i++)
		{
			list.Add(this.groups[i].id.ToString());
		}
		this.leftToGenerate = list.ToArray();
	}

	private void Start()
	{
		KBatchedAnimUpdater.CreateInstance();
		DebugHandler.FreeCameraMode = true;
		this.slots = new AccessorySlots(null, this.head_default_anim, this.head_swap_anim, this.body_swap_anim);
		KAnimFileData data = this.head_default_anim.GetData();
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			this.emotes.Add(anim.hash);
			this.faceAnimNames.Add(anim.name);
		}
	}

	private void LateUpdate()
	{
		KBatchedAnimUpdater.instance.LateUpdate();
	}

	private void OnGUI()
	{
		if (this.groups == null)
		{
			this.groups = KAnimGroupFile.GetGroupFile().GetData();
			this.UpdateLeftToGenerate();
		}
		if (this.leftToGenerate != null && GUILayout.Button("Display all Animations", new GUILayoutOption[0]))
		{
			this.Generate();
		}
		if (this.didGenerate && GUILayout.Button("Toggle Cycler [" + ((!BatchedAnimBulkDisplayer.RunCycler) ? "on" : "off") + "]", new GUILayoutOption[0]))
		{
			BatchedAnimBulkDisplayer.RunCycler = !BatchedAnimBulkDisplayer.RunCycler;
		}
		if (this.leftToGenerate != null)
		{
			int num = GUILayout.SelectionGrid(-1, this.leftToGenerate, 8, new GUILayoutOption[0]);
			if (num != -1)
			{
				this.CreateOneGroup(this.groups[num]);
				this.groups.RemoveAt(num);
				this.UpdateLeftToGenerate();
			}
		}
		if (this.minonsVisible)
		{
			this.DrawMinionChoice();
		}
	}

	private void Generate()
	{
		for (int i = 0; i < this.groups.Count; i++)
		{
			this.CreateOneGroup(this.groups[i]);
		}
		this.leftToGenerate = null;
	}

	private void CreateOneGroup(KAnimGroupFile.Group group)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = group.id.ToString();
		gameObject.transform.parent = base.transform;
		for (int i = 0; i < group.files.Count; i++)
		{
			try
			{
				this.CreateOneAnimCycler(gameObject.transform, group, group.files[i]);
			}
			catch (Exception ex)
			{
				global::Debug.LogError(string.Concat(new string[]
				{
					"Exception while creating [",
					group.files[i].name,
					"] in group [",
					group.id.ToString(),
					"] ",
					ex.Message,
					"\n",
					ex.StackTrace
				}), null);
				return;
			}
			this.offsetx += 4;
			if (this.itemCount % 20 == 19)
			{
				this.offsety += 4;
				this.offsetx = 0;
			}
			this.itemCount++;
		}
		if (group.id == this.human_anim)
		{
			this.minonsVisible = true;
			return;
		}
	}

	private void DrawMinionChoice()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("hair: " + this.bodyData.hair, new GUILayoutOption[0]);
		this.bodyData.hair = (int)GUILayout.HorizontalSlider((float)this.bodyData.hair, 0f, (float)(this.slots.Hair.accessories.Count - 1), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("eyes: " + this.bodyData.eyes, new GUILayoutOption[0]);
		this.bodyData.eyes = (int)GUILayout.HorizontalSlider((float)this.bodyData.eyes, 0f, (float)(this.slots.Eyes.accessories.Count - 1), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("headShape: " + this.bodyData.headShape, new GUILayoutOption[0]);
		this.bodyData.headShape = (int)GUILayout.HorizontalSlider((float)this.bodyData.headShape, 0f, (float)(this.slots.HeadShape.accessories.Count - 1), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("mouth: " + this.bodyData.mouth, new GUILayoutOption[0]);
		this.bodyData.mouth = (int)GUILayout.HorizontalSlider((float)this.bodyData.mouth, 0f, (float)(this.slots.Mouth.accessories.Count - 1), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("body: " + this.bodyData.body, new GUILayoutOption[0]);
		this.bodyData.body = (int)GUILayout.HorizontalSlider((float)this.bodyData.body, 0f, (float)(this.slots.Body.accessories.Count - 1), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		this.hideGuides = GUILayout.Toggle(this.hideGuides, "Hide guides", new GUILayoutOption[0]);
		int num = GUILayout.SelectionGrid(this.faceAnimIdx, this.faceAnimNames.ToArray(), 8, new GUILayoutOption[0]);
		if (num != this.faceAnimIdx)
		{
			this.faceAnimIdx = num;
			this.AppyFaceChange();
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Apply Changes", new GUILayoutOption[0]))
		{
			this.ApplyToMinions();
		}
		if (GUILayout.Button("Apply changes just using the head comp", new GUILayoutOption[0]))
		{
			this.AppyUsingKComp();
		}
		GUILayout.EndHorizontal();
	}

	private void ApplyToMinions()
	{
		FrontEndBackground.AnimChoice animChoice = default(FrontEndBackground.AnimChoice);
		for (int i = 0; i < this.minions.Count; i++)
		{
			this.Apply(this.minions[i], animChoice);
		}
	}

	private void AppyUsingKComp()
	{
		for (int i = 0; i < this.minions.Count; i++)
		{
			this.faces[i] = new KCompBuildInstance(this.bodyData, this.minions[i]);
			this.faces[i].Refresh(null);
		}
	}

	private void AppyFaceChange()
	{
		for (int i = 0; i < this.minions.Count; i++)
		{
			this.faces[i].SetAnimOverride(this.emotes[this.faceAnimIdx]);
		}
	}

	private void Apply(KBatchedAnimController minon, FrontEndBackground.AnimChoice anim)
	{
		if (anim.curHair.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHair);
		}
		anim.curHair = FrontEndBackground.AddAccessory(minon, this.slots.Hair.accessories[this.bodyData.hair]);
		if (anim.curEyes.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curEyes);
		}
		anim.curEyes = FrontEndBackground.AddAccessory(minon, this.slots.Eyes.accessories[this.bodyData.eyes]);
		if (anim.curHeadShape.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHeadShape);
		}
		anim.curHeadShape = FrontEndBackground.AddAccessory(minon, this.slots.HeadShape.accessories[this.bodyData.headShape]);
		if (anim.curMouth.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curMouth);
		}
		anim.curMouth = FrontEndBackground.AddAccessory(minon, this.slots.Mouth.accessories[this.bodyData.mouth]);
		if (anim.curTorso.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curTorso);
			minon.RemoveSymbolOverride(anim.curArm);
		}
		anim.curTorso = FrontEndBackground.AddAccessory(minon, this.slots.Body.accessories[this.bodyData.body]);
		anim.curArm = FrontEndBackground.AddAccessory(minon, this.slots.Arm.accessories[this.bodyData.body]);
		if (this.hideGuides)
		{
			minon.HideSymbol(true, this.snapto_pivot);
			minon.HideSymbol(true, this.snapTo_rgthand);
			minon.HideSymbol(true, this.snapTo_chest);
		}
		minon.UpdateSymbolLookups();
	}

	private void CreateOneAnimCycler(Transform parent, KAnimGroupFile.Group group, KAnimFile file)
	{
		this.didGenerate = true;
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = parent;
		gameObject.transform.SetPosition(new Vector3((float)this.offsetx, (float)this.offsety, 0f));
		gameObject.SetActive(false);
		gameObject.name = file.name;
		gameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(file.name);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		if (group.id == this.human_anim)
		{
			this.minions.Add(kbatchedAnimController);
			this.faces.Add(new KCompBuildInstance(this.bodyData, kbatchedAnimController));
		}
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

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public static bool RunCycler = true;

	public int offsetx;

	public int offsety;

	public int maxx = 2000;

	public int itemCount;

	private List<KAnimGroupFile.Group> groups;

	private string[] leftToGenerate;

	private AccessorySlots slots;

	private List<KBatchedAnimController> minions = new List<KBatchedAnimController>();

	private List<KCompBuildInstance> faces = new List<KCompBuildInstance>();

	private bool minonsVisible;

	private bool hideGuides;

	private KCompBuilder.BodyData bodyData = default(KCompBuilder.BodyData);

	private int faceAnimIdx;

	private bool didGenerate;

	private List<HashedString> emotes = new List<HashedString>();

	private List<string> faceAnimNames = new List<string>();

	private HashedString snapto_pivot = new HashedString("snapTo_pivot");

	private HashedString snapTo_rgthand = new HashedString("snapTo_rgthand");

	private HashedString snapTo_chest = new HashedString("snapTo_chest");

	private HashedString human_anim = new HashedString("human_anim");
}
