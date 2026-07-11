using System;

[Serializable]
public struct SicknessExposureInfo
{
	public SicknessExposureInfo(string id, string infection_source_info)
	{
		this.sicknessID = id;
		this.sourceInfo = infection_source_info;
	}

	public string sicknessID;

	public string sourceInfo;
}
