using System;

public interface IShearable
{
	bool IsFullyGrown();

	void Shear();

	global::Tuple<Tag, float> GetItemDroppedOnShear();
}
