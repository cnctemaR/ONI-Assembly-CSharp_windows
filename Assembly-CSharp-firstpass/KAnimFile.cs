using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public HashedString batchTag
	{
		get
		{
			if (this._batchTag.isValid)
			{
				return this._batchTag;
			}
			if (this.homedirectory == null || this.homedirectory == string.Empty)
			{
				return KAnimBatchManager.NO_BATCH;
			}
			string text = this.homedirectory + "/mygroup.yaml";
			if (File.Exists(text))
			{
				KAnimGroupFile.GroupFile groupFile = YamlIO<KAnimGroupFile.GroupFile>.LoadFile(text);
				this._batchTag = new HashedString(groupFile.groupID);
			}
			else
			{
				this._batchTag = KAnimGroupFile.GetGroupFile().GetGroupForHomeDirectory(new HashedString(this.homedirectory));
			}
			return this._batchTag;
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

	public string homedirectory = string.Empty;
}
