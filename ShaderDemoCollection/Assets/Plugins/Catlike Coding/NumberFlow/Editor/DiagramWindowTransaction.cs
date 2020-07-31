// Copyright 2013, Catlike Coding, http://catlikecoding.com
namespace CatlikeCoding.NumberFlow.Editor {
	
	public interface DiagramWindowTransaction {
		
		bool OnGUI (DiagramWindowEvent e);
	}
}