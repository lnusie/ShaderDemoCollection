// Copyright 2013, Catlike Coding, http://catlikecoding.com
using UnityEngine;

namespace CatlikeCoding.NumberFlow.Editor {
	
	public enum DiagramWindowEventType {
		None,
		Used,
		TouchBegin,
		TouchMove,
		TouchEnd,
		ContextClick,
		Layout,
		Repaint
	}
	
	public class DiagramWindowEvent {
		
		public DiagramWindowEventType type;
		public Vector2 touchPosition, scrolledTouchPosition;
		public DiagramWindowNode targetWindowNode, targetOutputNode;
		
		public void Reset (DiagramWindowNode[] windowNodes, Vector2 scrollPosition, int selectedNodeIndex) {
			type = DiagramWindowEventType.None;
			if (selectedNodeIndex < 0 || selectedNodeIndex >= windowNodes.Length ||
				!windowNodes[selectedNodeIndex].SelectedWindowRect.Contains(Event.current.mousePosition)) {
				if (Event.current.button == 0) {
					switch (Event.current.type) {
					case EventType.MouseDown:
						type = DiagramWindowEventType.TouchBegin;
						break;
					case EventType.MouseUp:
						type = DiagramWindowEventType.TouchEnd;
						break;
					case EventType.MouseDrag:
						type = DiagramWindowEventType.TouchMove;
						break;
					}
				}
				if (Event.current.type == EventType.ContextClick) {
					type = DiagramWindowEventType.ContextClick;	
				}
			}
			if (type == DiagramWindowEventType.None) {
				switch (Event.current.type) {
				case EventType.Layout:
					type = DiagramWindowEventType.Layout;
					break;
				case EventType.Repaint:
					type = DiagramWindowEventType.Repaint;
					break;
				}
				return;
			}
			
			touchPosition = Event.current.mousePosition;
			scrolledTouchPosition = touchPosition + scrollPosition;
			
			if (type == DiagramWindowEventType.TouchMove) {
				// Keep targeting the same node while a drag is in progress.
				return;
			}
			
			targetWindowNode = null;
			targetOutputNode = null;
			for (int i = windowNodes.Length - 1; i >= 0; i--) {
				if (windowNodes[i].Contains(scrolledTouchPosition)) {
					targetWindowNode = windowNodes[i];
					break;
				}
				if (windowNodes[i].OutputConnectionRect.Contains(scrolledTouchPosition)) {
					targetOutputNode = windowNodes[i];
					break;
				}
			}
		}
		
		public void Use () {
			type = DiagramWindowEventType.Used;
			// Use Unity event as well.
			Event.current.Use();
		}
		
		public bool IsTouchBeginInsideRect (Rect r) {
			return type == DiagramWindowEventType.TouchBegin && r.Contains(scrolledTouchPosition);
		}
	}
}