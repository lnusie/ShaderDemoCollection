// Copyright 2015, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Vector2s {
	
	public sealed class Add : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Vector2/+ Add";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2
				a = arguments[0].Vector2,
				b = arguments[1].Vector2;
			a.x += b.x;
			a.y += b.y;
			output.Vector2 = a;
		}
	}
	
	public sealed class AddFloat : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Vector2/+ Add Float";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 a = arguments[0].Vector2;
			float b = arguments[1].Float;
			a.x += b;
			a.y += b;
			output.Vector2 = a;
		}
	}
	
	public sealed class Subtract : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Vector2/- Subtract";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2
				a = arguments[0].Vector2,
				b = arguments[1].Vector2;
			a.x -= b.x;
			a.y -= b.y;
			output.Vector2 = a;
		}
	}
	
	public sealed class SubtractFloat : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Vector2/- Subtract Float";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 a = arguments[0].Vector2;
			float b = arguments[1].Float;
			a.x -= b;
			a.y -= b;
			output.Vector2 = a;
		}
	}
	
	public sealed class Scale : Function {
		
		protected override void Configure () {
			name = "Scale";
			menuName = "Vector2/Scale";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "Vector", "Factor" };
			propertyTypes = new ValueType[] { ValueType.Vector2, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			float f = arguments[1].Float;
			v.x *= f;
			v.y *= f;
			output.Vector2 = v;
		}
	}
	
	public sealed class Dot : Function {
		
		protected override void Configure () {
			name = "\u00b7";
			menuName = "Vector2/\u00b7 Dot";
			returnType = ValueType.Float;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2, ValueType.Vector2
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2
				a = arguments[0].Vector2,
				b = arguments[1].Vector2;
			output.Float = a.x * b.x + a.y * b.y;
		}
	}
	
	public sealed class FromFloats : Function {
		
		protected override void Configure () {
			name = "\u2198";
			menuName = "Vector2/From Floats";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "X", "Y" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v;
			v.x = arguments[0].Float;
			v.y = arguments[1].Float;
			output.Vector2 = v;
		}
	}
	
	public sealed class Length : Function {
		
		protected override void Configure () {
			name = "Length";
			menuName = "Vector2/Length";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = Mathf.Sqrt(v.x * v.x + v.y * v.y);
		}
	}
	
	public sealed class SquareLength : Function {
		
		protected override void Configure () {
			name = "Length\u00b2";
			menuName = "Vector2/Square Length";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] {
				ValueType.Vector2
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = v.x * v.x + v.y * v.y;
		}
	}
	
	public sealed class Negate : Function {
		
		protected override void Configure () {
			name = "Negate";
			menuName = "Vector2/Negate";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "Vector" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			v.x = -v.x;
			v.y = -v.y;
			output.Vector2 = v;
		}
	}
	
	public sealed class Normalize : Function {
		
		protected override void Configure () {
			name = "Normalize";
			menuName = "Vector2/Normalize";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "Vector" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector2 = arguments[0].Vector2.normalized;
		}
	}

	#region Axes
	
	public sealed class GetX : Function {
		
		protected override void Configure () {
			name = "X";
			menuName = "Vector2/Axes/Get X";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Vector2.x;
		}
	}

	public sealed class GetY : Function {
		
		protected override void Configure () {
			name = "Y";
			menuName = "Vector2/Axes/Get Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Vector2.y;
		}
	}
	
	public sealed class SetX : Function {
		
		protected override void Configure () {
			name = "Set X";
			menuName = "Vector2/Axes/Set X";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "Vector", "X" };
			propertyTypes = new ValueType[] { ValueType.Vector2, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector2 = arguments[0].Vector2;
			output.Vector2.x = arguments[1].Float;
		}
	}
	
	public sealed class SetY : Function {
		
		protected override void Configure () {
			name = "Set Y";
			menuName = "Vector2/Axes/Set Y";
			returnType = ValueType.Vector2;
			propertyNames = new string[] { "Vector", "Y" };
			propertyTypes = new ValueType[] { ValueType.Vector2, ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector2 = arguments[0].Vector2;
			output.Vector2.y = arguments[1].Float;
		}
	}
	
	public sealed class XPlusY : Function {
		
		protected override void Configure () {
			name = "X + Y";
			menuName = "Vector2/Axes/X + Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = v.x + v.y;
		}
	}

	public sealed class XTimesY : Function {
		
		protected override void Configure () {
			name = "X \u00d7 Y";
			menuName = "Vector2/Axes/X \u00d7 Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = v.x * v.y;
		}
	}

	public sealed class XMinusY : Function {
		
		protected override void Configure () {
			name = "X - Y";
			menuName = "Vector2/Axes/X - Y";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = v.x - v.y;
		}
	}

	public sealed class YMinusX : Function {
		
		protected override void Configure () {
			name = "Y - X";
			menuName = "Vector2/Axes/Y - X";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Vector" };
			propertyTypes = new ValueType[] { ValueType.Vector2 };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Vector2 v = arguments[0].Vector2;
			output.Float = v.y - v.x;
		}
	}

	#endregion
}