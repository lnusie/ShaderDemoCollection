// Copyright 2013, Catlike Coding, http://catlikecoding.com
using System.Collections.Generic;
using UnityEngine;

namespace CatlikeCoding.NumberFlow {
	
	/// <summary>
	/// Base class for NumberFlow function libraries.
	/// Extend to create your own library.
	/// </summary>
	public abstract class FunctionLibrary : ScriptableObject {
		
		private Dictionary<string, Function> registeredFunctions;
		
		/// <summary>
		/// Get the <see cref="CatlikeCoding.NumberFlow.Function"/> with the specified id.
		/// </summary>
		/// <param name='id'>
		/// Function identifier, being its fully qualified type/
		/// </param>
		public Function this[string id] {
			get {
				if (registeredFunctions == null) {
					registeredFunctions = new Dictionary<string, Function>();
					RegisterFunctions();
				}
				Function function = null;
				registeredFunctions.TryGetValue(id, out function);
				return function;
			}
		}
		
		/// <summary>
		/// Register a <see cref="CatlikeCoding.NumberFlow.Function"/> with this library.
		/// </summary>
		/// <param name='function'>
		/// <see cref="CatlikeCoding.NumberFlow.Function"/> to register.
		/// </param>
		protected void Register (Function function) {
			registeredFunctions.Add(function.GetType().FullName, function);
		}
		
		/// <summary>
		/// Lists all functions in this library.
		/// </summary>
		/// <returns>
		/// The function enumerator.
		/// </returns>
		public IEnumerable<Function> ListFunctions () {
			if (registeredFunctions == null) {
				registeredFunctions = new Dictionary<string, Function>();
				RegisterFunctions();
			}
			foreach (var entry in registeredFunctions){
				yield return entry.Value;
			}
		}
		
		/// <summary>
		/// Override this method to register all your functions with this library.
		/// </summary>
		protected abstract void RegisterFunctions ();
	}
	
}