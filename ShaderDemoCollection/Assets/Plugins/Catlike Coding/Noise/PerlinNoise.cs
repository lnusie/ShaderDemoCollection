// Copyright 2015, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.Noise {

	/// <summary>A collection of methods to sample Perlin Noise.</summary>
	/// <description>
	/// The single-frequency Perlin noise methods produce values from -1 to 1.
	/// The multi-frequency Perlin noise methods instead produce values from 0 to 1.
	/// 
	/// Turbulence noise is a variant of Perlin noise where the absolute values of multiple frequencies are summed.
	/// 
	/// Tiled versions have offsets used for moving the tiling area to another spot in the noise domain. For the tiling dimensions,
	/// the integer parts are used to offset the cells, while the fractional parts are used to offset sampling within the tile.
	/// Animating these offsets will result in popping when they cross integer boundaries as the sampling switches to another tile.
	/// Frequency and lacunarity are integers for the tiled versions because they must be aligned with cell boundaries.
	/// 
	/// The 3D Perlin noise code is based on <a href="http://mrl.nyu.edu/~perlin/noise/">Ken Perlin's Improved Noise reference implementation</a>.
	/// The 2D versions use 16 gradients, which produces a slightly more wavy pattern than the 3D versions.
	/// </description>
	public static class PerlinNoise {

		const float sqrt2 = 1.414213562f;
		
		/// <summary>Sample 2D Perlin noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample2D (Vector2 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;

			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;

			ix0 &= NoiseMath.hashMask;
			iy0 &= NoiseMath.hashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int h0 = NoiseMath.hash[ix0];
			int h1 = NoiseMath.hash[ix1];
			Vector2 g00 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g10 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g01 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy1] & NoiseMath.gradientsMask2D];
			Vector2 g11 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy1] & NoiseMath.gradientsMask2D];

			float v00 = NoiseMath.Dot(g00, tx0, ty0);
			float v10 = NoiseMath.Dot(g10, tx1, ty0);
			float v01 = NoiseMath.Dot(g01, tx0, ty1);
			float v11 = NoiseMath.Dot(g11, tx1, ty1);

			float a = v00;
			float b = v10 - v00;
			float c = v01 - v00;
			float d = v11 - v01 - v10 + v00;

			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			return (a + b * tx + (c + d * tx) * ty) * sqrt2;
		}

		
		/// <summary>Sample multi-frequency 2D Perlin noise.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}

		/// <summary>Sample multi-frequency 2D Perlin Turbulence noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float
		SampleTurbulence2D (Vector2 point, float frequency, int octaves, float lacunarity, float persistence) {
			float amplitude = 1f, range = 1f, sample = Sample2D(point, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample2D(point, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 2D Perlin noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
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
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			
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
			Vector2 g00 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g10 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g01 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy1] & NoiseMath.gradientsMask2D];
			Vector2 g11 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy1] & NoiseMath.gradientsMask2D];
			
			float v00 = NoiseMath.Dot(g00, tx0, ty0);
			float v10 = NoiseMath.Dot(g10, tx1, ty0);
			float v01 = NoiseMath.Dot(g01, tx0, ty1);
			float v11 = NoiseMath.Dot(g11, tx1, ty1);
			
			float a = v00;
			float b = v10 - v00;
			float c = v01 - v00;
			float d = v11 - v01 - v10 + v00;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			return (a + b * tx + (c + d * tx) * ty) * sqrt2;
		}

		/// <summary>Sample multi-frequency 2D Perlin noise, tiled in the X dimension.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}
		
		/// <summary>Sample multi-frequency 2D Perlin Turbulence noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence2DTiledX
		(Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f, sample = Sample2DTiledX(point, xOffset, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample2DTiledX(point, xOffset, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 2D Perlin noise, tiled in both dimensions.</summary>
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
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;

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
			Vector2 g00 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g10 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy0] & NoiseMath.gradientsMask2D];
			Vector2 g01 = NoiseMath.gradients2D[NoiseMath.hash[h0 + iy1] & NoiseMath.gradientsMask2D];
			Vector2 g11 = NoiseMath.gradients2D[NoiseMath.hash[h1 + iy1] & NoiseMath.gradientsMask2D];
			
			float v00 = NoiseMath.Dot(g00, tx0, ty0);
			float v10 = NoiseMath.Dot(g10, tx1, ty0);
			float v01 = NoiseMath.Dot(g01, tx0, ty1);
			float v11 = NoiseMath.Dot(g11, tx1, ty1);
			
			float a = v00;
			float b = v10 - v00;
			float c = v01 - v00;
			float d = v11 - v01 - v10 + v00;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);

			return (a + b * tx + (c + d * tx) * ty) * sqrt2;
		}

		/// <summary>Sample multi-frequency 2D Perlin noise, tiled in both dimensions.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}
		
		/// <summary>Sample multi-frequency 2D Perlin Turbulence noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			
			float amplitude = 1f, range = 1f, sample = Sample2DTiledXY(point, xOffset, yOffset, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample2DTiledXY(point, xOffset, yOffset, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Perlin noise.</summary>
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
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tz0 = point.z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;

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
			Vector3 g000 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g100 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g010 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g110 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g001 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g101 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g011 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g111 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz1] & NoiseMath.gradientsMask3D];
			
			float v000 = NoiseMath.Dot(g000, tx0, ty0, tz0);
			float v100 = NoiseMath.Dot(g100, tx1, ty0, tz0);
			float v010 = NoiseMath.Dot(g010, tx0, ty1, tz0);
			float v110 = NoiseMath.Dot(g110, tx1, ty1, tz0);
			float v001 = NoiseMath.Dot(g001, tx0, ty0, tz1);
			float v101 = NoiseMath.Dot(g101, tx1, ty0, tz1);
			float v011 = NoiseMath.Dot(g011, tx0, ty1, tz1);
			float v111 = NoiseMath.Dot(g111, tx1, ty1, tz1);
			
			float a = v000;
			float b = v100 - v000;
			float c = v010 - v000;
			float d = v001 - v000;
			float e = v110 - v010 - v100 + v000;
			float f = v101 - v001 - v100 + v000;
			float g = v011 - v001 - v010 + v000;
			float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			float tz = NoiseMath.Smooth(tz0);
			return a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
		}
		
		/// <summary>Sample multi-frequency 3D Perlin noise.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}

		/// <summary>Sample multi-frequency 3D Perlin Turbulence noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f, sample = Sample3D(point, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample3D(point, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Perlin noise, tiled in the X dimension.</summary>
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
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tz0 = point.z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;
			
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
			Vector3 g000 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g100 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g010 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g110 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g001 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g101 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g011 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g111 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz1] & NoiseMath.gradientsMask3D];
			
			float v000 = NoiseMath.Dot(g000, tx0, ty0, tz0);
			float v100 = NoiseMath.Dot(g100, tx1, ty0, tz0);
			float v010 = NoiseMath.Dot(g010, tx0, ty1, tz0);
			float v110 = NoiseMath.Dot(g110, tx1, ty1, tz0);
			float v001 = NoiseMath.Dot(g001, tx0, ty0, tz1);
			float v101 = NoiseMath.Dot(g101, tx1, ty0, tz1);
			float v011 = NoiseMath.Dot(g011, tx0, ty1, tz1);
			float v111 = NoiseMath.Dot(g111, tx1, ty1, tz1);
			
			float a = v000;
			float b = v100 - v000;
			float c = v010 - v000;
			float d = v001 - v000;
			float e = v110 - v010 - v100 + v000;
			float f = v101 - v001 - v100 + v000;
			float g = v011 - v001 - v010 + v000;
			float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			float tz = NoiseMath.Smooth(tz0);
			return a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
		}
		
		/// <summary>Sample multi-frequency 3D Perlin noise, tiled in the X dimension.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}

		/// <summary>Sample multi-frequency 3D Perlin Turbulence noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f, sample = Sample3DTiledX(point, xOffset, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample3DTiledX(point, xOffset, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Perlin noise, tiled in the X and Y dimensions.</summary>
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
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tz0 = point.z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;
			
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
			Vector3 g000 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g100 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g010 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g110 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g001 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g101 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g011 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g111 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz1] & NoiseMath.gradientsMask3D];
			
			float v000 = NoiseMath.Dot(g000, tx0, ty0, tz0);
			float v100 = NoiseMath.Dot(g100, tx1, ty0, tz0);
			float v010 = NoiseMath.Dot(g010, tx0, ty1, tz0);
			float v110 = NoiseMath.Dot(g110, tx1, ty1, tz0);
			float v001 = NoiseMath.Dot(g001, tx0, ty0, tz1);
			float v101 = NoiseMath.Dot(g101, tx1, ty0, tz1);
			float v011 = NoiseMath.Dot(g011, tx0, ty1, tz1);
			float v111 = NoiseMath.Dot(g111, tx1, ty1, tz1);
			
			float a = v000;
			float b = v100 - v000;
			float c = v010 - v000;
			float d = v001 - v000;
			float e = v110 - v010 - v100 + v000;
			float f = v101 - v001 - v100 + v000;
			float g = v011 - v001 - v010 + v000;
			float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			float tz = NoiseMath.Smooth(tz0);
			return a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
		}
		
		/// <summary>Sample multi-frequency 3D Perlin noise, tiled in the X and Y dimensions.</summary>
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
			return sum * (0.5f / range) + 0.5f;
		}

		/// <summary>Sample multi-frequency 3D Perlin Turbulence noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f, sample = Sample3DTiledXY(point, xOffset, yOffset, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample3DTiledXY(point, xOffset, yOffset, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}

		/// <summary>Sample 3D Perlin noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="zOffset">Z offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static float Sample3DTiledXYZ (Vector3 point, int xOffset, int yOffset, int zOffset, int frequency) {
			if (frequency == 0) {
				return 0f;
			}

			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;
			
			int ix0 = NoiseMath.FloorToInt(point.x);
			int iy0 = NoiseMath.FloorToInt(point.y);
			int iz0 = NoiseMath.FloorToInt(point.z);
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tz0 = point.z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;
			
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
			Vector3 g000 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g100 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g010 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g110 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz0] & NoiseMath.gradientsMask3D];
			Vector3 g001 = NoiseMath.gradients3D[NoiseMath.hash[h00 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g101 = NoiseMath.gradients3D[NoiseMath.hash[h10 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g011 = NoiseMath.gradients3D[NoiseMath.hash[h01 + iz1] & NoiseMath.gradientsMask3D];
			Vector3 g111 = NoiseMath.gradients3D[NoiseMath.hash[h11 + iz1] & NoiseMath.gradientsMask3D];
			
			float v000 = NoiseMath.Dot(g000, tx0, ty0, tz0);
			float v100 = NoiseMath.Dot(g100, tx1, ty0, tz0);
			float v010 = NoiseMath.Dot(g010, tx0, ty1, tz0);
			float v110 = NoiseMath.Dot(g110, tx1, ty1, tz0);
			float v001 = NoiseMath.Dot(g001, tx0, ty0, tz1);
			float v101 = NoiseMath.Dot(g101, tx1, ty0, tz1);
			float v011 = NoiseMath.Dot(g011, tx0, ty1, tz1);
			float v111 = NoiseMath.Dot(g111, tx1, ty1, tz1);
			
			float a = v000;
			float b = v100 - v000;
			float c = v010 - v000;
			float d = v001 - v000;
			float e = v110 - v010 - v100 + v000;
			float f = v101 - v001 - v100 + v000;
			float g = v011 - v001 - v010 + v000;
			float h = v111 - v011 - v101 + v001 - v110 + v010 + v100 - v000;
			
			float tx = NoiseMath.Smooth(tx0);
			float ty = NoiseMath.Smooth(ty0);
			float tz = NoiseMath.Smooth(tz0);
			return a + b * tx + (c + e * tx) * ty + (d + f * tx + (g + h * tx) * ty) * tz;
		}
		
		/// <summary>Sample multi-frequency 3D Perlin noise, tiled in all three dimensions.</summary>
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
			point.z += offset.z;
			float amplitude = 1f, range = 1f, sum = Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sum += Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency) * amplitude;
			}
			return sum * (0.5f / range) + 0.5f;
		}

		/// <summary>Sample multi-frequency 3D Perlin Turbulence noise, also known as turbulence, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static float SampleTurbulence3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f, sample = Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			float sum = sample >= 0f ? sample : -sample;
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				sample = Sample3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency) * amplitude;
				sum += sample >= 0f ? sample : -sample;
			}
			return sum * (1f / range);
		}
	}
}