using System;
using UnityEngine;

public interface IPolluter
{
	int GetRadius();

	int GetNoise();

	void SetAttributes(Vector2 pos, int dB, string name = null);

	string GetName();

	void AddCell(Pair<int, int> cell);

	Pair<int, int> GetCell(int index);

	int GetCellCount();

	void Clear();

	Vector2 GetPosition();
}
