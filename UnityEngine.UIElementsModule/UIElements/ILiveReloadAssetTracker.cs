using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal interface ILiveReloadAssetTracker<T> where T : ScriptableObject
	{
		int StartTrackingAsset(T asset);

		void StopTrackingAsset(T asset);

		bool IsTrackingAsset(T asset);

		bool IsTrackingAssets();

		bool CheckTrackedAssetsDirty();

		void UpdateAssetTrackerCounts(T asset, int newDirtyCount, int newElementCount, int newInlinePropertiesCount, int newAttributePropertiesDirtyCount);

		bool OnAssetsImported(HashSet<T> changedAssets, HashSet<string> deletedAssets);

		void OnTrackedAssetChanged();
	}
}
