/// <summary>
/// Attribute.cs
/// Jeanette Mathews
/// 7/9/2014
/// 
/// This is the base class for attributes - used to modify
/// Snow's statistics.  Each attribute will modify different
/// aspects of Snow's combat attributes.
/// </summary>

public class Attribute {
	private int _baseValue;
	private int _buffValue;

	public Attribute(){
		_baseValue = 0;  //The unbuffed value of the attribute
		_buffValue = 0;  //The buffed value of the attribute
	}

	//The different attributes available.
	public enum AttributeName{
		Might, Precision, Agility, Resilience
	}

	#region Setters and Getters
	public int BaseValue{
		get{ return _baseValue; }
		set{ _baseValue = value; }
	}

	//Basic Setters and Getters
	public int BuffValue{
		get{ return _buffValue; }
		set{ _buffValue = value; }
	}
	#endregion

	//Recalculate the adjusted value of the attribute
	//with buffs and return it.
	public int AdjustedValue(){
		return _baseValue + _buffValue;
	}


}
