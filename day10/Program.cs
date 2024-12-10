using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s, "0123456789");

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() => m[0].Sum(p =>
    Enumerable.Range(1, 9).Aggregate(new Grid(p), (a, i) =>
        new(a.SelectMany(q =>
            m[i].GetNeighbors(q).Where(m[i].Contains)))).Count);

int Part2() =>
    Enumerable.Range(1, 9).Aggregate(m[0].ToArray(), (a, i) =>
        a.SelectMany(q =>
            m[i].GetNeighbors(q).Where(m[i].Contains)).ToArray()).Length;
