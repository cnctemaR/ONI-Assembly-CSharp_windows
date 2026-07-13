using System;

public class EntitySizeVisualizer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		OreSizeVisualizerData oreSizeVisualizerData = new OreSizeVisualizerData(base.gameObject);
		oreSizeVisualizerData.tierSetType = this.TierSetType;
		GameComps.OreSizeVisualizers.Add(base.gameObject, oreSizeVisualizerData);
		base.OnPrefabInit();
	}

	protected override void OnCleanUp()
	{
		GameComps.OreSizeVisualizers.Remove(base.gameObject);
		base.OnCleanUp();
	}

	public OreSizeVisualizerComponents.TiersSetType TierSetType;
}
