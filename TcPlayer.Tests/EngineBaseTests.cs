using TcPlayer.Engine;

namespace TcPlayer.Tests;

[TestFixture]
public class EngineBaseTests
{
    private class TestEngineBase : EngineBase
    {
        public TestEngineBase(IMediator mediator) : base(mediator)
        {
        }
    }

    private TestEngineBase _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new TestEngineBase(Substitute.For<IMediator>());
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
    }

    [Test]
    public void Ensure_That_GetDevices_Retuns_NotEmptyCollection()
    {
        Assert.That(_sut.GetDevices(), Is.Not.Empty);
    }

    [Test]
    public void Ensure_That_GetDevices_FillsOutDeviceInfo()
    {
        var info = _sut.GetDevices().First();

        Assert.Multiple(() =>
        {
            Assert.That(info.Index, Is.GreaterThan(-1));
            Assert.That(info.Name, Is.Not.Empty);
            Assert.That(info.Frequency, Is.GreaterThan(0));
            Assert.That(info.Channels, Is.GreaterThan(0));
            Assert.That(info.UpdatePeriod, Is.GreaterThan(0));
        });
    }
}