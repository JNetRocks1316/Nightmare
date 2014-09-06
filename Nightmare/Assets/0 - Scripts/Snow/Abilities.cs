public class Ability {

	//Is this skill learned by the player?
	private bool _known;

	public Ability(){
		//By default skills are not learned
		_known = false;
	}

	public bool Known{
		get{ return _known; }
		set{ _known = value; }
	}

	public enum Abilities{
		Attack,
		Dodge,
		Climb
	}
}
