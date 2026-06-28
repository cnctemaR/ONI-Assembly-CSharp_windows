using System;
using System.IO;

public interface ISaveLoadableDetailJson
{
	void Serialize(BinaryWriter writer);

	void Deserialize(IReader reader);
}
