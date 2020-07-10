using System;
using TippingPoint.Attributes;
using TippingPoint.Extensions;

namespace TippingPoint.Dto {
  public class BarDto {
    private static int Identity = 1; // Reduce orchestration code by just handling the identity aspect ourselves...

    [TvpColumn("dbo.BarTvp", 1)]
    public int BarId { get; set; }

    [TvpColumn("dbo.BarTvp", 2)]
    public Guid? FooId { get; set; }

    [TvpColumn("dbo.BarTvp", 3)]
    public int BarDatum1 { get; set; }

    [TvpColumn("dbo.BarTvp", 4)]
    public int BarDatum2 { get; set; }

    [TvpColumn("dbo.BarTvp", 5)]
    public int BarDatum3 { get; set; }

    [TvpColumn("dbo.BarTvp", 6)]
    public DateTime DateCreated { get; set; }

    [TvpColumn("dbo.BarTvp", 7)]
    public DateTime DateModified { get; set; }

    [TvpColumn("dbo.BarTvp", 8)]
    public string UserCreated { get; set; }

    [TvpColumn("dbo.BarTvp", 9)]
    public string? UserModified { get; set; }

    // For Dapper...
    public BarDto(
      int barId,
      Guid fooId,
      int barDatum1,
      int barDatum2,
      int barDatum3,
      DateTime dateCreated,
      DateTime dateModified,
      string userCreated,
      string? userModified) {
      BarId = barId;
      FooId = fooId;
      BarDatum1 = barDatum1;
      BarDatum2 = barDatum2;
      BarDatum3 = barDatum3;
      DateCreated = dateCreated;
      DateModified = dateModified;
      UserCreated = userCreated;
      UserModified = userModified;
    }

    public BarDto(Random random) {
      var utcNow = DateTime.UtcNow;

      BarId = Identity++;
      BarDatum1 = random.Next();
      BarDatum2 = random.Next();
      BarDatum3 = random.Next();
      DateCreated = utcNow;
      DateModified = utcNow;
      UserCreated = random.NextUsername();
      UserModified = random.NextDouble() < 0.1d ? random.NextUsername() : null;
    }
  }
}