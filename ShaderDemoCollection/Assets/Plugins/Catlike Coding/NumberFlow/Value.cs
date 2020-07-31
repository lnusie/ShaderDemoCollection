// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow {

	/// <summary>
	/// The possible types for a NumberFlow Value.
	/// </summary>
	public enum ValueType {
		None,
		Bool,
		Color,
		Float,
		Int,
		String,
		Vector2,
		Vector3,
		AnimationCurve,
		Gradient,
		Pixels
	}

	/// <summary>
	/// Value object for NumberFlow diagrams.
	/// Contains all possible value types, but only one is used per instance.
	/// </summary>
	[Serializable]
	public sealed class Value {

		/// <summary>
		/// Boolean value.
		/// </summary>
		public bool Bool;

		/// <summary>
		/// Color value.
		/// </summary>
		public Color Color;

		/// <summary>
		/// Float value.
		/// </summary>
		public float Float;

		/// <summary>
		/// Int value.
		/// </summary>
		public int Int;

		/// <summary>
		/// String value.
		/// </summary>
		public string String;

		/// <summary>
		/// Vector2 value.
		/// </summary>
		public Vector2 Vector2;

		/// <summary>
		/// Vector3 value.
		/// </summary>
		public Vector3 Vector3;

		/// <summary>
		/// Curve value.
		/// </summary>
		public AnimationCurve AnimationCurve = null;

		/// <summary>
		/// Gradient value.
		/// </summary>
		public Gradient Gradient = null;

		/// <summary>
		/// Pixels value.
		/// </summary>
		public DiagramPixels Pixels = null;

		/// <summary>
		/// Initialize a new instance of the <see cref="CatlikeCoding.NumberFlow.Value"/> class.
		/// </summary>
		public Value () {}

		/// <summary>
		/// Initialize a new instance of the <see cref="CatlikeCoding.NumberFlow.Value"/> class.
		/// </summary>
		/// <param name="value">Value to create a deep copy of.</param>
		public Value (Value value) {
			this.Bool = value.Bool;
			this.Color = value.Color;
			this.Float = value.Float;
			this.Int = value.Int;
			this.String = value.String;
			this.Vector2 = value.Vector2;
			this.Vector3 = value.Vector3;
			this.AnimationCurve = value.AnimationCurve;
			if (value.AnimationCurve == null) {
				this.AnimationCurve = new AnimationCurve();
			}
			else {
				this.AnimationCurve = new AnimationCurve(value.AnimationCurve.keys);
				this.AnimationCurve.preWrapMode = value.AnimationCurve.preWrapMode;
				this.AnimationCurve.postWrapMode = value.AnimationCurve.postWrapMode;
			}
			this.Gradient = new Gradient();
			if (value.Gradient != null) {
				this.Gradient.SetKeys(value.Gradient.colorKeys, value.Gradient.alphaKeys);
			}
			this.Pixels = new DiagramPixels(value.Pixels);
		}
	}
}