using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStorage
{
	bool ShouldShowInUI();

	bool allowUIItemRemoval { get; set; }

	GameObject Drop(GameObject go, bool do_disease_transfer = true);

	List<GameObject> GetItems();

	bool IsFull();

	bool IsEmpty();

	float Capacity();

	float RemainingCapacity();

	float GetAmountAvailable(Tag tag);

	void ConsumeIgnoringDisease(Tag tag, float amount);
}
