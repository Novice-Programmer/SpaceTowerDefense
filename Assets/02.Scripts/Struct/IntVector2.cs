using System;

[Serializable]
public struct IntVector2
{

	public int x;
	public int y;

	public static readonly IntVector2 zero = new IntVector2(0, 0);

	public IntVector2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static IntVector2 operator +(IntVector2 left, IntVector2 right)
	{
		return new IntVector2(left.x + right.x, left.y + right.y);
	}

	public static IntVector2 operator -(IntVector2 left, IntVector2 right)
	{
		return new IntVector2(left.x - right.x, left.y - right.y);
	}
}
