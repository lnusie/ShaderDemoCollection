// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEditor;
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {

	[CustomPropertyDrawer(typeof(DiagramPixels))]
	public class DiagramPixelDrawer : PropertyDrawer {
		
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			property = property.FindPropertyRelative("texture");
			Texture2D oldTexture = property.objectReferenceValue as Texture2D;
			EditorGUI.PropertyField(position, property, label);
			Texture2D newTexture = property.objectReferenceValue as Texture2D;
			if (newTexture != null && oldTexture != newTexture) {
				try {
					newTexture.GetPixel(0, 0);
				}
				catch {
					Debug.LogWarning("Texture \"" + newTexture.name + "\" is not readable and cannot be used in a NumberFlow diagram. Check \"Read/Write Enabled\" in its advanced import settings.");
					property.objectReferenceValue = oldTexture;
				}
			}
		}
	}
}