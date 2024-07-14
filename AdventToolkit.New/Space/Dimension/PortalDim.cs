using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// Extends a dimension by allowing different sides of a position to teleport
/// to any other position.
/// </summary>
/// <param name="sideMap"></param>
/// <param name="dimension"></param>
/// <param name="sides"></param>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TDim"></typeparam>
/// <typeparam name="TSides"></typeparam>
/// <typeparam name="TSide"></typeparam>
/// <typeparam name="TMap"></typeparam>
public class PortalDim<TPos, TDim, TSides, TSide, TMap>(Dictionary<TPos, TMap> sideMap, TDim dimension, TSides sides)
    : IDimension<TPos>
    where TPos : notnull
    where TDim : IDimension<TPos>
    where TSides : IDimSides<TPos, TSide>
    where TMap : IDictionary<TSide, TPos>, new()
{
    public TDim Dimension { get; set; } = dimension;

    public TSides Sides { get; set; } = sides;

    /// <summary>
    /// Stores the custom mappings for a position.
    /// Whenever a position is mapped, it is assumed that all sides
    /// are defined in the mapping.
    /// </summary>
    public readonly Dictionary<TPos, TMap> SideMap = sideMap;

    public PortalDim(TDim dimension, TSides sides) : this([], dimension, sides) { }

    /// <summary>
    /// Get or create the mapping for a position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private TMap GetMap(TPos pos)
    {
        if (SideMap.TryGetValue(pos, out var map)) return map;
        
        // Create and populate the position mapping
        SideMap[pos] = map = new TMap();
        foreach (var s in Sides.GetSides(pos))
        {
            map[s] = Sides.GetSide(pos, s);
        }
        
        return map;
    }

    /// <summary>
    /// Remove all portal mappings.
    /// </summary>
    public void Clear() => SideMap.Clear();

    /// <summary>
    /// Create a portal.
    /// The specific side of the position will lead to the new target location.
    /// </summary>
    /// <param name="pos">Original position.</param>
    /// <param name="side">Position side.</param>
    /// <param name="other">New side target.</param>
    public void Link(TPos pos, TSide side, TPos other)
    {
        var map = GetMap(pos);
        map[side] = other;
    }

    /// <summary>
    /// Remove the custom mapping for a particular side.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="side"></param>
    public void Unlink(TPos pos, TSide side)
    {
        if (!SideMap.TryGetValue(pos, out var map)) return;
        map[side] = Sides.GetSide(pos, side);
    }

    /// <summary>
    /// Remove the custom mapping for all sides of a position.
    /// </summary>
    /// <param name="pos"></param>
    public void Remove(TPos pos) => SideMap.Remove(pos);

    public IEnumerable<TPos> GetNeighbors(TPos pos)
    {
        return !SideMap.TryGetValue(pos, out var map) ? Dimension.GetNeighbors(pos) : map.Values;
    }
}