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

	public enum SymbolFlags
	{
		Bloom = 1,
		OnLight,
		SnapTo = 4,
		FG = 8
	}

	[DebuggerDisplay("{id} {animFile}")]
	public class Anim
	{
		public int index { get; private set; }

		public KAnimFileData animFile { get; private set; }

		public Anim(KAnimFileData anim_file, int idx)
		{
			this.animFile = anim_file;
			this.index = idx;
		}

		public int GetFrameIdx(KAnim.PlayMode mode, float elapsedSeconds)
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
				elapsedSeconds %= this.totalTime;
			}
			if (elapsedSeconds > 0f)
			{
				float num2 = 0.01f / this.frameRate;
				float num3 = elapsedSeconds * this.frameRate + num2;
				num = Math.Min(this.numFrames - 1, (int)num3);
			}
			return num;
		}

		public bool TryGetFrame(HashedString batchTag, int idx, out KAnim.Anim.Frame frame)
		{
			return KAnimBatchManager.Instance().GetBatchGroupData(batchTag).TryGetFrame(idx + this.firstFrameIdx, out frame);
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

		public struct Frame
		{
			public int firstElementIdx;

			public int numElements;

			public bool hasHead;
		}

		public struct FrameElement
		{
			public Matrix2x3 transform;

			public KAnimHashedString symbol;

			public int frame;

			public float multAlpha;
		}
	}

	public class Build
	{
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
			return this.GetTexture(index, KAnimBatchManager.Instance().GetBatchGroupData(this.batchTag));
		}

		public Texture2D GetTexture(int index, KBatchGroupData batch_group_data)
		{
			if (index < 0 || index >= this.textureCount)
			{
				global::Debug.LogError("Invalid texture index:" + index.ToString());
			}
			return batch_group_data.GetTexure(this.textureStartIdx + index);
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

		public struct SymbolFrameInstance
		{
			public int sourceFrameNum;

			public int duration;

			public Vector2 uvMin;

			public Vector2 uvMax;

			public Vector2 bboxMin;

			public Vector2 bboxMax;

			public int buildImageIdx;

			public int symbolIdx;
		}

		[DebuggerDisplay("{hash} {path} {folder} {colourChannel}")]
		public class Symbol
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

			public KAnim.Build.SymbolFrameInstance GetFrame(int frame)
			{
				return this.GetFrame(frame, KAnimBatchManager.Instance().GetBatchGroupData(this.build.batchTag));
			}

			public KAnim.Build.SymbolFrameInstance GetFrame(int frame, KBatchGroupData batch_group_data)
			{
				int frameIdx = this.GetFrameIdx(frame);
				return batch_group_data.GetSymbolFrameInstance(frameIdx);
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

			public int symbolIndexInSourceBuild;
		}
	}
}
