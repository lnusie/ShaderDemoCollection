// Copyright 2013, Catlike Coding, http://catlikecoding.com
namespace CatlikeCoding.NumberFlow.Functions.Booleans {
	
	public sealed class And : Function {
		
		protected override void Configure () {
			name = "And";
			menuName = "Booleans/And";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool && arguments[1].Bool;
		}
	}
	
	public sealed class Or : Function {
		
		protected override void Configure () {
			name = "Or";
			menuName = "Booleans/Or";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool || arguments[1].Bool;
		}
	}
	
	public sealed class Not : Function {
		
		protected override void Configure () {
			name = "Not";
			menuName = "Booleans/Not";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "Bool" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = !arguments[0].Bool;
		}
	}
	
	public sealed class BoolEqual : Function {
		
		protected override void Configure () {
			name = "==";
			menuName = "Booleans/Bool ==";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool == arguments[1].Bool;
		}
	}
	
	public sealed class BoolNotEqual : Function {
		
		protected override void Configure () {
			name = "!=";
			menuName = "Booleans/Bool !=";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool != arguments[1].Bool;
		}
	}
	
	public sealed class ChooseBool : Function {
		
		protected override void Configure () {
			name = "Choose";
			menuName = "Booleans/Choose/Bool";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "Pick A?", "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool ? arguments[1].Bool : arguments[2].Bool;
		}
	}
	
	public sealed class ChooseColor : Function {
		
		protected override void Configure () {
			name = "Choose";
			menuName = "Booleans/Choose/Color";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Pick A?", "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Bool,
				ValueType.Color,
				ValueType.Color
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Bool ? arguments[1].Color : arguments[2].Color;
		}
	}
	
	public sealed class ChooseFloat : Function {
		
		protected override void Configure () {
			name = "Choose";
			menuName = "Booleans/Choose/Float";
			returnType = ValueType.Float;
			propertyNames = new string[] { "Pick A?", "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Bool,
				ValueType.Float,
				ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Bool ? arguments[1].Float : arguments[2].Float;
		}
	}
	
	public sealed class ChooseInt : Function {
		
		protected override void Configure () {
			name = "Choose";
			menuName = "Booleans/Choose/Int";
			returnType = ValueType.Int;
			propertyNames = new string[] { "Pick A?", "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Bool,
				ValueType.Int,
				ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Bool ? arguments[1].Int : arguments[2].Int;
		}
	}
	
	public sealed class ChooseVector3 : Function {
		
		protected override void Configure () {
			name = "Choose";
			menuName = "Booleans/Choose/Vector3";
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Pick A?", "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Bool,
				ValueType.Vector3,
				ValueType.Vector3
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Bool ? arguments[1].Vector3 : arguments[2].Vector3;
		}
	}
	
	public sealed class FloatLess : Function {
		
		protected override void Configure () {
			name = "<";
			menuName = "Booleans/Float <";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Float < arguments[1].Float;
		}
	}
	
	public sealed class FloatLessOrEqual : Function {
		
		protected override void Configure () {
			name = "\u2264";
			menuName = "Booleans/Float \u2264";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Float <= arguments[1].Float;
		}
	}
	
	public sealed class FloatEqual : Function {
		
		protected override void Configure () {
			name = "==";
			menuName = "Booleans/Float ==";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Float == arguments[1].Float;
		}
	}
	
	public sealed class FloatGreaterOrEqual : Function {
		
		protected override void Configure () {
			name = "\u2265";
			menuName = "Booleans/Float \u2265";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Float >= arguments[1].Float;
		}
	}
	
	public sealed class FloatGreater : Function {
		
		protected override void Configure () {
			name = ">";
			menuName = "Booleans/Float >";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Float > arguments[1].Float;
		}
	}
	
	public sealed class FloatBetweenInclusive : Function {
		
		protected override void Configure () {
			name = "\u2264 \u2264";
			menuName = "Booleans/Float \u2264 \u2264";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B", "C" };
			propertyTypes = new ValueType[] {
				ValueType.Float, ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			float b = arguments[1].Float;
			output.Bool = arguments[0].Float <= b && b <= arguments[2].Float;
		}
	}
	
	public sealed class IntLess : Function {
		
		protected override void Configure () {
			name = "<";
			menuName = "Booleans/Int <";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Int < arguments[1].Int;
		}
	}
	
	public sealed class IntLessOrEqual : Function {
		
		protected override void Configure () {
			name = "\u2264";
			menuName = "Booleans/Int \u2264";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Int <= arguments[1].Int;
		}
	}
	
	public sealed class IntEqual : Function {
		
		protected override void Configure () {
			name = "==";
			menuName = "Booleans/Int ==";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Int == arguments[1].Int;
		}
	}
	
	public sealed class IntGreaterOrEqual : Function {
		
		protected override void Configure () {
			name = "\u2265";
			menuName = "Booleans/Int \u2265";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Int >= arguments[1].Int;
		}
	}
	
	public sealed class IntGreater : Function {
		
		protected override void Configure () {
			name = ">";
			menuName = "Booleans/Int >";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Int <= arguments[1].Int;
		}
	}
	
	public sealed class IntBetweenInclusive : Function {
		
		protected override void Configure () {
			name = "\u2264 \u2264";
			menuName = "Booleans/Int \u2264 \u2264";
			returnType = ValueType.Bool;
			propertyNames = new string[] { "A", "B", "C" };
			propertyTypes = new ValueType[] {
				ValueType.Int, ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int b = arguments[1].Int;
			output.Bool = arguments[0].Int <= b && b <= arguments[2].Int;
		}
	}
}