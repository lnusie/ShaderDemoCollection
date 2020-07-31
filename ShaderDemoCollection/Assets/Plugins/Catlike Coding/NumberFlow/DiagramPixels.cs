// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;
using System;

namespace CatlikeCoding.NumberFlow {
	
	/// <summary>
	/// Container for NumberFlow pixel input data.
	/// 
	/// Can be used to reference a Texture2D, or a Color array at runtime.
	/// 
	/// Colors will be used when the array is not null and both width and height are larger than zero.
	/// Otherwise it will fall back to the texture, if any.
	/// </summary>
	[Serializable]
	public class DiagramPixels {
		
		/// <summary>
		/// Color array to use instead of the texture.
		/// 
		/// Its length should be width * height.
		/// </summary>
		[NonSerialized]
		public Color[] colors;
		
		/// <summary>
		/// Width, used for the Color array.
		/// </summary>
		[NonSerialized]
		public int width;
		
		/// <summary>
		/// Height, used for the Color array.
		/// </summary>
		[NonSerialized]
		public int height;
		
		/// <summary>
		/// Texture2D that will be used, unless colors are provided.
		/// </summary>
		public Texture2D texture;
		
		/// <summary>
		/// Initialize a new instance. There is no reason to do this yourself.
		/// </summary>
		public DiagramPixels () {}
		
		/// <summary>
		/// Initializes a new instance as a copy of another. There is no reason to do this yourself.
		/// </summary>
		/// <param name='pixels'>
		/// DiagramPixels instance to copy.
		/// </param>
		public DiagramPixels (DiagramPixels pixels) {
			this.texture = pixels.texture;
		}
		
		/// <summary>
		/// Initialize a Color array, representing a pixel grid.
		/// 
		/// The Color array length should be width * height.
		/// </summary>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		/// <param name='colors'>
		/// Colors.
		/// </param>
		public void Init (int width, int height, Color[] colors) {
			this.width = width;
			this.height = height;
			this.colors = colors;
		}
		
		/// <summary>
		/// Initialize a Color array, representing a square pixel grid.
		/// 
		/// Sets both width and height to size.
		/// The Color array length should be size squared.
		/// </summary>
		/// <param name='size'>
		/// Size, to use for both width and height.
		/// </param>
		/// <param name='colors'>
		/// Colors.
		/// </param>
		public void Init (int size, Color[] colors) {
			width = height = size;
			this.colors = colors;
		}
	}
}