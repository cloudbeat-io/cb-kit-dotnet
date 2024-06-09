namespace CloudBeat.Kit.Common.Attributes;

public class CbOwner
{
    public CbOwner(string name)
    {
        Name = name;
    }
    public string Name { get; internal set; }
}