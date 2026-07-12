using System;

namespace rail
{
	public interface IRailAssetsHelper
	{
		IRailAssets OpenAssets();

		IRailAssets OpenGameServerAssets();
	}
}
