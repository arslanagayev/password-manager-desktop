# Password Manager

A small Windows desktop app for keeping track of website logins, built with **C#, WPF (.NET 8)** and **SQLite**.
It was my first desktop project: a dark-themed window where you add a site, username and password, see your saved entries in a list and delete the ones you no longer need.

> [!WARNING]
> **Learning project: don't store real passwords in it yet.**
> Entries are saved **unencrypted** in a local SQLite file and are shown in plain text in the list.
> Encrypting the vault behind a master password is the next item on the [roadmap](#roadmap).

## Features

- **Add entries:** site name, username and password, with validation so empty fields are rejected.
- **View entries:** all saved logins in one list.
- **Delete entries:** with a confirmation dialog so nothing is removed by accident.
- **Local storage:** everything stays on your machine in a SQLite database; no network access, no cloud.
- **Safe SQL:** every query uses parameters (`$site`, `$username`, …), so input can't inject SQL.
- **Dark theme:** custom WPF styles for the inputs, buttons and list.

## Tech stack

| Layer | Technology |
|---|---|
| Language | C# 12 |
| UI | WPF on .NET 8 (`net8.0-windows`) |
| Storage | SQLite via [Microsoft.Data.Sqlite](https://learn.microsoft.com/dotnet/standard/data/sqlite/) |
| Code scanning | GitHub CodeQL (C#) on every push and pull request |

## Getting started

### Prerequisites

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Optional: Visual Studio 2022 with the *.NET desktop development* workload

### Run it

```bash
git clone https://github.com/arslanagayev/password-manager-desktop.git
cd password-manager-desktop
dotnet run
```

Or open `PasswordManager.sln` in Visual Studio and press <kbd>F5</kbd>.

On first launch the app creates a `passwords.db` file in its working directory (when started from Visual Studio, that is the build output folder such as `bin/Debug/net8.0-windows/`).

## How it works

```text
MainWindow.xaml        UI layout: input form + list of saved entries
MainWindow.xaml.cs     Event handlers and data access (create table, insert, select, delete)
PasswordEntry.cs       Model for one saved login (Id, Site, Username, Password)
App.xaml               Application entry point
```

When the window opens it creates the `Passwords` table if needed and loads every row into an `ObservableCollection`, so the list updates automatically after each save or delete.

## Roadmap

- [ ] **Master password + encryption:** derive a key with PBKDF2 and encrypt each password with AES-GCM before it touches the disk
- [ ] **Masked passwords:** hide passwords in the list with show and copy-to-clipboard buttons (clipboard cleared after a timeout)
- [ ] **Store the vault in `%LOCALAPPDATA%`** instead of the working directory
- [ ] **Keep passwords exactly as typed** (no trimming of leading or trailing spaces)
- [ ] **Separate data access from the UI** and cover it with unit tests
- [ ] Search, edit entries and a password generator

## License

[MIT](LICENSE) © 2026 Arslan Agayev
