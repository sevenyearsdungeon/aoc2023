using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class ParsingUtils
{
    public static string GetStringAfter(this string input, string split, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        return input.Split(split, stringSplitOptions)[1];
    }
    public static string GetStringAfter(this string input, char split, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        return input.Split(split, stringSplitOptions)[1];
    }

    public static string[] GetSequenceAfter(this string input, string split, string sequenceSplitter, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        return input.GetStringAfter(split, stringSplitOptions).Split(sequenceSplitter, stringSplitOptions);
    }

    public static string[] GetSequenceBefore(this string input, string split, string sequenceSplitter, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        return input.GetStringBefore(split, stringSplitOptions).Split(sequenceSplitter, stringSplitOptions);
    }

    public static string GetStringBefore(this string input, string split, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        return input.Split(split, stringSplitOptions)[0];
    }
}

