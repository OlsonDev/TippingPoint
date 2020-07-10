using System.Collections.Generic;
using TippingPoint.Dto;

namespace TippingPoint {
  public class ScaleUpResult {
    public IReadOnlyList<FooDto> AddedFoos { get; }
    public IReadOnlyList<BarDto> AddedBars { get; }
    public IReadOnlyList<QuxDto> AddedQuxs { get; }

    public ScaleUpResult(
      IReadOnlyList<FooDto> addedFoos,
      IReadOnlyList<BarDto> addedBars,
      IReadOnlyList<QuxDto> addedQuxs) {
      AddedFoos = addedFoos;
      AddedBars = addedBars;
      AddedQuxs = addedQuxs;
    }
  }
}