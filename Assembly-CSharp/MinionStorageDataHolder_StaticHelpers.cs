using System;

public static class MinionStorageDataHolder_StaticHelpers
{
	public static void UpdateData<T>(this MinionStorageDataHolder dataHolderComponent, MinionStorageDataHolder.DataPackData data)
	{
		dataHolderComponent.Internal_UpdateData(typeof(T).ToString(), data);
	}

	public static MinionStorageDataHolder.DataPack GetDataPack<T>(this MinionStorageDataHolder dataHolderComponent)
	{
		return dataHolderComponent.Internal_GetDataPack(typeof(T).ToString());
	}
}
