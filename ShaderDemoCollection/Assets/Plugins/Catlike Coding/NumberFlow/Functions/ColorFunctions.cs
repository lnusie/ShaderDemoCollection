// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Colors {
	
	public sealed class FromFloats : Function {
		
		protected override void Configure () {
			name = "Color";
			menuName = "Colors/From Floats";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Red", "Green", "Blue", "Alpha" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float, ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color.r = arguments[0].Float;
			output.Color.g = arguments[1].Float;
			output.Color.b = arguments[2].Float;
			output.Color.a = arguments[3].Float;
		}
	}
	
	public sealed class FromVector3 : Function {
		
		protected override void Configure () {
			name = "Color";
			menuName = "Colors/From Vector3";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] {
				ValueType.Vector3
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector3 v = arguments[0].Vector3;
			output.Color.r = v.x;
			output.Color.g = v.y;
			output.Color.b = v.z;
			output.Color.a = 1f;
		}
	}

	public sealed class Lerp : Function {
		
		protected override void Configure () {
			name = "Lerp";
			menuName = "Colors/Lerp";
			returnType = ValueType.Color;
			propertyNames = new string[] { "A", "B", "T" };
			propertyTypes = new ValueType[] {
				ValueType.Color, ValueType.Color, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				a = arguments[0].Color,
				b = arguments[1].Color;
			float t = arguments[2].Float;
			if (t <= 0f) {
				output.Color = a;
			}
			else if (t >= 1f) {
				output.Color = b;
			}
			else {
				a.r += (b.r - a.r) * t;
				a.g += (b.g - a.g) * t;
				a.b += (b.b - a.b) * t;
				a.a += (b.a - a.a) * t;
				output.Color = a;
			}
		}
	}

	#region Channels

	public sealed class GetRed : Function {
		
		protected override void Configure () {
			name = "Red";
			menuName = "Colors/Channels/Get Red";
			propertyNames = new string[] { "Color" };
			propertyTypes = new ValueType[] { ValueType.Color };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Color.r;
		}
	}
	
	public sealed class GetGreen : Function {
		
		protected override void Configure () {
			name = "Green";
			menuName = "Colors/Channels/Get Green";
			propertyNames = new string[] { "Color" };
			propertyTypes = new ValueType[] { ValueType.Color };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Color.g;
		}
	}
	
	public sealed class GetBlue : Function {
		
		protected override void Configure () {
			name = "Blue";
			menuName = "Colors/Channels/Get Blue";
			propertyNames = new string[] { "Color" };
			propertyTypes = new ValueType[] { ValueType.Color };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Color.b;
		}
	}
	
	public sealed class GetAlpha : Function {
		
		protected override void Configure () {
			name = "Alpha";
			menuName = "Colors/Channels/Get Alpha";
			propertyNames = new string[] { "Color" };
			propertyTypes = new ValueType[] { ValueType.Color };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Color.a;
		}
	}
	
	public sealed class GetChannel : Function {
		
		protected override void Configure () {
			name = "Channel";
			menuName = "Colors/Channels/Get Channel";
			propertyNames = new string[] { "Color", "Channel" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Int };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int i = arguments[1].Int;
			if (i < 0 || i > 3) {
				i = 0;
			}
			output.Float = arguments[0].Color[i];
		}
	}
	
	public sealed class SetRed : Function {
		
		protected override void Configure () {
			name = "Set Red";
			menuName = "Colors/Channels/Set Red";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Red" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
			output.Color.r = arguments[1].Float;
		}
	}
	
	public sealed class SetGreen : Function {
		
		protected override void Configure () {
			name = "Set Green";
			menuName = "Colors/Channels/Set Green";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Green" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
			output.Color.g = arguments[1].Float;
		}
	}
	
	public sealed class SetBlue : Function {
		
		protected override void Configure () {
			name = "Set Blue";
			menuName = "Colors/Channels/Set Blue";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Blue" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
			output.Color.b = arguments[1].Float;
		}
	}
	
	public sealed class SetAlpha : Function {
		
		protected override void Configure () {
			name = "Set Alpha";
			menuName = "Colors/Channels/Set Alpha";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Alpha" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
			output.Color.a = arguments[1].Float;
		}
	}
	
	public sealed class SetChannel : Function {
		
		protected override void Configure () {
			name = "Set Channel";
			menuName = "Colors/Channels/Set Channel";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Channel" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Int };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int i = arguments[1].Int;
			if (i < 0 || i > 3) {
				i = 0;
			}
			output.Color = arguments[0].Color;
			output.Color[i] = arguments[1].Float;
		}
	}

	#endregion
	
	public sealed class Add : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Colors/Add";
			returnType = ValueType.Color;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				a = arguments[0].Color,
				b = arguments[1].Color;
			a.r += b.r;
			a.g += b.g;
			a.b += b.b;
			a.a += b.a;
			output.Color = a;
		}
	}
	
	public sealed class AddAdd : Function {
		
		protected override void Configure () {
			name = "+ +";
			menuName = "Colors/Add Add";
			returnType = ValueType.Color;
			propertyNames = new string[] { "A", "B", "C" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				a = arguments[0].Color,
				b = arguments[1].Color,
				c = arguments[2].Color;
			a.r += b.r + c.r;
			a.g += b.g + c.g;
			a.b += b.b + c.b;
			a.a += b.a + c.a;
			output.Color = a;
		}
	}
	
	public sealed class Scale : Function {
		
		protected override void Configure () {
			name = "Scale";
			menuName = "Colors/Scale";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Factor" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color c = arguments[0].Color;
			float f = arguments[1].Float;
			c.r *= f;
			c.g *= f;
			c.b *= f;
			c.a *= f;
			output.Color = c;
		}
	}

	#region Blend Modes

	public sealed class BlendNormal : Function {
		
		protected override void Configure () {
			name = "Normal";
			menuName = "Colors/Blend Modes/Normal";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color;

			float t;
			if (bottom.a == 1f) {
				t = 1f - top.a;
				bottom.r = top.r * top.a + bottom.r * t;
				bottom.g = top.g * top.a + bottom.g * t;
				bottom.b = top.b * top.a + bottom.b * t;
			}
			else {
				t = bottom.a * (1f - top.a);
				bottom.a = top.a + t;
				float invA = bottom.a == 0f ? 1f : (1f / bottom.a);
				bottom.r = (top.r * top.a + bottom.r * t) * invA;
				bottom.g = (top.g * top.a + bottom.g * t) * invA;
				bottom.b = (top.b * top.a + bottom.b * t) * invA;
			}
			output.Color = bottom;
		}
	}
	
	public sealed class BlendDarken : Function {
		
		protected override void Configure () {
			name = "Darken";
			menuName = "Colors/Blend Modes/Darken";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (top.r < bottom.r ? top.r : bottom.r) * top.a;
			blend.g = bottom.g * blend.a + (top.g < bottom.g ? top.g : bottom.g) * top.a;
			blend.b = bottom.b * blend.a + (top.b < bottom.b ? top.b : bottom.b) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendMultiply : Function {
		
		protected override void Configure () {
			name = "Multiply";
			menuName = "Colors/Blend Modes/Multiply";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (top.r * bottom.r) * top.a;
			blend.g = bottom.g * blend.a + (top.g * bottom.g) * top.a;
			blend.b = bottom.b * blend.a + (top.b * bottom.b) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendColorBurn : Function {
		
		protected override void Configure () {
			name = "Color Burn";
			menuName = "Colors/Blend Modes/Color Burn";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;

			blend.r = bottom.r * blend.a + (1f - (1f - bottom.r) / (top.r != 0f ? top.r : 1e-10f)) * top.a;
			blend.g = bottom.g * blend.a + (1f - (1f - bottom.g) / (top.g != 0f ? top.g : 1e-10f)) * top.a;
			blend.b = bottom.b * blend.a + (1f - (1f - bottom.b) / (top.b != 0f ? top.b : 1e-10f)) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendLinearBurn : Function {
		
		protected override void Configure () {
			name = "Linear Burn";
			menuName = "Colors/Blend Modes/Linear Burn";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (bottom.r + top.r - 1f) * top.a;
			blend.g = bottom.g * blend.a + (bottom.g + top.g - 1f) * top.a;
			blend.b = bottom.b * blend.a + (bottom.b + top.b - 1f) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendLighten : Function {
		
		protected override void Configure () {
			name = "Lighten";
			menuName = "Colors/Blend Modes/Lighten";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (top.r > bottom.r ? top.r : bottom.r) * top.a;
			blend.g = bottom.g * blend.a + (top.g > bottom.g ? top.g : bottom.g) * top.a;
			blend.b = bottom.b * blend.a + (top.b > bottom.b ? top.b : bottom.b) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendScreen : Function {
		
		protected override void Configure () {
			name = "Screen";
			menuName = "Colors/Blend Modes/Screen";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (1f - (1f - bottom.r) * (1f - top.r)) * top.a;
			blend.g = bottom.g * blend.a + (1f - (1f - bottom.g) * (1f - top.g)) * top.a;
			blend.b = bottom.b * blend.a + (1f - (1f - bottom.b) * (1f - top.b)) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendColorDodge : Function {
		
		protected override void Configure () {
			name = "Color Dodge";
			menuName = "Colors/Blend Modes/Color Dodge";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;

			blend.r = bottom.r * blend.a + (bottom.r / (top.r == 1f ? 1e-10f : (1f - top.r))) * top.a;
			blend.g = bottom.g * blend.a + (bottom.g / (top.g == 1f ? 1e-10f : (1f - top.g))) * top.a;
			blend.b = bottom.b * blend.a + (bottom.b / (top.b == 1f ? 1e-10f : (1f - top.b))) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendLinearDodge : Function {
		
		protected override void Configure () {
			name = "Linear Dodge";
			menuName = "Colors/Blend Modes/Linear Dodge";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (bottom.r + top.r) * top.a;
			blend.g = bottom.g * blend.a + (bottom.g + top.g) * top.a;
			blend.b = bottom.b * blend.a + (bottom.b + top.b) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendOverlay : Function {
		
		protected override void Configure () {
			name = "Overlay";
			menuName = "Colors/Blend Modes/Overlay";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (bottom.r < 0.5f ? 2f * bottom.r * top.r : (1f - 2f * (1f - bottom.r) * (1f - top.r))) * top.a;
			blend.g = bottom.g * blend.a + (bottom.g < 0.5f ? 2f * bottom.g * top.g : (1f - 2f * (1f - bottom.g) * (1f - top.g))) * top.a;
			blend.b = bottom.b * blend.a + (bottom.b < 0.5f ? 2f * bottom.b * top.b : (1f - 2f * (1f - bottom.b) * (1f - top.b))) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendHardLight : Function {
		
		protected override void Configure () {
			name = "Hard Light";
			menuName = "Colors/Blend Modes/Hard Light";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (top.r < 0.5f ? 2f * bottom.r * top.r : (1f - 2f * (1f - bottom.r) * (1f - top.r))) * top.a;
			blend.g = bottom.g * blend.a + (top.g < 0.5f ? 2f * bottom.g * top.g : (1f - 2f * (1f - bottom.g) * (1f - top.g))) * top.a;
			blend.b = bottom.b * blend.a + (top.b < 0.5f ? 2f * bottom.b * top.b : (1f - 2f * (1f - bottom.b) * (1f - top.b))) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}
	
	public sealed class BlendSoftLight : Function {
		
		protected override void Configure () {
			name = "Soft Light";
			menuName = "Colors/Blend Modes/Soft Light";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Top", "Bottom" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color
				top = arguments[0].Color,
				bottom = arguments[1].Color,
				blend;
			blend.a = 1f - top.a;
			
			blend.r = bottom.r * blend.a + (bottom.r * bottom.r * (1f - 2f * top.r) + 2f * bottom.r * top.r) * top.a;
			blend.g = bottom.g * blend.a + (bottom.g * bottom.g * (1f - 2f * top.g) + 2f * bottom.g * top.g) * top.a;
			blend.b = bottom.b * blend.a + (bottom.b * bottom.b * (1f - 2f * top.b) + 2f * bottom.b * top.b) * top.a;
			blend.a = bottom.a;
			output.Color = blend;
		}
	}

	#endregion
	
	public sealed class Clamp : Function {
		
		protected override void Configure () {
			name = "Clamp";
			menuName = "Colors/Clamp";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color c = arguments[0].Color;
			if (c.r < 0f) {
				c.r = 0f;
			}
			else if (c.r > 1f) {
				c.r = 1f;
			}
			if (c.g < 0f) {
				c.b = 0f;
			}
			else if (c.g > 1f) {
				c.g = 1f;
			}
			if (c.b < 0f) {
				c.g = 0f;
			}
			else if (c.b > 1f) {
				c.b = 1f;
			}
			if (c.a < 0f) {
				c.a = 0f;
			}
			else if (c.a > 1f) {
				c.a = 1f;
			}
			output.Color = c;
		}
	}
	
	public sealed class Gradient : Function {
		
		protected override void Configure () {
			name = "Gradient";
			menuName = "Colors/Gradient";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Gradient", "T" };
			propertyTypes = new ValueType[] {
				ValueType.Gradient, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Gradient.Evaluate(arguments[1].Float);
		}
	}
}