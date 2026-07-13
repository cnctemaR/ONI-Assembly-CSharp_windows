using System;

namespace Unity.Multiplayer.Center.Common
{
	[Serializable]
	public class SelectedSolutionsData
	{
		public SelectedSolutionsData.HostingModel SelectedHostingModel;

		public SelectedSolutionsData.NetcodeSolution SelectedNetcodeSolution;

		public enum HostingModel
		{
			None,
			ClientHosted,
			DedicatedServer,
			CloudCode,
			DistributedAuthority
		}

		public enum NetcodeSolution
		{
			None,
			NGO,
			N4E,
			CustomNetcode,
			NoNetcode
		}
	}
}
