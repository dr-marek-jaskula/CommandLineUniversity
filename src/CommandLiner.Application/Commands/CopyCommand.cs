using Cocona;

namespace CommandLiner.Application.Commands;

public sealed class CopyCommand
{
    [Command("hello")]
    public void Hello()
    {
        Console.WriteLine("Hello!");
    }

    public enum Member
    {
        Alice,
        Karen,
    }

    [Command("konnichiwa")]
    public void Konnichiwa(Member member)
    {
        Console.WriteLine($"Konnichiwa! {member}");
    }
}