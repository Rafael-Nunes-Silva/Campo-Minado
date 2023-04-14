using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

static public class InputHandler
{
    static readonly Dictionary<char, int> letterIndexMap = new Dictionary<char, int>
    {
        { 'A', 0 },
        { 'B', 1 },
        { 'C', 2 },
        { 'D', 3 },
        { 'E', 4 },
        { 'F', 5 },
        { 'G', 6 },
        { 'H', 7 },
        { 'I', 8 },
        { 'J', 9 },
        { 'K', 10 },
        { 'L', 11 },
        { 'M', 12 },
        { 'N', 13 },
        { 'O', 14 },
        { 'P', 15 },
        { 'Q', 16 },
        { 'R', 17 },
        { 'S', 18 },
        { 'T', 19 },
        { 'U', 20 },
        { 'V', 21 },
        { 'W', 22 },
        { 'X', 23 },
        { 'Y', 24 },
        { 'Z', 25 }
    };

    public enum InputType
    {
        UNKNOWN,
        PLAY,
        FLAG,
        HILIGHT
    }

    public struct InputCell
    {
        public readonly int column, line;

        public InputCell(int column, int line)
        {
            this.column = column;
            this.line = line;
        }
    }

    static public InputCell[] GetInputCells(Table table, string input)
    {
        Regex playRegex = new Regex("[A-Z][0-9]?[0-9]");
        MatchCollection playMatches = playRegex.Matches(input);

        List<InputCell> gameInputs = new List<InputCell>(playMatches.Count);
        for (int i = 0; i < playMatches.Count; i++)
        {
            int column = letterIndexMap[playMatches[i].ToString()[0]],
                line = Convert.ToInt32(playMatches[i].ToString().Substring(1))-1;

            if (column < 0 || column > table.numOfColumns-1)
                continue;
            if (line < 0 || line > table.numOfLines-1)
                continue;

            gameInputs.Add(new InputCell(column, line));
        }
        return gameInputs.ToArray();
    }

    static public int GetHighlightInputLine(Table table, string input)
    {
        Regex lineRegex = new Regex("^[0-9]?[0-9]");

        int line = Convert.ToInt32(lineRegex.Match(input).ToString())-1;

        if (line < 0 || line > table.numOfLines - 1)
            return -1;

        return line;
    }

    static public InputType GetGameInput(out string input)
    {
        Console.ResetColor();
        Console.Write("Insira seu comando: ");

        input = Console.ReadLine().ToUpper();

        Regex playRegex = new Regex("[A-Z][0-9]?[0-9]"),
            flagRegex = new Regex("-F$"),
            highlightRegex = new Regex("^[0-9]?[0-9] -H$");

        if (highlightRegex.IsMatch(input))
            return InputType.HILIGHT;

        if (flagRegex.IsMatch(input))
            return InputType.FLAG;

        if (playRegex.IsMatch(input))
            return InputType.PLAY;

        return InputType.UNKNOWN;
    }
}