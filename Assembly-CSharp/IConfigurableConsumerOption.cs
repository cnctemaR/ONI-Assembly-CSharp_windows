using System;
using UnityEngine;

public interface IConfigurableConsumerOption
{
	Tag GetID();

	string GetName();

	string GetDetailedDescription();

	string GetDescription();

	Sprite GetIcon();

	IConfigurableConsumerIngredient[] GetIngredients();
}
