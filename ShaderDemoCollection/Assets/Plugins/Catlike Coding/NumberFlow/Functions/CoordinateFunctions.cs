// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Coordinates {
	
	public sealed class U : Function {
		
		protected override void Configure () {
			name = "U";
			menuName = "Coordinates/U";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Float = diagram.uv.x;
		}
	}
	
	public sealed class V : Function {
		
		protected override void Configure () {
			name = "V";
			menuName = "Coordinates/V";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Float = diagram.uv.y;
		}
	}
	
	public sealed class OneMinusU : Function {
		
		protected override void Configure () {
			name = "1 - U";
			menuName = "Coordinates/1 - U";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Float = 1f - diagram.uv.x;
		}
	}
	
	public sealed class OneMinusV : Function {
		
		protected override void Configure () {
			name = "1 - V";
			menuName = "Coordinates/1 - V";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Float = 1f - diagram.uv.y;
		}
	}
	
	public sealed class UVCenterDistance : Function {
		
		protected override void Configure () {
			name = "UV Center Distance";
			menuName = "Coordinates/UV Center Distance";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			float
				u = diagram.uv.x - 0.5f,
				v = diagram.uv.y - 0.5f;
			output.Float = Mathf.Sqrt(u * u + v * v);
		}
	}
	
	public sealed class UVCenterDistanceSquared : Function {
		
		protected override void Configure () {
			name = "UV Center Distance\u00b2";
			menuName = "Coordinates/UV Center Distance Squared";
			type = FunctionType.Coordinate;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			float
				u = diagram.uv.x - 0.5f,
				v = diagram.uv.y - 0.5f;
			output.Float = u * u + v * v;
		}
	}
	
	public sealed class X : Function {
		
		protected override void Configure () {
			name = "X";
			menuName = "Coordinates/X";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelX;
		}
	}
	
	public sealed class Y : Function {
		
		protected override void Configure () {
			name = "Y";
			menuName = "Coordinates/Y";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelY;
		}
	}
	
	public sealed class XMinusOne : Function {
		
		protected override void Configure () {
			name = "X - 1";
			menuName = "Coordinates/X - 1";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelX - 1;
		}
	}
	
	public sealed class YMinusOne : Function {
		
		protected override void Configure () {
			name = "Y - 1";
			menuName = "Coordinates/Y - 1";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelY - 1;
		}
	}
	
	public sealed class XPlusOne : Function {
		
		protected override void Configure () {
			name = "X + 1";
			menuName = "Coordinates/X + 1";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelX + 1;
		}
	}
	
	public sealed class YPlusOne : Function {
		
		protected override void Configure () {
			name = "Y + 1";
			menuName = "Coordinates/Y + 1";
			type = FunctionType.Coordinate;
			returnType = ValueType.Int;
		}
		
		public override void ComputeCoordinate (Value output, Diagram diagram) {
			output.Int = diagram.pixelY + 1;
		}
	}
}