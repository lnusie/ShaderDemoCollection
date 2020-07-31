// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEditor;
using UnityEngine;
using System.IO;

namespace CatlikeCoding.NumberFlow.Editor {
	
	[CanEditMultipleObjects, CustomEditor(typeof(Diagram))]
	public sealed class DiagramEditor : UnityEditor.Editor {

		private static GUIContent
			tileHorizontallyLabel = new GUIContent("Horizontally"),
			tileVerticallyLabel = new GUIContent("Vertically"),
			animatePreviewInputLabel = new GUIContent("Input Name"),
			cubemapPreviewDirectionLabel = new GUIContent("Preview Direction"),
			cubemapPreviewTypeLabel = new GUIContent("Preview Type"),
			exportAnimationLoopLabel = new GUIContent("Loop"),
			exportAnimationFramesLabel = new GUIContent("Frames"),
			exportAnimationFromLabel = new GUIContent("From"),
			exportAnimationToLabel = new GUIContent("To"),
			exportAnimationInputLabel = new GUIContent("Input Name");

		private SerializedObject diagramSO;
		private SerializedProperty
			libraries, width, height, exportWidth, exportHeight, tileH, tileV, isCubemap, cubemapDirection, cubemapType,
			animate, animateName, exportAnimation, exportAnimationLoop, exportAnimationFrames, exportFrom, exportTo,
			exportAnimationInput, derivativeScale;
		
		private Diagram diagram;
		
		private string exportFilePath;
		private int exportRowIndex = -1, exportFrame;
		private Color[] exportPixels;
		bool exporting;

		CubemapFace exportDirection;
		Texture2D exportCubemapTexture;
		
		void OnEnable () {
			diagram = target as Diagram;
			diagramSO = new SerializedObject(targets);
			libraries = diagramSO.FindProperty("functionLibraries");
			width = diagramSO.FindProperty("width");
			height = diagramSO.FindProperty("height");
			exportWidth = diagramSO.FindProperty("exportWidth");
			exportHeight = diagramSO.FindProperty("exportHeight");
			tileH = diagramSO.FindProperty("tilePreviewHorizontally");
			tileV = diagramSO.FindProperty("tilePreviewVertically");
			isCubemap = diagramSO.FindProperty("isCubemap");
			cubemapDirection = diagramSO.FindProperty("previewCubemapDirection");
			cubemapType = diagramSO.FindProperty("previewCubemapType");
			animate = diagramSO.FindProperty("animatePreview");
			animateName = diagramSO.FindProperty("animatePreviewInput");
			exportAnimation = diagramSO.FindProperty("exportAnimation");
			exportAnimationLoop = diagramSO.FindProperty("exportAnimationLoop");
			exportAnimationFrames = diagramSO.FindProperty("exportAnimationFrames");
			exportFrom = diagramSO.FindProperty("exportAnimateFrom");
			exportTo = diagramSO.FindProperty("exportAnimateTo");
			exportAnimationInput = diagramSO.FindProperty("exportAnimationInput");
			derivativeScale = diagramSO.FindProperty("derivativeScale");
		}
		
		void OnDisable () {
			if (exportRowIndex >= 0) {
				EditorUtility.ClearProgressBar();
				Debug.LogWarning("Canceled export.");
			}
		}
		
		public override void OnInspectorGUI () {
			diagramSO.Update();

			EditorGUILayout.PropertyField(isCubemap);
			if (isCubemap.boolValue) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(cubemapDirection, cubemapPreviewDirectionLabel);
				EditorGUILayout.PropertyField(cubemapType, cubemapPreviewTypeLabel);
				EditorGUI.indentLevel -= 1;
			}
			EditorGUILayout.PropertyField(derivativeScale);

