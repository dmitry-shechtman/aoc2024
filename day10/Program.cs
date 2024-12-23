using aoc;
using aoc.Grids;

var s = File.ReadAllText("input.txt").Trim();
var m = MultiGrid.Parse(s);

Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() => m[0].Sum(p =>
    m[1..10].Aggregate(new Grid(p), (a, g) =>
        a.MoveNext(g.Contains)).Count);

int Part2() =>
    m[1..10].Aggregate(m[0].AsEnumerable(), (a, g) =>
        a.SelectMany(p =>
            Grid.GetNeighbors(p).Where(g.Contains))).Count();
