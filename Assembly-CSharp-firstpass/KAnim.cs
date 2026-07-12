using System;
using System.Diagnostics;
using UnityEngine;

public class KAnim
{
	public enum PlayMode
	{
		Loop,
		Once,
		Paused
	}

	public enum LayerFlags
	{
		FG = 1
	}

	public enum SymbolFlags
	{
		Bloom = 1,
		OnLight,
		SnapTo = 4,
		FG = 8
	}

	[Serializable]
	public struct AnimHashTable
	{
		public KAnimHashedString[] hashes;
	}

	[DebuggerDisplay("{id} {animFile}")]
	[Serializable]
	public class Anim
	{
		public int index { get; private set; }

		public KAnimFileData animFile { get; private set; }

		public Anim(KAnimFileData anim_file, int idx)
		{
			this.animFile = anim_file;
			this.index = idx;
		}

		public int GetFrameIdx(KAnim.PlayMode mode, float t)
		{
			if (this.numFrames <= 0)
			{
				return -1;
			}
			int num = 0;
			if (mode != KAnim.PlayMode.Loop)
			{
				if (mode != KAnim.PlayMode.Once)
				{
				}
			}
			else
			{
				t %= this.totalTime;
			}
			if (t > 0f)
			{
				float num2 = t * this.frameRate + 0.49999997f;
				num = Math.Min(this.numFrames - 1, (int)num2);
			}
			return num;
		}

		private static KBatchGroupData GetAnimBatchGroupData(KAnimFileData animFile)
		{
			if (!animFile.batchTag.IsValid)
			{
				global::Debug.LogErrorFormat("Invalid batchTag for anim [{0}]", new object[] { animFile.name });
			}
			global::Debug.Assert(animFile.batchTag.IsValid, "Invalid batch tag");
			KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(animFile.batchTag);
			if (group == null)
			{
				global::Debug.LogErrorFormat("Null group for tag [{0}]", new object[] { animFile.batchTag });
			}
			HashedString hashedString = animFile.batchTag;
			if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				if (!group.swapTarget.IsValid)
				{
					global::Debug.LogErrorFormat("Invalid swap target for group [{0}]", new object[] { group.id });
				}
				hashedString = group.swapTarget;
			}
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(hashedString);
			if (batchGroupData == null)
			{
				global::Debug.LogErrorFormat("Null batch group for tag [{0}]", new object[] { hashedString });
			}
			return batchGroupData;
		}

		public KAnim.Anim.Frame GetFrame(KAnimFileData animFile, KAnim.PlayMode mode, float t)
		{
			int frameIdx = this.GetFrameIdx(mode, t);
			KAnim.Anim.Frame frame;
			if (frameIdx >= 0 && animFile.batchTag.IsValid && animFile.batchTag != KAnimBatchManager.NO_BATCH)
			{
				frame = KAnim.Anim.GetAnimBatchGroupData(animFile).GetFrame(this.firstFrameIdx + frameIdx);
			}
			else
			{
				frame = KAnim.Anim.Frame.InvalidFrame;
			}
			return frame;
		}

		public KAnim.Anim.Frame GetFrame(HashedString batchTag, int idx)
		{
			return KAnimBatchManager.Instance().GetBatchGroupData(batchTag).GetFrame(idx + this.firstFrameIdx);
		}

		public KAnim.Anim Copy()
		{
			return new KAnim.Anim(this.animFile, this.index)
			{
				name = this.name,
				id = this.id,
				hash = this.hash,
				rootSymbol = this.rootSymbol,
				frameRate = this.frameRate,
				firstFrameIdx = this.firstFrameIdx,
				numFrames = this.numFrames,
				totalTime = this.totalTime,
				scaledBoundingRadius = this.scaledBoundingRadius,
				unScaledSize = this.unScaledSize
			};
		}

		public string name;

		public HashedString id;

		public float frameRate;

		public int firstFrameIdx;

		public int numFrames;

		public HashedString rootSymbol;

		public HashedString hash;

		public float totalTime;

		public float scaledBoundingRadius;

		public Vector2 unScaledSize = Vector2.zero;

		[Serializable]
		public struct Frame
		{
			public bool IsValid()
			{
				return this.idx != -1;
			}

			public static bool operator ==(KAnim.Anim.Frame a, KAnim.Anim.Frame b)
			{
				return a.idx == b.idx;
			}

			public static bool operator !=(KAnim.Anim.Frame a, KAnim.Anim.Frame b)
			{
				return a.idx != b.idx;
			}

			public override bool Equals(object obj)
			{
				KAnim.Anim.Frame frame = (KAnim.Anim.Frame)obj;
				return this.idx == frame.idx;
			}

			public override int GetHashCode()
			{
				return this.idx;
			}

			public AABB3 bbox;

			public int firstElementIdx;

			public int idx;

			public int numElements;

			public static readonly KAnim.Anim.Frame InvalidFrame = new KAnim.Anim.Frame
			{
				idx = -1
			};
		}

		[Serializable]
		public struct FrameElement
		{
			public KAnimHashedString fileHash;

			public KAnimHashedString symbol;

			public int symbolIdx;

			public KAnimHashedString folder;

			public int frame;

