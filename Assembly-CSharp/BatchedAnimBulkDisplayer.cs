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
		AudioEventManager.Get();
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
		UIDupeRandomizer.AnimChoice animChoice = default(UIDupeRandomizer.AnimChoice);
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

	private void Apply(KBatchedAnimController minon, UIDupeRandomizer.AnimChoice anim)
	{
		if (anim.curHair.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHair);
		}
		anim.curHair = UIDupeRandomizer.AddAccessory(minon, this.slots.Hair.Lookup(this.bodyData.hair));
		if (anim.curEyes.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curEyes);
		}
		anim.curEyes = UIDupeRandomizer.AddAccessory(minon, this.slots.Eyes.Lookup(this.bodyData.eyes));
		if (anim.curHeadShape.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curHeadShape);
		}
		anim.curHeadShape = UIDupeRandomizer.AddAccessory(minon, this.slots.HeadShape.Lookup(this.bodyData.headShape));
		if (anim.curMouth.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curMouth);
		}
		anim.curMouth = UIDupeRandomizer.AddAccessory(minon, this.slots.Mouth.Lookup(this.bodyData.mouth));
		if (anim.curTorso.IsValid())
		{
			minon.RemoveSymbolOverride(anim.curTorso);
			minon.RemoveSymbolOverride(anim.curArm);
		}
		anim.curTorso = UIDupeRandomizer.AddAccessory(minon, this.slots.Body.Lookup(this.bodyData.body));
		anim.curArm = UIDupeRandomizer.AddAccessory(minon, this.slots.Arm.Lookup(this.bodyData.body));
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
		gameObject.transform.SetPosition(new Vector3((float)this.offsetx + this.globalOffset.x, (float)this.offsety + this.globalOffset.y, 0f));
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

	public Vector2 globalOffset = Vector2.left * 20f;

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
