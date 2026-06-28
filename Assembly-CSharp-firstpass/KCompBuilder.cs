using System;
using System.Collections.Generic;
using UnityEngine;

public class KCompBuilder : MonoBehaviour
{
	private void GetSymbolsFromBuild(KBatchGroupData batch_group, KAnim.Build src_build, KAnim.Build target_build, KAnimHashedString src_name, KAnimHashedString target_name, List<KAnim.Build.Symbol> symbols, List<KAnim.Build.SymbolFrame> frames, List<Texture2D> textures)
	{
		Debug.Assert(src_build != null);
		Debug.Assert(src_name.IsValid());
		Debug.Assert(symbols != null);
		Debug.Assert(frames != null);
		Debug.Assert(textures != null);
		KAnim.Build.Symbol symbol = src_build.GetSymbol(src_name);
		Debug.AssertFormat(symbol != null, "Please check the DB: Couldnt find symbol [{1} {0} ] in Group [{2}] format is eyes_hair_headshape_mouth_torso_arms", new object[]
		{
			src_name.ToString(),
			HashCache.Get().Get(src_name),
			batch_group.groupID.ToString()
		});
		if (symbol == null)
		{
			return;
		}
		KAnim.Build.Symbol symbol2 = symbol.Copy();
		symbol2.build = target_build;
		symbol2.hash = target_name;
		symbols.Add(symbol2);
		int firstFrameIdx = symbol2.firstFrameIdx;
		symbol2.firstFrameIdx = batch_group.symbolFrameInstances.Count;
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(src_build.batchTag, false);
		for (int i = 0; i < symbol2.frameLookup.Length; i++)
		{
			symbol2.frameLookup[i] = symbol2.firstFrameIdx + (symbol2.frameLookup[i] - firstFrameIdx);
		}
		for (int j = 0; j < symbol.numFrames; j++)
		{
			KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData.symbolFrameInstances[firstFrameIdx + j];
			Texture2D texture2D = batchGroupData.textures[symbolFrameInstance.buildImageIdx];
			int num = textures.IndexOf(texture2D);
			if (num == -1)
			{
				num = textures.Count;
				textures.Add(texture2D);
			}
			symbolFrameInstance.buildImageIdx = num;
			symbolFrameInstance.symbolIdx = batch_group.GetSymbolCount();
			frames.Add(symbolFrameInstance.symbolFrame);
			batch_group.symbolFrameInstances.Add(symbolFrameInstance);
		}
		batch_group.AddBuildSymbol(symbol2);
	}

