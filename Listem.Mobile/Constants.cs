﻿namespace Listem.Mobile;

public static class Constants
{
    public const string HttpClientName = "ListemClient";
    public const string BaseUrlLocalhost = "http://10.0.2.2:5041";
    public const string User = "CurrentUser";

    // Error messages
    public const string UnauthorisedMessage = "You are not authorised to make this request";
    public const string ForbiddenMessage = "You are not allowed to make this request";
    public const string DefaultMessage = "Sorry, something went wrong - please try again";
    public const string TimeoutMessage = "Sorry, can't reach the server - please try again later";
}
