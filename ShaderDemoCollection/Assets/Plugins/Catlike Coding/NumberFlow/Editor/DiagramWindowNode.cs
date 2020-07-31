// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEditor;
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {
	
	public class DiagramWindowNode {
		
		public const float CONNECTOR_SIZE = 14f;
		public const float CONNECTOR_OFFSET_Y = 16f;
		public const float PADDING = 4f;
		public const float DOUBLE_PADDING = PADDING + PADDING;
		
		protected static string[] typeBoxStrings = new string[]{
			" -",
			" B",
			" C",
			" F",
			" I",
			" S",
			"V2",
			"V3",
			"Cu",
			" G",
			" P"
		};
		
		public float propertyNameLabelWidth, nodeNameLabelWidth;
		
		public Rect nodeRect;
		
		public int index;
		public DiagramNode node;
		public SerializedProperty nodeSP;
		
		public bool IsFocused;
		
		protected DiagramWindow window;
		
		public DiagramWindowNode (int index, DiagramNode node, SerializedProperty nodeSP, DiagramWindow window) {
			this.index = index;
			this.node = node;
			this.nodeSP = nodeSP;
			this.window = window;
			propertyNameLabelWidth = 0f;
			nodeNameLabelWidth = GUI.skin.label.CalcSize(new GUIContent(node.Name)).x;
			nodeRect.width = nodeNameLabelWidth;
			for (int i = 0, l = node.PropertyCount; i < l; i++) {
				float labelWidth = GUI.skin.label.CalcSize(new GUIContent(node.GetPropertyName(i))).x;
				if (labelWidth > nodeRect.width) {
					nodeRect.width = labelWidth;
				}
				if (labelWidth > propertyNameLabelWidth) {
					propertyNameLabelWidth = labelWidth;
				}
			}
			nodeRect.width += DOUBLE_PADDING;
			if (node.Function == null || node.Function.type != FunctionType.Input) {
				nodeRect.height = Mathf.Max(1, node.PropertyCount) * CONNECTOR_OFFSET_Y + 22f;
			}
			else {
				nodeRect.height = CONNECTOR_OFFSET_Y + 22f;
			}
			nodeRect.x = node.position.x;
			nodeRect.y = node.position.y;
		}
		
		public Vector2 Position {
			get {
				return node.position;
			}
			set {
				if (value.x < 20f) {
					value.x = 20f;
				}
				if (value.y < 20f) {
					value.y = 20f;
				}
				nodeRect.x = value.x;
				nodeRect.y = value.y;
				nodeSP.FindPropertyRelative("position").vector2Value = new Vector2(value.x, value.y);
			}
		}

		public Vector2 OutputConnectionPosition {
			get {
				if (node.Function == null || node.Function.type != FunctionType.Input) {
					return new Vector2(nodeRect.xMax + 3f, nodeRect.y + 21f + Mathf.Max(0f, node.PropertyCount - 1) * (CONNECTOR_OFFSET_Y * 0.5f));	
				}
				return new Vector2(nodeRect.xMax + 3f, nodeRect.y + 21f);
			}
		}
		
		public Vector2 OutputConnectorPosition {
			get {
				Vector2 p = OutputConnectionPosition;
				p.x += CONNECTOR_SIZE;
				p.y += CONNECTOR_SIZE * 0.5f;
				return p;
			}
		}
		
		public Rect OutputConnectionRect {
			get {
				Vector2 p = OutputConnectionPosition;
				return new Rect (p.x, p.y, CONNECTOR_SIZE, CONNECTOR_SIZE);
			}
		}
		
		public bool Contains (Vector2 point) {
			return nodeRect.Contains(point);
		}
		
		public virtual void OnNodeGUI (DiagramWindowEvent e) {
			// Draw the window with property labels.
			Rect r = new Rect(1f, 20f, nodeRect.width - DOUBLE_PADDING, 20f);
			GUI.BeginGroup(nodeRect, node.Name, IsFocused ? window.activeNodeStyle : window.normalNodeStyle);
			
			if (node.Function == null || node.Function.type != FunctionType.Input) {
				for (int i = 0, l = node.PropertyCount; i < l; i++) {
					GUI.Label(r, node.GetPropertyName(i));
					r.y += 16f;
				}
				GUI.EndGroup();
				
				// Draw the property connectors.
				float
					xOffset = nodeRect.x - 17f,
					yOffset = nodeRect.y + 21f;
				r.width = r.height = CONNECTOR_SIZE;
				for (int i = 0, l = node.PropertyCount; i < l; i++) {
					r.x = xOffset;	
					r.y = yOffset;
					if(e.IsTouchBeginInsideRect(r)) {
						window.StartTransaction(new ConnectionDragTransaction(this, i, new Vector2(r.x, r.center.y) - window.scrollPosition));
						e.Use();
					}
					GUI.Box(r, node.Function == null ? " ?" : typeBoxStrings[(int)node.Function.propertyTypes[i]], window.connectorBoxStyle);
					int argumentIndex = node.argumentIndices[i];
					if (argumentIndex >= 0) {
						DrawConnection(argumentIndex, r);
					}
					else {
						DrawFixedValue(i, r);
					}
					yOffset += CONNECTOR_OFFSET_Y;
				}
			}
			else {
				GUI.Label(r, node.GetPropertyName(0));
				GUI.EndGroup();
			}
			
			// Draw the result connector.
			if (node.Function == null) {
				GUI.Box(OutputConnectionRect, " ?", window.connectorBoxStyle);
			}
			else if (node.Function.returnType != ValueType.None) {
				GUI.Box(OutputConnectionRect, typeBoxStrings[(int)node.Function.returnType], window.connectorBoxStyle);
			}
		}
		
		protected void DrawConnection (int argumentIndex, Rect connectorRect) {
			Vector3 e = window.windowNodes[argumentIndex].OutputConnectorPosition;
			Vector3 b = connectorRect.center;
			b.x -= CONNECTOR_SIZE * 0.5f;
			
			float factor = Mathf.Max(Mathf.Abs(b.x - e.x), Mathf.Abs(b.y - e.y)) * 0.5f;
			Vector3 bTangent = b, eTangent = e;
			bTangent.x -= factor;
			eTangent.x += factor;
			Handles.DrawBezier(b, e, bTangent, eTangent, IsFocused ? window.focusedConnectionColor : Color.gray, null, 2f);
		}

		protected void DrawFixedValue (int propertyIndex, Rect r) {
			Value value = node.fixedValues[propertyIndex];
			r.x -= 50f;
			r.y -= 1f;
			r.width = 50f;
			switch (node.Function.propertyTypes[propertyIndex]) {
			case ValueType.Float:
				GUI.Label(r, value.Float.ToString(), window.fixedValueStyle);
				break;
			case ValueType.Int:
				GUI.Label(r, value.Int.ToString(), window.fixedValueStyle);
				break;
			}
		}
		
		public bool SetArgument (int argumentIndex, DiagramWindowNode argumentNode) {
			if (argumentNode == null && node.argumentIndices[argumentIndex] < 0 || argumentNode != null && argumentNode.index == node.argumentIndices[argumentIndex]) {
				// No change.
				return false;
			}
			if (argumentNode == null) {
				// Disconnect property.
				nodeSP.FindPropertyRelative("argumentIndices").GetArrayElementAtIndex(argumentIndex).intValue = -1;
				return true;
			}
			if (node.Function.propertyTypes[argumentIndex] != argumentNode.node.Function.returnType) {
				window.ShowNotification(new GUIContent("Cannot connect " + node.Function.propertyTypes[argumentIndex] + " to " + argumentNode.node.Function.returnType));
				return false;
			}
			if(argumentNode.node.IsDependentOn(node)) {
				window.ShowNotification(new GUIContent("Cannot create a loop."));
				return false;
			}
			nodeSP.FindPropertyRelative("argumentIndices").GetArrayElementAtIndex(argumentIndex).intValue = argumentNode.index;
			return true;
		}
		
		public virtual Rect SelectedWindowRect {
			get {
				float width = Mathf.Max(108f, Mathf.Max(propertyNameLabelWidth * 2f + 8f, nodeNameLabelWidth + 8f));
				return new Rect(window.position.width - width - 20f, 20f, width, (node.PropertyCount + 1) * 18f + 23f);
			}
		}
		
		public virtual void OnSelectedWindowGUI (DiagramWindowEvent e) {
			SerializedProperty fixedValuesSP = nodeSP.FindPropertyRelative("fixedValues");
			
			float width = Mathf.Max(108f, Mathf.Max(propertyNameLabelWidth * 2f + 8f, nodeNameLabelWidth + 8f));
			float labelWidth = (width - 8f) * 0.5f;
			
			Rect r = new Rect(window.position.width - width - 20f, 20f, width, (node.PropertyCount + 1) * 18f + 23f);
			GUI.BeginGroup(r, node.Name, window.normalNodeStyle);
			r.y = 20f;
			r.height = 16f;
			if (node.Function != null) {
				#if !UNITY_4_2
					EditorGUIUtility.labelWidth = labelWidth;
				#endif
				for (int i = 0, l = fixedValuesSP.arraySize; i < l; i++) {
					r.x = 4f;
					if (node.argumentIndices[i] >= 0) {
						r.width = labelWidth;
						GUI.Label(r, node.GetPropertyName(i));
						r.x = labelWidth + 4f;
						GUI.Label(r, "linked");
					}
					else {
						SerializedProperty field = fixedValuesSP.GetArrayElementAtIndex(i).
							FindPropertyRelative(node.Function.propertyTypes[i].ToString());
						r.width = width - 8f;
						if (field.propertyType == SerializedPropertyType.Vector3) {
							Rect subR = r;
							subR.width *= 0.5f;
							GUI.Label(subR, node.GetPropertyName(i));
							subR.x += subR.width;
							subR.width = subR.width / 3f - 1f;
							EditorGUI.PropertyField(subR, field.FindPropertyRelative("x"), GUIContent.none);
							subR.x += subR.width + 1f;
							EditorGUI.PropertyField(subR, field.FindPropertyRelative("y"), GUIContent.none);
							subR.x += subR.width + 1f;
							EditorGUI.PropertyField(subR, field.FindPropertyRelative("z"), GUIContent.none);
						}
						else if (field.propertyType == SerializedPropertyType.Vector2) {
							Rect subR = r;
							subR.width *= 0.5f;
							GUI.Label(subR, node.GetPropertyName(i));
							subR.x += subR.width;
							subR.width = subR.width / 2f - 1f;
							EditorGUI.PropertyField(subR, field.FindPropertyRelative("x"), GUIContent.none);
							subR.x += subR.width + 1f;
							EditorGUI.PropertyField(subR, field.FindPropertyRelative("y"), GUIContent.none);
						}
						else {
							EditorGUI.PropertyField(r, field, new GUIContent(node.GetPropertyName(i)));
						}
					}
					r.y += 18f;
				}
			}
			r.x = 4f;
			r.width = width - 8f;
			if (GUI.Button(r, "Delete")) {
				window.DeleteSelectedNode();
			}
			GUI.EndGroup();
		}
	}
}