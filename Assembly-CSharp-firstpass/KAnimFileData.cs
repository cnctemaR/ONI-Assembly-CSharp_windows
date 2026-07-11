using System;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class KAnimFileData
{
	public string name { get; private set; }

	public KAnimHashedString hashName { get; private set; }

	public KAnimFileData(string name)
	{
		this.name = name;
		this.firstAnimIndex = -1;
		this.buildIndex = -1;
		this.firstElementIndex = -1;
		this.animCount = 0;
		this.frameCount = 0;
		this.elementCount = 0;
		this.maxVisSymbolFrames = 0;
		this.hashName = new KAnimHashedString(name);
	}

	public KAnim.Build build
	{
		get
		{
			if (this.buildIndex == -1)
			{
				return null;
			}
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.batchTag);
			if (batchGroupData == null)
			{
				global::Debug.LogErrorFormat("[{0}] No such batch group [{1}]", new object[]
				{
					this.name,
					this.batchTag.ToString()
				});
			}
			return batchGroupData.GetBuild(this.buildIndex);
		}
	}

	public KAnim.Anim GetAnim(int index)
	{
		global::Debug.Assert(index >= 0 && index < this.animCount);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.animBatchTag);
		if (batchGroupData == null)
		{
			global::Debug.LogError(string.Format("[{0}] No such batch group [{1}]", this.name, this.animBatchTag.ToString()));
		}
		return batchGroupData.GetAnim(index + this.firstAnimIndex);
	}

	public KAnim.Anim.FrameElement GetAnimFrameElement(int index)
	{
		global::Debug.Assert(index >= 0 && index < this.elementCount);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.animBatchTag);
		if (batchGroupData == null)
		{
			global::Debug.LogErrorFormat("[{0}] No such batch group [{1}]", new object[]
			{
				this.name,
				this.animBatchTag.ToString()
			});
		}
		return batchGroupData.GetFrameElement(this.firstElementIndex + index);
	}

	public KAnim.Anim.FrameElement FindAnimFrameElement(KAnimHashedString symbolName)
	{
		return KAnimBatchManager.Instance().GetBatchGroupData(this.animBatchTag).frameElements.Find((KAnim.Anim.FrameElement match) => match.symbol == symbolName);
	}

	public const int NO_RECORD = -1;

	public int index;

	public HashedString batchTag;

	public int buildIndex;

	public HashedString animBatchTag;

	public int firstAnimIndex;

	public int animCount;

	public int frameCount;

	public int firstElementIndex;

	public int elementCount;

	public int maxVisSymbolFrames;
}
