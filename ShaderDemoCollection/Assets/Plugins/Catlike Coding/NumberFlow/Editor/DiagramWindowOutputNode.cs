// Copyright 2014, Catlike Coding, http://catlikecoding.com
using UnityEditor;
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {

	public class DiagramWindowOutputNode : DiagramWindowNode {

		private const float panelWidth = 200f;
		private const float panelPaddingTop = 20f;
		private const float panelPadding = 4f;
		private const float rowWidth = panelWidth - 2f * panelPadding;
		private const float labelWidth = rowWidth / 3f;
		private const float rowHeight = 18f, rowContentHeight = 16f;

		private static GUIContent
			previewLabel = new GUIContent("Preview"),
			filteringLabel = new GUIContent("Filtering");

		private SerializedProperty outputSP;
		private DiagramOutput output;

		private SerializedProperty
			spName, spType, spPrevieType, spNormalFiltering, spStrength;

		public DiagramWindowOutputNode (int index, DiagramNode node, SerializedProperty nodeSP, DiagramWindow window) :
			base(index, node, nodeSP, window) {

			DiagramOutput[] outputs = (nodeSP.serializedObject.targetObject as Diagram).outputs;
			if (outputs != null) {
				for (int i = 0; i < outputs.Length; i++) {
					if (outputs[i].nodeIndex == index) {
						outputSP = nodeSP.serializedObject.FindProperty("outputs").GetArrayElementAtIndex(i);
						output = outputs[i];
						FindSerializedProperties();
						break;
					}
				}
			}

			nodeRect.width = 100f;
		}

		private void FindSerializedProperties () {
			spName = outputSP.FindPropertyRelative("name");
			spType = outputSP.FindPropertyRelative("type");
			spPrevieType = outputSP.FindPropertyRelative("previewType");
			spNormalFiltering = outputSP.FindPropertyRelative("normalFiltering");
			spStrength = outputSP.FindPropertyRelative("strength");
		}

		public override Rect SelectedWindowRect {
			get {
				float lines;
				switch(output.type) {
				case DiagramTextureType.ARGB:
					lines = 3f;
					break;
				case DiagramTextureType.NormalMap:
					lines = 5f;
					break;
				default:
					lines = 2f;
					break;
				}
				return new Rect(window.position.width - (panelWidth + 20f), 20f, panelWidth, lines * 18f + 23f);
			}
		}

		public override void OnNodeGUI (DiagramWindowEvent e) {
			// Draw the window with property label.
			Rect r = new Rect(1f, 20f, nodeRect.width - DOUBLE_PADDING, 20f);
			GUI.BeginGroup(nodeRect, output.name, IsFocused ? window.activeNodeStyle : window.normalNodeStyle);
			GUI.Label(r, node.GetPropertyName(0));
			GUI.EndGroup();
			
			// Draw the property connector.
			r.x = nodeRect.x - 17f;	
			r.y = nodeRect.y + 21f;
			r.width = r.height = CONNECTOR_SIZE;
			if(e.IsTouchBeginInsideRect(r)) {
				window.StartTransaction(new ConnectionDragTransaction(this, 0, new Vector2(r.x, r.center.y) - window.scrollPosition));
				e.Use();
			}
			GUI.Box(r, typeBoxStrings[(int)node.Function.propertyTypes[0]], window.connectorBoxStyle);
			int argumentIndex = node.argumentIndices[0];
			if (argumentIndex >= 0) {
				DrawConnection(argumentIndex, r);
			}
		}

		public override void OnSelectedWindowGUI (DiagramWindowEvent e) {
			Rect r = SelectedWindowRect;
			GUI.BeginGroup(r, node.Name, window.normalNodeStyle);

			#if !UNITY_4_2
				EditorGUIUtility.labelWidth = labelWidth;
			#endif

			r.x = panelPadding;
			r.y = panelPaddingTop;
			r.height = rowContentHeight;
			r.width = rowWidth;

			EditorGUI.PropertyField(r, spName);
			r.y += rowHeight;
			EditorGUI.PropertyField(r, spType);
			r.y += rowHeight;
			if (output.type == DiagramTextureType.NormalMap) {
				if (node.Diagram.isCubemap) {
					r.height = rowHeight * 3f;
					EditorGUI.HelpBox(r, "Normal maps are not supported for cubemap diagrams.", MessageType.Warning);
				}
				else {
					EditorGUI.PropertyField(r, spPrevieType, previewLabel);
					r.y += rowHeight;
					EditorGUI.PropertyField(r, spNormalFiltering, filteringLabel);
					r.y += rowHeight;
					EditorGUI.PropertyField(r, spStrength);
				}
			}
			else if(output.type == DiagramTextureType.ARGB) {
				EditorGUI.PropertyField(r, spPrevieType, previewLabel);
			}
			GUI.EndGroup();
		}
	}
}