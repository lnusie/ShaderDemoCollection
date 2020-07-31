// Copyright 2015, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.Noise {

	/// <summary>A collection of methods to sample Voronoi Noise.</summary>
	/// <description>
	/// All noise methods instead produce values from 0 to 1.
	/// 
	/// Tiled versions have offsets used for moving the tiling area to another spot in the noise domain. For the tiling dimensions,
	/// the integer parts are used to offset the cells, while the fractional parts are used to offset sampling within the tile.
	/// Animating these offsets will result in popping when they cross integer boundaries as the sampling switches to another tile.
	/// Frequency and lacunarity are integers for the tiled versions because they must be aligned with cell boundaries.
	/// 
	/// Voronoi noise was first described by <a href="https://en.wikipedia.org/wiki/Worley_noise">Steven Worley</a>.
	/// It is also known as Worley noise and sometimes cell noise.
	/// </description>
	public static class VoronoiNoise {

		private const float stepSize = 1f / (NoiseMath.hashMask + 1);

		/// <summary>Sample 2D Squared Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared2D (Vector2 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			point.x -= X;
			point.y -= Y;

			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx, hashX = NoiseMath.hash[Xx = (X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int Yy, hash;
							v.x = x + (hash = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Squared Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared2D (
			Vector2 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample multi-frequency 2D Linear Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear2D (
			Vector2 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2D(point, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2D(point, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Squared Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared2DTiledX (Vector2 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}
			
			point.x *= frequency;
			point.y *= frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int Yy, hash;
							v.x = x + (hash = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}
		
		/// <summary>Sample multi-frequency 2D Squared Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared2DTiledX (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}
		
		/// <summary>Sample multi-frequency 2D Linear Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear2DTiledX (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2DTiledX(point, xOffset, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2DTiledX(point, xOffset, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Squared Voronoi noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared2DTiledXY (Vector2 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}
			
			point.x *= frequency;
			point.y *= frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int hash, Yy = TiledIndex(Y + y, frequency, yOffset);
							v.x = x + (hash = NoiseMath.hash[hashX + Yy]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = v.x * v.x + v.y * v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Squared Voronoi noise, tiled in both dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}
		
		/// <summary>Sample multi-frequency 2D Linear Voronoi noise, tiled in both dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared2DTiledXY(point, xOffset, yOffset, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared2DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Squared Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared3D (Vector3 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y),
				dzn = point.z * point.z,
				dzp = (1f - point.z) * (1f - point.z);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx, hashX = NoiseMath.hash[Xx = (X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									float d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + dzn;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Squared Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample multi-frequency 3D Linear Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3D(point, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3D(point, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Squared Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared3DTiledX (Vector3 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y),
				dzn = point.z * point.z,
				dzp = (1f - point.z) * (1f - point.z);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									float d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + dzn;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Squared Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample multi-frequency 3D Linear Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledX(point, xOffset, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledX(point, xOffset, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Squared Voronoi noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared3DTiledXY (Vector3 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y),
				dzn = point.z * point.z,
				dzp = (1f - point.z) * (1f - point.z);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									float d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + dzn;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Squared Voronoi noise, tiled in the X and Y dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample multi-frequency 3D Linear Voronoi noise, tiled in the X and Y dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledXY(point, xOffset, yOffset, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Squared Voronoi noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="zOffset">Z offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleSquared3DTiledXYZ (
			Vector3 point, int xOffset, int yOffset, int zOffset, int frequency
		) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyn = point.y * point.y,
				dyp = (1f - point.y) * (1f - point.y),
				dzn = point.z * point.z,
				dzp = (1f - point.z) * (1f - point.z);
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz = TiledIndex(Z + z, frequency, zOffset);
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + Zz]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									float d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = v.x * v.x + v.y * v.y + v.z * v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + dzn;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + dyn;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x * point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = (1f - point.x) * (1f - point.x);
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Squared Voronoi noise, tiled in all three dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleSquared3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample multi-frequency 3D Linear Voronoi noise, tiled in all three dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleLinear3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleSquared3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			sum.x = Mathf.Sqrt(sum.x);
			sum.y = Mathf.Sqrt(sum.y);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleSquared3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
				sum.x += Mathf.Sqrt(sample.x) * amplitude;
				sum.y += Mathf.Sqrt(sample.y) * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Manhattan Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan2D (Vector2 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;

			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx, hashX = NoiseMath.hash[Xx = (X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int Yy, hash;
							v.x = x + (hash = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Manhattan Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan2D (
			Vector2 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan2D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan2D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Manhattan Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan2DTiledX (Vector2 point, int xOffset, int frequency) {
			point.x *= frequency;
			point.y *= frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int Yy, hash;
							v.x = x + (hash = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Manhattan Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan2DTiledX (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan2DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan2DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}
		
		/// <summary>Sample 2D Manhattan Voronoi noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan2DTiledXY (Vector2 point, int xOffset, int yOffset, int frequency) {
			point.x *= frequency;
			point.y *= frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							Vector2 v;
							int hash, Yy = TiledIndex(Y + y, frequency, yOffset);
							v.x = x + (hash = NoiseMath.hash[hashX + Yy]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
							v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
							d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y);
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Manhattan Voronoi noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan2DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan2DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Manhattan Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan3D (Vector3 point, float frequency) {
			point.x *= frequency;
			point.y *= frequency;
			point.z *= frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx, hashX = NoiseMath.hash[Xx = (X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}
		
		/// <summary>Sample multi-frequency 3D Manhattan Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan3D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan3D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Manhattan Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan3DTiledX (Vector3 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Manhattan Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan3DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan3DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Manhattan Voronoi noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan3DTiledXY (Vector3 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Manhattan Voronoi noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan3DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan3DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}
		
		/// <summary>Sample 3D Manhattan Voronoi noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="zOffset">Z offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleManhattan3DTiledXYZ (
			Vector3 point, int xOffset, int yOffset, int zOffset, int frequency
		) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = float.MaxValue;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz = TiledIndex(Z + z, frequency, zOffset);
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + Zz]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									float d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
									v.x = x + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									d = (v.x >= 0f ? v.x : -v.x) + (v.y >= 0f ? v.y : -v.y) + (v.z >= 0f ? v.z : -v.z);
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy + point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy + dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx + point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx + dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			if (r.x > 1f) {
				r.x = 1f;
			}
			if (r.y > 1f) {
				r.y = 1f;
			}
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}
		
		/// <summary>Sample multi-frequency 3D Manhattan Voronoi noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleManhattan3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleManhattan3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleManhattan3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Squared Chebyshev noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev2D (Vector2 point, float frequency) {
			point.x = point.x * frequency;
			point.y = point.y * frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;

			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx, hashX = NoiseMath.hash[Xx = (X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int hash;
							Vector2 v;
							v.x = x + (hash = NoiseMath.hash[hashX + ((Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							if (v.x < 0f) {
								v.x = -v.x;
							}
							if (v.y < 0f) {
								v.y = -v.y;
							}
							float d = v.x >= v.y ? v.x : v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Chebyshev Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev2D (
			Vector2 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev2D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev2D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Squared Chebyshev noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev2DTiledX (Vector2 point, int xOffset, int frequency) {
			point.x = point.x * frequency;
			point.y = point.y * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int hash;
							Vector2 v;
							v.x = x + (hash = NoiseMath.hash[hashX + ((Y + y) & NoiseMath.hashMask)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							if (v.x < 0f) {
								v.x = -v.x;
							}
							if (v.y < 0f) {
								v.y = -v.y;
							}
							float d = v.x >= v.y ? v.x : v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Chebyshev Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev2DTiledX (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev2DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev2DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 2D Squared Chebyshev noise, tiled in both dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev2DTiledXY (Vector2 point, int xOffset, int yOffset, int frequency) {
			point.x = point.x * frequency;
			point.y = point.y * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y);
			
			point.x -= X;
			point.y -= Y;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float dyp = 1f - point.y;
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int Xx = TiledIndex(X + x, frequency, xOffset);
					int hashX = NoiseMath.hash[Xx];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int hash;
							Vector2 v;
							v.x = x + (hash = NoiseMath.hash[hashX + TiledIndex(Y + y, frequency, yOffset)]) * stepSize - point.x;
							v.y = y + (hash = NoiseMath.hash[hash + Xx]) * stepSize - point.y;
							if (v.x < 0f) {
								v.x = -v.x;
							}
							if (v.y < 0f) {
								v.y = -v.y;
							}
							float d = v.x >= v.y ? v.x : v.y;
							if (d < r.x) {
								r.z = hash;
								r.y = r.x;
								r.x = d;
							}
							else if (d < r.y) {
								r.y = d;
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 2D Chebyshev Voronoi noise, tiled in both dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 2D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev2DTiledXY (
			Vector2 point, Vector2 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev2DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev2DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Squared Chebyshev noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev3D (Vector3 point, float frequency) {
			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;

			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int hashX = NoiseMath.hash[(X + x) & NoiseMath.hashMask];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									if (v.x < 0f) {
										v.x = -v.x;
									}
									if (v.y < 0f) {
										v.y = -v.y;
									}
									if (v.z < 0f) {
										v.z = -v.z;
									}
									float d = v.x >= v.y ? (v.x >= v.z ? v.x : v.z) : v.y >= v.z ? v.y : v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy >= point.z ? dy : point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy >= dzp ? dy : dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Chebyshev Voronoi noise.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev3D (
			Vector3 point, float frequency, int octaves, float lacunarity, float persistence
		) {
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev3D(point, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev3D(point, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Chebyshev Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev3DTiledX (Vector3 point, int xOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int hashX = NoiseMath.hash[TiledIndex(X + x, frequency, xOffset)];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy, hashY = NoiseMath.hash[hashX + (Yy = (Y + y) & NoiseMath.hashMask)];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									if (v.x < 0f) {
										v.x = -v.x;
									}
									if (v.y < 0f) {
										v.y = -v.y;
									}
									if (v.z < 0f) {
										v.z = -v.z;
									}
									float d = v.x >= v.y ? (v.x >= v.z ? v.x : v.z) : v.y >= v.z ? v.y : v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy >= point.z ? dy : point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy >= dzp ? dy : dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}

		/// <summary>Sample multi-frequency 3D Chebyshev Voronoi noise, tiled in the X dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev3DTiledX (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x);
			point.x += offset.x - xOffset;
			point.y += offset.y;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev3DTiledX(point, xOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev3DTiledX(point, xOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Chebyshev Voronoi noise, tiled in the X and Y dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev3DTiledXY (Vector3 point, int xOffset, int yOffset, int frequency) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int hashX = NoiseMath.hash[TiledIndex(X + x, frequency, xOffset)];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz;
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + (Zz = (Z + z) & NoiseMath.hashMask)]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									if (v.x < 0f) {
										v.x = -v.x;
									}
									if (v.y < 0f) {
										v.y = -v.y;
									}
									if (v.z < 0f) {
										v.z = -v.z;
									}
									float d = v.x >= v.y ? (v.x >= v.z ? v.x : v.z) : v.y >= v.z ? v.y : v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy >= point.z ? dy : point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy >= dzp ? dy : dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}
		
		/// <summary>Sample multi-frequency 3D Chebyshev Voronoi noise, tiled in the X and Y dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev3DTiledXY (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int xOffset = NoiseMath.FloorToInt(offset.x), yOffset = NoiseMath.FloorToInt(offset.y);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev3DTiledXY(point, xOffset, yOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev3DTiledXY(point, xOffset, yOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}

		/// <summary>Sample 3D Chebyshev Voronoi noise, tiled in all three dimensions.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="xOffset">X offset of the tiling domain.</param>
		/// <param name="yOffset">Y offset of the tiling domain.</param>
		/// <param name="zOffset">Z offset of the tiling domain.</param>
		/// <param name="frequency">Frequency of the noise.</param>
		public static Vector3 SampleChebyshev3DTiledXYZ (
			Vector3 point, int xOffset, int yOffset, int zOffset, int frequency
		) {
			if (frequency == 0) {
				return Vector3.zero;
			}

			point.x = point.x * frequency;
			point.y = point.y * frequency;
			point.z = point.z * frequency;
			
			int
				X = NoiseMath.FloorToInt(point.x),
				Y = NoiseMath.FloorToInt(point.y),
				Z = NoiseMath.FloorToInt(point.z);
			
			point.x -= X;
			point.y -= Y;
			point.z -= Z;
			
			Vector3 r;
			r.x = r.y = r.z = 1.0001f;
			
			float
				dyp = 1f - point.y,
				dzp = 1f - point.z;
			
			float dx = 0;
			int x = 0;
			while (true) {
				if (r.y > dx) {
					int hashX = NoiseMath.hash[TiledIndex(X + x, frequency, xOffset)];
					float dy = dx;
					int y = 0;
					while (true) {
						if (r.y > dy) {
							int Yy = TiledIndex(Y + y, frequency, yOffset);
							int hashY = NoiseMath.hash[hashX + Yy];
							float dz = dy;
							int z = 0;
							while (true) {
								if (r.y > dz) {
									int hash, Zz = TiledIndex(Z + z, frequency, zOffset);
									Vector3 v;
									v.x = x + (hash = NoiseMath.hash[hashY + Zz]) * stepSize - point.x;
									v.y = y + (hash = NoiseMath.hash[hash + Yy]) * stepSize - point.y;
									v.z = z + (hash = NoiseMath.hash[hash + Zz]) * stepSize - point.z;
									
									if (v.x < 0f) {
										v.x = -v.x;
									}
									if (v.y < 0f) {
										v.y = -v.y;
									}
									if (v.z < 0f) {
										v.z = -v.z;
									}
									float d = v.x >= v.y ? (v.x >= v.z ? v.x : v.z) : v.y >= v.z ? v.y : v.z;
									if (d < r.x) {
										r.z = hash;
										r.y = r.x;
										r.x = d;
									}
									else if (d < r.y) {
										r.y = d;
									}
								}
								
								if (z == 0) {
									z = -1;
									dz = dy >= point.z ? dy : point.z;
								}
								else if (z == -1) {
									z = 1;
									dz = dy >= dzp ? dy : dzp;
								}
								else {
									break;
								}
							}
						}
						
						if (y == 0) {
							y = -1;
							dy = dx >= point.y ? dx : point.y;
						}
						else if (y == -1) {
							y = 1;
							dy = dx >= dyp ? dx : dyp;
						}
						else {
							break;
						}
					}
				}
				
				if (x == 0) {
					x = -1;
					dx = point.x;
				}
				else if (x == -1) {
					x = 1;
					dx = 1f - point.x;
				}
				else {
					break;
				}
			}
			
			r.z *= 1f / NoiseMath.hashMask;
			return r;
		}
		
		/// <summary>Sample multi-frequency 3D Chebyshev Voronoi noise, tiled in all three dimension.</summary>
		/// <returns>The noise value.</returns>
		/// <param name="point">Sample point in 3D.</param>
		/// <param name="offset">Offset of the tiling domain.</param>
		/// <param name="frequency">Base frequency.</param>
		/// <param name="octaves">Amount of octaves.</param>
		/// <param name="lacunarity">Frequency multiplier for successive octaves.</param>
		/// <param name="persistence">Amplitude multiplier for succesive octaves.</param>
		public static Vector3 SampleChebyshev3DTiledXYZ (
			Vector3 point, Vector3 offset, int frequency, int octaves, int lacunarity, float persistence
		) {
			int
				xOffset = NoiseMath.FloorToInt(offset.x),
				yOffset = NoiseMath.FloorToInt(offset.y),
				zOffset = NoiseMath.FloorToInt(offset.z);
			point.x += offset.x - xOffset;
			point.y += offset.y - yOffset;
			point.z += offset.z - zOffset;
			float amplitude = 1f, range = 1f;
			Vector3 sum = SampleChebyshev3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
			while (--octaves > 0) {
				frequency *= lacunarity;
				amplitude *= persistence;
				range += amplitude;
				Vector3 sample = SampleChebyshev3DTiledXYZ(point, xOffset, yOffset, zOffset, frequency);
				sum.x += sample.x * amplitude;
				sum.y += sample.y * amplitude;
				sum.z += sample.z * amplitude;
			}
			float scale = 1f / range;
			sum.x *= scale;
			sum.y *= scale;
			sum.z *= scale;
			return sum;
		}
		
		static int TiledIndex (int i, int frequency, int offset) {
			i %= frequency;
			if (i < 0) {
				i += frequency;
			}
			return (i + offset) & NoiseMath.hashMask;
		}
	}
}