using System;
using TippingPoint.Attributes;
using TippingPoint.Extensions;

namespace TippingPoint.Dto {
  public class FooDto {
    [TvpColumn("dbo.FooTvp", 1)]
    public Guid FooId { get; set; }

    [TvpColumn("dbo.FooTvp", 2)]
    public int FooDatum1 { get; set; }

    [TvpColumn("dbo.FooTvp", 3)]
    public int FooDatum2 { get; set; }

    [TvpColumn("dbo.FooTvp", 4)]
    public int? FooDatum3 { get; set; }

    [TvpColumn("dbo.FooTvp", 5)]
    public DateTime DateCreated { get; set; }

    [TvpColumn("dbo.FooTvp", 6)]
    public DateTime DateModified { get; set; }

    [TvpColumn("dbo.FooTvp", 7)]
    public string UserCreated { get; set; }

    [TvpColumn("dbo.FooTvp", 8)]
    public string? UserModified { get; set; }

    // For Dapper...
    public FooDto(
      Guid fooId,
      int fooDatum1,
      int fooDatum2,
      int? fooDatum3,
      DateTime dateCreated,
      DateTime dateModified,
      string userCreated,
      string? userModified) {
      FooId = fooId;
      FooDatum1 = fooDatum1;
      FooDatum2 = fooDatum2;
      FooDatum3 = fooDatum3;
      DateCreated = dateCreated;
      DateModified = dateModified;
      UserCreated = userCreated;
      UserModified = userModified;
    }

    public FooDto(Random random) {
      var utcNow = DateTime.UtcNow;

      FooId = Guid.NewGuid();
      FooDatum1 = random.Next();
      FooDatum2 = random.Next();
      FooDatum3 = random.NextDouble() < 0.8d ? random.Next() : (int?)null;
      DateCreated = utcNow;
      DateModified = utcNow;
      UserCreated = random.NextUsername();
      UserModified = random.NextDouble() < 0.1d ? random.NextUsername() : null;
    }
  }
}