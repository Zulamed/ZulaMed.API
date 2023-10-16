namespace ZulaMed.API.Extensions;

public static class StringExtensions
{
    // modified version of https://stackoverflow.com/a/27073919 using spans
    public static string Capitalize(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("There is no first letter");

        Span<char> a = stackalloc char[s.Length];
        s.CopyTo(a);
        a[0] = char.ToUpper(a[0]);
        return a.ToString();
    }
}