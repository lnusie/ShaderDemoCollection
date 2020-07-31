// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace CatlikeCoding.NumberFlow.Examples {
	
	/// <summary>
	/// Script that controls the Demo.
	/// </summary>
	public class DemoController : MonoBehaviour {
		
		private const int BL = 0, BR = 1, TL = 2, TR = 3, C = 4;
		
		public Diagram[] diagrams;
		
		public string[] descriptions;

		public Text infoText;

		public RawImage display;
		
		private int currentSize, newSize;
		private Diagram diagram;
		private Value timeValue, tweakValue;
		private float tweak = 0f;
		private bool animate = true, tiling;

		private Texture2D texture;
		private Color[] pixels;

		private Thread thread;
		private AutoResetEvent textureFilled;
		private bool pixelsReady;

		void Start () {
			currentSize = newSize = 64;
			pixels = new Color[currentSize * currentSize];
			texture = new Texture2D(currentSize, currentSize, TextureFormat.RGB24, false);
			texture.SetPixels(pixels);
			texture.Apply();
			display.texture = texture;
			SetTile(false);
			SetDiagram(0);

			PrepareMonaDiagrams();

			textureFilled = new AutoResetEvent(false);
			thread = new Thread(new ThreadStart(FillPixels));
			thread.IsBackground = true;
			thread.Start();
		}

		public void SetSize (int size) {
			newSize = size;
			UpdateDisplaySize();
		}

		public void SetAnimation (bool animate) {
			this.animate = animate;
		}

		public void SetTile (bool tile) {
			tiling = tile;
			UpdateDisplaySize();
		}

		public void SetTweak (float tweak) {
			this.tweak = tweak;
		}

		public void SetDiagram (int index) {
			diagram = diagrams[index];
			timeValue = diagram.GetInputValue("Time");
			tweakValue = diagram.GetInputValue("Tweak");
			infoText.text = descriptions[index];
		}

		private void UpdateDisplaySize () {
			float scale = tiling ? 2f : 1f;
			display.uvRect = new Rect(0f, 0f, scale, scale);
			float displaySize = scale * currentSize;
			display.rectTransform.sizeDelta = new Vector2(displaySize, displaySize);
		}
		
		private void PrepareMonaDiagrams () {
			// Use color array instead of texture for Mona diagrams, so they work in a separate thread.
			int width = 0, height = 0;
			Color[] colors = null;
			for (int i = 0; i < diagrams.Length; i++) {
				if (diagrams[i].name.StartsWith("Mona ")) {
					Value value = diagrams[i].GetInputValue("Texture");
					if (colors == null) {
						Texture2D t = value.Pixels.texture;
						colors = t.GetPixels();
						width = t.width;
						height = t.height;
					}
					value.Pixels.Init(width, height, colors);
				}
			}
		}
		
		void Update () {
			if (pixelsReady) {
				// FillPixels is done and waiting.
				if (currentSize != newSize) {
					currentSize = newSize;
					texture.Resize(currentSize, currentSize);
					pixels = new Color[currentSize * currentSize];
					display.SetNativeSize();
				}
				texture.SetPixels(pixels);
				pixelsReady = false;
				texture.Apply();
				if (animate && timeValue != null) {
					timeValue.Float = Time.realtimeSinceStartup;
				}
				if (tweakValue != null) {
					tweakValue.Float = tweak;
				}
				// Signal FillPixels to continue.
				textureFilled.Set();
			}
		}

		private void FillPixels () {
			// This method runs in a separate thread.
			while (true) {
				diagram.Fill(pixels, currentSize, currentSize);
				pixelsReady = true;
				// Wait for signal from Update.
				textureFilled.WaitOne();
			}
		}
	}
}