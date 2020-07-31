// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEditor;
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {
	
	public sealed class ConnectionDragTransaction : DiagramWindowTransaction {
		
		private Vector2 startPosition, startTangent;
		
		private int argumentIndex;
		
		private DiagramWindowNode nodeToConnect;
		
		public ConnectionDragTransaction (DiagramWindowNode nodeToConnect, int argumentIndex, Vector2 startPosition) {
			this.nodeToConnect = nodeToConnect;
			this.argumentIndex = argumentIndex;
			this.startPosition = startPosition;
			startTangent = startPosition;
			startTangent.x -= 60f;
		}
		
		public bool OnGUI (DiagramWindowEvent e) {
			if (e.type == DiagramWindowEventType.TouchMove) {
				e.Use();
				return true;
			}
			if (e.type == DiagramWindowEventType.TouchEnd) {
				nodeToConnect.SetArgument(argumentIndex, e.targetWindowNode ?? e.targetOutputNode);
				e.Use();
				return false;
			}
			if (e.type == DiagramWindowEventType.Repaint) {
				Handles.DrawBezier(startPosition, e.touchPosition, startTangent, e.touchPosition, Color.grey, null, 2f);
			}
			return true;
		}
	}
}