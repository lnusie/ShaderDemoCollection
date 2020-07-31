// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow {
	
	/// <summary>
	/// Node of a NumberFlow diagram.
	/// 
	/// Should only be used by diagrams and their editor scripts. Do not touch it.
	/// </summary>
	[Serializable]
	public sealed class DiagramNode {

		/// <summary>
		/// Canvas position.
		/// </summary>
		public Vector2 position;

		/// <summary>
		/// Function identifier.
		/// </summary>
		public string functionId;

		/// <summary>
		/// Argument node indices.
		/// </summary>
		public int[] argumentIndices;

		/// <summary>
		/// Fixed argument values.
		/// </summary>
		public Value[] fixedValues;

		[NonSerialized]
		private Function function;

		[NonSerialized]
		private string[] propertyNames;

		[NonSerialized]
		internal Value computedValue;
		
		[NonSerialized]
		internal Value[] argumentValues;
		
		[NonSerialized]
		internal DiagramNode[] argumentNodes;
		
		[NonSerialized]
		private Diagram diagram;
		
		[NonSerialized]
		private int lastPixelIndex = -1;

		/// <summary>
		/// Get the function.
		/// </summary>
		/// <value>Function.</value>
		public Function Function {
			get {
				return function;
			}
		}

		/// <summary>
		/// Get the input name, if applicable.
		/// </summary>
		/// <value>Name of the input, or null.</value>
		public string InputName {
			get {
				return function == null || function.type != FunctionType.Input ? null : fixedValues[1].String;
			}
		}

		/// <summary>
		/// Get the diagram that this node belongs to.
		/// </summary>
		/// <value>The diagram.</value>
		public Diagram Diagram {
			get {
				return diagram;
			}
		}

		/// <summary>
		/// Get the name.
		/// </summary>
		/// <value>Name.</value>
		public string Name {
			get {
				return function == null ? "???" : function.name;
			}
		}

		/// <summary>
		/// Get the type name.
		/// </summary>
		/// <value>Name of the type.</value>
		public string TypeName {
			get {
				return function == null ? "???" : function.returnType.ToString();
			}
		}

		/// <summary>
		/// Get the property count.
		/// </summary>
		/// <value>Property count.</value>
		public int PropertyCount {
			get {
				return argumentIndices.Length;
			}
		}

		/// <summary>
		/// Get the name of a property.
		/// </summary>
		/// <returns>Property name.</returns>
		/// <param name="index">Property index.</param>
		public string GetPropertyName (int index) {
			return function == null ? "???" : function.propertyNames[index];
		}

		/// <summary>
		/// Initialize the node. Will be invoked by its diagram.
		/// </summary>
		/// <param name="diagram">Diagram the node is a part of.</param>
		public void Init (Diagram diagram) {
			this.diagram = diagram;
			function = DefaultFunctionLibrary.Instance[functionId];
			if (function == null) {
				for (int i = 0; i < diagram.functionLibraries.Length; i++) {
					if (diagram.functionLibraries[i] != null) {
						function = diagram.functionLibraries[i][functionId];
						if (function != null) {
							break;
						}
					}
				}
			}
			if (function == null) {
				if (functionId == "CatlikeCoding.NumberFlow.Functions.Coordinates.CubeMapCoordinates") {
					functionId = "CatlikeCoding.NumberFlow.Functions.Cubemap.Direction";
					function = DefaultFunctionLibrary.Instance[functionId];
				}
				else {
					Debug.LogError("Unknown NumberFlow diagram function \"" + functionId + "\" in diagram " + diagram.name, diagram);
				}
			}
			else {
				if(argumentIndices == null || argumentIndices.Length != function.propertyNames.Length) {
					argumentIndices = new int[function.propertyNames.Length];
					for (int i = 0; i < argumentIndices.Length; i++) {
						argumentIndices[i] = -1;
					}
				}
				if(fixedValues == null || fixedValues.Length != function.propertyNames.Length) {
					fixedValues = new Value[function.propertyNames.Length];
					for (int i = 0; i < fixedValues.Length; i++) {
						fixedValues[i] = new Value();
					}
				}
			}
			computedValue = new Value();
		}

		/// <summary>
		/// Prepare the node for usage. Will be called by its diagram.
		/// </summary>
		/// <param name="diagram">Diagram the node is part of.</param>
		public void Prepare (Diagram diagram) {
			int argumentNodeCount = 0;
			argumentValues = new Value[argumentIndices.Length];
			for (int i = 0; i < argumentIndices.Length; i++) {
				if (argumentIndices[i] >= 0) {
					argumentValues[i] = diagram.nodes[argumentIndices[i]].computedValue;
					argumentNodeCount += 1;
				}
				else {
					argumentValues[i] = fixedValues[i];
				}
			}
			if (function != null) {
				if (function.type == FunctionType.Input) {
					// Clone value object to allow runtime changes without persisting them.
					argumentValues[0] = new Value(argumentValues[0]);
				}
			}
			
			argumentNodes = new DiagramNode[argumentNodeCount];
			for (int i = 0, iA = 0; i < argumentIndices.Length; i++) {
				if (argumentIndices[i] >= 0) {
					argumentNodes[iA++] = diagram.nodes[argumentIndices[i]];
				}
			}
		}

		/// <summary>
		/// Compute the current value of the node.
		/// </summary>
		public void Compute () {
			if (lastPixelIndex != diagram.currentPixelIndex && function != null) {
				lastPixelIndex = diagram.currentPixelIndex;
				if (function.type == FunctionType.Custom) {
					function.ComputeCustom(this);
					return;
				}
				for (int i = 0; i < argumentNodes.Length; i++) {
					if (argumentNodes[i] != null) {
						argumentNodes[i].Compute();
					}
				}
				if (function.type != FunctionType.Coordinate) {
					function.Compute(computedValue, argumentValues);
				}
				else {
					function.ComputeCoordinate(computedValue, diagram);
				}
			}
		}

		/// <summary>
		/// Compute the current value of the node and directly return the color value.
		/// </summary>
		/// <returns>Computed color.</returns>
		public Color ComputeColor () {
			if (lastPixelIndex != diagram.currentPixelIndex && function != null) {
				lastPixelIndex = diagram.currentPixelIndex;
				if (function.type == FunctionType.Custom) {
					function.ComputeCustom(this);
					return computedValue.Color;
				}
				for (int i = 0; i < argumentNodes.Length; i++) {
					if (argumentNodes[i] != null) {
						argumentNodes[i].Compute();
					}
				}
				if (function.type != FunctionType.Coordinate) {
					function.Compute(computedValue, argumentValues);
				}
				else {
					function.ComputeCoordinate(computedValue, diagram);
				}
			}
			return computedValue.Color;
		}

		/// <summary>
		/// Determine whether this node is dependent on another node. Used to prevent loops while editing a diagram.
		/// </summary>
		/// <returns>Whether the dependency exists.</returns>
		/// <param name="otherNode">Another node.</param>
		public bool IsDependentOn (DiagramNode otherNode) {
			if (this == otherNode) {
				return true;
			}
			foreach (DiagramNode node in argumentNodes) {
				if (node.IsDependentOn(otherNode)) {
					return true;
				}
			}
			return false;
		}
		
		internal void ComputeArgumentNodes () {
			for (int i = 0; i < argumentNodes.Length; i++) {
				if (argumentNodes[i] != null) {
					argumentNodes[i].Compute();
				}
			}
		}
	}
}