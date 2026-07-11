using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public byte[] animBytes
	{
		get
		{
			return (this.mod != null) ? this.mod.anim : ((!(this.animFile != null)) ? null : this.animFile.bytes);
		}
	}

	public byte[] buildBytes
	{
		get
		{
			return (this.mod != null) ? this.mod.build : ((!(this.buildFile != null)) ? null : this.buildFile.bytes);
		}
	}

	public List<Texture2D> textureList
	{
		get
		{
			return (this.mod != null) ? this.mod.textures : this.textures;
		}
	}

	public void Initialize(TextAsset anim, TextAsset build, IList<Texture2D> textures)
	{
		this.animFile = anim;
		this.buildFile = build;
		this.textures.Clear();
		this.textures.AddRange(textures);
	}

	public HashedString batchTag
	{
		get
		{
			if (this._batchTag.IsValid)
			{
				return this._batchTag;
			}
			if (this.homedirectory == null || this.homedirectory == string.Empty)
			{
				return KAnimBatchManager.NO_BATCH;
			}
			this._batchTag = KAnimGroupFile.GetGroupFile().GetGroupForHomeDirectory(new HashedString(this.homedirectory));
			return this._batchTag;
		}
	}

	public KAnimFileData GetData()
	{
		if (this.data == null)
		{
			KGlobalAnimParser kglobalAnimParser = KGlobalAnimParser.Get();
			if (kglobalAnimParser != null)
			{
				this.data = kglobalAnimParser.Load(this);
			}
		}
		return this.data;
	}

	public const string ANIM_ROOT_PATH = "Assets/anim";

	[SerializeField]
	private TextAsset animFile;

	[SerializeField]
	private TextAsset buildFile;

	[SerializeField]
	private List<Texture2D> textures = new List<Texture2D>();

	public KAnimFile.Mod mod;

	private KAnimFileData data;

	private HashedString _batchTag;

	public string homedirectory = string.Empty;

	public class Mod
	{
		public bool IsValid()
		{
			return this.anim != null;
		}

		public byte[] anim;

		public byte[] build;

		public List<Texture2D> textures = new List<Texture2D>();
	}
}