	private KAnim.Build GetBuildForVariation(KBatchGroupData batch_group, KAnimHashedString fileHash, string eyes, string hair, string headshape, string mouth, string body, string arms)
	{
		Debug.Assert(this.master_anims != null);
		KAnimFileData data = this.master_anims.GetData();
		Debug.Assert(data != null);
		KAnim.Build build = data.build;
		Debug.Assert(build != null);
		Debug.Assert(this.face_variations != null);
		KAnimFileData data2 = this.face_variations.GetData();
		Debug.Assert(data2 != null);
		KAnim.Build build2 = data2.build;
		Debug.Assert(build2 != null);
		Debug.Assert(this.body_variations != null);
		KAnimFileData data3 = this.body_variations.GetData();
		Debug.Assert(data3 != null);
		KAnim.Build build3 = data3.build;
		Debug.Assert(build3 != null);
		List<KAnim.Build.Symbol> list = new List<KAnim.Build.Symbol>();
		List<KAnim.Build.SymbolFrame> list2 = new List<KAnim.Build.SymbolFrame>();
		List<Texture2D> list3 = new List<Texture2D>();
		KAnim.Build build4 = batch_group.GetBuild(fileHash);
		if (build4 != null)
		{
			return build4;
		}
		KAnimGroupFile.AddDynamicGroup(batch_group.groupID);
		build4 = batch_group.AddNewBuildFile(fileHash);
		if (eyes == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_eyes, KCompBuilder.snapTo_eyes, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build2, build4, new HashedString("eyes_" + eyes), KCompBuilder.snapTo_eyes, list, list2, list3);
		}
		if (hair == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_hair, KCompBuilder.snapTo_hair, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build2, build4, new HashedString("hair_" + hair), KCompBuilder.snapTo_hair, list, list2, list3);
		}
		if (headshape == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_headshape, KCompBuilder.snapTo_headshape, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build2, build4, new HashedString("headshape_" + headshape), KCompBuilder.snapTo_headshape, list, list2, list3);
		}
		if (mouth == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_mouth, KCompBuilder.snapTo_mouth, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build2, build4, new HashedString("mouth_" + mouth), KCompBuilder.snapTo_mouth, list, list2, list3);
		}
		if (body == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_body, KCompBuilder.snapTo_body, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build3, build4, new HashedString("body_" + body), KCompBuilder.snapTo_body, list, list2, list3);
		}
		if (arms == KCompBuilder.default_build)
		{
			this.GetSymbolsFromBuild(batch_group, build, build4, KCompBuilder.snapTo_arm, KCompBuilder.snapTo_arm, list, list2, list3);
		}
		else
		{
			this.GetSymbolsFromBuild(batch_group, build3, build4, new HashedString("arm_" + arms), KCompBuilder.snapTo_arm, list, list2, list3);
		}
		build4.symbols = list.ToArray();
		build4.frames = list2.ToArray();
		batch_group.AddTextures(list3);
		return build4;
	}

	public KAnimFileData GenerateDefaultPose(KCompBuilder.BodyData bodyData)
	{
		string text = string.Format("{0:000}", bodyData.eyes);
		string text2 = string.Format("{0:000}", bodyData.hair);
		string text3 = string.Format("{0:000}", bodyData.headShape);
		string text4 = string.Format("{0:000}", bodyData.mouth);
		string text5 = string.Format("{0:000}", bodyData.body);
		string text6 = string.Format("{0:000}", bodyData.arms);
		string text7 = string.Concat(new string[]
		{
			text, "_", text2, "_", text3, "_", text4, "_", text5, "_",
			text6
		});
		HashedString hashedString = new HashedString(text7);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(hashedString, true);
		KAnimFileData dynamicFile = KGlobalAnimParser.Get().GetDynamicFile(hashedString, "default_" + text7);
		if (dynamicFile.buildIndex != -1)
		{
			return dynamicFile;
		}
		KAnimHashedString kanimHashedString = new KAnimHashedString(dynamicFile.name);
		KAnim.Build build = batchGroupData.GetBuild(kanimHashedString);
		if (build == null)
		{
			build = this.GetBuildForVariation(batchGroupData, kanimHashedString, text, text2, text3, text4, text5, text6);
			build.batchTag = hashedString;
			build.name = dynamicFile.name;
			build.fileHash = kanimHashedString;
			dynamicFile.buildIndex = build.index;
			dynamicFile.animBatchTag = hashedString;
			KAnimFileData data = this.master_anims.GetData();
			KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(data.batchTag, false);
			dynamicFile.maxVisSymbolFrames = data.maxVisSymbolFrames;
			batchGroupData.animIndex.Add(kanimHashedString, batchGroupData.anims.Count);
			batchGroupData.animFrameIndex.Add(kanimHashedString, batchGroupData.animFrames.Count);
			dynamicFile.animCount = 0;
			dynamicFile.frameCount = 0;
			dynamicFile.elementCount = 0;
			dynamicFile.firstAnimIndex = batchGroupData.anims.Count;
			dynamicFile.firstElementIndex = batchGroupData.frameElements.Count;
			for (int i = 0; i < data.animCount; i++)
			{
				KAnim.Anim anim = data.GetAnim(i).Copy();
				int firstFrameIdx = anim.firstFrameIdx;
				anim.firstFrameIdx = batchGroupData.animFrames.Count;
				for (int j = 0; j < anim.numFrames; j++)
				{
					KAnim.Anim.Frame frame = batchGroupData2.animFrames[firstFrameIdx + j];
					frame.idx = batchGroupData.animFrames.Count;
					int firstElementIdx = frame.firstElementIdx;
					frame.firstElementIdx = batchGroupData.frameElements.Count;
					for (int k = 0; k < frame.numElements; k++)
					{
						KAnim.Anim.FrameElement frameElement = batchGroupData2.frameElements[firstElementIdx + k];
						batchGroupData.frameElements.Add(frameElement);
						dynamicFile.elementCount++;
					}
					batchGroupData.animFrames.Add(frame);
					dynamicFile.frameCount++;
				}
				batchGroupData.anims.Add(anim);
				dynamicFile.animCount++;
			}
			return dynamicFile;
		}
		dynamicFile.buildIndex = build.index;
		return dynamicFile;
	}

	public static KCompBuilder Instance
	{
		get
		{
			if (KCompBuilder.instance == null)
			{
				Debug.LogError("No CompBuilder instance");
			}
			return KCompBuilder.instance;
		}
	}

	private void Start()
	{
		if (KCompBuilder.instance == null)
		{
			KCompBuilder.instance = this;
		}
		else
		{
			global::UnityEngine.Object.DestroyImmediate(this);
		}
	}

	private void OnDestroy()
	{
		if (KCompBuilder.instance == this)
		{
			KCompBuilder.instance = null;
		}
	}

	public KAnimFile master_anims;

	public KAnimFile face_variations;

	public KAnimFile body_variations;

	private static string default_build = "000";

	public static HashedString snapTo_eyes = new HashedString("snapTo_eyes");

	public static HashedString snapTo_hair = new HashedString("snapTo_hair");

	public static HashedString snapTo_headshape = new HashedString("snapTo_headshape");

	public static HashedString snapTo_mouth = new HashedString("snapTo_mouth");

	public static HashedString snapTo_body = new HashedString("snapTo_body");

	public static HashedString snapTo_arm = new HashedString("snapTo_arm");

	public static HashedString head_comp = new HashedString("head_comp");

	public static HashedString head_comp_side = new HashedString("head_comp_side");

	public static HashedString head_comp_back = new HashedString("head_comp_back");

	public static HashedString neutral = new HashedString("neutral");

	private static KCompBuilder instance = null;

	[Serializable]
	public struct BodyData
	{
		public int headShape;

		public int mouth;

		public int neck;

		public int eyes;

		public int hair;

		public int body;

		public int arms;
	}
}