			GUILayout.Label("Preview and Default Texture Size");
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField(width);
			EditorGUILayout.PropertyField(height);
			EditorGUI.indentLevel -= 1;
			GUILayout.Label("Preview Tiling");
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField(tileH, tileHorizontallyLabel);
			EditorGUILayout.PropertyField(tileV, tileVerticallyLabel);
			EditorGUI.indentLevel -= 1;
			EditorGUILayout.PropertyField(animate);
			if (animate.boolValue) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(animateName, animatePreviewInputLabel);
				EditorGUI.indentLevel -= 1;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(libraries, true);
			
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(exportWidth);
			EditorGUILayout.PropertyField(exportHeight);
			EditorGUILayout.PropertyField(exportAnimation);
			if (exportAnimation.boolValue) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(exportAnimationFrames, exportAnimationFramesLabel);
				EditorGUILayout.PropertyField(exportAnimationLoop, exportAnimationLoopLabel);
				EditorGUILayout.PropertyField(exportAnimationInput, exportAnimationInputLabel);
				EditorGUILayout.PropertyField(exportFrom, exportAnimationFromLabel);
				EditorGUILayout.PropertyField(exportTo, exportAnimationToLabel);
				EditorGUI.indentLevel -= 1;
			}
			if (targets.Length == 1) {
				DoExport();
			}
			
			EditorGUILayout.Space();
			if (GUILayout.Button("Open Editor")) {
				EditorWindow.GetWindow<DiagramWindow>(false, "NumberFlow");
			}

