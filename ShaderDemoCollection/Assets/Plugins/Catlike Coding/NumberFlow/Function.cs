// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow {

	/// <summary>
	/// The possible types of NumberFlow functions.
	/// </summary>
	public enum FunctionType {
		Coordinate,
		Function,
		Input,
		Output,
		Custom
	}

	/// <summary>
	/// Base class for NumberFlow functions.
	/// Extend to create your own custom functions.
	/// </summary>
	public abstract class Function {

		/// <summary>
		/// Menu name.
		/// </summary>
		public string menuName;

		/// <summary>
		/// Function name.
		/// </summary>
		public string name;

		/// <summary>
		/// Function type.
		/// </summary>
		public FunctionType type = FunctionType.Function;

		/// <summary>
		/// Type of the return value.
		/// </summary>
		public ValueType returnType = ValueType.Float;

		/// <summary>
		/// Property names.
		/// </summary>
		public string[] propertyNames;

		/// <summary>
		/// Property types.
		/// </summary>
		public ValueType[] propertyTypes;

		/// <summary>
		/// Initialize a new instance of the <see cref="CatlikeCoding.NumberFlow.Function"/> class.
		/// </summary>
		public Function () {
			Configure();
			if (name == null) {
				name = "???";
				Debug.LogError("Function has no name.");
			}
			if (menuName == null) {
				menuName = "???";
				Debug.LogError("Function \"" + name + "\" has no menuName.");
			}
			if (propertyNames == null) {
				propertyNames = new string[0];
			}
			if (propertyTypes == null) {
				propertyTypes = new ValueType[propertyNames.Length];
				for (int i = 0; i < propertyTypes.Length; i++) {
					propertyTypes[i] = returnType;
				}
			}
			if (propertyNames.Length != propertyTypes.Length) {
				Debug.LogError("Function \"" + name + "\" has propertyNames and propertyTypes of different length.");
			}
		}
		
		/// <summary>
		/// Override this method to configure your function.
		/// </summary>
		protected virtual void Configure () {}
		
		/// <summary>
		/// Override this method to perform your computation.
		/// </summary>
		/// <param name='output'>
		/// Value object used to store the output.
		/// </param>
		/// <param name='arguments'>
		/// Value objects used as arguments.
		/// </param>
		public virtual void Compute (Value output, Value[] arguments) {}
		
		internal virtual void ComputeCustom (DiagramNode node) {}
		
		/// <summary>
		/// Override this method to perform your coordinate-based computation.
		/// </summary>
		/// <param name='output'>
		/// Value object used to store the output.
		/// </param>
		/// <param name='diagram'>
		/// Diagram to retrieve coordinates from.
		/// </param>
		public virtual void ComputeCoordinate (Value output, Diagram diagram) {}
	}
}