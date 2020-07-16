using System;
using TippingPoint.Attributes;
using TippingPoint.Extensions;

namespace TippingPoint.Dto {
  public class QuxDto {
    [TvpColumn("dbo.QuxTvp", 1)]
    public Guid QuxId { get; set; }

    [TvpColumn("dbo.QuxTvp", 2)]
    public Guid? FooId { get; set; }

    [TvpColumn("dbo.QuxTvp", 3)]
    public int? BarId { get; set; }

    [TvpColumn("dbo.QuxTvp", 4)]
    public int QuxDatum1 { get; set; }

    [TvpColumn("dbo.QuxTvp", 5)]
    public int QuxDatum2 { get; set; }

    [TvpColumn("dbo.QuxTvp", 6)]
    public int QuxDatum3 { get; set; }

    [TvpColumn("dbo.QuxTvp", 7)]
    public DateTime DateCreated { get; set; }

    [TvpColumn("dbo.QuxTvp", 8)]
    public DateTime DateModified { get; set; }

    [TvpColumn("dbo.QuxTvp", 9)]
    public string UserCreated { get; set; }

    [TvpColumn("dbo.QuxTvp", 10)]
    public string? UserModified { get; set; }

    // For Dapper...
    public QuxDto(
      Guid quxId,
      Guid fooId,
      int barId,
      int quxDatum1,
      int quxDatum2,
      int quxDatum3,
      DateTime dateCreated,
      DateTime dateModified,
      string userCreated,
      string? userModified) {
      QuxId = quxId;
      FooId = fooId;
      BarId = barId;
      QuxDatum1 = quxDatum1;
      QuxDatum2 = quxDatum2;
      QuxDatum3 = quxDatum3;
      DateCreated = dateCreated;
      DateModified = dateModified;
      UserCreated = userCreated;
      UserModified = userModified;
    }

    public QuxDto(Random random) {
      var utcNow = DateTime.UtcNow;

      QuxId = Guid.NewGuid();
      QuxDatum1 = GetRandomQuxDatum1(random); // dbo.NotificationSettings.NotificationType; 58 of these spread out from 0 to 203; database kinda groups up in a few places
      QuxDatum2 = random.NextDouble() < 0.3 ? 0 : 1; // dbo.NotificationSettings.NotificationMethod; only two of them right now
      QuxDatum3 = GetRandomQuxDatum3(random);
      DateCreated = utcNow;
      DateModified = utcNow;
      UserCreated = random.NextUsername();
      UserModified = random.NextDouble() < 0.1d ? random.NextUsername() : null;
    }

    private int GetRandomQuxDatum1(Random random) {
      var value = random.NextDouble();
      if (value < 0.2630454759953307006) return random.Next(  0,  11);
      if (value < 0.4501154906489829371) return random.Next( 20,  31);
      if (value < 0.6276978863969401186) return random.Next(110, 121);
      if (value < 0.7845913121227926382) return random.Next(100, 111);
      if (value < 0.9385043340039241981) return random.Next( 10,  21);
      if (value < 0.9745672205250478103) return random.Next( 30,  41);
      if (value < 0.9952810272458584801) return random.Next(120, 131);
      return random.Next(50, 61);
    }

    private int GetRandomQuxDatum3(Random random) {
      var value = random.NextDouble();
      if (value < 0.0005141843971631205) return 35;
      if (value < 0.0011524822695035460) return  5;
      if (value < 0.0044680851063829786) return  9;
      if (value < 0.0100177304964539005) return 32;
      if (value < 0.0220390070921985813) return 30;
      if (value < 0.0412588652482269500) return 20;
      if (value < 0.0650709219858156024) return  8;
      if (value < 0.1150177304964539002) return 10;
      if (value < 0.3193971631205673753) return -1;
      if (value < 0.5422695035460992901) return 50;
      return 60;
    }
  }
}