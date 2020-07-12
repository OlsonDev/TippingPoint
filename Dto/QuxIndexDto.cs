using System;

namespace TippingPoint.Dto {
  public class QuxIndexDto {
    public Guid QuxId { get; set; }
    public Guid FooId { get; set; }
    public int? BarId { get; set; }
    public int QuxDatum1 { get; set; }
    public int QuxDatum2 { get; set; }
    public int? QuxDatum3 { get; set; }

    public QuxIndexDto(
      Guid quxId,
      Guid fooId,
      int barId,
      int quxDatum1,
      int quxDatum2,
      int? quxDatum3) {
      QuxId = quxId;
      FooId = fooId;
      BarId = barId;
      QuxDatum1 = quxDatum1;
      QuxDatum2 = quxDatum2;
      QuxDatum3 = quxDatum3;
    }
  }
}