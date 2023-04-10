using System.Collections.Generic;

namespace Voting.Server.Tests.Utils.TestData;

internal static class SeedDataOld
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
        10U, 20U, 30U, 40U, 50U, 60U, 70U, 80U, 90U, 100U, 110U,
        120U, 130U, 140U, 150U, 160U, 170U, 180U, 190U, 200U, 210U,
        220U, 230U, 240U, 250U, 260U, 270U, 280U, 290U, 300
    };

    internal static List<uint> Candidates { get; } = new() { 15U, 25U, 35U, 55U };

    internal static string Timestamp => "2020-04-10T17:50:00Z";

    internal const string CompressedSectionData = "G94BIJwHdizgzsxREIheKVn2+ijCM5SBTfuH/EpkC9popr6Q/lBBUx78v7vns9a8BdQeXQDNWkte1hZQmAfgH8w+wsk2QRAKbEL8jBfFp/0va3jEE10q56GngWgZbWsOtRZrd7doGuKgeTyWrTOYsoaFMP3OYJyjTjMWIVaxYiwMXeVpG2e1DYRNUmpRngrGtYtoXtCWvOHFszOoVvzRTpRKZ+IkmwuFFY5whivc4Qlv+MIfFa3oiM7oiu7oid7oi/5UDaQWa//EKhz4iYnvhYNb4TM=";
}