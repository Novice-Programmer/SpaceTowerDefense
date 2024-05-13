using System.Collections.Generic;

public enum ETargetType
{
    Enemy,
    Player
}


public class CheckTarget
{
    public static bool TargetTagCheck(ETargetType targetType, string tag)
    {
        if (targetType == ETargetType.Player)
        {
            if (tag == "Tower")
                return true;
            else if (tag == "Obstacle")
                return true;
            else if (tag == "Commander")
                return true;
            else
                return false;
        }
        else
        {
            if (tag == "Enemy")
                return true;

            else
                return false;
        }
    }

    public static string[] GetTargetTags(ETargetType targetType)
    {
        List<string> tags = new List<string>();
        if (targetType == ETargetType.Player)
        {
            tags.Add("Tower");
            tags.Add("Obstacle");
            tags.Add("Commander");
        }
        else
        {
            tags.Add("Enemy");
        }
        return tags.ToArray();
    }
}
