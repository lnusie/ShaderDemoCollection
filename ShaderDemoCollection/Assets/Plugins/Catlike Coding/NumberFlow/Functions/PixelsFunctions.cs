// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Functions.Pixels {
	
	public sealed class GetXYClamped : Function {
		
		protected override void Configure () {
			name = "XY Clamped";
			menuName = "Pixels/XY Clamped";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Pixels", "X", "Y" };
			propertyTypes = new ValueType[] {
				ValueType.Pixels, ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			DiagramPixels pixels = arguments[0].Pixels;
			int
				x = arguments[1].Int,
				y = arguments[2].Int;
			if (pixels.colors == null || pixels.width <= 0 || pixels.height <= 0) {
				if (pixels.texture == null) {
					output.Color = Color.clear;
					return;
				}
				output.Color = pixels.texture.GetPixel(
					x < 0 ? 0 : x >= pixels.texture.width ? pixels.texture.width - 1 : x,
					y < 0 ? 0 : y >= pixels.texture.height ? pixels.texture.height - 1 : y);
			}
			else {
				output.Color = pixels.colors[
					(x < 0 ? 0 : x >= pixels.width ? pixels.width - 1 : x) +
					(y < 0 ? 0 : y >= pixels.height ? pixels.height - 1 : y) * pixels.width];
			}
		}
	}
		
	public sealed class GetXYRepeating : Function {
		
		protected override void Configure () {
			name = "XY Repeating";
			menuName = "Pixels/XY Repeating";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Pixels", "X", "Y" };
			propertyTypes = new ValueType[] {
				ValueType.Pixels, ValueType.Int, ValueType.Int
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			DiagramPixels pixels = arguments[0].Pixels;
			int
				x = arguments[1].Int,
				y = arguments[2].Int;
			if (pixels.colors == null || pixels.width <= 0 || pixels.height <= 0) {
				if (pixels.texture == null) {
					output.Color = Color.clear;
					return;
				}
				Texture2D texture = pixels.texture;
				output.Color = texture.GetPixel(
					x < 0 ? (texture.width - 1 + (x + 1) % texture.width) : x >= texture.width ? x % texture.width : x,
					y < 0 ? (texture.height - 1 + (y + 1) % texture.height) : y >= texture.height ? y % texture.height : y);
			}
			else {
				output.Color = pixels.colors[
					(x < 0 ? (pixels.width - 1 + (x + 1) % pixels.width) : x >= pixels.width ? x % pixels.width : x) +
					(y < 0 ? (pixels.height - 1 + (y + 1) % pixels.height) : y >= pixels.height ? y % pixels.height : y) * pixels.width];
			}
		}
	}

	public sealed class GetUVClamped : Function {
		
		protected override void Configure () {
			name = "UV Clamped";
			menuName = "Pixels/UV Clamped";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Pixels", "U", "V" };
			propertyTypes = new ValueType[] {
				ValueType.Pixels, ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			DiagramPixels pixels = arguments[0].Pixels;
			float
				u = arguments[1].Float,
				v = arguments[2].Float;
			int x0, x1, y0, y1;
			Color c0, c1, c2;
			float tu, tv;
			if (pixels.colors == null || pixels.width <= 0 || pixels.height <= 0) {
				if (pixels.texture == null) {
					output.Color = Color.clear;
					return;
				}
				Texture2D texture = pixels.texture;
				u = u * texture.width + 0.5f;
				v = v * texture.height + 0.5f;
				tu = u - (x1 = (int)u);
				tv = v - (y1 = (int)v);
				if (x1 <= 0) {
					x0 = x1 = 0;
				}
				else if (x1 >= texture.width) {
					x0 = x1 = texture.width - 1;
				}
				else {
					x0 = x1 - 1;
				}
				if (y1 <= 0) {
					y0 = y1 = 0;
				}
				else if (y1 >= texture.height) {
					y0 = y1 = texture.height - 1;
				}
				else {
					y0 = y1 - 1;
				}
				c0 = texture.GetPixel(x0, y0);
				c1 = texture.GetPixel(x1, y0);
				
				c0.r += (c1.r - c0.r) * tu;
				c0.g += (c1.g - c0.g) * tu;
				c0.b += (c1.b - c0.b) * tu;
				c0.a += (c1.a - c0.a) * tu;
				
				c1 = texture.GetPixel(x0, y1);
				c2 = texture.GetPixel(x1, y1);
			}
			else {
				u = u * pixels.width + 0.5f;
				v = v * pixels.height + 0.5f;
				tu = u - (x1 = (int)u);
				tv = v - (y1 = (int)v);
				if (x1 <= 0) {
					x0 = x1 = 0;
				}
				else if (x1 >= pixels.width) {
					x0 = x1 = pixels.width - 1;
				}
				else {
					x0 = x1 - 1;
				}
				if (y1 <= 0) {
					y0 = y1 = 0;
				}
				else if (y1 >= pixels.height) {
					y0 = y1 = (pixels.height - 1) * pixels.width;
				}
				else {
					y1 *= pixels.width;
					y0 = y1 - pixels.width;
				}
				c0 = pixels.colors[x0 + y0];
				c1 = pixels.colors[x1 + y0];
				
				c0.r += (c1.r - c0.r) * tu;
				c0.g += (c1.g - c0.g) * tu;
				c0.b += (c1.b - c0.b) * tu;
				c0.a += (c1.a - c0.a) * tu;
				
				c1 = pixels.colors[x0 + y1];
				c2 = pixels.colors[x1 + y1];
			}
			
			c1.r += (c2.r - c1.r) * tu;
			c1.g += (c2.g - c1.g) * tu;
			c1.b += (c2.b - c1.b) * tu;
			c1.a += (c2.a - c1.a) * tu;
			
			c0.r += (c1.r - c0.r) * tv;
			c0.g += (c1.g - c0.g) * tv;
			c0.b += (c1.b - c0.b) * tv;
			c0.a += (c1.a - c0.a) * tv;
			
			output.Color = c0;
		}
	}
	
	public sealed class GetUVRepeating : Function {
		
		protected override void Configure () {
			name = "UV Repeating";
			menuName = "Pixels/UV Repeating";
			returnType = ValueType.Color;
			propertyNames = new string[] { "Pixels", "U", "V" };
			propertyTypes = new ValueType[] {
				ValueType.Pixels, ValueType.Float, ValueType.Float
			};
		}
		
		public override void Compute (Value output, Value[] arguments) {
			DiagramPixels pixels = arguments[0].Pixels;
			float
				u = arguments[1].Float,
				v = arguments[2].Float;
			u = (u < 0f ? u + 1f - (int)u : u) % 1f;
			v = (v < 0f ? v + 1f - (int)v : v) % 1f;
			int x0, x1, y0, y1;
			Color c0, c1, c2;
			float tu, tv;
			if (pixels.colors == null || pixels.width <= 0 || pixels.height <= 0) {
				if (pixels.texture == null) {
					output.Color = Color.clear;
					return;
				}
				Texture2D texture = pixels.texture;
				u = u * texture.width + 0.5f;
				v = v * texture.height + 0.5f;
				tu = u - (x1 = (int)u);
				tv = v - (y1 = (int)v);
				if (x1 == 0) {
					x0 = texture.width - 1;
				}
				else {
					x0 = x1 - 1;
					if (x1 == texture.width) {
						x1 = 0;
					}
				}
				if (y1 == 0) {
					y0 = texture.height - 1;
				}
				else {
					y0 = y1 - 1;
					if (y1 == texture.height) {
						y1 = 0;
					}
				}
				c0 = texture.GetPixel(x0, y0);
				c1 = texture.GetPixel(x1, y0);
				
				c0.r += (c1.r - c0.r) * tu;
				c0.g += (c1.g - c0.g) * tu;
				c0.b += (c1.b - c0.b) * tu;
				c0.a += (c1.a - c0.a) * tu;
				
				c1 = texture.GetPixel(x0, y1);
				c2 = texture.GetPixel(x1, y1);
			}
			else {
				u = u * pixels.width + 0.5f;
				v = v * pixels.height + 0.5f;
				tu = u - (x1 = (int)u);
				tv = v - (y1 = (int)v);
				if (x1 == 0) {
					x0 = pixels.width - 1;
				}
				else {
					x0 = x1 - 1;
					if (x1 == pixels.width) {
						x1 = 0;
					}
				}
				if (y1 == 0) {
					y0 = (pixels.height - 1) * pixels.width;
				}
				else {
					y0 = (y1 - 1) * pixels.width;
					if (y1 == pixels.height) {
						y1 = 0;
					}
					else {
						y1 = y0 + pixels.width;
					}
				}
				c0 = pixels.colors[x0 + y0];
				c1 = pixels.colors[x1 + y0];
				
				c0.r += (c1.r - c0.r) * tu;
				c0.g += (c1.g - c0.g) * tu;
				c0.b += (c1.b - c0.b) * tu;
				c0.a += (c1.a - c0.a) * tu;
				
				c1 = pixels.colors[x0 + y1];
				c2 = pixels.colors[x1 + y1];
			}
			
			c1.r += (c2.r - c1.r) * tu;
			c1.g += (c2.g - c1.g) * tu;
			c1.b += (c2.b - c1.b) * tu;
			c1.a += (c2.a - c1.a) * tu;
			
			c0.r += (c1.r - c0.r) * tv;
			c0.g += (c1.g - c0.g) * tv;
			c0.b += (c1.b - c0.b) * tv;
			c0.a += (c1.a - c0.a) * tv;
			
			output.Color = c0;
		}
	}
}