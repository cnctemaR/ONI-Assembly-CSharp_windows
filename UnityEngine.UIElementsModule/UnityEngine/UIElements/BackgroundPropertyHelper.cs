using System;

namespace UnityEngine.UIElements
{
	public static class BackgroundPropertyHelper
	{
		public static BackgroundPosition ConvertScaleModeToBackgroundPosition(ScaleMode scaleMode = ScaleMode.StretchToFill)
		{
			return new BackgroundPosition(BackgroundPositionKeyword.Center);
		}

		public static BackgroundRepeat ConvertScaleModeToBackgroundRepeat(ScaleMode scaleMode = ScaleMode.StretchToFill)
		{
			return new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
		}

		public static BackgroundSize ConvertScaleModeToBackgroundSize(ScaleMode scaleMode = ScaleMode.StretchToFill)
		{
			bool flag = scaleMode == ScaleMode.ScaleAndCrop;
			BackgroundSize backgroundSize;
			if (flag)
			{
				backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
			}
			else
			{
				bool flag2 = scaleMode == ScaleMode.ScaleToFit;
				if (flag2)
				{
					backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
				}
				else
				{
					backgroundSize = new BackgroundSize(Length.Percent(100f), Length.Percent(100f));
				}
			}
			return backgroundSize;
		}

		public static ScaleMode ResolveUnityBackgroundScaleMode(BackgroundPosition backgroundPositionX, BackgroundPosition backgroundPositionY, BackgroundRepeat backgroundRepeat, BackgroundSize backgroundSize, out bool valid)
		{
			bool flag = backgroundPositionX == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.ScaleAndCrop) && backgroundPositionY == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.ScaleAndCrop) && backgroundRepeat == BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.ScaleAndCrop) && backgroundSize == BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.ScaleAndCrop);
			ScaleMode scaleMode;
			if (flag)
			{
				valid = true;
				scaleMode = ScaleMode.ScaleAndCrop;
			}
			else
			{
				bool flag2 = backgroundPositionX == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.ScaleToFit) && backgroundPositionY == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.ScaleToFit) && backgroundRepeat == BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.ScaleToFit) && backgroundSize == BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.ScaleToFit);
				if (flag2)
				{
					valid = true;
					scaleMode = ScaleMode.ScaleToFit;
				}
				else
				{
					bool flag3 = backgroundPositionX == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill) && backgroundPositionY == BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill) && backgroundRepeat == BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill) && backgroundSize == BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.StretchToFill);
					if (flag3)
					{
						valid = true;
						scaleMode = ScaleMode.StretchToFill;
					}
					else
					{
						valid = false;
						scaleMode = ScaleMode.StretchToFill;
					}
				}
			}
			return scaleMode;
		}
	}
}
