using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimFileManager
{
	public static KAnimFileManager Get()
	{
		if (KAnimFileManager.instance == null)
		{
			KAnimFileManager.instance = new KAnimFileManager();
		}
		return KAnimFileManager.instance;
	}

	public void Put(KAnimFile anim_file, KAnimFileData data)
	{
		int instanceID = anim_file.GetInstanceID();
		this.files[instanceID] = data;
	}

	public KAnimFileData Load(KAnimFile anim_file)
	{
		KAnimFileData kanimFileData = null;
		int instanceID = anim_file.GetInstanceID();
		if (!this.files.TryGetValue(instanceID, out kanimFileData) || !Application.isPlaying)
		{
			kanimFileData = KGlobalAnimParser.Get().Parse(anim_file);
			this.files[instanceID] = kanimFileData;
		}
		return kanimFileData;
	}

	private static KAnimFileManager instance;

	private Dictionary<int, KAnimFileData> files = new Dictionary<int, KAnimFileData>();
}
