namespace App;

internal class SeedData1
{
    internal List<List<uint>> Votes { get; } = new()
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

    internal List<uint> Sections { get; } = new()
    {
        10U, 20U, 30U, 40U, 50U, 60U, 70U, 80U, 90U, 100U, 110U,
        120U, 130U, 140U, 150U, 160U, 170U, 180U, 190U, 200U, 210U,
        220U, 230U, 240U, 250U, 260U, 270U, 280U, 290U, 300
    };

    internal List<uint> Candidates { get; } = new() { 15U, 25U, 35U, 55U };

    internal string Timestamp => "2020-04-10T17:50:00Z";

    internal string CompressedSectionData =
        "G94BIJwHdizgzsxREIheKVn2+ijCM5SBTfuH/EpkC9popr6Q/lBBUx78v7vns9a8BdQeXQDNWkte1hZQmAfgH8w+wsk2QRAKbEL8jBfFp/0va3jEE10q56GngWgZbWsOtRZrd7doGuKgeTyWrTOYsoaFMP3OYJyjTjMWIVaxYiwMXeVpG2e1DYRNUmpRngrGtYtoXtCWvOHFszOoVvzRTpRKZ+IkmwuFFY5whivc4Qlv+MIfFa3oiM7oiu7oid7oi/5UDaQWa//EKhz4iYnvhYNb4TM=";
}


//internal string SectionSHA256 = "7831e7b9bbb1673b1f97bb1e7baad0dc6259a66d2f7bb35f941adf1a21a35aa0";

//{"Votes":[[3,8,5,6],[1,0,7,4],[2,9,0,1],[7,8,5,7],[3,5,4,8],[9,0,2,4],[2,6,4,9],[4,7,4,2],[7,9,5,1],[6,8,3,7],[3,8,9,5],[9,6,2,3],[4,1,8,9],[1,7,5,2],[9,5,4,6],[6,0,7,3],[2,4,6,1],[3,2,9,8],[1,8,7,5],[7,6,1,9],[8,3,0,4],[5,9,8,6],[4,5,1,7],[8,4,2,3],[6,7,3,0],[2,1,5,9],[9,3,6,4],[5,0,8,2],[4,2,7,1],[7,1,0,8]],"Sections":[10,20,30,40,50,60,70,80,90,100,110,120,130,140,150,160,170,180,190,200,210,220,230,240,250,260,270,280,290,300],"Candidates":["John","Mark","David","Paul"]}

// "section_votes": {
//     "10": {"John": 3, "Mark": 8, "David": 5, "Paul": 6},
//     "20": {"John": 1, "Mark": 0, "David": 7, "Paul": 4},
//     "30": {"John": 2, "Mark": 9, "David": 0, "Paul": 1},
//     "40": {"John": 7, "Mark": 8, "David": 5, "Paul": 7},
//     "50": {"John": 3, "Mark": 5, "David": 4, "Paul": 8},
//     "60": {"John": 9, "Mark": 0, "David": 2, "Paul": 4},
//     "70": {"John": 2, "Mark": 6, "David": 4, "Paul": 9},
//     "80": {"John": 4, "Mark": 7, "David": 4, "Paul": 2},
//     "90": {"John": 7, "Mark": 9, "David": 5, "Paul": 1},
//     "100": {"John": 6, "Mark": 8, "David": 3, "Paul": 7},
//     "110": {"John": 3, "Mark": 8, "David": 9, "Paul": 5},
//     "120": {"John": 9, "Mark": 6, "David": 2, "Paul": 3},
//     "130": {"John": 4, "Mark": 1, "David": 8, "Paul": 9},
//     "140": {"John": 1, "Mark": 7, "David": 5, "Paul": 2},
//     "150": {"John": 9, "Mark": 5, "David": 4, "Paul": 6},
//     "160": {"John": 6, "Mark": 0, "David": 7, "Paul": 3},
//     "170": {"John": 2, "Mark": 4, "David": 6, "Paul": 1},
//     "180": {"John": 3, "Mark": 2, "David": 9, "Paul": 8},
//     "190": {"John": 1, "Mark": 8, "David": 7, "Paul": 5},
//     "200": {"John": 7, "Mark": 6, "David": 1, "Paul": 9},
//     "210": {"John": 8, "Mark": 3, "David": 0, "Paul": 4},
//     "220": {"John": 5, "Mark": 9, "David": 8, "Paul": 6},
//     "230": {"John": 4, "Mark": 5, "David": 1, "Paul": 7},
//     "240": {"John": 8, "Mark": 4, "David": 2, "Paul": 3},
//     "250": {"John": 6, "Mark": 7, "David": 3, "Paul": 0},
//     "260": {"John": 2, "Mark": 1, "David": 5, "Paul": 9},
//     "270": {"John": 9, "Mark": 3, "David": 6, "Paul": 4},
//     "280": {"John": 5, "Mark": 9, "David": 8, "Paul": 6},
//     "290": {"John": 4, "Mark": 5, "David": 1, "Paul": 7},
//     "300": {"John": 7, "Mark": 1, "David": 0, "Paul": 8}
// }