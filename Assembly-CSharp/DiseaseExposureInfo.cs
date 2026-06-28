using System;

[Serializable]
public struct DiseaseExposureInfo
{
	public DiseaseExposureInfo(string id, string infection_source_info)
	{
		this.diseaseID = id;
		this.infectionSourceInfo = infection_source_info;
	}

	public string diseaseID;

	public string infectionSourceInfo;
}
