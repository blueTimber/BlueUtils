using BlueUtils.Properties;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BlueUtils.Editor.Properties
{
	[CustomPropertyDrawer(typeof(PropertyBase), true)]
	public class PropertyDrawerUIE : PropertyDrawer
	{
		public VisualElement CreateOverrideInfo(SerializedProperty property)
		{
			// Create a foldout with left indent and a label "Overrides"
			Foldout foldout = new() { text = "Overrides", value = false, style = { marginLeft = 30 } };
			VisualElement container = new();
			container.SetEnabled(false);
			foldout.Add(container);

			void Rebuild()
			{
				container.Clear();

				object owner = property.serializedObject.targetObject;
				PropertyBase instance = (PropertyBase)fieldInfo.GetValue(owner);
				(string, OverrideType, int, string)?[] overrides = instance.GetOverridesEditor();

				foreach ((string, OverrideType, int, string)? overrideVal in overrides)
				{
					if (!overrideVal.HasValue)
					{
						container.Add(new Label("Null override"));
					}
					else
					{
						(string value, OverrideType type, int priority, string name) = overrideVal.Value;
						container.Add(new Label($"{name} ({type}, {priority}): {value}"));
					}
				}

				if (overrides.Length == 0)
					container.Add(new Label("No overrides"));
			}
			Rebuild();

			SerializedProperty listProp = property.FindPropertyRelative("_overrides").FindPropertyRelative("_list");
			foldout.TrackPropertyValue(listProp, _ => Rebuild());

			return foldout;
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new();
			VisualElement generalInfo = new() { style = { flexDirection = FlexDirection.Row } };

			PropertyField baseField = new(property.FindPropertyRelative("_base"), property.displayName);
			PropertyField currentField = new(property.FindPropertyRelative("_currentVal"));
			currentField.SetEnabled(false);

			baseField.style.flexGrow = 1;
			currentField.style.flexGrow = 1;

			generalInfo.Add(baseField);
			generalInfo.Add(currentField);

			root.Add(generalInfo);
			root.Add(CreateOverrideInfo(property));
			return root;
		}
	}
}
