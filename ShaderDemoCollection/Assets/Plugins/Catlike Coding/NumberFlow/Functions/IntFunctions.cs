// Copyright 2013, Catlike Coding, http://catlikecoding.com
namespace CatlikeCoding.NumberFlow.Functions.Ints {
	
	public sealed class Add : Function {
		
		protected override void Configure () {
			name = "+";
			menuName = "Ints/+ Add";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int + arguments[1].Int;
		}
	}
	
	public sealed class AddAdd : Function {
		
		protected override void Configure () {
			name = "+ +";
			menuName = "Ints/+ + Add Add";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B", "C" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int + arguments[1].Int + arguments[2].Int;
		}
	}
	
	public sealed class Subtract : Function {
		
		protected override void Configure () {
			name = "-";
			menuName = "Ints/- Subtract";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int - arguments[1].Int;
		}
	}
	
	public sealed class Multiply : Function {
		
		protected override void Configure () {
			name = "\u00d7";
			menuName = "Ints/\u00d7 Multiply";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int * arguments[1].Int;
		}
	}
	
	public sealed class Divide : Function {
		
		protected override void Configure () {
			name = "\u00f7";
			menuName = "Ints/\u00f7 Divide";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int / arguments[1].Int;
		}
	}
	
	public sealed class Modulo : Function {
		
		protected override void Configure () {
			name = "%";
			menuName = "Ints/% Modulo";
			returnType = ValueType.Int;
			propertyNames = new string[] { "A", "B" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int % arguments[1].Int;
		}
	}
	
	public sealed class Absolute : Function {
		
		protected override void Configure () {
			name = "Abs";
			menuName = "Ints/Absolute";
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int v = arguments[0].Int;
			output.Int = v >= 0 ? v : -v;
		}
	}
	
	public sealed class Square : Function {
		
		protected override void Configure () {
			name = "x\u00b2";
			menuName = "Ints/x\u00b2 Square";
			returnType = ValueType.Int;
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int v = arguments[0].Int;
			output.Int = v * v;
		}
	}
	
	public sealed class Cube : Function {
		
		protected override void Configure () {
			name = "x\u00b3";
			menuName = "Ints/x\u00b3 Cube";
			returnType = ValueType.Int;
			propertyNames = new string[] { "Value" };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			int v = arguments[0].Int;
			output.Int = v * v * v;
		}
	}
	
	public sealed class FromFloat : Function {
		
		protected override void Configure () {
			name = "Int";
			menuName = "Ints/From Float";
			returnType = ValueType.Int;
			propertyNames = new string[] { "Float" };
			propertyTypes = new ValueType[] { ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = (int)arguments[0].Float;
		}
	}
}