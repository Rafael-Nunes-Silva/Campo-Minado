using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

static class InputHandler
{
    static readonly Dictionary<char, uint> letterIndexMap = new Dictionary<char, uint>
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
        uint column, line;
        bool flag;

        public GameInput(uint column, uint line, bool flag)
        {
            this.column = column;
            this.line = line;
            this.flag = flag;
        }
    }

    static public GameInput[] GetGameInput(Table table)
    {
        string input = Console.ReadLine().ToUpper();

        Regex playRegex = new Regex("^[A-Z][0-9]?[0-9]"),
            flagRegex = new Regex("-f$");
        MatchCollection playMatches = playRegex.Matches(input);
        Match flagMatch = flagRegex.Match(input);

        List<GameInput> gameInputs = new List<GameInput>(playMatches.Count);
        for (int i = 0; i < playMatches.Count; i++)
        {
            uint column = letterIndexMap[playMatches[i].ToString()[0]] - 1,
                line = Convert.ToUInt32(playMatches[i].ToString().Substring(1)) - 1;

            if (column < 0 || column > table.numOfColumns-1)
                continue;
            if (line < 0 || line > table.numofLines-1)
                continue;

            gameInputs.Add(new GameInput(column, line, flagMatch.Success));
        }
        return gameInputs.ToArray();
    }
}