			if (diagramSO.ApplyModifiedProperties()) {
				for (int i = 0; i < targets.Length; i++) {
					(targets[i] as Diagram).prepared = false;
				}
			}
		}

		private int exportOutputIndex = -1;
		
		private void DoExport () {
			if (GUILayout.Button("Export to PNG")) {
				exportFilePath = EditorUtility.SaveFilePanel(
					"Export NumberFlow diagram to PNG",
					new FileInfo(AssetDatabase.GetAssetPath(diagram)).DirectoryName,
					diagram.name,
					"png");
				if (exportFilePath.Length > 0) {
					exporting = true;
					exportFrame = -1;
					exportRowIndex = 0;
					exportOutputIndex = 0;
					if (diagram.isCubemap) {
						exportDirection = CubemapFace.PositiveX;
						exportPixels = new Color[diagram.exportWidth * diagram.exportWidth];
						exportCubemapTexture = new Texture2D(diagram.exportWidth * 6, diagram.exportWidth, TextureFormat.ARGB32, false);
					}
					else {
						exportPixels = new Color[diagram.exportWidth * diagram.exportHeight];
					}
				}
			}
			if (exporting) {
				EditorUtility.DisplayProgressBar(
					"Exporting to PNG",
					"Output " + diagram.outputs[exportOutputIndex].name,
					diagram.isCubemap ? ((float)exportDirection / 6f) : (exportRowIndex / (float)diagram.exportHeight)
				);
				if (Event.current.type == EventType.Repaint && exportRowIndex == 0 && diagram.exportAnimation) {
					exportFrame += 1;
					Value animatedValue = diagram.GetInputValue(diagram.exportAnimationInput);
					if (animatedValue != null) {
						animatedValue.Float = Mathf.Lerp(
							diagram.exportAnimateFrom,
							diagram.exportAnimateTo,
							exportFrame / (float)(diagram.exportAnimationLoop ? diagram.exportAnimationFrames : (diagram.exportAnimationFrames - 1))
						);
					}
				}
				if (Event.current.type == EventType.Repaint) {
					if (diagram.isCubemap) {
						ExportCubemaps();
					}
					else {
						ExportTextures();
					}
				}
				Repaint();
			}
		}

		void ExportTextures () {
			exportRowIndex = diagram.FillRows(
				exportPixels,
				exportOutputIndex,
				diagram.exportWidth,
				diagram.exportHeight,
				exportRowIndex,
				diagram.exportHeight / 8 + 1);
			if (exportRowIndex >= diagram.exportHeight) {
				if (diagram.outputs[exportOutputIndex].type == DiagramTextureType.NormalMap) {
					diagram.outputs[exportOutputIndex].GenerateNormalMap(diagram.exportWidth, diagram.exportHeight, exportPixels, DiagramNormalFormat.RGB);
				}
				Texture2D texture = new Texture2D(
					diagram.exportWidth,
					diagram.exportHeight,
					diagram.outputs[exportOutputIndex].type == DiagramTextureType.RGB || diagram.outputs[exportOutputIndex].type == DiagramTextureType.NormalMap ? TextureFormat.RGB24 : TextureFormat.ARGB32,
					false);
				texture.SetPixels(exportPixels);
				if (diagram.exportAnimation) {
					exportRowIndex = exportFrame >= diagram.exportAnimationFrames - 1 ? -1 : 0;
					File.WriteAllBytes(exportFilePath.Insert(exportFilePath.Length - 4, diagram.outputs[exportOutputIndex].name + "_" + exportFrame), texture.EncodeToPNG());
				}
				else {
					exportRowIndex = -1;
					File.WriteAllBytes(exportFilePath.Insert(exportFilePath.Length - 4, diagram.outputs[exportOutputIndex].name), texture.EncodeToPNG());
				}
				if (exportRowIndex == -1) {
					EditorUtility.ClearProgressBar();
					AssetDatabase.Refresh();
					if (++exportOutputIndex < diagram.outputs.Length) {
						exportRowIndex = 0;
						exportFrame = -1;
					}
				}
				DestroyImmediate(texture);
				if (exportRowIndex == -1) {
					exporting = false;
				}
			}
		}

		void ExportCubemaps () {
			ExportCubemapFace();
			exportRowIndex = -1;
			if (exportDirection == CubemapFace.NegativeZ) {
				if (diagram.exportAnimation) {
					File.WriteAllBytes(
						exportFilePath.Insert(exportFilePath.Length - 4, diagram.outputs[exportOutputIndex].name + "_" + exportFrame),
						exportCubemapTexture.EncodeToPNG()
					);
					exportRowIndex = 0;
					if (exportFrame >= diagram.exportAnimationFrames - 1) {
						exportOutputIndex += 1;
						exportFrame = -1;
					}
					exporting = exportOutputIndex < diagram.outputs.Length;
				}
				else {
					File.WriteAllBytes(
						exportFilePath.Insert(exportFilePath.Length - 4, diagram.outputs[exportOutputIndex].name),
						exportCubemapTexture.EncodeToPNG()
					
					);
					exportOutputIndex += 1;
					exporting = exportOutputIndex < diagram.outputs.Length;
				}
				exportDirection = CubemapFace.PositiveX;
				if (!exporting) {
					DestroyImmediate(exportCubemapTexture);
					EditorUtility.ClearProgressBar();
					AssetDatabase.Refresh();
					exportRowIndex = -1;
				}
			}
			else {
				exportDirection += 1;
			}
		}

		void ExportCubemapFace () {
			int size = diagram.exportWidth;
			diagram.cubemapDirection = exportDirection;
			diagram.Fill(exportPixels, exportOutputIndex, size, size);
			int offset = (int)exportDirection * size;
			for (int y = 0; y < size; y++) {
				// Need to reverse rows for export.
				int row = size * (size - 1 - y);
				for (int x = 0; x < size; x++) {
					exportCubemapTexture.SetPixel(x + offset, y, exportPixels[row + x]);
				}
			}
		}
		
		[MenuItem("Assets/Create/NumberFlow Diagram")]
		public static void CreateNumberFlowDiagram () {
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path.Length == 0) {
				// No asset selected, place in asset root.
				path = "Assets/New NumberFlow Diagram.asset";
			}
			else if (Directory.Exists(path)) {
				// Place in currently selected directory.
				path += "/New NumberFlow Diagram.asset";
			}
			else {
				// Place in current selection's containing directory.
				path = Path.GetDirectoryName(path) + "/New NumberFlow Diagram.asset";
			}
			Diagram newDiagram = ScriptableObject.CreateInstance<Diagram>();
			AssetDatabase.CreateAsset(newDiagram, AssetDatabase.GenerateUniqueAssetPath(path));
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = newDiagram;
		}
	}
}