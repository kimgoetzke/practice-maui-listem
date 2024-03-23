﻿using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listem.Mobile.Events;
using Listem.Mobile.Models;
using Listem.Mobile.Services;
using Listem.Mobile.Utilities;
using Listem.Mobile.Views;
using User = Listem.Mobile.Models.User;
#if __ANDROID__
using CommunityToolkit.Maui.Core.Platform;
#endif

namespace Listem.Mobile.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _isUserRegistered;

    [ObservableProperty]
    private bool _isUserSignedIn;

    [ObservableProperty]
    private Status _userStatus;

    [ObservableProperty]
    private string? _userEmail;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private string? _passwordConfirmed;

    private readonly IServiceProvider _serviceProvider;

    // TODO: Enable displaying a loading state on app load (i.e. while attempting to refresh token)
    public LoginViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Initialise();
    }

    private async void Initialise()
    {
        WeakReferenceMessenger.Default.Register<UserStatusChangedMessage>(
            this,
            (_, m) =>
            {
                Logger.Log(
                    $"[LoginViewModel] Received message: Current user status has changed to: {m.Value}"
                );
                UpdateUser(m.Value);
            }
        );

        await RealmService.Init();
    }

    private void UpdateUser(User user)
    {
        IsUserRegistered = user.IsRegistered;
        IsUserSignedIn = user.IsSignedIn;
        UserEmail = user.EmailAddress;
        UserStatus = user.Status;
    }

    public void RedirectIfUserIsSignedIn()
    {
        if (!IsUserSignedIn)
            return;

        Logger.Log("User is signed in, redirecting now...");
        Shell.Current.Navigation.PushAsync(_serviceProvider.GetService<MainPage>());
    }

    [RelayCommand]
    private static async Task Back()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    [RelayCommand]
    private async Task SignUp(ITextInput view)
    {
        if (!IsInputValid())
            return;

        if (Password != PasswordConfirmed)
        {
            await Notifier.ShowAlertAsync("Sign up failed", "Passwords do not match", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            await RealmService.SignUpAsync(UserEmail!, Password!);
            HideKeyboard(view);
            await Shell.Current.Navigation.PopAsync();
            IsUserRegistered = true;
            Password = null;
            PasswordConfirmed = null;
            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            Logger.Log("Sign up failed: " + ex);
            await Notifier.ShowAlertAsync("Sign up failed", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SignIn(ITextInput view)
    {
        if (!IsInputValid())
            return;

        try
        {
            IsBusy = true;
            await RealmService.SignInAsync(UserEmail!, Password!);
            HideKeyboard(view);
            await Shell.Current.Navigation.PopAsync();
            IsUserSignedIn = true;
            Password = null;
            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            Logger.Log("Sign in failed: " + ex);
            await Notifier.ShowAlertAsync("Sign in failed", ex.Message, "OK");
        }
    }

    // TODO: Add password reset functionality (requires sending emails though)
    [RelayCommand]
    private static Task ForgotPassword()
    {
        Notifier.ShowToast("Sorry, not implemented yet");
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoToSignUp()
    {
        await Shell.Current.Navigation.PushAsync(_serviceProvider.GetService<SignUpPage>());
    }

    [RelayCommand]
    private async Task GoToSignIn()
    {
        await Shell.Current.Navigation.PushAsync(_serviceProvider.GetService<SignInPage>());
    }

    [RelayCommand]
    private static void SignOut()
    {
        RealmService.SignOutAsync().SafeFireAndForget();
    }

    [RelayCommand]
    private async Task GoToMainPage()
    {
        await Shell.Current.Navigation.PushAsync(_serviceProvider.GetService<MainPage>());
    }

    private bool IsInputValid()
    {
        if (!string.IsNullOrEmpty(UserEmail) && !string.IsNullOrEmpty(Password))
            return true;

        Notifier.ShowToast("You must enter both email and password");
        return false;
    }

    // ReSharper disable once UnusedParameter.Local
    private static void HideKeyboard(ITextInput view)
    {
#if __ANDROID__
        var isKeyboardHidden = view.HideKeyboardAsync(CancellationToken.None);
        Logger.Log("Keyboard hidden: " + isKeyboardHidden);
#endif
    }
}
