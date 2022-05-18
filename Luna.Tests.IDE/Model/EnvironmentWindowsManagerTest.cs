using Luna.IDE.Model;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Model;

public class EnvironmentWindowsManagerTest
{
    private Mock<IEnvironmentWindowModel> _environmentWindowModel;
    private object _environmentWindowView;
    private EnvironmentWindowsManager _manager;

    [SetUp]
    public void Setup()
    {
        _environmentWindowModel = new Mock<IEnvironmentWindowModel>();
        _environmentWindowView = new object();
        _manager = new EnvironmentWindowsManager();
    }

    [Test]
    public void OpenWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView);
        Assert.NotNull(_manager.FindWindowById("id"));
        Assert.AreEqual("id", window.Id);
        Assert.AreEqual(_environmentWindowModel.Object, window.Model);
        Assert.AreEqual(_environmentWindowView, window.View);
        Assert.AreEqual(null, _manager.SelectedWindow);
    }

    [Test]
    public void ActivateWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(window);
        Assert.AreEqual(window, _manager.SelectedWindow);
    }

    [Test]
    public void CloseWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView);
        _manager.CloseWindow(window);
        Assert.AreEqual(null, _manager.FindWindowById("id"));
    }

    [Test]
    public void CloseNotActive()
    {
        var notActive = _manager.OpenWindow("notActive", _environmentWindowModel.Object, _environmentWindowView);
        var active = _manager.OpenWindow("active", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(active);
        Assert.AreEqual(active, _manager.SelectedWindow);
        _manager.CloseWindow(notActive);
        Assert.AreEqual(active, _manager.SelectedWindow);
    }

    [Test]
    public void CloseLast()
    {
        var window = _manager.OpenWindow("notActive", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(window);
        Assert.AreEqual(window, _manager.SelectedWindow);
        _manager.CloseWindow(window);
        Assert.AreEqual(null, _manager.SelectedWindow);
    }

    [Test]
    public void CloseAndActivate_Left()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(left);
        _manager.CloseWindow(left);
        Assert.AreEqual(middle, _manager.SelectedWindow);
    }

    [Test]
    public void CloseAndActivate_Middle()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(middle);
        _manager.CloseWindow(middle);
        Assert.AreEqual(right, _manager.SelectedWindow);
    }

    [Test]
    public void CloseAndActivate_Right()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView);
        _manager.ActivateWindow(right);
        _manager.CloseWindow(right);
        Assert.AreEqual(middle, _manager.SelectedWindow);
    }

    [Test]
    public void CloseAndActivate_Right_All()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView);

        _manager.ActivateWindow(right);
        _manager.CloseWindow(right);
        Assert.AreEqual(middle, _manager.SelectedWindow);

        _manager.CloseWindow(middle);
        Assert.AreEqual(left, _manager.SelectedWindow);

        _manager.CloseWindow(left);
        Assert.AreEqual(null, _manager.SelectedWindow);
    }
}
