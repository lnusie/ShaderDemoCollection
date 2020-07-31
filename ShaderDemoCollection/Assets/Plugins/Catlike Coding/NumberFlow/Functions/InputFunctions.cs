// Copyright 2013, Catlike Coding, http://catlikecoding.com
namespace CatlikeCoding.NumberFlow.Functions.Input {
	
	public sealed class BoolInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Bool";
			type = FunctionType.Input;
			returnType = ValueType.Bool;
			propertyNames = new string[] { "Bool", "Name" };
			propertyTypes = new ValueType[] { ValueType.Bool, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Bool = arguments[0].Bool;
		}
	}
	
	
	public sealed class ColorInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Color";
			type = FunctionType.Input;
			returnType = ValueType.Color;
			propertyNames = new string[] { "Color", "Name" };
			propertyTypes = new ValueType[] { ValueType.Color, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
		}
	}
	
	public sealed class AnimationCurveInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Curve";
			type = FunctionType.Input;
			returnType = ValueType.AnimationCurve;
			propertyNames = new string[] { "Curve", "Name" };
			propertyTypes = new ValueType[] { ValueType.AnimationCurve, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.AnimationCurve = arguments[0].AnimationCurve;
		}
	}
	
	public sealed class GradientInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Gradient";
			type = FunctionType.Input;
			returnType = ValueType.Gradient;
			propertyNames = new string[] { "Gradient", "Name" };
			propertyTypes = new ValueType[] { ValueType.Gradient, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Gradient = arguments[0].Gradient;
		}
	}
	
	public sealed class FloatInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Float";
			type = FunctionType.Input;
			propertyNames = new string[] { "Float", "Name" };
			propertyTypes = new ValueType[] { ValueType.Float, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Float = arguments[0].Float;
		}
	}
	
	public sealed class IntInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Int";
			type = FunctionType.Input;
			returnType = ValueType.Int;
			propertyNames = new string[] { "Int", "Name" };
			propertyTypes = new ValueType[] { ValueType.Int, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Int = arguments[0].Int;
		}
	}
	
	public sealed class Vector3Input : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Vector3";
			type = FunctionType.Input;
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Vector3", "Name" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Vector3 = arguments[0].Vector3;
		}
	}
	
	public sealed class PixelsInput : Function {
		
		protected override void Configure () {
			name = "Input";
			menuName = "Input/Pixels";
			type = FunctionType.Input;
			returnType = ValueType.Pixels;
			propertyNames = new string[] { "Pixels", "Name" };
			propertyTypes = new ValueType[] { ValueType.Pixels, ValueType.String };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Pixels = arguments[0].Pixels;
		}
	}
}