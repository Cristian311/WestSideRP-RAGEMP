public class Animation
{
    public string Name { get; set; }
    public string AnimDict { get; set; }
    public string AnimName { get; set; }
    public int Flag { get; set; } = 2; // Default to 0

    public Animation(string name, string animDict, string animName, int flag = 2)
    {
        Name = name;
        AnimDict = animDict;
        AnimName = animName;
        Flag = flag;
    }
}