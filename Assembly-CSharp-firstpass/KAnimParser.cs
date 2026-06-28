using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KAnimParser
{
	public static KAnimParser Get()
	{
		if (KAnimParser.instance == null)
		{
			KAnimParser.instance = new KAnimParser();
		}
		return KAnimParser.instance;
	}

	public KAnimFileData Parse(KAnimFile file)
	{
		KAnimFileData kanimFileData = null;
		if (!(file.animFile != null))
		{
			if (!(file.buildFile != null))
			{
				return kanimFileData;
			}
		}
		try
		{
			kanimFileData = KAnimParser.Parse(file.animFile, file.buildFile, file.textures);
			kanimFileData.name = file.name;
			this.PostParse(kanimFileData);
		}
		catch (Exception ex)
		{
			Output.LogError(new object[] { "Error importing", file.name });
			throw ex;
		}
		return kanimFileData;
	}

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidDataException(message);
		}
	}

	public static void CheckHeader(string header, FastReader reader)
	{
		char[] array = reader.ReadChars(header.Length);
		for (int i = 0; i < header.Length; i++)
		{
			if (array[i] != header[i])
			{
				throw new InvalidDataException("Expected " + header);
			}
		}
	}

	private static KAnimFileData Parse(TextAsset animData, TextAsset buildData, List<Texture2D> textures)
	{
		KAnimFileData kanimFileData = new KAnimFileData();
		kanimFileData.anims = new KAnim.Anim[0];
		kanimFileData.animFrames = new KAnim.Anim.Frame[0];
		kanimFileData.animFrameElements = new KAnim.Anim.FrameElement[0];
		kanimFileData.maxVisSymbolFrames = 0;
		kanimFileData.build = null;
		if (animData != null)
		{
			KAnimParser.ParseAnimData(new FastReader(animData.bytes), kanimFileData);
		}
		if (buildData != null)
		{
			KAnimParser.ParseBuildData(new FastReader(buildData.bytes), kanimFileData, textures);
		}
		return kanimFileData;
	}

	private static void ParseAnimData(FastReader reader, KAnimFileData animFile)
	{
		KAnimParser.CheckHeader("ANIM", reader);
		uint num = reader.ReadUInt32();
		KAnimParser.Assert(num == 5U, "Invalid anim.bytes version");
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		int num4 = reader.ReadInt32();
		int num5 = 0;
		int num6 = 0;
		animFile.anims = new KAnim.Anim[num4];
		animFile.animFrames = new KAnim.Anim.Frame[num3];
		animFile.animFrameElements = new KAnim.Anim.FrameElement[num2];
		animFile.maxVisSymbolFrames = 0;
		for (int i = 0; i < num4; i++)
		{
			KAnim.Anim anim = new KAnim.Anim();
			anim.name = reader.ReadKleiString();
			anim.id = animFile.name + "." + anim.name;
			anim.hash = new HashedString(anim.name);
			anim.rootSymbol.HashValue = reader.ReadInt32();
			anim.frameRate = reader.ReadSingle();
			anim.firstFrameIdx = num5;
			anim.numFrames = reader.ReadInt32();
			anim.totalTime = (float)anim.numFrames / anim.frameRate;
			anim.scaledBoundingRadius = 0f;
			num5 += anim.numFrames;
			for (int j = 0; j < anim.numFrames; j++)
			{
				KAnim.Anim.Frame frame = default(KAnim.Anim.Frame);
				float num7 = reader.ReadSingle();
				float num8 = reader.ReadSingle();
				float num9 = reader.ReadSingle();
				float num10 = reader.ReadSingle();
				frame.bbox = new AABB3(new Vector3(num7 - num9 * 0.5f, -(num8 + num10 * 0.5f), 0f) * 0.005f, new Vector3(num7 + num9 * 0.5f, -(num8 - num10 * 0.5f), 0f) * 0.005f);
				float num11 = Math.Max(Math.Abs(frame.bbox.max.x), Math.Abs(frame.bbox.min.x));
				float num12 = Math.Max(Math.Abs(frame.bbox.max.y), Math.Abs(frame.bbox.min.y));
				float num13 = Math.Max(num11, num12);
				anim.unScaledSize.x = Math.Max(anim.unScaledSize.x, num11 / 0.005f);
				anim.unScaledSize.y = Math.Max(anim.unScaledSize.y, num12 / 0.005f);
				anim.scaledBoundingRadius = Math.Max(anim.scaledBoundingRadius, Mathf.Sqrt(num13 * num13 + num13 * num13));
				frame.idx = j;
				frame.firstElementIdx = num6;
				frame.numElements = reader.ReadInt32();
				num6 += frame.numElements;
				for (int k = 0; k < frame.numElements; k++)
				{
					KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
					frameElement.symbol = new KAnimHashedString(reader.ReadInt32());
					frameElement.frame = reader.ReadInt32();
					frameElement.folder = new KAnimHashedString(reader.ReadInt32());
					frameElement.flags = reader.ReadInt32();
					float num14 = reader.ReadSingle();
					float num15 = reader.ReadSingle();
					float num16 = reader.ReadSingle();
					float num17 = reader.ReadSingle();
					frameElement.multColour = new Color(num17, num16, num15, num14);
					float num18 = reader.ReadSingle();
					float num19 = reader.ReadSingle();
					float num20 = reader.ReadSingle();
					float num21 = reader.ReadSingle();
					float num22 = reader.ReadSingle();
					float num23 = reader.ReadSingle();
					reader.ReadSingle();
					frameElement.transform.m00 = num18;
					frameElement.transform.m01 = num20;
					frameElement.transform.m02 = num22;
					frameElement.transform.m10 = num19;
					frameElement.transform.m11 = num21;
					frameElement.transform.m12 = num23;
					animFile.animFrameElements[frame.firstElementIdx + k] = frameElement;
				}
				animFile.animFrames[anim.firstFrameIdx + j] = frame;
			}
			animFile.anims[i] = anim;
		}
		animFile.maxVisSymbolFrames = Math.Max(animFile.maxVisSymbolFrames, reader.ReadInt32());
		KAnimParser.ParseHashTable(reader, animFile);
	}

	private static void ParseHashTable(FastReader reader, KAnimFileData animFile)
	{
		List<KAnim.AnimHash> list = new List<KAnim.AnimHash>();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			KAnim.AnimHash animHash = new KAnim.AnimHash
			{
				Hash = reader.ReadInt32(),
				String = reader.ReadKleiString()
			};
			list.Add(animHash);
			HashCache.Get().Add(animHash.Hash, animHash.String);
		}
		animFile.hashTable.hashes = list.ToArray();
	}

	private static void ParseBuildData(FastReader reader, KAnimFileData animFile, List<Texture2D> textures)
	{
		KAnimParser.CheckHeader("BILD", reader);
		int num = reader.ReadInt32();
		KAnimParser.Assert(num == 10 || num == 9, "Invalid build.bytes version");
		KAnim.Build build = new KAnim.Build();
		animFile.build = build;
		build.textures = ((textures.Count <= 0) ? null : textures.ToArray());
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		build.symbols = new KAnim.Build.Symbol[num2];
		build.frames = new KAnim.Build.SymbolFrame[num3];
		build.name = reader.ReadKleiString();
		int num4 = 0;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
			symbol.build = build;
			symbol.hash = new KAnimHashedString(reader.ReadInt32());
			if (num > 9)
			{
				symbol.path = new KAnimHashedString(reader.ReadInt32());
			}
			symbol.colourChannel = new KAnimHashedString(reader.ReadInt32());
			symbol.flags = reader.ReadInt32();
			symbol.firstFrameIdx = num4;
			symbol.numFrames = reader.ReadInt32();
			int num5 = 0;
			for (int j = 0; j < symbol.numFrames; j++)
			{
				KAnim.Build.SymbolFrame symbolFrame = new KAnim.Build.SymbolFrame();
				KAnim.Build.SymbolFrameInstance symbolFrameInstance = default(KAnim.Build.SymbolFrameInstance);
				symbolFrameInstance.symbolFrame = symbolFrame;
				symbolFrame.sourceFrameNum = reader.ReadInt32();
				symbolFrame.duration = reader.ReadInt32();
				symbolFrameInstance.buildImageIdx = reader.ReadInt32();
				num5 = Math.Max(symbolFrame.sourceFrameNum + symbolFrame.duration, num5);
				float num6 = reader.ReadSingle();
				float num7 = reader.ReadSingle();
				float num8 = reader.ReadSingle();
				float num9 = reader.ReadSingle();
				symbolFrame.bboxMin = new Vector2(num6 - num8 * 0.5f, num7 - num9 * 0.5f);
				symbolFrame.bboxMax = new Vector2(num6 + num8 * 0.5f, num7 + num9 * 0.5f);
				float num10 = reader.ReadSingle();
				float num11 = reader.ReadSingle();
				float num12 = reader.ReadSingle();
				float num13 = reader.ReadSingle();
				Vector2 vector = new Vector2(num10, num11);
				Vector2 vector2 = new Vector2(num12, num13);
				symbolFrame.v0 = new Vector3(symbolFrame.bboxMin.x, symbolFrame.bboxMin.y, 0f);
				symbolFrame.v1 = new Vector3(symbolFrame.bboxMax.x, symbolFrame.bboxMin.y, 0f);
				symbolFrame.v2 = new Vector3(symbolFrame.bboxMin.x, symbolFrame.bboxMax.y, 0f);
				symbolFrame.v3 = new Vector3(symbolFrame.bboxMax.x, symbolFrame.bboxMax.y, 0f);
				symbolFrame.uv0 = new Vector2(vector.x, 1f - vector.y);
				symbolFrame.uv1 = new Vector2(vector2.x, 1f - vector.y);
				symbolFrame.uv2 = new Vector2(vector.x, 1f - vector2.y);
				symbolFrame.uv3 = new Vector2(vector2.x, 1f - vector2.y);
				build.frames[num4] = symbolFrame;
				num4++;
			}
			symbol.numLookupFrames = num5;
			build.symbols[i] = symbol;
		}
		KAnimParser.ParseHashTable(reader, animFile);
	}

	private void PostParse(KAnimFileData anim_file)
	{
		if (anim_file.build != null && anim_file.build.symbols != null)
		{
			for (int i = 0; i < anim_file.build.symbols.Length; i++)
			{
				KAnim.Build.Symbol symbol = anim_file.build.symbols[i];
				symbol.frameLookup = new int[symbol.numLookupFrames];
				for (int j = 0; j < symbol.numLookupFrames; j++)
				{
					symbol.frameLookup[j] = -1;
				}
				for (int k = symbol.firstFrameIdx; k < symbol.firstFrameIdx + symbol.numFrames; k++)
				{
					KAnim.Build.SymbolFrame symbolFrame = anim_file.build.frames[k];
					for (int l = symbolFrame.sourceFrameNum; l < symbolFrame.sourceFrameNum + symbolFrame.duration; l++)
					{
						symbol.frameLookup[l] = k;
					}
				}
			}
		}
	}

	public static KAnimParser instance;
}
