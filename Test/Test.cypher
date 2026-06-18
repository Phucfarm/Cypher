public enum PlayerState
{
	Idle,
	Running
}
public struct Vector2
{
	public float x;
	public float y;
}
public class Player
{
	private string name;
	public Vector2 position;
	public bool isActive;
}
public static void fn Main()
{
	Player p = new Player();
	p.position.x = 10.5;
	p.isActive = true;
	int count = 0;
	for (int i = 0; i < 5; i++)
	{
		if (i == 2)
		{
			next;
		}
		if (i == 4)
		{
			break;
		}
		count += 1;
	}
	count -= 1;
	count *= 2;
	count /= 1;
	count %= 2;
	if (count == 0 && !p.isActive || count != 1)
	{
		count++;
	}
	else
	{
		count--;
	}
}