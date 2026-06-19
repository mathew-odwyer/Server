using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Rooms;
using Winterhaven.API.Core.Domain.Entities.Rooms;
using Winterhaven.API.Infrastructure.Services.Seeds.Rooms;

namespace Winterhaven.API.Tests.Infrastructure.Services.Seeds.Rooms;

[TestFixture]
internal sealed class RoomSeedServiceTests
{
    private IRoomRepository roomRepository;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    [Test]
    public async Task SeedAsyncShouldCreateUnitOfWorkWhenCalled()
    {
        // Act
        await RoomSeedService.SeedAsync(this.unitOfWorkFactory, this.roomRepository).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task SeedAsyncShouldInvokeAddRangeAsyncWhenCalled()
    {
        // Act
        await RoomSeedService.SeedAsync(this.unitOfWorkFactory, this.roomRepository).ConfigureAwait(false);

        // Assert
        await this.roomRepository.Received(1).AddRangeAsync(Arg.Any<List<Room>>()).ConfigureAwait(false);
    }

    [Test]
    public async Task SeedAsyncShouldInvokeDeleteAllBeforeAddRangeAsyncWhenCalled()
    {
        // Act
        await RoomSeedService.SeedAsync(this.unitOfWorkFactory, this.roomRepository).ConfigureAwait(false);

        // Assert
        Received.InOrder(() =>
        {
            this.roomRepository.DeleteAll();
            this.roomRepository.AddRangeAsync(Arg.Any<List<Room>>());
        });
    }

    [Test]
    public async Task SeedAsyncShouldInvokeDeleteAllWhenCalled()
    {
        // Act
        await RoomSeedService.SeedAsync(this.unitOfWorkFactory, this.roomRepository).ConfigureAwait(false);

        // Assert
        this.roomRepository.Received(1).DeleteAll();
    }

    [Test]
    public async Task SeedAsyncShouldInvokeSaveAsyncWhenCalled()
    {
        // Act
        await RoomSeedService.SeedAsync(this.unitOfWorkFactory, this.roomRepository).ConfigureAwait(false);

        // Assert
        await this.unitOfWork.Received(1).SaveAsync().ConfigureAwait(false);
    }

    [Test]
    public void SeedAsyncShouldThrowArgumentNullExceptionWhenRoomRepositoryIsNull()
    {
        // Arrange
        IRoomRepository roomRepository = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => RoomSeedService.SeedAsync(this.unitOfWorkFactory, roomRepository));
    }

    [Test]
    public void SeedAsyncShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Arrange
        IUnitOfWorkFactory unitOfWorkFactory = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => RoomSeedService.SeedAsync(unitOfWorkFactory, this.roomRepository));
    }

    [SetUp]
    public void SetUp()
    {
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.roomRepository = Substitute.For<IRoomRepository>();

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);
    }
}
