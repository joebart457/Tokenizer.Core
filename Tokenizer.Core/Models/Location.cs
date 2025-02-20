
namespace Tokenizer.Core.Models;

public class Location
{
    public int Line { get; set; }
    public int Column { get; set; }
    public Location(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override int GetHashCode()
    {
        return Line.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Location location)
        {
            return Line == location.Line && Column == location.Column;
        }
        return false;
    }

    public override string ToString()
    {
        return $"Ln. {Line}, Col. {Column}";
    }

    public static Location Zero => new Location(0, 0);
}
