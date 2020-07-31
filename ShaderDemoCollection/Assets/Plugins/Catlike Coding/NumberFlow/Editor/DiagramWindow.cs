// Copyright 2013, Catlike Coding, http://catlikecoding.com
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {
	
	public class DiagramWindow : EditorWindow {
		
		private Diagram diagram;
		
		private SerializedObject diagramSO;
		private SerializedProperty nodesSP;
		
		private List<DiagramWindowTransaction> transactions;
		
		private int selectedNodeIndex;
		
		private Texture2D previewTexture;
		
		private Vector2 contextMenuPosition;
		
		private Rect canvas = new Rect();
		
		private Color[] pixels = new Color[64 * 64];
		
		public Color focusedConnectionColor;
		
		public Vector2 scrollPosition;
		
		public DiagramWindowNode[] windowNodes;
		
		public DiagramWindowEvent diagramEvent;
		
		public GUIStyle nameLabelStyle, normalNodeStyle, activeNodeStyle, connectorBoxStyle, fixedValueStyle;

		private bool disabled;
		
		public void StartTransaction (DiagramWindowTransaction t) {
			transactions.Add(t);
		}
		
		private void InitializeStyles () {
			diagramEvent = new DiagramWindowEvent();
			
			focusedConnectionColor = Color.white * (GUI.skin.name == "LightSkin" ? 0.25f : 0.75f);
			focusedConnectionColor.a = 1f;
			
			nameLabelStyle = new GUIStyle(GUI.skin.label);
			nameLabelStyle.alignment = TextAnchor.MiddleCenter;
			
			normalNodeStyle = new GUIStyle(GUI.skin.window);
			normalNodeStyle.font = GUI.skin.label.font;
			activeNodeStyle = new GUIStyle(normalNodeStyle);
			activeNodeStyle.normal = activeNodeStyle.onNormal;
			
			connectorBoxStyle = new GUIStyle(GUI.skin.box);
			connectorBoxStyle.contentOffset = Vector2.zero;
			connectorBoxStyle = new GUIStyle(GUI.skin.box);
			connectorBoxStyle.alignment = TextAnchor.LowerLeft;
			connectorBoxStyle.clipping = TextClipping.Overflow;
			connectorBoxStyle.fontSize = 8;
			connectorBoxStyle.contentOffset = new Vector2(-1f, 0f);
			connectorBoxStyle.wordWrap = false;

			fixedValueStyle = new GUIStyle(GUI.skin.label);
			fixedValueStyle.alignment = TextAnchor.LowerRight;
			fixedValueStyle.fontSize = 8;
		}

		private void InitializeWindow () {
			diagramSO = new SerializedObject(diagram);
			nodesSP = diagramSO.FindProperty("nodes");
			diagramEvent = new DiagramWindowEvent();
			if (selectedNodeIndex >= diagram.nodes.Length) {
				selectedNodeIndex = -1;
			}
			
			windowNodes = new DiagramWindowNode[diagram.nodes.Length];
			for (int i = 0; i < diagram.nodes.Length; i++) {
				if (diagram.nodes[i].Function == null || diagram.nodes[i].Function.type != FunctionType.Output) {
					windowNodes[i] = new DiagramWindowNode(i, diagram.nodes[i], nodesSP.GetArrayElementAtIndex(i), this);
				}
				else {
					windowNodes[i] = new DiagramWindowOutputNode(i, diagram.nodes[i], nodesSP.GetArrayElementAtIndex(i), this);
				}
				if (selectedNodeIndex == i) {
					windowNodes[i].IsFocused = true;
				}
			}
			ComputeCanvas();
			transactions = new List<DiagramWindowTransaction>();
			diagram.prepared = false;
		}
		
		private void OnEnable () {
			previewTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false, true);
			previewTexture.name = "NumberFlow preview";
			previewTexture.hideFlags = HideFlags.HideAndDontSave;
			normalNodeStyle = null;
		}
		
		private void OnDisable () {
			DestroyImmediate(previewTexture);
		}
		
		private void OnFocus () {
			Diagram selectedDiagram = Selection.activeObject as Diagram;
			if (selectedDiagram && selectedDiagram != diagram) {
				// Another diagram is currently selected.
				diagram = selectedDiagram;
				diagramSO = null;
				Repaint();
			}
		}
		
		private void OnProjectChange () {
			if (!diagram) {
				// The diagram has been deleted.
				diagramSO = null;
				Repaint();
			}
		}
		
		private void OnSelectionChange () {
			Diagram selectedDiagram = Selection.activeObject as Diagram;
			if (selectedDiagram && selectedDiagram != diagram) {
				// Another diagram has been selected.
				diagram = selectedDiagram;
				selectedNodeIndex = -1;
				diagramSO = null;
				scrollPosition = Vector2.zero;
				Repaint();
			}
		}
		
		private void OnGUI () {
			if (disabled) {
				GUI.Label(position, "Diagram editor is disabled while in play mode.");
				return;
			}
			if (normalNodeStyle == null) {
				InitializeStyles();
			}
			if (diagram == null) {
				// No diagram to show.
				return;
			}
			if (diagram.nodes.Length > 0 && diagram.nodes[0].Function == null) {
				// Intialize if needed.
				diagram.Init();
			}
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed") {
				// Start from scratch after detecting undo or redo.
				diagramSO = null;
				Repaint();
				return;
			}
			if(diagramSO == null) {
				InitializeWindow();
			}
			diagramSO.Update();
			
			diagramEvent.Reset(windowNodes, scrollPosition, selectedNodeIndex);
			
			for (int i = 0; i < transactions.Count; i++) {
				if (!transactions[i].OnGUI(diagramEvent)) {
					transactions.RemoveAt(i--);
				}
			}

			if (diagramEvent.type == DiagramWindowEventType.ContextClick) {
				if (diagramEvent.targetWindowNode != null) {
					DoNodeContextMenu(diagramEvent.targetWindowNode);
					diagramEvent.Use();
					return;
				}
				if (PreviewRect.Contains(diagramEvent.touchPosition)) {
					ShowPreviewContextMenu();
				}
				else {
					contextMenuPosition = diagramEvent.scrolledTouchPosition;
					DoContextMenu();
				}
				diagramEvent.Use();
				return;
			}
			
			if (diagramEvent.type == DiagramWindowEventType.TouchBegin) {
				// Clear input focus.
				GUI.FocusControl("");
			}
			if (diagramEvent.type == DiagramWindowEventType.TouchBegin && diagramEvent.targetWindowNode != null) {
				if (selectedNodeIndex != diagramEvent.targetWindowNode.index) {
					if (selectedNodeIndex >= 0) {
						windowNodes[selectedNodeIndex].IsFocused = false;
					}
					selectedNodeIndex = diagramEvent.targetWindowNode.index;
					diagramEvent.targetWindowNode.IsFocused = true;
					Repaint();
				}
				selectedNodeIndex = diagramEvent.targetWindowNode.index;
				
				StartTransaction(new NodeDragTransaction(diagramEvent.targetWindowNode, diagramEvent.touchPosition));
			}
			
			GUI.Label(new Rect(0f, 0f, position.width, 20f), diagram.name, nameLabelStyle);
			
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, position.width, position.height), scrollPosition, canvas);
			for (int i = 0; i < windowNodes.Length; i++) {
				windowNodes[i].OnNodeGUI(diagramEvent);
			}
			GUI.EndScrollView();
			
			if (selectedNodeIndex >= 0 && selectedNodeIndex < nodesSP.arraySize) {
				windowNodes[selectedNodeIndex].OnSelectedWindowGUI(diagramEvent);
			}
			
			if (diagramSO != null && diagramSO.ApplyModifiedProperties()) {
				diagram.prepared = false;
			}
			
			if (diagramEvent.type == DiagramWindowEventType.Used) {
				// Something happened, request a repaint.
				ComputeCanvas();
				Repaint();
				return;
			}
			
			if (diagramEvent.type == DiagramWindowEventType.Repaint && diagram.outputs != null && diagram.outputs.Length > 0) {
				System.Action<Rect, Texture2D> drawMethod = drawMethods[(int)diagram.outputs[diagram.previewOutputIndex].type, (int)diagram.outputs[diagram.previewOutputIndex].previewType];
				GUI.BeginGroup(PreviewRect, diagram.outputs[diagram.previewOutputIndex].name, normalNodeStyle);
				Rect r;
				if (diagram.isCubemap) {
					// Flip preview.
					if (diagram.previewCubemapType == DiagramCubemapPreviewType.Inside) {
						r = new Rect(1f, 16f + diagram.height, diagram.width, -diagram.height);
					}
					else {
						r = new Rect(1f + diagram.width, 16f + diagram.height, -diagram.width, -diagram.height);
					}
				}
				else {
					r = new Rect(1f, 16f, diagram.width, diagram.height);
				}
				drawMethod(r, previewTexture);
				if (diagram.tilePreviewHorizontally) {
					r.x += diagram.width;
					drawMethod(r, previewTexture);
				}
				if (diagram.tilePreviewVertically) {
					r.y += diagram.height;
					drawMethod(r, previewTexture);
					if (diagram.tilePreviewHorizontally) {
						r.x -= diagram.width;
						drawMethod(r, previewTexture);
					}
				}
				GUI.EndGroup();
			}
		}

		private static System.Action<Rect, Texture2D>[,] drawMethods = {
			{ EditorGUI.DrawTextureTransparent, EditorGUI.DrawPreviewTexture, EditorGUI.DrawTextureAlpha },
			{ EditorGUI.DrawPreviewTexture, EditorGUI.DrawPreviewTexture, EditorGUI.DrawPreviewTexture },
			{ EditorGUI.DrawTextureAlpha, EditorGUI.DrawTextureAlpha, EditorGUI.DrawTextureAlpha },
			{ EditorGUI.DrawTextureTransparent, EditorGUI.DrawPreviewTexture, EditorGUI.DrawTextureAlpha }
		};

		private Rect PreviewRect {
			get {
				Rect r = position;
				r.width = diagram.width + 1f;
				if (diagram.tilePreviewHorizontally) {
					r.width += diagram.width;
				}
				r.height = diagram.height + 17f;
				if (diagram.tilePreviewVertically) {
					r.height += diagram.height;
				}
				r.x = position.width - r.width - 20f;
				r.y = position.height - r.height - 20f;
				return r;
			}
		}
		
		private float previousTime;
		private int currentRow = -1;
		private int rowsPerUpdate = 1;
		
		private void Update () {
			if (disabled != Application.isPlaying) {
				disabled = Application.isPlaying;
				Repaint();
			}
			if (disabled) {
				return;
			}
			float deltaTime = Time.realtimeSinceStartup - previousTime;
			previousTime = Time.realtimeSinceStartup;
			
			if (diagram == null) {
				return;
			}
			if (previewTexture.width != diagram.width || previewTexture.height != diagram.height) {
				previewTexture.Resize(diagram.width, diagram.height);
				pixels = new Color[diagram.width * diagram.height];
				previewTexture.SetPixels(pixels);
				previewTexture.Apply();
			}
			if (diagram.previewOutputIndex < 0 && diagram.outputs != null && diagram.outputs.Length > 0) {
				diagram.previewOutputIndex = 0;
			}
			
			if (!diagram.prepared) {
				currentRow = 0;
				// Precaution slow down.
				rowsPerUpdate = rowsPerUpdate / 2 + 1;
			}
			if (currentRow < diagram.height) {
				if (currentRow > 0) {
					// We're filling rows.
					if (rowsPerUpdate <= 0) {
						// We took a break, now continue.
						rowsPerUpdate = 1;
					}
					else if (deltaTime < 0.05f) {
						// We can speed up.
						rowsPerUpdate *= 2;
					}
					else if (deltaTime > 0.1f) {
						// Emergency stop.
						rowsPerUpdate = 0;
					}
					else if (deltaTime > 0.05f) {
						// Slow down.
						rowsPerUpdate /= 2;
					}
				}
				else if (diagram.animatePreview) {
					Value inputValue = diagram.GetInputValue(diagram.animatePreviewInput);
					if (inputValue != null) {
						inputValue.Float = Time.realtimeSinceStartup;
					}
				}
				if (rowsPerUpdate > 0) {
					diagram.cubemapDirection = diagram.previewCubemapDirection;
					currentRow = diagram.FillRows(pixels, diagram.previewOutputIndex, currentRow, rowsPerUpdate);
					if (diagram.previewOutputIndex < diagram.outputs.Length &&
					    diagram.outputs[diagram.previewOutputIndex].type == DiagramTextureType.NormalMap) {
						diagram.outputs[diagram.previewOutputIndex].GenerateNormalMap(diagram.width, diagram.height, pixels, DiagramNormalFormat.RGB);
					}

					if (!diagram.animatePreview) {
						previewTexture.SetPixels(pixels);
						previewTexture.Apply();
						Repaint();
					}
					else if (currentRow >= diagram.height) {
						previewTexture.SetPixels(pixels);
						previewTexture.Apply();
						Repaint();
					}
				}
			}
			else if (diagram.animatePreview && !EditorApplication.isPlaying) {
				currentRow = 0;
			}
		}
		
		private void ComputeCanvas () {
			float width = 0f, height = 0f;
			foreach (DiagramWindowNode node in windowNodes) {
				width = Mathf.Max(width, node.nodeRect.xMax);
				height = Mathf.Max(height, node.nodeRect.yMax);
			}
			canvas.width = width + 20f;
			canvas.height = height + 20f;
		}
		
		private void DoNodeContextMenu (DiagramWindowNode node) {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Delete"), false, DeleteNodeCallback, node);
			menu.AddItem(new GUIContent("Duplicate"), false, DuplicateNodeCallback, node);
			menu.ShowAsContext();
		}
		
		private void DoContextMenu () {
			GenericMenu menu = new GenericMenu();
			foreach (Function function in DefaultFunctionLibrary.Instance.ListFunctions()) {
				menu.AddItem(new GUIContent(function.menuName), false, AddNodeCallback, function);
			}
			foreach (FunctionLibrary library in diagram.functionLibraries) {
				if (library != null) {
					foreach (Function function in library.ListFunctions()) {
						menu.AddItem(new GUIContent(function.menuName), false, AddNodeCallback, function);
					}
				}
			}
			menu.ShowAsContext();
		}

		private void ShowPreviewContextMenu () {
			if (diagram.outputs.Length == 0) {
				return;
			}
			GenericMenu menu = new GenericMenu();
			for (int i = 0; i < diagram.outputs.Length; i++) {
				menu.AddItem(new GUIContent(diagram.outputs[i].name), i == diagram.previewOutputIndex, ShowPreviewCallback, i);
			}
			menu.ShowAsContext();
		}

		private void ShowPreviewCallback (object index) {
			diagramSO.Update();
			diagramSO.FindProperty("previewOutputIndex").intValue = (int)index;
			diagramSO.ApplyModifiedProperties();
			diagramSO = null;
			Repaint();
		}
		
		private void DeleteNodeCallback (object node) {
			selectedNodeIndex = (node as DiagramWindowNode).index;
			DeleteSelectedNode();
		}
		
		private void DuplicateNodeCallback (object node) {
			int index = (node as DiagramWindowNode).index;
			SerializedProperty nodesSP = diagramSO.FindProperty("nodes");
			diagramSO.Update();
			nodesSP.InsertArrayElementAtIndex(index);
			nodesSP.GetArrayElementAtIndex(index + 1).FindPropertyRelative("position").vector2Value += new Vector2(10f, 10f);
			nodesSP.MoveArrayElement(index + 1, nodesSP.arraySize - 1);

			if (diagram.nodes[index].Function.type == FunctionType.Output) {
				AddOutput(nodesSP.arraySize - 1);
			}

			diagramSO.ApplyModifiedProperties();
			diagramSO = null;
			selectedNodeIndex = diagram.nodes.Length - 1;
			Repaint();
		}
		
		private void AddNodeCallback (object function) {
			diagramSO.Update();
			SerializedProperty spNodes = diagramSO.FindProperty("nodes");
			int index = spNodes.arraySize;
			spNodes.InsertArrayElementAtIndex(index);
			SerializedProperty spNode = spNodes.GetArrayElementAtIndex(index);
			spNode.FindPropertyRelative("functionId").stringValue = function.GetType().FullName;
			spNode.FindPropertyRelative("position").vector2Value = contextMenuPosition;
			spNode.FindPropertyRelative("argumentIndices").arraySize = 0;
			spNode.FindPropertyRelative("fixedValues").arraySize = 0;

			if ((function as Function).type == FunctionType.Output) {
				AddOutput(index);
			}

			diagramSO.ApplyModifiedProperties();
			diagramSO = null;
			selectedNodeIndex = diagram.nodes.Length - 1;
			Repaint();
		}

		private void AddOutput(int nodeIndex) {
			SerializedProperty spOutputs = diagramSO.FindProperty("outputs");
			int outputIndex = spOutputs.arraySize;
			spOutputs.InsertArrayElementAtIndex(outputIndex);
			spOutputs.GetArrayElementAtIndex(outputIndex).FindPropertyRelative("nodeIndex").intValue = nodeIndex;
		}
		
		public void DeleteSelectedNode () {
			SerializedProperty nodesSO = diagramSO.FindProperty("nodes");
			nodesSO.DeleteArrayElementAtIndex(selectedNodeIndex);
			// Adjust all indices.
			for (int i = 0, l = nodesSO.arraySize; i < l; i++) {
				SerializedProperty argumentIndicesSO = nodesSO.GetArrayElementAtIndex(i).FindPropertyRelative("argumentIndices");
				for (int iA = 0, lA = argumentIndicesSO.arraySize; iA < lA; iA++) {
					SerializedProperty indexSO = argumentIndicesSO.GetArrayElementAtIndex(iA);
					if (indexSO.intValue == selectedNodeIndex) {
						// Drop connection.
						indexSO.intValue = -1;
					}
					else if (indexSO.intValue > selectedNodeIndex) {
						// Shift index.
						indexSO.intValue -= 1;
					}
				}
			}
			UpdateOutputsAfterNodeDeletion(selectedNodeIndex);
			diagramSO.ApplyModifiedProperties();
			diagramSO = null;
			diagram.Init();
			selectedNodeIndex = -1;
		}

		private void UpdateOutputsAfterNodeDeletion (int deletedIndex) {
			SerializedProperty spOutputs = diagramSO.FindProperty("outputs");
			for (int i = 0, l = spOutputs.arraySize; i < l; i++) {
				SerializedProperty spNodeIndex = spOutputs.GetArrayElementAtIndex(i).FindPropertyRelative("nodeIndex");
				if (spNodeIndex.intValue > selectedNodeIndex) {
					// Shift index.
					spNodeIndex.intValue -= 1;
				}
				else if (spNodeIndex.intValue == selectedNodeIndex) {
					// Drop output.
					if (diagram.previewOutputIndex >= i) {
						diagramSO.FindProperty("previewOutputIndex").intValue -= 1;
					}
					spOutputs.DeleteArrayElementAtIndex(i--);
					l -= 1;
				}
			}
		}
		
		[MenuItem("Window/NumberFlow")]
		public static void OpenWindow () {
			EditorWindow.GetWindow<DiagramWindow>(false, "NumberFlow");
		}
	}
}