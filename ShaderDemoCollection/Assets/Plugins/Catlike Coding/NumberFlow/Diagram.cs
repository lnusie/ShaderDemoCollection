// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow {

	/// <summary>
	/// Format to use for normal maps.
	/// </summary>
	public enum DiagramNormalFormat {
		/// <summary>
		/// Either DXT5nm or RGB, depending on the platform.
		/// </summary>
		Automatic,
		/// <summary>
		/// Stores Y in G and X in A of an ARGB channel and omitting Z, which is optimal for DXT5 compression.
		/// </summary>
		DXT5nm,
		/// <summary>
		/// Stores XYZ in RGB, converted from -1..1 to 0..1.
		/// </summary>
		RGB,
		/// <summary>
		/// The same as RGB, but also stores the height in A.
		/// </summary>
		ARGB
	}

	/// <summary>
	/// How to show the diagram preview image for cubemaps.
	/// </summary>
	public enum DiagramCubemapPreviewType {
		Inside,
		Outside
	}
	
	/// <summary>
	/// Indication of direction for derivatives.
	/// </summary>
	public enum DiagramDerivativeDirection {
		None,
		PositiveX,
		NegativeX,
		PositiveY,
		NegativeY
	}
	
	/// <summary>
	/// NumberFlow diagram.
	/// 
	/// These diagrams are created and edited in the Unity editor.
	/// Input values can be configured and colors can be filled via scripts at run time.
	/// 
	/// See the <a href="http://catlikecoding.com/unity/products/numberflow/">product page</a> for an overview.
	/// </summary>
	public sealed class Diagram : ScriptableObject {

		#region Variables

		/// <summary>
		/// Nodes in the diagram.
		/// 
		/// Used internally and by editor scripts. Do not touch it.
		/// </summary>
		public DiagramNode[] nodes;

		/// <summary>
		/// Output definitions.
		/// </summary>
		public DiagramOutput[] outputs;

		/// <summary>
		/// Index of the output to use for the editor preview.
		/// </summary>
		public int previewOutputIndex;
		
		/// <summary>
		/// Additional function libraries available to the diagram.
		/// 
		/// You can add your own libraries here. Do not set it to null.
		/// </summary>
		public FunctionLibrary[] functionLibraries;
		
		/// <summary>
		/// Default and editor preview width.
		/// 
		/// Should be in the range 1-4096.
		/// </summary>
		[Range(1, 4096)]
		public int width = 128;
		
		/// <summary>
		/// Default and editor preview height.
		/// 
		/// Should be in the range 1-4096.
		/// </summary>
		[Range(1, 4096)]
		public int height = 128;
		
		/// <summary>
		/// Editor export width.
		/// 
		/// Should be in the range 1-4096.
		/// </summary>
		[Range(1, 4096)]
		public int exportWidth = 1024;
		
		/// <summary>
		/// Editor export height.
		/// 
		/// Should be in the range 1-4096.
		/// </summary>
		[Range(1, 4096)]
		public int exportHeight = 1024;
		
		/// <summary>
		/// How many frames to use when exporting an animation.
		/// </summary>
		public int exportAnimationFrames;
		
		/// <summary>
		/// Initial value for the export animation input value.
		/// </summary>
		public float exportAnimateFrom = 0;
		
		/// <summary>
		/// Final value for the export animation input value.
		/// </summary>
		public float exportAnimateTo = 1;
		
		/// <summary>
		/// Whether to tile the editor preview horizontally.
		/// </summary>
		public bool tilePreviewHorizontally;
		
		/// <summary>
		/// Whether to tile the editor preview vertically.
		/// </summary>
		public bool tilePreviewVertically;
		
		/// <summary>
		/// Whether to animate the editor preview.
		/// </summary>
		public bool animatePreview;

		/// <summary>
		/// Whether the diagram is a cubemap.
		/// </summary>
		public bool isCubemap;
		
		/// <summary>
		/// Scale factor for derivatives.
		/// </summary>
		/// <description>
		/// The delta used for derivatives is equal to derivativeScale / max(width, height).
		/// The default scale is 1, so the delta covers one pixel, if width and height are equal.
		/// </description>
		[Range(0.5f, 2f)]
		public float derivativeScale = 1f;

		/// <summary>
		/// Which cubemap direction to preview in the editor.
		/// </summary>
		public CubemapFace previewCubemapDirection = CubemapFace.PositiveZ;

		/// <summary>
		/// How to display the cubemap preview.
		/// </summary>
		public DiagramCubemapPreviewType previewCubemapType;

		/// <summary>
		/// Whether to export an animation.
		/// </summary>
		public bool exportAnimation;
		
		/// <summary>
		/// Whether to export an animation loop.
		/// 
		/// This conveniently adjusts exportAnimateTo so it overlaps with the first frame.
		/// </summary>
		public bool exportAnimationLoop;
		
		/// <summary>
		/// Input name of the value to animate for the editor preview.
		/// </summary>
		public string animatePreviewInput;
		
		/// <summary>
		/// Input name of the value to animate for the export.
		/// </summary>
		public string exportAnimationInput;
		
		/// <summary>
		/// Whether the diagram is prepared to be used.
		/// 
		/// Used by editor scripts.
		/// </summary>
		[NonSerialized]
		public bool prepared;

		/// <summary>
		/// The index of the pixel currently being processed.
		/// </summary>
		[NonSerialized]
		public int currentPixelIndex;
		
		/// <summary>
		/// The X coordinate of the pixel currently being processed.
		/// </summary>
		[NonSerialized]
		public int pixelX;
		
		/// <summary>
		/// The Y coordinate of the pixel currently being processed.
		/// </summary>
		[NonSerialized]
		public int pixelY;
		
		/// <summary>
		/// The uv coordinates of the pixel currently being processed.
		/// </summary>
		[NonSerialized]
		public Vector2 uv;

		/// <summary>
		/// Which cubemap direction to use when filling. You have to change this when manually generatic a cubemap.
		/// </summary>
		[NonSerialized]
		public CubemapFace cubemapDirection;
		
		[NonSerialized]
		internal DiagramDerivativeDirection derivativeDirection;
		
		[NonSerialized]
		internal float derivativeDelta;

		#endregion

		private void OnEnable () {
			Init();
		}
		
		/// <summary>
		/// Initialize the diagram.
		/// 
		/// Used internally and by editor scripts. Do not invoke it.
		/// </summary>
		public void Init () {
			prepared = false;
			if (nodes == null) {
				nodes = new DiagramNode[0];
			}
			else {
				foreach (DiagramNode node in nodes) {
					node.Init(this);
				}
			}
			if (outputs == null || outputs.Length == 0) {
				// Generate missing output data.
				if (outputs == null) {
					outputs = new DiagramOutput[0];
				}
				for (int i = 0; i < nodes.Length; i++) {
					if (nodes[i].Function.type == FunctionType.Output) {
						Array.Resize(ref outputs, outputs.Length + 1);
						DiagramOutput d = outputs[outputs.Length - 1] = new DiagramOutput();
						d.name = "_MainTex";
						d.nodeIndex = i;
						d.type = DiagramTextureType.ARGB;
					}
				}
			}
			if (functionLibraries == null) {
				functionLibraries = new FunctionLibrary[0];
			}
		}
		
		private void Prepare () {
			if (prepared) {
				return;
			}
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i].Prepare(this);
			}
			for (int i = 0; i < outputs.Length; i++) {
				outputs[i].node = nodes[outputs[i].nodeIndex];
			}
			prepared = true;
		}
		
		/// <summary>
		/// Get a named input value, which can be used to configure the diagram at runtime.
		/// 
		/// It is a good idea to cache the Value object reference to prevent constantly searching the diagram.
		/// However, accessing the diagram in the editor will break these caches. If this is an issue, use conditional compilation
		/// to only fetch the inputs again in the editor and not in builds.
		/// </summary>
		/// <returns>
		/// Value object instance, or null when it couldn't be found.
		/// </returns>
		/// <param name='name'>
		/// Input name.
		/// </param>
		public Value GetInputValue (string name) {
			Prepare();
			for (int i = 0; i < nodes.Length; i++) {
				if (name == nodes[i].InputName) {
					return nodes[i].argumentValues[0];
				}
			}
			return null;
		}

		#region Fill

		/// <summary>
		/// Fill pixel buffers with all outputs.
		/// </summary>
		/// <param name='buffers'>
		/// Buffer of Color arrays representing grids of Pixels.
		/// Must have at least one array per output and their lengths must be at least width * height.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		public void Fill(Color[][] buffers, int width, int height) {
			FillRows(buffers, width, height, 0, height);
		}
		
		/// <summary>
		/// Fill pixel buffers with all outputs. Use the default width and height.
		/// </summary>
		/// <param name='buffers'>
		/// Buffer of Color arrays representing grids of Pixels.
		/// Must have at least one array per output and their lengths must be at least width * height.
		/// </param>
		public void Fill(Color[][] buffers) {
			FillRows(buffers, width, height, 0, height);
		}
		
		/// <summary>
		/// Fill pixel data with the first output.
		/// </summary>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		/// <param name='outputIndex'>
		/// Index of the output to use.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		public void Fill (Color[] pixels, int outputIndex, int width, int height) {
			FillRows(pixels, outputIndex, width, height, 0, height);
		}
		
		/// <summary>
		/// Fill pixels data with the first output. Use the default width and height.
		/// </summary>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		/// <param name='outputIndex'>
		/// Index of the output to use.
		/// </param>
		public void Fill (Color[] pixels, int outputIndex) {
			FillRows(pixels, outputIndex, width, height, 0, height);
		}

		/// <summary>
		/// Fill pixel data with the first output.
		/// </summary>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		public void Fill (Color[] pixels, int width, int height) {
			FillRows(pixels, 0, width, height, 0, height);
		}

		/// <summary>
		/// Fill pixels data with the first output. Use the default width and height.
		/// </summary>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		public void Fill (Color[] pixels) {
			FillRows(pixels, 0, width, height, 0, height);
		}

		#endregion

		#region FillRows

		/// <summary>
		/// Fill multiple rows of pixel buffers. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='buffers'>
		/// Buffer of Color arrays representing grids of Pixels.
		/// Must have at least one array per output and their lengths must be at least width * height.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[][] buffers, int width, int height, int rowIndex, int rowCount) {
			Prepare();
			if (outputs.Length == 0) {
				return height;
			}
			
			derivativeDelta = derivativeScale / (width > height ? width : height);
			float uDelta = 1f / width, vDelta = 1f / height;
			Vector2 oldUV = uv;
			int
				oldX = pixelX,
				oldY = pixelY;
			
			int i = currentPixelIndex = rowIndex * width;
			for (int maxY = Mathf.Min(height, rowIndex + rowCount); rowIndex < maxY; rowIndex++) {
				pixelY = rowIndex;
				uv.y = (rowIndex + 0.5f) * vDelta;
				for (int x = 0; x < width; x++) {
					pixelX = x;
					uv.x = (x + 0.5f) * uDelta;
					for (int o = 0; o < outputs.Length; o++) {
						buffers[o][i] = outputs[o].node.ComputeColor();
					}
					currentPixelIndex = ++i;
				}
			}
			uv = oldUV;
			pixelX = oldX;
			pixelY = oldY;
			return rowIndex;
		}

		/// <summary>
		/// Fill multiple rows of pixel buffers. Use the default width and height. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='buffers'>
		/// Buffer of Color arrays representing grids of Pixels.
		/// Must have at least one array per output and their lengths must be at least width * height.
		/// </param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[][] buffers, int rowIndex, int rowCount) {
			return FillRows(buffers, width, height, rowIndex, rowCount);
		}

		/// <summary>
		/// Fill multiple rows of pixels. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be at least width * height.
		/// </param>
		/// <param name='outputIndex'>
		/// Index of the output to use.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[] pixels, int outputIndex, int width, int height, int rowIndex, int rowCount) {
			Prepare();
			if (outputs.Length == 0) {
				return height;
			}
			if (outputIndex < 0 || outputIndex >= outputs.Length) {
				outputIndex = 0;
			}

			DiagramNode output = outputs[outputIndex].node;
			derivativeDelta = derivativeScale / (width > height ? width : height);
			float uDelta = 1f / width, vDelta = 1f / height;
			Vector2 oldUV = uv;
			int
				oldX = pixelX,
				oldY = pixelY;
			
			int i = currentPixelIndex = rowIndex * width;
			for (int maxY = Mathf.Min(height, rowIndex + rowCount); rowIndex < maxY; rowIndex++) {
				pixelY = rowIndex;
				uv.y = (rowIndex + 0.5f) * vDelta;
				for (int x = 0; x < width; x++) {
					pixelX = x;
					uv.x = (x + 0.5f) * uDelta;
					pixels[i] = output.ComputeColor();
					currentPixelIndex = ++i;
				}
			}
			uv = oldUV;
			pixelX = oldX;
			pixelY = oldY;
			return rowIndex;
		}

		/// <summary>
		/// Fill multiple rows of pixels. Use the default width and height. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be at least width * height.
		/// </param>
		/// <param name='outputIndex'>
		/// Index of the output to use.</param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[] pixels, int outputIndex, int rowIndex, int rowCount) {
			return FillRows(pixels, outputIndex, width, height, rowIndex, rowCount);
		}

		/// <summary>
		/// Fill multiple rows of pixels with the first output. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[] pixels, int width, int height, int rowIndex, int rowCount) {
			return FillRows(pixels, 0, width, height, rowIndex, rowCount);
		}

		/// <summary>
		/// Fill multiple rows of pixels with the first output. Use the default width and height. Filling will stop after the last row.
		/// </summary>
		/// <returns>
		/// Index of the next row that should be filled. When finished, this will be larger than or equal to height.
		/// </returns>
		/// <param name='pixels'>
		/// Color array representing a grid of Pixels. Its length must be width * height.
		/// </param>
		/// <param name='rowIndex'>
		/// Row index to start from.
		/// </param>
		/// <param name='rowCount'>
		/// Amount of rows to fill.
		/// </param>
		public int FillRows (Color[] pixels, int rowIndex, int rowCount) {
			return FillRows(pixels, 0, width, height, rowIndex, rowCount);
		}

		#endregion

		#region PostProcess
		
		/// <summary>
		/// Posts process filled pixel buffers. Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffers">The buffers.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[][] buffers, int width, int height, DiagramNormalFormat normalFormat) {
			for (int i = 0; i < outputs.Length; i++) {
				DiagramOutput o = outputs[i];
				if (o.type == DiagramTextureType.NormalMap) {
					o.GenerateNormalMap(width, height, buffers[i], normalFormat);
				}
			}
		}
		
		/// <summary>
		/// Posts process filled pixel buffers. Use the default width and height. Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffers">The buffers.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[][] buffers, DiagramNormalFormat normalFormat) {
			PostProcess(buffers, width, height, normalFormat);
		}

		/// <summary>
		/// Posts process filled pixel buffer. Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name='outputIndex'>Index of the output to use.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[] buffer, int outputIndex, int width, int height, DiagramNormalFormat normalFormat) {
			if (outputIndex >= outputs.Length) {
				return;
			}
			DiagramOutput o = outputs[outputIndex];
			if (o.type == DiagramTextureType.NormalMap) {
				o.GenerateNormalMap(width, height, buffer, normalFormat);
			}
		}

		/// <summary>
		/// Posts process filled pixel buffer. Use the default width and height. Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name='outputIndex'>Index of the output to use.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[] buffer, int outputIndex, DiagramNormalFormat normalFormat) {
			PostProcess(buffer, outputIndex, width, height, normalFormat);
		}

		/// <summary>
		/// Posts process filled pixel buffer for the first output. Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[] buffer, int width, int height, DiagramNormalFormat normalFormat) {
			PostProcess(buffer, 0, width, height, normalFormat);
		}
		
		/// <summary>
		/// Posts process filled pixel buffer for the first output. Use the default width and height.
		/// Call after Fill or FillRows with the same pixel data.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void PostProcess (Color[] buffer, DiagramNormalFormat normalFormat) {
			PostProcess(buffer, 0, width, height, normalFormat);
		}

		#endregion
	}
}