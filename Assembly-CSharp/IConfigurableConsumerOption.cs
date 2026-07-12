using System;
using UnityEngine;

public interface IConfigurableConsumerOption
{
	Tag GetID();

	string GetName();

	string GetDescription();

	Sprite GetIcon();
}
