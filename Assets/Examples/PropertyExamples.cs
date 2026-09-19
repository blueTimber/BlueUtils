using BlueUtils.Datastructures;
using BlueUtils.Properties;
using UnityEngine;

public class PropertyExamples : MonoBehaviour, IPropertyOverrider
{
    public Property<float> FloatProperty;
    public Property<bool> BoolProperty;
    public Property<string> StringProperty;

	public SortedList<int, float> Overrides;

	public string Name => "Examples";

	private void Start()
	{
		FloatProperty.Override(OverrideType.Add, 10f, 1, this);
	}
}
