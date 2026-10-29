
namespace LibraryManagement.Helper;

public static class InputHelper
{
    public static string ReadRequiredInput(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine(
                "Input cannot be empty. Please try again.");
        }
    }

    public static int ReadInt(string message, int minimum = 0)
    {
        while (true)
        {
            string input = ReadRequiredInput(message);

            if (int.TryParse(input, out int value)
                && value >= minimum)
            {
                return value;
            }

            Console.WriteLine(
                $"Enter a valid integer greater than or equal to {minimum}.");
        }
    }

    public static int ReadOptionalInt(
        string message, int current)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return current;

            if (int.TryParse(input, out int value) && value >= 0)
                return value;

            Console.WriteLine(
                "Enter a valid non-negative integer.");
        }
    }
}
