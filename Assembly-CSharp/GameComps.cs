using System;
using System.Collections.Generic;
using System.Reflection;

public class GameComps : KComponents
{
	public GameComps()
	{
		FieldInfo[] fields = typeof(GameComps).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object obj = Activator.CreateInstance(fieldInfo.FieldType);
			fieldInfo.SetValue(null, obj);
			base.Add<IComponentManager>(obj as IComponentManager);
			if (obj is IKComponentManager)
			{
				IKComponentManager ikcomponentManager = obj as IKComponentManager;
				GameComps.AddKComponentManager(fieldInfo.FieldType, ikcomponentManager);
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		FieldInfo[] fields = typeof(GameComps).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(null, null);
		}
		GameComps.kcomponentManagers.Clear();
	}

	public static void AddKComponentManager(Type kcomponent, IKComponentManager inst)
	{
		GameComps.kcomponentManagers[kcomponent] = inst;
	}

	public static IKComponentManager GetKComponentManager(Type kcomponent_type)
	{
		return GameComps.kcomponentManagers[kcomponent_type];
	}

	public static GravityComponents Gravities;

	public static FallerComponents Fallers;

	public static InfraredVisualizerComponents InfraredVisualizers;

	public static ElementSplitterComponents ElementSplitters;

	public static OreSizeVisualizerComponents OreSizeVisualizers;

	public static StructureTemperatureComponents StructureTemperatures;

	public static DiseaseContainers DiseaseContainers;

	public static RequiresFoundation RequiresFoundations;

	public static WhiteBoard WhiteBoards;

	private static Dictionary<Type, IKComponentManager> kcomponentManagers = new Dictionary<Type, IKComponentManager>();
}
