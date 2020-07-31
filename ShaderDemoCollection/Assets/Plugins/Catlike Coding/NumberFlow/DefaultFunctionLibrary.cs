// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow {
	
	/// <summary>
	/// Default function library that is available to all NumberFlow diagrams.
	/// </summary>
	public sealed class DefaultFunctionLibrary : FunctionLibrary {
		
		private static DefaultFunctionLibrary instance;

		/// <summary>
		/// Get the default function library.
		/// </summary>
		/// <value>Library instance.</value>
		public static DefaultFunctionLibrary Instance {
			get {
				if (instance == null) {
					instance = ScriptableObject.CreateInstance<DefaultFunctionLibrary>();
					instance.hideFlags = HideFlags.HideAndDontSave;
				}
				return instance;
			}
		}
		
		protected override void RegisterFunctions () {

			#region Floats

			Register(new Functions.Floats.Add());
			Register(new Functions.Floats.AddAdd());
			Register(new Functions.Floats.Subtract());
			Register(new Functions.Floats.Multiply());
			Register(new Functions.Floats.MultiplyAdd());
			Register(new Functions.Floats.Divide());
			Register(new Functions.Floats.Modulo());

			Register(new Functions.Floats.PlusOne());
			Register(new Functions.Floats.MinusOne());
			Register(new Functions.Floats.OneMinus());
			Register(new Functions.Floats.DivideOne());

			Register(new Functions.Floats.Highest());
			Register(new Functions.Floats.Lowest());
			Register(new Functions.Floats.Average());

			Register(new Functions.Floats.Square());
			Register(new Functions.Floats.Cube());
			Register(new Functions.Floats.SquareRoot());
			
			Register(new Functions.Floats.Absolute());
			
			Register(new Functions.Floats.Curve());

			Register(new Functions.Floats.Lerp());
			Register(new Functions.Floats.FromInt());

			Register(new Functions.Floats.Clamp01());
			Register(new Functions.Floats.Loop01());
			Register(new Functions.Floats.PingPong01());
			
			Register(new Functions.Floats.Cosine());
			Register(new Functions.Floats.Sine());
			Register(new Functions.Floats.Sinusoid());

			#endregion

			#region Ints

			Register(new Functions.Ints.Add());
			Register(new Functions.Ints.AddAdd());
			Register(new Functions.Ints.Subtract());
			Register(new Functions.Ints.Multiply());
			Register(new Functions.Ints.Divide());
			Register(new Functions.Ints.Modulo());
			Register(new Functions.Ints.Square());
			Register(new Functions.Ints.Cube());
			
			Register(new Functions.Ints.Absolute());
			Register(new Functions.Ints.FromFloat());

			#endregion

			#region Colors
			
			Register(new Functions.Colors.Gradient());
			Register(new Functions.Colors.FromFloats());
			Register(new Functions.Colors.FromVector3());
			Register(new Functions.Colors.Lerp());
			Register(new Functions.Colors.Add());
			Register(new Functions.Colors.AddAdd());
			Register(new Functions.Colors.Scale());
			Register(new Functions.Colors.Clamp());
			
			Register(new Functions.Colors.BlendNormal());
			
			Register(new Functions.Colors.BlendDarken());
			Register(new Functions.Colors.BlendMultiply());
			Register(new Functions.Colors.BlendColorBurn());
			Register(new Functions.Colors.BlendLinearBurn());
			
			Register(new Functions.Colors.BlendLighten());
			Register(new Functions.Colors.BlendScreen());
			Register(new Functions.Colors.BlendColorDodge());
			Register(new Functions.Colors.BlendLinearDodge());
			
			Register(new Functions.Colors.BlendOverlay());
			Register(new Functions.Colors.BlendSoftLight());
			Register(new Functions.Colors.BlendHardLight());
			
			Register(new Functions.Colors.GetRed());
			Register(new Functions.Colors.SetRed());
			Register(new Functions.Colors.GetGreen());
			Register(new Functions.Colors.SetGreen());
			Register(new Functions.Colors.GetBlue());
			Register(new Functions.Colors.SetBlue());
			Register(new Functions.Colors.GetAlpha());
			Register(new Functions.Colors.SetAlpha());
			Register(new Functions.Colors.GetChannel());
			Register(new Functions.Colors.SetChannel());

			#endregion

			#region Pixels

			Register(new Functions.Pixels.GetXYClamped());
			Register(new Functions.Pixels.GetXYRepeating());
			Register(new Functions.Pixels.GetUVClamped());
			Register(new Functions.Pixels.GetUVRepeating());

			#endregion

			#region Vector3s

			Register(new Functions.Vector3s.Add());
			Register(new Functions.Vector3s.AddFloat());
			Register(new Functions.Vector3s.Subtract());
			Register(new Functions.Vector3s.SubtractFloat());
			Register(new Functions.Vector3s.Cross());
			Register(new Functions.Vector3s.Dot());
			Register(new Functions.Vector3s.Scale());
			Register(new Functions.Vector3s.FromFloats());
			Register(new Functions.Vector3s.SphereVector());
			
			Register(new Functions.Vector3s.Length());
			Register(new Functions.Vector3s.SquareLength());
			Register(new Functions.Vector3s.Negate());
			Register(new Functions.Vector3s.Normalize());

			Register(new Functions.Vector3s.GetX());
			Register(new Functions.Vector3s.SetX());
			Register(new Functions.Vector3s.GetY());
			Register(new Functions.Vector3s.SetY());
			Register(new Functions.Vector3s.GetZ());
			Register(new Functions.Vector3s.SetZ());

			Register(new Functions.Vector3s.XPlusY());
			Register(new Functions.Vector3s.XTimesY());
			Register(new Functions.Vector3s.XMinusY());
			Register(new Functions.Vector3s.YMinusX());
			
			Register(new Functions.Vector3s.PackNormalRGB());
			Register(new Functions.Vector3s.PackNormalDXT5());

			#endregion

			#region Vector2

			Register(new Functions.Vector2s.Add());
			Register(new Functions.Vector2s.AddFloat());
			Register(new Functions.Vector2s.Subtract());
			Register(new Functions.Vector2s.SubtractFloat());
			Register(new Functions.Vector2s.Dot());
			Register(new Functions.Vector2s.Scale());
			Register(new Functions.Vector2s.FromFloats());

			Register(new Functions.Vector2s.Length());
			Register(new Functions.Vector2s.SquareLength());
			Register(new Functions.Vector2s.Negate());
			Register(new Functions.Vector2s.Normalize());
			
			Register(new Functions.Vector2s.GetX());
			Register(new Functions.Vector2s.SetX());
			Register(new Functions.Vector2s.GetY());
			Register(new Functions.Vector2s.SetY());

			Register(new Functions.Vector2s.XPlusY());
			Register(new Functions.Vector2s.XTimesY());
			Register(new Functions.Vector2s.XMinusY());
			Register(new Functions.Vector2s.YMinusX());

			#endregion

			#region Booleans

			Register(new Functions.Booleans.ChooseBool());
			Register(new Functions.Booleans.ChooseColor());
			Register(new Functions.Booleans.ChooseFloat());
			Register(new Functions.Booleans.ChooseInt());
			Register(new Functions.Booleans.ChooseVector3());
			
			Register(new Functions.Booleans.And());
			Register(new Functions.Booleans.Or());
			Register(new Functions.Booleans.Not());
			Register(new Functions.Booleans.BoolEqual());
			Register(new Functions.Booleans.BoolNotEqual());
			
			Register(new Functions.Booleans.FloatLess());
			Register(new Functions.Booleans.FloatLessOrEqual());
			Register(new Functions.Booleans.FloatEqual());
			Register(new Functions.Booleans.FloatGreaterOrEqual());
			Register(new Functions.Booleans.FloatGreater());
			Register(new Functions.Booleans.FloatBetweenInclusive());
			Register(new Functions.Booleans.IntLess());
			Register(new Functions.Booleans.IntLessOrEqual());
			Register(new Functions.Booleans.IntEqual());
			Register(new Functions.Booleans.IntGreaterOrEqual());
			Register(new Functions.Booleans.IntGreater());
			Register(new Functions.Booleans.IntBetweenInclusive());

			#endregion

			#region Noise

			Register(new Functions.Floats.PerlinNoise());
			Register(new Functions.Floats.PerlinNoiseTiledX());
			Register(new Functions.Floats.PerlinNoiseTiledXY());
			Register(new Functions.Floats.PerlinNoiseTiledXYZ());

			Register(new Functions.Floats.PerlinNoise2D());
			Register(new Functions.Floats.PerlinNoise2DTiledX());
			Register(new Functions.Floats.PerlinNoise2DTiledXY());

			Register(new Functions.Floats.PerlinTurbulence());
			Register(new Functions.Floats.PerlinTurbulenceTiledX());
			Register(new Functions.Floats.PerlinTurbulenceTiledXY());
			Register(new Functions.Floats.PerlinTurbulenceTiledXYZ());

			Register(new Functions.Floats.PerlinTurbulence2D());
			Register(new Functions.Floats.PerlinTurbulence2DTiledX());
			Register(new Functions.Floats.PerlinTurbulence2DTiledXY());

			Register(new Functions.Floats.ValueNoise());
			Register(new Functions.Floats.ValueNoiseTiledX());
			Register(new Functions.Floats.ValueNoiseTiledXY());
			Register(new Functions.Floats.ValueNoiseTiledXYZ());

			Register(new Functions.Floats.ValueNoise2D());
			Register(new Functions.Floats.ValueNoise2DTiledX());
			Register(new Functions.Floats.ValueNoise2DTiledXY());

			Register(new Functions.Vector3s.VoronoiLinear());
			Register(new Functions.Vector3s.VoronoiLinearTiledX());
			Register(new Functions.Vector3s.VoronoiLinearTiledXY());
			Register(new Functions.Vector3s.VoronoiLinearTiledXYZ());

			Register(new Functions.Vector3s.VoronoiLinear2D());
			Register(new Functions.Vector3s.VoronoiLinear2DTiledX());
			Register(new Functions.Vector3s.VoronoiLinear2DTiledXY());
			
			Register(new Functions.Vector3s.VoronoiSquared());
			Register(new Functions.Vector3s.VoronoiSquaredTiledX());
			Register(new Functions.Vector3s.VoronoiSquaredTiledXY());
			Register(new Functions.Vector3s.VoronoiSquaredTiledXYZ());

			Register(new Functions.Vector3s.VoronoiSquared2D());
			Register(new Functions.Vector3s.VoronoiSquared2DTiledX());
			Register(new Functions.Vector3s.VoronoiSquared2DTiledXY());

			Register(new Functions.Vector3s.VoronoiManhattan());
			Register(new Functions.Vector3s.VoronoiManhattanTiledX());
			Register(new Functions.Vector3s.VoronoiManhattanTiledXY());
			Register(new Functions.Vector3s.VoronoiManhattanTiledXYZ());

			Register(new Functions.Vector3s.VoronoiManhattan2D());
			Register(new Functions.Vector3s.VoronoiManhattan2DTiledX());
			Register(new Functions.Vector3s.VoronoiManhattan2DTiledXY());
			
			Register(new Functions.Vector3s.VoronoiChebyshev());
			Register(new Functions.Vector3s.VoronoiChebyshevTiledX());
			Register(new Functions.Vector3s.VoronoiChebyshevTiledXY());
			Register(new Functions.Vector3s.VoronoiChebyshevTiledXYZ());

			Register(new Functions.Vector3s.VoronoiChebyshev2D());
			Register(new Functions.Vector3s.VoronoiChebyshev2DTiledX());
			Register(new Functions.Vector3s.VoronoiChebyshev2DTiledXY());

			#endregion
			
			#region Coordinates
			
			Register(new Functions.Coordinates.X());
			Register(new Functions.Coordinates.Y());
			Register(new Functions.Coordinates.U());
			Register(new Functions.Coordinates.V());
			Register(new Functions.Coordinates.OneMinusU());
			Register(new Functions.Coordinates.OneMinusV());
			Register(new Functions.Coordinates.UVCenterDistance());
			Register(new Functions.Coordinates.UVCenterDistanceSquared());
			
			Register(new Functions.Coordinates.XMinusOne());
			Register(new Functions.Coordinates.XPlusOne());
			Register(new Functions.Coordinates.YMinusOne());
			Register(new Functions.Coordinates.YPlusOne());

			#endregion

			#region Input
			
			Register(new Functions.Input.BoolInput());
			Register(new Functions.Input.ColorInput());
			Register(new Functions.Input.AnimationCurveInput());
			Register(new Functions.Input.GradientInput());
			Register(new Functions.Input.FloatInput());
			Register(new Functions.Input.IntInput());
			Register(new Functions.Input.PixelsInput());
			Register(new Functions.Input.Vector3Input());
			
			#endregion

			#region Output

			Register(new Functions.Outputs.ColorOutput());
			Register(new Functions.Outputs.ValueOutput());

			#endregion
			
			#region Cubemap
			
			Register(new Functions.Cubemap.Direction());
			Register(new Functions.Cubemap.NormalCross());
			
			#endregion
		}
	}
}