			public Matrix2x3 transform;

			public Color multColour;

			public int flags;
		}
	}

	[Serializable]
	public class Build : ISerializationCallbackReceiver
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.symbols != null)
			{
				for (int i = 0; i < this.symbols.Length; i++)
				{
					this.symbols[i].build = this;
				}
			}
		}

		public KAnim.Build.Symbol GetSymbolByIndex(uint index)
		{
			if ((ulong)index >= (ulong)((long)this.symbols.Length))
			{
				return null;
			}
			return this.symbols[(int)index];
		}

		public Texture2D GetTexture(int index)
		{
			if (index < 0 || index >= this.textureCount)
			{
				global::Debug.LogError("Invalid texture index:" + index.ToString());
			}
			return KAnimBatchManager.Instance().GetBatchGroupData(this.batchTag).GetTexure(this.textureStartIdx + index);
		}

		public int GetSymbolOffset(KAnimHashedString symbol_name)
		{
			for (int i = 0; i < this.symbols.Length; i++)
			{
				if (this.symbols[i].hash == symbol_name)
				{
					return i;
				}
			}
			return -1;
		}

		public KAnim.Build.Symbol GetSymbol(KAnimHashedString symbol_name)
		{
			for (int i = 0; i < this.symbols.Length; i++)
			{
				if (this.symbols[i].hash == symbol_name)
				{
					return this.symbols[i];
				}
			}
			return null;
		}

		public override string ToString()
		{
			return this.name;
		}

		public KAnimHashedString fileHash;

		public int index;

		public string name;

		public HashedString batchTag;

		public int textureStartIdx;

		public int textureCount;

		public KAnim.Build.Symbol[] symbols;

		public KAnim.Build.SymbolFrame[] frames;

		[Serializable]
		public class SymbolFrame : IComparable<KAnim.Build.SymbolFrame>
		{
			public int CompareTo(KAnim.Build.SymbolFrame obj)
			{
				return this.sourceFrameNum.CompareTo(obj.sourceFrameNum);
			}

			public int sourceFrameNum;

			public int duration;

			public KAnimHashedString fileNameHash;

			public Vector2 uvMin;

			public Vector2 uvMax;

			public Vector2 bboxMin;

			public Vector2 bboxMax;
		}

		public struct SymbolFrameInstance
		{
			public KAnim.Build.SymbolFrame symbolFrame;

			public int buildImageIdx;

			public int symbolIdx;
		}

		[DebuggerDisplay("{hash} {path} {folder} {colourChannel}")]
		[Serializable]
		public class Symbol : IComparable
		{
			public int GetFrameIdx(int frame)
			{
				if (this.frameLookup == null)
				{
					global::Debug.LogErrorFormat("Cant get frame [{2}] because Symbol [{0}] for build [{1}] batch [{3}] has no frameLookup", new object[]
					{
						this.hash.ToString(),
						this.build.name,
						frame,
						this.build.batchTag.ToString()
					});
				}
				if (this.frameLookup.Length == 0 || frame >= this.frameLookup.Length)
				{
					return -1;
				}
				frame = Math.Min(frame, this.frameLookup.Length - 1);
				return this.frameLookup[frame];
			}

			public bool HasFrame(int frame)
			{
				return this.GetFrameIdx(frame) >= 0;
			}

			public KAnim.Build.SymbolFrameInstance GetFrame(int frame)
			{
				int frameIdx = this.GetFrameIdx(frame);
				return KAnimBatchManager.Instance().GetBatchGroupData(this.build.batchTag).GetSymbolFrameInstance(frameIdx);
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				if (obj.GetType() == typeof(HashedString))
				{
					HashedString hashedString = (HashedString)obj;
					return this.hash.HashValue.CompareTo(hashedString.HashValue);
				}
				KAnim.Build.Symbol symbol = (KAnim.Build.Symbol)obj;
				return this.hash.HashValue.CompareTo(symbol.hash.HashValue);
			}

			public bool HasFlag(KAnim.SymbolFlags flag)
			{
				return (this.flags & (int)flag) != 0;
			}

			public KAnim.Build.Symbol Copy()
			{
				KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
				symbol.hash = this.hash;
				symbol.path = this.path;
				symbol.folder = this.folder;
				symbol.colourChannel = this.colourChannel;
				symbol.flags = this.flags;
				symbol.firstFrameIdx = this.firstFrameIdx;
				symbol.numFrames = this.numFrames;
				symbol.numLookupFrames = this.numLookupFrames;
				symbol.frameLookup = new int[this.frameLookup.Length];
				symbol.symbolIndexInSourceBuild = this.symbolIndexInSourceBuild;
				Array.Copy(this.frameLookup, symbol.frameLookup, symbol.frameLookup.Length);
				return symbol;
			}

			[NonSerialized]
			public KAnim.Build build;

			public KAnimHashedString hash;

			public KAnimHashedString path;

			public KAnimHashedString folder;

			public KAnimHashedString colourChannel;

			public int flags;

			public int firstFrameIdx;

			public int numFrames;

			public int numLookupFrames;

			public int[] frameLookup;

			public int index;

			public int symbolIndexInSourceBuild;
		}
	}
}
