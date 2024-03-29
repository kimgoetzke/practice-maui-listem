# The Listem Project

This repository contains an Android application and the relevant backend. The first one is a simple, minimalist to-do
list Android app written in C# using .NET 8 MAUI, the CommunityToolkit, and SQLite. It also contains the backend for
this app, also written in C# using .NET 8, Entity Framework Core, and ASP Core Authentication.

> [!IMPORTANT]  
> This project is a work in progress and actively being worked on. While the backend has been connected, there is no
> data sync when switching between online and offline modes. The app is also configured for localhost only.

The goal was to learn something about .NET MAUI and Android app development by building on
my first ever mobile app, the [Shopping List app](https://github.com/kimgoetzke/practice-maui-shopping-list) which I created the week before, and make a
look a little less nasty and also use some shared, custom controls. In addition, I wanted to learn about web development
with .NET and create a basic but fully-deployable service.

![Screenshots PNG](./assets/screenshots.png)

## MAUI Android application

### Overview

- A super basic, minimalist to-do list app targeting Android
- Users can register and log in to save their lists and categories in the cloud
- Alternatively, the app can be used without account/connection, storing all data in a SQLite database on the device
- Lists can be somewhat customised by adding categories or list types (e.g. changing to shopping list exposes a
  quantity control)
- A list's content can be exported to the clipboard as text
- List items can be imported from a comma-separated string from the clipboard and merged with the current list
- Native confirmation prompts are used for destructive actions
- Theming hasn't been implemented this time but can be enabled by configuring `DarkTheme.xaml` and exposing a control to
  change theme
- Icons used are CC0 from [iconsDB.com](https://www.iconsdb.com/) or self-made
- Colour scheme and topography inspired by Mailin
  Hülsmann's [Tennis App - UX/UI Design Case Study](https://www.behance.net/gallery/124361333/Tennis-App-UXUI-Design-Case-Study)

### Demo

![Demo GIF](./assets/demo.gif)

### How to configure your environment for development

1. Set environment variables for builds and running tests
    1. `ANDROID_HOME` - the absolute path of the Android SDK
    2. `SHOPPING_LIST_DEBUG_APK` - the absolute path of the debug APK, used by UI tests only
    3. `SHOPPING_LIST_RELEASE_APK` - the absolute path of the release APK, used by UI tests only
2. Run `dotnet restore` in the base directory to restore all dependencies

### How to build the APK

Create APK with:

```shell
cd Listem
dotnet publish -f:net8.0-android -c:Release /p:AndroidSdkDirectory=$env:ANDROID_HOME
```

This assumes that the Android SDK is installed and the `ANDROID_HOME` environment variable is set.

APK file can then be found in `ShoppingList\bin\Release\net8.0-android\publish\` and installed directly on any Android
phone.

### How to run UI tests

_Note: Currently, I am unable to get Appium to install the APK correctly on the emulator. The only way to make the app
start during tests is to first install the APK on the device and then run the tests. If the APK is ever installed by
Appium, the device needs to be wiped and the APK installed again without Appium for the tests to run._

To run the tests:

1. Install the APK on the device/emulator
2. Navigate to the `Listem.UITests` project with `cd Listem.UITests`
3. Run the tests via your IDE or `donet test`

## Backend

### Overview

- A minimal REST API for managing lists, categories, and items
- Token based authentication flow
- Separate Entity Framework Core database contexts for each entity type
- Middleware that creates and makes available a request context (e.g. containing the user id) for each authenticated
  endpoint
- Custom logging middleware
- Custom exception handling middleware

### How to configure the backend for development

1. Run `dotnet restore` in the base directory to restore all dependencies, if you haven't already done so.
2. The first time running the application, you'll need to create the database and run the migrations. This can be done
   by running the following command from the root of the repository:

    ```shell
    cd Listem.API && dotnet ef migrations add InitialCreate --context ListDbContext --output-dir Migrations/Lists  && dotnet ef database update --context ListDbContext && dotnet ef migrations add InitialCreate --context CategoryDbContext --output-dir Migrations/Categories && dotnet ef database update --context CategoryDbContext && dotnet ef migrations add InitialCreate --context ItemDbContext --output-dir Migrations/Items && dotnet ef database update --context ItemDbContext && dotnet ef migrations add InitialCreate --context UserDbContext --output-dir Migrations/Users && dotnet ef database update --context UserDbContext
    ```

   Alternatively, if you want to run each command separately:

    ```shell
    cd Listem.API
    dotnet ef migrations add InitialCreate --context ListDbContext --output-dir Migrations/Lists
    dotnet ef database update --context ListDbContext
    dotnet ef migrations add InitialCreate --context CategoryDbContext --output-dir Migrations/Categories 
    dotnet ef database update --context CategoryDbContext
    dotnet ef migrations add InitialCreate --context ItemDbContext --output-dir Migrations/Items
    dotnet ef database update --context ItemDbContext
    dotnet ef migrations add InitialCreate --context UserDbContext --output-dir Migrations/Users 
    dotnet ef database update --context UserDbContext
    ```

3. You can use the Postman collection in the `/assets` directory to test the API.

Note for the future: You can create a new certificate if you are running the application for the first time in HTTPS
mode:

```shell
dotnet dev-certs https --trust
```