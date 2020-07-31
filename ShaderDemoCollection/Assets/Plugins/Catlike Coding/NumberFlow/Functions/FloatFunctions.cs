// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Floats {
	
	public sealed class Add : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Floats/+ Add";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float + arguments[1].Float;
		}
	}
	
	public sealed class AddAdd : Function {
		
		protected override void Configure () {
			name = "+ +";
			menuName = "Floats/+ + Add Add";
			propertyNames = new string[] { "A", "B", "C" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float + arguments[1].Float + arguments[2].Float;
		}
	}
	
	public sealed class Subtract : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Floats/- Subtract";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float - arguments[1].Float;
		}
	}
	
	public sealed class Multiply : Function {
		
		protected override void Configure () {
			name = "\u00d7";
			menuName = "Floats/\u00d7 Multiply";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float * arguments[1].Float;
		}
	}

	public sealed class MultiplyAdd : Function {
		
		protected override void Configure () {
			name = "\u00d7 +";
			menuName = "Floats/\u00d7 + Multiply Add";
			propertyNames = new string[] { "A", "B", "C" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float * arguments[1].Float + arguments[2].Float;
		}
	}
	
	public sealed class Divide : Function {
		
		protected override void Configure () {
			name = "\u00f7";
			menuName = "Floats/\u00f7 Divide";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float / arguments[1].Float;
		}
	}
	
	public sealed class Modulo : Function {
		
		protected override void Configure () {
			name = "%";
			menuName = "Floats/% Modulo";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float % arguments[1].Float;
		}
	}

	#region With 1

	public sealed class PlusOne : Function {
		
		protected override void Configure () {
			name = "+ 1";
			menuName = "Floats/With 1/+ 1";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float + 1f;
		}
	}

	public sealed class MinusOne : Function {
		
		protected override void Configure () {
			name = "- 1";
			menuName = "Floats/With 1/- 1";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float - 1f;
		}
	}

	public sealed class OneMinus : Function {
		
		protected override void Configure () {
			name = "1 -";
			menuName = "Floats/With 1/1 -";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = 1f - arguments[0].Float;
		}
	}

	public sealed class DivideOne : Function {
		
		protected override void Configure () {
			name = "1 \u00f7";
			menuName = "Floats/With 1/1 \u00f7";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = 1f / arguments[0].Float;
		}
	}

	#endregion

	#region Relative

	public sealed class Highest : Function {
		
		protected override void Configure () {
			name = "Highest";
			menuName = "Floats/Relative/Highest";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float
				a = arguments[0].Float,
				b = arguments[1].Float;
			output.Float = a >= b ? a : b;
		}
	}

	public sealed class Lowest : Function {
		
		protected override void Configure () {
			name = "Lowest";
			menuName = "Floats/Relative/Lowest";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float
				a = arguments[0].Float,
				b = arguments[1].Float;
			output.Float = a <= b ? a : b;
		}
	}

	public sealed class Average : Function {
		
		protected override void Configure () {
			name = "Average";
			menuName = "Floats/Relative/Average";
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = (arguments[0].Float + arguments[1].Float) * 0.5f;
		}
	}

	#endregion

	public sealed class Absolute : Function {
		
		protected override void Configure () {
			name = "Abs";
			menuName = "Floats/Absolute";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			output.Float = v >= 0f ? v : -v;
		}
	}
	
	public sealed class Square : Function {
		
		protected override void Configure () {
			name = "x\u00b2";
			menuName = "Floats/x\u00b2 Square";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			output.Float = v * v;
		}
	}
	
	public sealed class Cube : Function {
		
		protected override void Configure () {
			name = "x\u00b3";
			menuName = "Floats/x\u00b3 Cube";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			output.Float = v * v * v;
		}
	}
	
	public sealed class SquareRoot : Function {
		
		protected override void Configure () {
			name = "\u221a";
			menuName = "Floats/\u221a Square Root";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = Mathf.Sqrt(arguments[0].Float);
		}
	}

	public sealed class FromInt : Function {
		
		protected override void Configure () {
			name = "Float";
			menuName = "Floats/From Int";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Int" };
			propertyTypes = new ValueType[] { ValueType.Int };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Int;
		}
	}
	
	public sealed class Curve : Function {
		
		protected override void Configure () {
			name = "Curve";
			menuName = "Floats/Curve";
			propertyNames = new string[] { "Curve", "T" };
			propertyTypes = new ValueType[] { ValueType.AnimationCurve, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].AnimationCurve.Evaluate(arguments[1].Float);
		}
	}
	
	public sealed class Lerp : Function {
		
		protected override void Configure () {
			name = "Lerp";
			menuName = "Floats/Lerp";
			propertyNames = new string[] { "A", "B", "T" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float
				a = arguments[0].Float,
				t = arguments[2].Float;
			if (t <= 0f) {
				output.Float = a;
			}
			else if (t >= 1f) {
				output.Float = arguments[1].Float;
			}
			else {
				output.Float = a + (arguments[1].Float - a) * t;
			}
		}
	}

	#region Trigonometry

	public sealed class Cosine : Function {
		
		protected override void Configure () {
			name = "Cosine";
			menuName = "Floats/Trigonometry/Cosine";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = Mathf.Cos(arguments[0].Float);
		}
	}
	
	public sealed class Sine : Function {
		
		protected override void Configure () {
			name = "Sine";
			menuName = "Floats/Trigonometry/Sine";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = Mathf.Sin(arguments[0].Float);
		}
	}
	
	public sealed class Sinusoid : Function {
		
		protected override void Configure () {
			name = "Sinusoid";
			menuName = "Floats/Trigonometry/Sinusoid";
			propertyNames = new string[] { "Value", "Frequency", "Offset" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = Mathf.Sin((arguments[0].Float * arguments[1].Float + arguments[2].Float) * (Mathf.PI * 2f));
		}
	}

	#endregion

	#region Range

	public sealed class Clamp01 : Function {
		
		protected override void Configure () {
			name = "Clamp 01";
			menuName = "Floats/Range/Clamp 01";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			if (v < 0f) {
				output.Float = 0f;
			}
			else if (v > 1f) {
				output.Float = 1f;
			}
			else {
				output.Float = v;
			}
		}
	}
	
	public sealed class Loop01 : Function {
		
		protected override void Configure () {
			name = "Loop 01";
			menuName = "Floats/Range/Loop 01";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			if (v < 0f ) {
				v += 1 - (int)v;
			}
			output.Float = v - (int)v;
		}
	}
	
	public sealed class PingPong01 : Function {
		
		protected override void Configure () {
			name = "Pingpong 01";
			menuName = "Floats/Range/Pingpong 01";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float v = arguments[0].Float;
			if (v < 0f) {
				v = -v;
			}
			int i = (int)v;
			output.Float = (i & 1) == 1 ? 1f - v + i : (v - i);
		}
	}

	#endregion

	#region Perlin

	public sealed class PerlinNoise : Function {
		
		protected override void Configure () {
			name = "Perlin";
			menuName = "Noise 3D/Perlin/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class PerlinNoiseTiledX : Function {
		
		protected override void Configure () {
			name = "Perlin tX";
			menuName = "Noise 3D/Perlin/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class PerlinNoiseTiledXY : Function {
		
		protected override void Configure () {
			name = "Perlin tXY";
			menuName = "Noise 3D/Perlin/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class PerlinNoiseTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Perlin tXYZ";
			menuName = "Noise 3D/Perlin/Tiled XYZ";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class PerlinNoise2D : Function {
		
		protected override void Configure () {
			name = "Perlin";
			menuName = "Noise 2D/Perlin/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class PerlinNoise2DTiledX : Function {
		
		protected override void Configure () {
			name = "Perlin tX";
			menuName = "Noise 2D/Perlin/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class PerlinNoise2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Perlin tXY";
			menuName = "Noise 2D/Perlin/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.Sample2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class PerlinTurbulence : Function {
		
		protected override void Configure () {
			name = "Turbulence";
			menuName = "Noise 3D/Turbulence/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class PerlinTurbulenceTiledX : Function {
		
		protected override void Configure () {
			name = "Turbulence tX";
			menuName = "Noise 3D/Turbulence/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class PerlinTurbulenceTiledXY : Function {
		
		protected override void Configure () {
			name = "Turbulence tXY";
			menuName = "Noise 3D/Turbulence/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class PerlinTurbulenceTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Turbulence tXYZ";
			menuName = "Noise 3D/Turbulence/Tiled XYZ";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class PerlinTurbulence2D : Function {
		
		protected override void Configure () {
			name = "Turbulence";
			menuName = "Noise 2D/Turbulence/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class PerlinTurbulence2DTiledX : Function {
		
		protected override void Configure () {
			name = "Turbulence tX";
			menuName = "Noise 2D/Turbulence/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class PerlinTurbulence2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Turbulence tXY";
			menuName = "Noise 2D/Turbulence/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.PerlinNoise.SampleTurbulence2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	#endregion

	#region Value

	public sealed class ValueNoise : Function {
		
		protected override void Configure () {
			name = "Value";
			menuName = "Noise 3D/Value/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}
	
	public sealed class ValueNoiseTiledX : Function {
		
		protected override void Configure () {
			name = "Value tX";
			menuName = "Noise 3D/Value/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class ValueNoiseTiledXY : Function {
		
		protected override void Configure () {
			name = "Value tXY";
			menuName = "Noise 3D/Value/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class ValueNoiseTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Noise tXYZ";
			menuName = "Noise 3D/Value/Tiled XYZ";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3,
				ValueType.Vector3,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class ValueNoise2D : Function {
		
		protected override void Configure () {
			name = "Value";
			menuName = "Noise 2D/Value/Regular";
			propertyNames = new string[] { "Point", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Float,
				ValueType.Int,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class ValueNoise2DTiledX : Function {
		
		protected override void Configure () {
			name = "Value tX";
			menuName = "Noise 2D/Value/Tiled X";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class ValueNoise2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Value tXY";
			menuName = "Noise 2D/Value/Tiled XY";
			propertyNames = new string[] { "Point", "Offset", "Frequency", "Octaves", "Lacunarity", "Persistence" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2,
				ValueType.Vector2,
				ValueType.Int,
				ValueType.Int,
				ValueType.Int,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = CatlikeCoding.Noise.ValueNoise.Sample2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	#endregion
}