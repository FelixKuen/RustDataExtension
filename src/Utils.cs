using System.Collections.Generic;

public static class Utils
{
    // Adjusts a string to be valid for enum names and handles reserved keywords.
    public static string MakeStringEnumReady(string input)
    {
        if (string.IsNullOrEmpty(input)) return "Invalid_Name";
        
        input = ReplaceReservedKeywords(input);
        input = AppendFirstNumbers(input);
        
        input = input.Replace(" ", "_")
            .Replace("-", "_")
            .Replace(".", "_")
            .Replace("(", string.Empty)
            .Replace(")", string.Empty);
        
        return input;
    }

    // Replaces reserved keywords with a unique suffix
    private static string ReplaceReservedKeywords(string input)
    {
        var reservedKeywords = new HashSet<string>
        {
            "throw", "lock", "event", "for", "while", "if", "else", "enum", "abstract"
        };

        if (reservedKeywords.Contains(input))
        {
            return input + "_Enum";
        }
        return input;
    }

    // Moves the first number in the string to the end if it starts with a number.
    private static string AppendFirstNumbers(string input)
    {
        if (char.IsDigit(input[0]))
        {
            int index = 0;
            while (index < input.Length && char.IsDigit(input[index]))
            {
                index++;
            }
            return input.Substring(index) + "_" + input.Substring(0, index);
        }
        return input;
    }
}