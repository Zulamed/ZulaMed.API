using System.Text;

namespace ZulaMed.VideoConversion;

public static class StringExtensions
{
    public static void RemoveLastCharacter(this StringBuilder stringBuilder)
    {
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        
    } 
}