using Luna.IDE.WindowsManagement;
using Moq;
using NUnit.Framework;
using WindowsEnvironment.Model;

namespace Luna.Tests.IDE.WindowsManagement;

public class EnvironmentWindowsManagerTest
{
    private Mock<IPanel> _panel;
    private Mock<IContentTab> _tab;
    private Mock<IFlexWindowsEnvironment> _flexWindowsEnvironment;
    private Mock<IEnvironmentWindowModel> _environmentWindowModel;
    private Mock<ISaveableEnvironmentWindow> _saveableEnvironmentWindow;
    private Mock<ICloseableEnvironmentWindow> _closeableEnvironmentWindow;
    private object _viewContent;
    private Mock<IEnvironmentWindowView> _environmentWindowView;
    private EnvironmentWindowsManager _manager;

    [SetUp]
    public void Setup()
    {
        _panel = new Mock<IPanel>();
        _panel.SetupGet(x => x.Name).Returns("panel_1");
        _tab = new Mock<IContentTab>();
        _tab.SetupGet(x => x.Name).Returns("tab_1");
        _flexWindowsEnvironment = new Mock<IFlexWindowsEnvironment>();
        _flexWindowsEnvironment.Setup(x => x.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, It.IsAny<Content>())).Returns((_panel.Object, _tab.Object));
        _environmentWindowModel = new Mock<IEnvironmentWindowModel>();
        _viewContent = new object();
        _environmentWindowView = new Mock<IEnvironmentWindowView>();
        _environmentWindowView.SetupGet(x => x.Content).Returns(_viewContent);
        _saveableEnvironmentWindow = _environmentWindowModel.As<ISaveableEnvironmentWindow>();
        _closeableEnvironmentWindow = _environmentWindowModel.As<ICloseableEnvironmentWindow>();
        _manager = new EnvironmentWindowsManager(_flexWindowsEnvironment.Object);
    }

    [Test]
    public void OpenWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView.Object);
        Assert.That(_manager.FindWindowById("id"), Is.Not.Null);
        Assert.That(window.Id, Is.EqualTo("id"));
        Assert.That(window.Model, Is.EqualTo(_environmentWindowModel.Object));
        Assert.That(window.View, Is.EqualTo(_environmentWindowView.Object));
        Assert.That(_manager.SelectedWindow, Is.EqualTo(null));
    }

    [Test]
    public void OpenWindow_SetPanelPosition()
    {
        Content content = null;
        _flexWindowsEnvironment.Setup(x => x.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, It.IsAny<Content>()))
            .Callback<string, PanelPosition, Content>((_, _, c) => content = c)
            .Returns((_panel.Object, _tab.Object));

        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView.Object);

        Assert.That(content, Is.Not.Null);
        Assert.That(content.Header, Is.EqualTo(new Header() { SourceObject = _environmentWindowModel.Object, PropertyName = "Header" }));
        Assert.That(content.View, Is.EqualTo(_viewContent));
        Assert.That(content.CloseCallback, Is.Not.Null);
    }

    [Test]
    public void ActivateWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(window);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(window));
        _flexWindowsEnvironment.Verify(x => x.SelectTab("panel_1", "tab_1"), Times.Once());
    }

    [Test]
    public void ActivateWindow_WrongWindow()
    {
        _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(new EnvironmentWindow("", _environmentWindowModel.Object, _environmentWindowView.Object));
        Assert.That(_manager.SelectedWindow, Is.Null);
        _flexWindowsEnvironment.Verify(x => x.SelectTab(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Test]
    public void CloseWindow()
    {
        var window = _manager.OpenWindow("id", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.CloseWindow(window);
        Assert.That(_manager.FindWindowById("id"), Is.EqualTo(null));
        _closeableEnvironmentWindow.Verify(x => x.Close(), Times.Once());
    }

    [Test]
    public void CloseNotActive()
    {
        var notActive = _manager.OpenWindow("notActive", _environmentWindowModel.Object, _environmentWindowView.Object);
        var active = _manager.OpenWindow("active", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(active);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(active));
        _manager.CloseWindow(notActive);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(active));
    }

    [Test]
    public void CloseLast()
    {
        var window = _manager.OpenWindow("notActive", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(window);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(window));
        _manager.CloseWindow(window);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(null));
    }

    [Test]
    public void CloseAndActivate_Left()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView.Object);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView.Object);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(left);
        _manager.CloseWindow(left);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(middle));
    }

    [Test]
    public void CloseAndActivate_Middle()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView.Object);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView.Object);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(middle);
        _manager.CloseWindow(middle);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(right));
    }

    [Test]
    public void CloseAndActivate_Right()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView.Object);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView.Object);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView.Object);
        _manager.ActivateWindow(right);
        _manager.CloseWindow(right);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(middle));
    }

    [Test]
    public void CloseAndActivate_Right_All()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView.Object);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView.Object);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView.Object);

        _manager.ActivateWindow(right);
        _manager.CloseWindow(right);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(middle));

        _manager.CloseWindow(middle);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(left));

        _manager.CloseWindow(left);
        Assert.That(_manager.SelectedWindow, Is.EqualTo(null));
    }

    [Test]
    public void CloseAllWindows()
    {
        var left = _manager.OpenWindow("left", _environmentWindowModel.Object, _environmentWindowView.Object);
        var middle = _manager.OpenWindow("middle", _environmentWindowModel.Object, _environmentWindowView.Object);
        var right = _manager.OpenWindow("right", _environmentWindowModel.Object, _environmentWindowView.Object);

        _manager.CloseAllWindows();

        Assert.That(_manager.SelectedWindow, Is.EqualTo(null));
        Assert.That(_manager.FindWindowById("left"), Is.EqualTo(null));
        Assert.That(_manager.FindWindowById("middle"), Is.EqualTo(null));
        Assert.That(_manager.FindWindowById("right"), Is.EqualTo(null));
        _saveableEnvironmentWindow.Verify(x => x.Save(), Times.Exactly(3));
        _closeableEnvironmentWindow.Verify(x => x.Close(), Times.Exactly(3));
    }

    [Test]
    public void CloseAllWindows_ClosePanels()
    {
        var nonClosableEnvironmentWindowModel = new Mock<IEnvironmentWindowModel>();
        var window = _manager.OpenWindow("left", nonClosableEnvironmentWindowModel.Object, _environmentWindowView.Object);

        _manager.CloseAllWindows();

        _flexWindowsEnvironment.Verify(x => x.RemoveTab("panel_1", "tab_1", RemoveTabMode.Close), Times.Never());
    }
}
