// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {
	
	public sealed class NodeDragTransaction : DiagramWindowTransaction {
		
		private Vector2 startPosition, nodeStartPosition;
		
		private DiagramWindowNode nodeToDrag;
		
		public NodeDragTransaction (DiagramWindowNode nodeToDrag, Vector2 dragStartPosition) {
			this.nodeToDrag = nodeToDrag;
			nodeStartPosition = nodeToDrag.Position;
			startPosition = dragStartPosition;
		}
		
		public bool OnGUI (DiagramWindowEvent e) {
			if (e.type == DiagramWindowEventType.TouchMove) {
				nodeToDrag.Position = nodeStartPosition + e.touchPosition - startPosition;
				e.Use();
				return true;
			}
			if (e.type == DiagramWindowEventType.Layout || e.type == DiagramWindowEventType.Repaint) {
				return true;
			}
			// Any event other than Move, Layout, or Repaint terminates the drag.
			// The TouchEnd event isn't reliable enough to depend on, as it doesn't happen when releasing a node outside of the canvas.
			e.Use();
			return false;
		}
	}
}