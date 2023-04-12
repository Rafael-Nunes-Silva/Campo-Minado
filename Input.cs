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

    public struct GameInput
    {
        public readonly int column, line;
        public readonly bool flag;

        public GameInput(int column, int line, bool flag)
        {
            this.column = column;
            this.line = line;
            this.flag = flag;
        }
    }

    static public GameInput[] GetGameInput(Table table)
    {
        Console.ResetColor();
        Console.Write("Insira onde deseja jogar: ");

        string input = Console.ReadLine().ToUpper();

        Regex playRegex = new Regex("[A-Z][0-9]?[0-9]"),
            flagRegex = new Regex("-F$");
        MatchCollection playMatches = playRegex.Matches(input);
        Match flagMatch = flagRegex.Match(input);

        List<GameInput> gameInputs = new List<GameInput>(playMatches.Count);
        for (int i = 0; i < playMatches.Count; i++)
        {
            int column = letterIndexMap[playMatches[i].ToString()[0]],
                line = Convert.ToInt32(playMatches[i].ToString().Substring(1))-1;

            if (column < 0 || column > table.numOfColumns-1)
                continue;
            if (line < 0 || line > table.numOfLines-1)
                continue;

            gameInputs.Add(new GameInput(column, line, flagMatch.Success));
        }
        return gameInputs.ToArray();
    }
}