using System;
using System.Collections.Generic;

namespace TippingPoint {
  public class MemoryTable<TDto, TId> {
    private readonly Random _random;
    private readonly double _scaleFactor = 1d;
    public List<TDto> Rows { get; }
    private readonly List<TId> _pks;
    public readonly List<Range> IterationRanges = new List<Range>();
    private readonly Func<Random, (TDto, TId)> _factory;
    private double _count = 0d;
    public int Count { get; private set; }

    public MemoryTable(
      int seed,
      int startCount,
      double scaleFactor,
      Func<Random, (TDto, TId)> factory) {
      _random = new Random(seed);
      Count = startCount;
      _scaleFactor = scaleFactor;
      _factory = factory;

      // Create lists at exactly size they'll need to be, so they don't resize.
      // Add one to prevent rounding errors.
      // Subtract one from exponent to account for first iteration.
      // Example of 4 iterations w/1.5 scale factor and 1000 start count:
      // 1000 => 1500 => 2250 => 3375
      // So, 1000 * Math.Pow(1.5, 4 - 1) = 3375.
      var endCount = 1 + (int)(startCount * Math.Pow(scaleFactor, Program.Iterations - 1));
      Rows = new List<TDto>(endCount);
      _pks = new List<TId>(endCount);
    }

    // Don't bother with bounds check; this code is PERFECT.
    public TId NextPrimaryKey()
      => _pks[_random.Next(_pks.Count)];

    public IReadOnlyList<TDto> ScaleUp() {
      var countToAdd = ScaleAndGetCountToAdd();
      var range = new List<TDto>(countToAdd);
      for (var i = 0; i < countToAdd; i++) {
        var (dto, pk) = _factory(_random);
        range.Add(dto);
        _pks.Add(pk);
      }
      var start = Rows.Count;
      Rows.AddRange(range);
      IterationRanges.Add(new Range(start, Rows.Count));
      return range;
    }

    private int ScaleAndGetCountToAdd() {
      // First execution, "scale" from 0 to N.
      if (_count == 0d) {
        _count = Count;
        return Count;
      }

      // Scale with precision, but round for discrete row counts.
      _count *= _scaleFactor;
      var newCountRounded = (int)_count;
      var diffCount = newCountRounded - Count;
      Count = newCountRounded;
      return diffCount;
    }
  }
}