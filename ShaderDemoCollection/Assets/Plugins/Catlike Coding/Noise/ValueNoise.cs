// Copyright 2015, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.Noise {

	/// <summary>A collection of methods to sample Value Noise.</summary>
	/// <description>
	/// All noise methods instead produce values from 0 to 1.
	/// 
	/// Tiled versions have offsets used for moving the tiling area to another spot in the noise domain. For the tiling dimensions,
	/// the integer parts are used to offset the cells, while the fractional parts are used to offset sampling within the tile.
	/// Animating these offsets will result in popping when they cross integer boundaries as the sampling switches to another tile.
	/// Frequency and lacunarity are integers for the tiled versions because they must be aligned with cell boundaries.
	/// </description>
	public static class ValueNoise {

		/// <summary>Sample 2D Value noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample2D (Vector2 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			
			ix0 &= NoiseMath.hashMask;
			iy0 &= NoiseMath.hashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			
			float a = h00;
			float b = h10 - h00;
			float c = h01 - h00;
			float d = h11 - h01 - h10 + h00;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			return (a + b * tx + (c + d * tx) * ty) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 2D Value noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample2D (
			Vector2 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f, sum = Sample2D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample2D(point, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 2D Value noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample2DTiledX (Vector2 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}

			point.x *= frequency;
			point.y *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			
			ix0 %= frequency;
			if (ix0 < 0) {
				ix0 += frequency;
			}

			iy0 &= NoiseMath.hashMask;
			int ix1 = ((ix0 + 1) % frequency + xOffset) & NoiseMath.hashMask;
			int iy1 = iy0 + 1;
			ix0 = (ix0 + xOffset) & NoiseMath.hashMask;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			
			float a = h00;
			float b = h10 - h00;
			float c = h01 - h00;
			float d = h11 - h01 - h10 + h00;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			return (a + b * tx + (c + d * tx) * ty) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 2D Value noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample2DTiledX (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f, sum = Sample2DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample2DTiledX(point, xOffset, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 2D Value noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample2DTiledXY (Vector2 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}
			
			point.x *= frequency;
			point.y *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			
			ix0 %= frequency;
			if (ix0 < 0) {
				ix0 += frequency;
			}
			iy0 %= frequency;
			if (iy0 < 0) {
				iy0 += frequency;
			}
			
			int ix1 = ((ix0 + 1) % frequency + xOffset) & NoiseMath.hashMask;
			int iy1 = ((iy0 + 1) % frequency + yOffset) & NoiseMath.hashMask;
			ix0 = (ix0 + xOffset) & NoiseMath.hashMask;
			iy0 = (iy0 + yOffset) & NoiseMath.hashMask;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			
			float a = h00;
			float b = h10 - h00;
			float c = h01 - h00;
			float d = h11 - h01 - h10 + h00;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			return (a + b * tx + (c + d * tx) * ty) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 2D Value noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			float amplitude = 1f, range = 1f, sum = Sample2DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample2DTiledXY(point, xOffset, yOffset, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Value noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample3D (Vector3 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			int iz0 = NoiseMath.FloorToInt(point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			
			ix0 &= NoiseMath.hashMask;
			iy0 &= NoiseMath.hashMask;
			iz0 &= NoiseMath.hashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			int h000 = NoiseMath.hash[h00 + iz0];
			int h100 = NoiseMath.hash[h10 + iz0];
			int h010 = NoiseMath.hash[h01 + iz0];
			int h110 = NoiseMath.hash[h11 + iz0];
			int h001 = NoiseMath.hash[h00 + iz1];
			int h101 = NoiseMath.hash[h10 + iz1];
			int h011 = NoiseMath.hash[h01 + iz1];
			int h111 = NoiseMath.hash[h11 + iz1];
			
			float a = h000;
			float b = h100 - h000;
			float c = h010 - h000;
			float d = h001 - h000;
			float e = h110 - h010 - h100 + h000;
			float f = h101 - h001 - h100 + h000;
			float g = h011 - h001 - h010 + h000;
			float h = h111 - h011 - h101 + h001 - h110 + h010 + h100 - h000;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			tz = NoiseMath.Smooth(tz);
			return
				(a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 3D Value noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f, sum = Sample3D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample3D(point, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Value noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample3DTiledX (Vector3 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}

			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			int iz0 = NoiseMath.FloorToInt(point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			
			ix0 %= frequency;
			if (ix0 < 0) {
				ix0 += frequency;
			}
			
			iy0 &= NoiseMath.hashMask;
			iz0 &= NoiseMath.hashMask;
			int ix1 = ((ix0 + 1) % frequency + xOffset) & NoiseMath.hashMask;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;
			ix0 = (ix0 + xOffset) & NoiseMath.hashMask;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			int h000 = NoiseMath.hash[h00 + iz0];
			int h100 = NoiseMath.hash[h10 + iz0];
			int h010 = NoiseMath.hash[h01 + iz0];
			int h110 = NoiseMath.hash[h11 + iz0];
			int h001 = NoiseMath.hash[h00 + iz1];
			int h101 = NoiseMath.hash[h10 + iz1];
			int h011 = NoiseMath.hash[h01 + iz1];
			int h111 = NoiseMath.hash[h11 + iz1];
			
			float a = h000;
			float b = h100 - h000;
			float c = h010 - h000;
			float d = h001 - h000;
			float e = h110 - h010 - h100 + h000;
			float f = h101 - h001 - h100 + h000;
			float g = h011 - h001 - h010 + h000;
			float h = h111 - h011 - h101 + h001 - h110 + h010 + h100 - h000;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			tz = NoiseMath.Smooth(tz);
			return
				(a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 3D Value noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f, sum = Sample3DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample3DTiledX(point, xOffset, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Value noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample3DTiledXY (Vector3 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}

			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			int iz0 = NoiseMath.FloorToInt(point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			
			ix0 %= frequency;
			if (ix0 < 0) {
				ix0 += frequency;
			}
			iy0 %= frequency;
			if (iy0 < 0) {
				iy0 += frequency;
			}
			
			iz0 &= NoiseMath.hashMask;
			int ix1 = ((ix0 + 1) % frequency + xOffset) & NoiseMath.hashMask;
			int iy1 = ((iy0 + 1) % frequency + yOffset) & NoiseMath.hashMask;
			int iz1 = iz0 + 1;
			ix0 = (ix0 + xOffset) & NoiseMath.hashMask;
			iy0 = (iy0 + yOffset) & NoiseMath.hashMask;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			int h000 = NoiseMath.hash[h00 + iz0];
			int h100 = NoiseMath.hash[h10 + iz0];
			int h010 = NoiseMath.hash[h01 + iz0];
			int h110 = NoiseMath.hash[h11 + iz0];
			int h001 = NoiseMath.hash[h00 + iz1];
			int h101 = NoiseMath.hash[h10 + iz1];
			int h011 = NoiseMath.hash[h01 + iz1];
			int h111 = NoiseMath.hash[h11 + iz1];
			
			float a = h000;
			float b = h100 - h000;
			float c = h010 - h000;
			float d = h001 - h000;
			float e = h110 - h010 - h100 + h000;
			float f = h101 - h001 - h100 + h000;
			float g = h011 - h001 - h010 + h000;
			float h = h111 - h011 - h101 + h001 - h110 + h010 + h100 - h000;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			tz = NoiseMath.Smooth(tz);
			return
				(a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 3D Value noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f, sum = Sample3DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample3DTiledXY(point, xOffset, yOffset, frequency) * amplitude;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Value noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="zOffset">Z offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample3DTiledXYZ (Vector3 point, int xOffset, int zOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}

			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			int iz0 = NoiseMath.FloorToInt(point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			
			ix0 %= frequency;
			if (ix0 < 0) {
				ix0 += frequency;
			}
			iy0 %= frequency;
			if (iy0 < 0) {
				iy0 += frequency;
			}
			iz0 %= frequency;
			if (iz0 < 0) {
				iz0 += frequency;
			}
			
			int ix1 = ((ix0 + 1) % frequency + xOffset) & NoiseMath.hashMask;
			int iy1 = ((iy0 + 1) % frequency + yOffset) & NoiseMath.hashMask;
			int iz1 = ((iz0 + 1) % frequency + zOffset) & NoiseMath.hashMask;
			ix0 = (ix0 + xOffset) & NoiseMath.hashMask;
			iy0 = (iy0 + yOffset) & NoiseMath.hashMask;
			iz0 = (iz0 + zOffset) & NoiseMath.hashMask;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			int h00 = NoiseMath.hash[h0 + iy0];
			int h10 = NoiseMath.hash[h1 + iy0];
			int h01 = NoiseMath.hash[h0 + iy1];
			int h11 = NoiseMath.hash[h1 + iy1];
			int h000 = NoiseMath.hash[h00 + iz0];
			int h100 = NoiseMath.hash[h10 + iz0];
			int h010 = NoiseMath.hash[h01 + iz0];
			int h110 = NoiseMath.hash[h11 + iz0];
			int h001 = NoiseMath.hash[h00 + iz1];
			int h101 = NoiseMath.hash[h10 + iz1];
			int h011 = NoiseMath.hash[h01 + iz1];
			int h111 = NoiseMath.hash[h11 + iz1];
			
			float a = h000;
			float b = h100 - h000;
			float c = h010 - h000;
			float d = h001 - h000;
			float e = h110 - h010 - h100 + h000;
			float f = h101 - h001 - h100 + h000;
			float g = h011 - h001 - h010 + h000;
			float h = h111 - h011 - h101 + h001 - h110 + h010 + h100 - h000;
			
			tx = NoiseMath.Smooth(tx);
			ty = NoiseMath.Smooth(ty);
			tz = NoiseMath.Smooth(tz);
			return
				(a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz) * (1f / NoiseMath.hashMask);
		}

		/// <summary>Sample multi-frequency 3D Value noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float Sample3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f, sum = Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency) * amplitude;
			}
			return sum * (1f / range);
		}
	}
}