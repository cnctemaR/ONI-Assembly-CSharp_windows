using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public HashedString batchTag
	{
		get
		{
			HashedString hashedString;
			if (this._batchTag.isValid)
			{
				hashedString = this._batchTag;
			}
			else if (this.homedirectory == null || this.homedirectory == "")
			{
				hashedString = KAnimBatchManager.NO_BATCH;
			}
			else
			{
				this._batchTag = KAnimGroupFile.GetGroupFile().GetGroupForHomeDirectory(new HashedString(this.homedirectory));
				hashedString = this._batchTag;
			}
			return hashedString;
		}
	}

	public KAnimFileData GetData()
	{
		if (this.data == null)
		{
			this.data = KGlobalAnimParser.Get().Load(this);
		}
		return this.data;
	}

	public const string ANIM_ROOT_PATH = "Assets/anim";

	public TextAsset animFile;

	public TextAsset buildFile;

	public List<Texture2D> textures = new List<Texture2D>();

	private KAnimFileData data;

	private HashedString _batchTag;

	public string homedirectory = "";
}
