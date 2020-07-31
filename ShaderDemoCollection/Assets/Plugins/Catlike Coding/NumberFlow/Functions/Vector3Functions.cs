// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Vector3s {
	
	public sealed class Add : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Vector3/+ Add";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3
				a = arguments[0].Vector3,
				b = arguments[1].Vector3;
			a.x += b.x;
			a.y += b.y;
			a.z += b.z;
			output.Vector3 = a;
		}
	}
	
	public sealed class AddFloat : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Vector3/+ Add Float";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 a = arguments[0].Vector3;
			float b = arguments[1].Float;
			a.x += b;
			a.y += b;
			a.z += b;
			output.Vector3 = a;
		}
	}
	
	public sealed class Subtract : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Vector3/- Subtract";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3
				a = arguments[0].Vector3,
				b = arguments[1].Vector3;
			a.x -= b.x;
			a.y -= b.y;
			a.z -= b.z;
			output.Vector3 = a;
		}
	}
	
	public sealed class SubtractFloat : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Vector3/- Subtract Float";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 a = arguments[0].Vector3;
			float b = arguments[1].Float;
			a.x -= b;
			a.y -= b;
			a.z -= b;
			output.Vector3 = a;
		}
	}
	
	public sealed class Scale : Function {
		
		protected override void Configure () {
			name = "Scale";
			menuName = "Vector3/Scale";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector", "Factor" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			float f = arguments[1].Float;
			v.x *= f;
			v.y *= f;
			v.z *= f;
			output.Vector3 = v;
		}
	}
	
	public sealed class Cross : Function {
		
		protected override void Configure () {
			name = "\u00d7";
			menuName = "Vector3/\u00d7 Cross";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3
				a = arguments[0].Vector3,
				b = arguments[1].Vector3,
				c;
			c.x = a.y * b.z - a.z * b.y;
			c.y = a.z * b.x - a.x * b.z;
			c.z = a.x * b.y - a.y * b.x;
			output.Vector3 = c;
		}
	}
	
	public sealed class Dot : Function {
		
		protected override void Configure () {
			name = "\u00b7";
			menuName = "Vector3/\u00b7 Dot";
			returnType = ValueType.Float;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3, ValueType.Vector3
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3
				a = arguments[0].Vector3,
				b = arguments[1].Vector3;
			output.Float = a.x * b.x + a.y * b.y + a.z * b.z;
		}
	}
	
	public sealed class FromFloats : Function {
		
		protected override void Configure () {
			name = "\u2198";
			menuName = "Vector3/From Floats";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "X", "Y", "Z" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v;
			v.x = arguments[0].Float;
			v.y = arguments[1].Float;
			v.z = arguments[2].Float;
			output.Vector3 = v;
		}
	}
	
	public sealed class Length : Function {
		
		protected override void Configure () {
			name = "Length";
			menuName = "Vector3/Length";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		}
	}
	
	public sealed class SquareLength : Function {
		
		protected override void Configure () {
			name = "Length\u00b2";
			menuName = "Vector3/Square Length";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = v.x * v.x + v.y * v.y + v.z * v.z;
		}
	}
	
	public sealed class Negate : Function {
		
		protected override void Configure () {
			name = "Negate";
			menuName = "Vector3/Negate";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			v.x = -v.x;
			v.y = -v.y;
			v.z = -v.z;
			output.Vector3 = v;
		}
	}
	
	public sealed class Normalize : Function {
		
		protected override void Configure () {
			name = "Normalize";
			menuName = "Vector3/Normalize";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Vector3.normalized;
		}
	}

	public sealed class SphereVector : Function {
		
		protected override void Configure () {
			name = "Sphere \u2198";
			menuName = "Vector3/Sphere \u2198";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Longitude", "Latitude" };
			propertyTypes = new ValueType[] { ValueType.Float, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 p;
			float
				longitude = arguments[0].Float,
				latitude = arguments[1].Float;
			p.y = Mathf.Sin(latitude * Mathf.PI);
			p.x = p.y * Mathf.Sin(longitude * (Mathf.PI * -2f));
			p.z = p.y * Mathf.Sin((longitude + 0.25f) * (Mathf.PI * 2f));
			p.y = Mathf.Sin((latitude - 0.5f) * Mathf.PI);
			output.Vector3 = p;
		}
	}

	#region Axes
	
	public sealed class GetX : Function {
		
		protected override void Configure () {
			name = "X";
			menuName = "Vector3/Axes/Get X";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Vector3.x;
		}
	}

	public sealed class GetY : Function {
		
		protected override void Configure () {
			name = "Y";
			menuName = "Vector3/Axes/Get Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Vector3.y;
		}
	}
	
	public sealed class GetZ : Function {
		
		protected override void Configure () {
			name = "Z";
			menuName = "Vector3/Axes/Get Z";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Vector3.z;
		}
	}
	
	public sealed class SetX : Function {
		
		protected override void Configure () {
			name = "Set X";
			menuName = "Vector3/Axes/Set X";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector", "X" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Vector3;
			output.Vector3.x = arguments[1].Float;
		}
	}
	
	public sealed class SetY : Function {
		
		protected override void Configure () {
			name = "Set Y";
			menuName = "Vector3/Axes/Set Y";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector", "Y" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Vector3;
			output.Vector3.y = arguments[1].Float;
		}
	}
	
	public sealed class SetZ : Function {
		
		protected override void Configure () {
			name = "Set Z";
			menuName = "Vector3/Axes/Set Z";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector", "Z" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Vector3;
			output.Vector3.z = arguments[1].Float;
		}
	}

	public sealed class XPlusY : Function {
		
		protected override void Configure () {
			name = "X + Y";
			menuName = "Vector3/Axes/X + Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = v.x + v.y;
		}
	}

	public sealed class XTimesY : Function {
		
		protected override void Configure () {
			name = "X \u00d7 Y";
			menuName = "Vector3/Axes/X \u00d7 Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = v.x * v.y;
		}
	}

	public sealed class XMinusY : Function {
		
		protected override void Configure () {
			name = "X - Y";
			menuName = "Vector3/Axes/X - Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = v.x - v.y;
		}
	}

	public sealed class YMinusX : Function {
		
		protected override void Configure () {
			name = "Y - X";
			menuName = "Vector3/Axes/Y - X";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Float = v.y - v.x;
		}
	}

	#endregion

	#region Normals

	public sealed class PackNormalRGB : Function {
		
		protected override void Configure () {
			name = "Normal RGB";
			menuName = "Vector3/Normals/RGB";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Color.r = v.x * 0.5f + 0.5f;
			output.Color.g = v.y * 0.5f + 0.5f;
			output.Color.b = v.z * 0.5f + 0.5f;
			output.Color.a = 1f;
		}
	}
	
	public sealed class PackNormalDXT5 : Function {
		
		protected override void Configure () {
			name = "Normal DXT5nm";
			menuName = "Vector3/Normals/DXT5nm";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector3 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Color.r = 0f;
			output.Color.g = v.y * 0.5f + 0.5f;
			output.Color.b = 0f;
			output.Color.a = v.x * 0.5f + 0.5f;
		}
	}

	#endregion

	#region Voronoi Linear

	public sealed class VoronoiLinear : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear";
			menuName = "Noise 3D/Voronoi/Linear/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class VoronoiLinearTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear tX";
			menuName = "Noise 3D/Voronoi/Linear/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiLinearTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear tXY";
			menuName = "Noise 3D/Voronoi/Linear/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiLinearTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear tXYZ";
			menuName = "Noise 3D/Voronoi/Linear/Tiled XYZ";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiLinear2D : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear";
			menuName = "Noise 2D/Voronoi/Linear/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class VoronoiLinear2DTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear tX";
			menuName = "Noise 2D/Voronoi/Linear/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiLinear2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Linear tXY";
			menuName = "Noise 2D/Voronoi/Linear/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleLinear2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	#endregion

	#region Voronoi Squared
	
	public sealed class VoronoiSquared : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared";
			menuName = "Noise 3D/Voronoi/Squared/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}
	
	public sealed class VoronoiSquaredTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared tX";
			menuName = "Noise 3D/Voronoi/Squared/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiSquaredTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared tXY";
			menuName = "Noise 3D/Voronoi/Squared/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiSquaredTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared tXYZ";
			menuName = "Noise 3D/Voronoi/Squared/Tiled XYZ";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiSquared2D : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared";
			menuName = "Noise 2D/Voronoi/Squared/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class VoronoiSquared2DTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared tX";
			menuName = "Noise 2D/Voronoi/Squared/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiSquared2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Squared tXY";
			menuName = "Noise 2D/Voronoi/Squared/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleSquared2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	#endregion

	#region Voronoi Manhattan

	public sealed class VoronoiManhattan : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan";
			menuName = "Noise 3D/Voronoi/Manhattan/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}
	
	public sealed class VoronoiManhattanTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan tX";
			menuName = "Noise 3D/Voronoi/Manhattan/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiManhattanTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan tXY";
			menuName = "Noise 3D/Voronoi/Manhattan/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiManhattanTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan tXYZ";
			menuName = "Noise 3D/Voronoi/Manhattan/Tiled XYZ";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiManhattan2D : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan";
			menuName = "Noise 2D/Voronoi/Manhattan/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class VoronoiManhattan2DTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan tX";
			menuName = "Noise 2D/Voronoi/Manhattan/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiManhattan2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Manhattan tXY";
			menuName = "Noise 2D/Voronoi/Manhattan/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleManhattan2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	#endregion

	#region Voronoi Chebyshev
	
	public sealed class VoronoiChebyshev : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev";
			menuName = "Noise 3D/Voronoi/Chebyshev/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev3D(
				arguments[0].Vector3, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}
	
	public sealed class VoronoiChebyshevTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev tX";
			menuName = "Noise 3D/Voronoi/Chebyshev/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev3DTiledX(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiChebyshevTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev tXY";
			menuName = "Noise 3D/Voronoi/Chebyshev/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev3DTiledXY(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiChebyshevTiledXYZ : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev tXYZ";
			menuName = "Noise 3D/Voronoi/Chebyshev/Tiled XYZ";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev3DTiledXYZ(
				arguments[0].Vector3, arguments[1].Vector3, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	public sealed class VoronoiChebyshev2D : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev";
			menuName = "Noise 2D/Voronoi/Chebyshev/Regular";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev2D(
				arguments[0].Vector2, arguments[1].Float, arguments[2].Int, arguments[3].Float, arguments[4].Float);
		}
	}

	public sealed class VoronoiChebyshev2DTiledX : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev tX";
			menuName = "Noise 2D/Voronoi/Chebyshev/Tiled X";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev2DTiledX(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}
	
	public sealed class VoronoiChebyshev2DTiledXY : Function {
		
		protected override void Configure () {
			name = "Voronoi Chebyshev tXY";
			menuName = "Noise 2D/Voronoi/Chebyshev/Tiled XY";
			returnType = ValueType.Vector3;
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
			output.Vector3 = CatlikeCoding.Noise.VoronoiNoise.SampleChebyshev2DTiledXY(
				arguments[0].Vector2, arguments[1].Vector2, arguments[2].Int, arguments[3].Int, arguments[4].Int, arguments[5].Float);
		}
	}

	#endregion
}