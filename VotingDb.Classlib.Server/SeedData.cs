namespace VotingDb.Classlib.Server;

internal static class SeedData
{
    internal static List<List<uint>> Votes { get; } = new()
    {
        new List<uint> { 3, 8, 5, 6 },
        new List<uint> { 1, 0, 7, 4 },
        new List<uint> { 2, 9, 0, 1 },
        new List<uint> { 7, 8, 5, 7 },
        new List<uint> { 3, 5, 4, 8 },
        new List<uint> { 9, 0, 2, 4 },
        new List<uint> { 2, 6, 4, 9 },
        new List<uint> { 4, 7, 4, 2 },
        new List<uint> { 7, 9, 5, 1 },
        new List<uint> { 6, 8, 3, 7 },
        new List<uint> { 3, 8, 9, 5 },
        new List<uint> { 9, 6, 2, 3 },
        new List<uint> { 4, 1, 8, 9 },
        new List<uint> { 1, 7, 5, 2 },
        new List<uint> { 9, 5, 4, 6 },
        new List<uint> { 6, 0, 7, 3 },
        new List<uint> { 2, 4, 6, 1 },
        new List<uint> { 3, 2, 9, 8 },
        new List<uint> { 1, 8, 7, 5 },
        new List<uint> { 7, 6, 1, 9 },
        new List<uint> { 8, 3, 0, 4 },
        new List<uint> { 5, 9, 8, 6 },
        new List<uint> { 4, 5, 1, 7 },
        new List<uint> { 8, 4, 2, 3 },
        new List<uint> { 6, 7, 3, 0 },
        new List<uint> { 2, 1, 5, 9 },
        new List<uint> { 9, 3, 6, 4 },
        new List<uint> { 5, 0, 8, 2 },
        new List<uint> { 4, 2, 7, 1 },
        new List<uint> { 7, 1, 0, 8 }
    };

    internal static List<uint> Sections { get; } = new()
    {
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110,
        120, 130, 140, 150, 160, 170, 180, 190, 200, 210,
        220, 230, 240, 250, 260, 270, 280, 290, 300
    };

    internal static List<string> Candidates { get; } = new() { "John", "Mark", "David", "Paul" };

    internal static string Timestamp => "2020-04-10T17:50:00Z";
}