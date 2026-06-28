using System;
using UnityEngine;

public interface IUsable
{
	bool IsUsable();

	Transform transform { get; }
}
