﻿using Listem.Views;

namespace Listem;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ListPage), typeof(ListPage));
        Routing.RegisterRoute(nameof(EditListPage), typeof(EditListPage));
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }
}
