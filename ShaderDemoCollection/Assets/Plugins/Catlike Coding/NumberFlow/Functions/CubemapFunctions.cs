// Copyright 2016, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Cubemap {
	
	internal static class CubemapUtils {
		
		internal static Vector3 GetDirection (Diagram diagram) {
			Vector2 uv = diagram.uv;
			uv.x = uv.x * 2f - 1f;
			uv.y = uv.y * 2f - 1f;
			Vector3 dir;
			switch (diagram.cubemapDirection) {
			case CubemapFace.PositiveX:
				dir.x = 1f;
				dir.y = -uv.y;
				dir.z = -uv.x;
				break;
			case CubemapFace.NegativeX:
				dir.x = -1f;
				dir.y = -uv.y;
				dir.z = uv.x;
				break;
			case CubemapFace.PositiveY:
				dir.x = uv.x;
				dir.y = 1f;
				dir.z = uv.y;
				break;
			case CubemapFace.NegativeY:
				dir.x = uv.x;
				dir.y = -1f;
				dir.z = -uv.y;
				break;
			case CubemapFace.PositiveZ:
				dir.x = uv.x;
				dir.y = -uv.y;
				dir.z = 1f;
				break;
			default:
				dir.x = -uv.x;
				dir.y = -uv.y;
				dir.z = -1f;
				break;
			}
			return NormalizeDir(dir);
		}
		
		internal static void GetCrossDirection (Value output, Diagram diagram) {
			Vector3 dir = GetDirection(diagram);
			float delta = diagram.derivativeDelta;
			if (diagram.derivativeDirection <= DiagramDerivativeDirection.NegativeX) {
				if (diagram.derivativeDirection == DiagramDerivativeDirection.NegativeX) {
					delta = -delta;
				}
				switch (diagram.cubemapDirection) {
				case CubemapFace.PositiveX:
				case CubemapFace.NegativeX:
					dir.x += delta * NorSin(dir.z, dir.y, dir.x);
					dir.z -= delta * dir.x;
					break;
				case CubemapFace.PositiveY:
					dir.x += delta * dir.y;
					dir.y -= delta * NorSin(dir.x, dir.z, dir.y);
					break;
				case CubemapFace.NegativeY:
					dir.x -= delta * dir.y;
					dir.y += delta * NorSin(dir.x, dir.z, dir.y);
					break;
				default:
					dir.x += delta * dir.z;
					dir.z -= delta * NorSin(dir.x, dir.y, dir.z);
					break;
				}
			}
			else {
				if (diagram.derivativeDirection == DiagramDerivativeDirection.PositiveY) {
					delta = -delta;
				}
				switch (diagram.cubemapDirection) {
				case CubemapFace.PositiveX:
					dir.x -= delta * NorSin(dir.y, dir.z, dir.x);
					dir.y += delta * dir.x;
					break;
				case CubemapFace.NegativeX:
					dir.x += delta * NorSin(dir.y, dir.z, dir.x);
					dir.y -= delta * dir.x;
					break;
				case CubemapFace.PositiveY:
				case CubemapFace.NegativeY:
					dir.y += delta * NorSin(dir.z, dir.x, dir.y);
					dir.z -= delta * dir.y;
					break;
				case CubemapFace.PositiveZ:
					dir.y += delta * dir.z;
					dir.z -= delta * NorSin(dir.y, dir.x, dir.z);
					break;
				default:
					dir.y -= delta * dir.z;
					dir.z += delta * NorSin(dir.y, dir.x, dir.z);
					break;
				}
			}
			output.Vector3 = NormalizeDir(dir);
		}
		
		static float NorSin (float x, float y, float cos) {
			return x * Mathf.Sqrt(1f - cos * cos) / Mathf.Sqrt(x * x + y * y);
		}
		
		static Vector3 NormalizeDir (Vector3 d) {
			float magnitude = Mathf.Sqrt(d.x * d.x + d.y * d.y + d.z * d.z);
			d.x /= magnitude;
			d.y /= magnitude;
			d.z /= magnitude;
			return d;
		}
	}
	
	public sealed class Direction : Function {

		protected override void Configure () {
			name = "Cubemap";
			menuName = "Coordinates/Cubemap Direction";
			type = FunctionType.Coordinate;
			returnType = ValueType.Vector3;
		}

		public override void ComputeCoordinate (Value output, Diagram diagram) {
			if (diagram.derivativeDirection == DiagramDerivativeDirection.None) {
				output.Vector3 = CubemapUtils.GetDirection(diagram);
			}
			else {
				CubemapUtils.GetCrossDirection(output, diagram);
			}
		}
	}
	
	public sealed class NormalCross : Function {
		
		protected override void Configure () {
			name = "Normal";
			menuName = "Vector3/Normals/Cubemap";
			type = FunctionType.Custom;
			returnType = ValueType.Vector3;
			propertyNames = new string[] { "Cubemap", "Height" };
			propertyTypes = new ValueType[] { ValueType.Vector3, ValueType.Float };
		}
		
		internal override void ComputeCustom (DiagramNode node) {
			Diagram diagram = node.Diagram;
			int oldPixelIndex = diagram.currentPixelIndex;
			DiagramDerivativeDirection oldDirection = diagram.derivativeDirection;
			
			Value cubemapValue = node.argumentValues[0];
			Value heightValue = node.argumentValues[1];
			
			diagram.currentPixelIndex -= 10;
			diagram.derivativeDirection = DiagramDerivativeDirection.NegativeX;
			node.ComputeArgumentNodes();
			Vector3 x = cubemapValue.Vector3 * heightValue.Float;
			
			diagram.currentPixelIndex -= 10;
			diagram.derivativeDirection = DiagramDerivativeDirection.PositiveX;
			node.ComputeArgumentNodes();
			x -= cubemapValue.Vector3 * heightValue.Float;
			
			diagram.currentPixelIndex -= 10;
			diagram.derivativeDirection = DiagramDerivativeDirection.NegativeY;
			node.ComputeArgumentNodes();
			Vector3 y = cubemapValue.Vector3 * heightValue.Float;
			
			diagram.currentPixelIndex -= 10;
			diagram.derivativeDirection = DiagramDerivativeDirection.PositiveY;
			node.ComputeArgumentNodes();
			y-= cubemapValue.Vector3 * heightValue.Float;
			
			node.computedValue.Vector3 = Vector3.Cross(y, x).normalized;
			
			diagram.currentPixelIndex = oldPixelIndex;
			diagram.derivativeDirection = oldDirection;
		}
	}
}