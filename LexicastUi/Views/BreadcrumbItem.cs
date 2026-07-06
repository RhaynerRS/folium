using System;

namespace LexicastUi.Views;

public sealed record BreadcrumbItem(string Label, Action? OnClick = null);
