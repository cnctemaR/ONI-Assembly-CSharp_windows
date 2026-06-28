using System;

[Serializable]
public struct DiseaseExposureInfo
{
	public DiseaseExposureInfo(string id, float exposure_count, string infection_source_info)
	{
		this.diseaseID = id;
		this.exposureCount = exposure_count;
		this.infectionSourceInfo = infection_source_info;
	}

	public string diseaseID;

	public float exposureCount;

	public string infectionSourceInfo;
}
