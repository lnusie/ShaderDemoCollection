// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Outputs {
	
	public sealed class ColorOutput : Function {
		
		protected override void Configure () {
			name = "Output";
			menuName = "Ouput/Color";
			type = FunctionType.Output;
			returnType = ValueType.None;
			propertyNames = new string[] { "Color" };
			propertyTypes = new ValueType[] { ValueType.Color };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			output.Color = arguments[0].Color;
		}
	}

	public sealed class ValueOutput : Function {
		
		protected override void Configure () {
			name = "Output";
			menuName = "Ouput/Value";
			type = FunctionType.Output;
			returnType = ValueType.None;
			propertyNames = new string[] { "Value" };
			propertyTypes = new ValueType[] { ValueType.Float };
		}
		
		public override void Compute (Value output, Value[] arguments) {
			Color c;
			c.r = c.g = c.b = c.a = arguments[0].Float;
			output.Color = c;
		}
	}
}