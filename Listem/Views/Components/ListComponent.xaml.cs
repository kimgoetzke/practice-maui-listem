﻿using Listem.Models;
using Listem.Utilities;

namespace Listem.Views.Components;

public partial class ListComponent
{
    public ListComponent()
    {
        InitializeComponent();
    }

    private void SwipeItemView_OnInvoked(object? sender, EventArgs e)
    {
        // TODO: Give user feedback through particles or animation
        Logger.Log("OnInvokedSwipeItem");
    }

    private async void CheckBox_OnTaskCompleted(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is not CheckBox { IsChecked: true } checkBox)
            return;

        if (checkBox.BindingContext is not Item item)
            return;

        // await _viewModel.RemoveItem(item);
        await Task.Delay(200);
    }
}
