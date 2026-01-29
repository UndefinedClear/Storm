![banner](images/banner.png)

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ✨ Features

- 🗑 Move files and directories to a safe trash with history
- ♻ Restore any previous version in seconds
- 🕒 Built-in history tracking
- 📦 Lightweight and fast
- 💻 Works directly from the command line

---

## 🚀 Installation

> ⚠ **.NET 9.0 SDK or newer is recommended**

```bash
# Clone the repository
git clone https://github.com/UndefinedClear/Storm.git
cd Storm

# Quick build (automatically selects SDK version)
dotnet build
```

### ⚡ Automated Build

You can also use the provided build scripts to generate a standalone executable in the `publish` folder:

- **Windows:** Run `Storm/build.bat`
- **Linux:** Run `Storm/build.sh` (make sure to `chmod +x build.sh` first)

These scripts create a self-contained, single-file release.
`

Or download the latest build from the **Releases** section.

---

## 📖 Commands

### 🆘 Help

```bash
storm
```

![Help](images/help.png)

---

### 🗑 Move to Trash

Move one or multiple files and directories into Storm history.

```bash
storm test.txt
storm folder1 folder2 image.png
```

![Move to trash](images/move_to_trash.png)

---

### ♻ Restore a Specific Version

Restore a file or directory using its history ID or version.

```bash
storm -r HISTORY_ID_OR_VERSION test.txt
storm -r 2026-01-29_21-03-03 folder1
```

![Restore](images/restore.png)

---

### 🕒 View History

Show all saved versions.

```bash
storm -h
```

![History](images/history.png)

---

## 🧠 How It Works

Storm keeps safe snapshots of deleted or replaced files so you can restore them later.
Each operation is versioned with a timestamp, making recovery simple and reliable.

---

## ⚙ Requirements

- **.NET 9.0 SDK (recommended)**
- Windows, Linux, or macOS
- Terminal with ANSI color support

---

## 📦 Releases

Stable builds are available in the **Releases** section:
➡ Download compiled binaries
➡ View changelogs
➡ Access previous versions

---

## 🐞 Issues

Found a bug or have a feature request?
Open an issue here:
👉 [https://github.com/UndefinedClear/Storm/issues](https://github.com/UndefinedClear/Storm/issues)

Please include:

- What happened
- Steps to reproduce
- Your OS and Storm version

---

## 🤝 Contributing

Pull Requests are welcome!

1. Fork the repository
2. Create a new branch
3. Make your changes
4. Submit a Pull Request

Before submitting, please:

- Follow the existing code style
- Test your changes

---

## 🔀 Pull Requests

All improvements, fixes, and ideas are appreciated.
Small PRs are reviewed faster 😉

---

## 🗺 Roadmap

Planned improvements and upcoming features:

- [ ] Auto-clean old history versions
- [ ] Config file support (`storm.config`)
- [ ] Ignore patterns (like `.gitignore`)
- [ ] Restore preview before confirming
- [ ] Compression for stored versions
- [ ] Global install via `dotnet tool`
- [ ] Interactive TUI mode
- [ ] Performance improvements for large directories

Have an idea? Open an issue and share it!

---

## 📜 License

This project is licensed under the MIT License.

---

## ⭐ Support

If Storm saved your files, give it a star ⭐
It helps the project grow!
