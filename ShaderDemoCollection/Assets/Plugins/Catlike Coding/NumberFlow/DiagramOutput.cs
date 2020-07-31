// Copyright 2014, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow {

	/// <summary>
	/// Diagram texture type.
	/// </summary>
	public enum DiagramTextureType {
		ARGB,
		RGB,
		Alpha,
		NormalMap
	}

	/// <summary>
	/// Diagram preview type.
	/// </summary>
	public enum DiagramPreviewType {
		ARGB,
		RGB,
		Alpha
	}

	/// <summary>
	/// Diagram texture normal filter mode.
	/// </summary>
	public enum DiagramTextureNormalFiltering {
		Sharp,
		Smooth
	}

	/// <summary>
	/// Configuration for diagram outputs.
	/// </summary>
	[Serializable]
	public sealed class DiagramOutput {

		/// <summary>
		/// Index of the corresponding diagram output node.
		/// </summary>
		public int nodeIndex;

		/// <summary>
		/// Output name. Should match a shader variable when used with a Diagram Texture Manager.
		/// </summary>
		public string name;

		/// <summary>
		/// Texture type.
		/// </summary>
		public DiagramTextureType type;

		/// <summary>
		/// Preview type.
		/// </summary>
		public DiagramPreviewType previewType;

		/// <summary>
		/// Normal filter mode.
		/// </summary>
		public DiagramTextureNormalFiltering normalFiltering;

		/// <summary>
		/// Normal strength.
		/// </summary>
		public float strength;

		/// <summary>
		/// Corresponding diagram node.
		/// </summary>
		[NonSerialized]
		public DiagramNode node;

		/// <summary>
		/// Generated texture.
		/// </summary>
		[NonSerialized]
		public Texture2D texture;

		/// <summary>
		/// Generate a normal map.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="pixels">Pixel buffer.</param>
		/// <param name="normalFormat">Normal format.</param>
		public void GenerateNormalMap (int width, int height, Color[] pixels, DiagramNormalFormat normalFormat) {
			if (normalFiltering == DiagramTextureNormalFiltering.Sharp) {
				GenerateSimpleNormalMap(width, height, strength, pixels);
			}
			else {
				GenerateSobelNormalMap(width, height, strength, pixels);
			}
			if (normalFormat == DiagramNormalFormat.Automatic) {
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8)
				ConvertNormalToDXT5(pixels);
#else
				ConvertNormalToRGB(pixels);
#endif
			}
			else if (normalFormat == DiagramNormalFormat.DXT5nm) {
				ConvertNormalToDXT5(pixels);
			}
			else {
				ConvertNormalToRGB(pixels);
			}
		}

		private static void GenerateSimpleNormalMap (int width, int height, float strength, Color[] buffer) {
			// Half strength because we use a delta of two pixels instead of one.
			// Negate as part of the cross product.
			// Multiply by width to keep the same effective strength regardless of resolution.
			strength *= -0.5f * width;
			int i = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++, i++) {
					Color c = buffer[i];
					c.r = (GetHeight(width, height, x + 1, y, buffer) - GetHeight(width, height, x - 1, y, buffer)) * strength;
					c.g = (GetHeight(width, height, x, y + 1, buffer) - GetHeight(width, height, x, y - 1, buffer)) * strength;
					c.b = 1f / Mathf.Sqrt(c.r * c.r + c.g * c.g + 1f);
					c.r *= c.b;
					c.g *= c.b;
					buffer[i] = c;
				}
			}
		}

		private static void GenerateSobelNormalMap (int width, int height, float strength, Color[] buffer) {
			// Half strength because we use a delta of two pixels instead of one.
			// Divide by four because the Sobel filter uses four samples per side (1, 2, 1).
			// Negate as part of the cross product.
			// Multiply by with to keep the same effective strength regardless of resolution.
			strength *= -0.125f * width;
			for (int y = 0, i = 0; y < height; y++) {
				for (int x = 0; x < width; x++, i++) {
					Color c = buffer[i];
					c.r = (
						GetHeight(width, height, x + 1, y - 1, buffer) +
						GetHeight(width, height, x + 1, y, buffer) * 2f +
						GetHeight(width, height, x + 1, y + 1, buffer) - 
						GetHeight(width, height, x - 1, y - 1, buffer) -
						GetHeight(width, height, x - 1, y, buffer) * 2f -
						GetHeight(width, height, x - 1, y + 1, buffer)
						) * strength;
					c.g = (
						GetHeight(width, height, x - 1, y + 1, buffer) +
						GetHeight(width, height, x, y + 1, buffer) * 2f +
						GetHeight(width, height, x + 1, y + 1, buffer) -
						GetHeight(width, height, x - 1, y - 1, buffer) -
						GetHeight(width, height, x, y - 1, buffer) * 2f -
						GetHeight(width, height, x + 1, y - 1, buffer)
						) * strength;
					c.b = 1f / Mathf.Sqrt(c.r * c.r + c.g * c.g + 1f);
					c.r *= c.b;
					c.g *= c.b;
					buffer[i] = c;
				}
			}
		}
		
		private static void ConvertNormalToRGB (Color[] normals) {
			for (int i = 0, l = normals.Length; i < l; i++) {
				Color c = normals[i];
				c.r = c.r * 0.5f + 0.5f;
				c.g = c.g * 0.5f + 0.5f;
				c.b = c.b * 0.5f + 0.5f;
				normals[i] = c;
			}
		}

		private static void ConvertNormalToDXT5 (Color[] normals) {
			for (int i = 0, l = normals.Length; i < l; i++) {
				Color c = normals[i];
				c.a = c.r * 0.5f + 0.5f;
				c.g = c.g * 0.5f + 0.5f;
				c.r = c.b = 1f;
				normals[i] = c;
			}
		}
		
		private static float GetHeight (int width, int height, int x, int y, Color[] buffer) {
			if (x == -1) {
				x = width - 1;
			}
			else if (x == width) {
				x = 0;
			}
			if (y == -1) {
				y = height - 1;
			}
			else if (y == height) {
				y = 0;
			}
			return buffer[x + y * width].a;
		}
	}
}