using System;
using System.Threading.Tasks;
using TippingPoint.Dto;

namespace TippingPoint {
  public class MemoryDb {
    // Pick different seeds, but use seeds for reproducability.
    // Current counts:
    //  23353  IUsers
    //   2499  Orgs
    //  40098  NotificationSettings
    public readonly MemoryTable<FooDto, Guid> Foo = new MemoryTable<FooDto, Guid>(1, 20000, 1.1, FooFactory);
    public readonly MemoryTable<BarDto, int> Bar = new MemoryTable<BarDto, int>(2, 2500, 1.1, BarFactory);
    public readonly MemoryTable<QuxDto, Guid> Qux = new MemoryTable<QuxDto, Guid>(3, 50000, 1.1, QuxFactory);

    public async Task<ScaleUpResult> ScaleUpAsync() {
      // Generate all objects in their own tasks.
      // Then when they're all done, associate FKs.
      // We could generate all of data up front and then associate the FKs, but instead we're going to
      // do this in batches based on Program.Iterations so the first batch of data has higher concentration
      // of FKs as this kinda emulates the real world more: the earlier users/orgs have more data associated
      // with them than the newer users/orgs.
      var fooTask = Task.Run(Foo.ScaleUp);
      var barTask = Task.Run(Bar.ScaleUp);
      var quxTask = Task.Run(Qux.ScaleUp);
      var tasks = new Task[] { fooTask, barTask, quxTask };
      await Task.WhenAll(tasks);
      var addedFoos = await fooTask;
      var addedBars = await barTask;
      var addedQuxs = await quxTask;
      foreach (var bar in addedBars) {
        bar.FooId = Foo.NextPrimaryKey();
      }
      foreach (var qux in addedQuxs) {
        qux.FooId = Foo.NextPrimaryKey();
        qux.BarId = Bar.NextPrimaryKey();
      }
      return new ScaleUpResult(addedFoos, addedBars, addedQuxs);
    }

    private static (FooDto, Guid) FooFactory(Random random) {
      var dto = new FooDto(random);
      return (dto, dto.FooId);
    }

    private static (BarDto, int) BarFactory(Random random) {
      var dto = new BarDto(random);
      return (dto, dto.BarId);
    }

    private static (QuxDto, Guid) QuxFactory(Random random) {
      var dto = new QuxDto(random);
      return (dto, dto.QuxId);
    }
  }
}