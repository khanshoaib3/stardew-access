# Contributing to Stardew Access

Thank you for your interest in contributing! We welcome improvements, bug fixes, new features, and documentation updates.

---

## How to Contribute

1. **Fork the repository** and clone your fork locally.
2. **Create a new branch** for your changes.
3. **Make your changes** and commit with clear messages.
4. **Push your branch** to your fork.
5. **Open a Pull Request** describing your changes.

---

## Project Setup

To set up Stardew Access locally:

1. **Clone your fork:**

   ```sh
   git clone https://github.com/your-username/stardew-access.git
   cd stardew-access
   ```

2. **Install dependencies:**
   - Ensure you have [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) installed.
   - You will need [Ruby](https://www.ruby-lang.org/en/downloads/) for compiling docs (see `docs/compiler_script.rb`).
   - Install the required Ruby gem for documentation:
     ```sh
     gem install kramdown
     ```
   - You need [Project Fluent](https://www.nexusmods.com/stardewvalley/mods/12638) (required for translations/localization).
     Download and extract it so your folder structure looks like:
     `	 <GamePath>/Mods/ProjectFluent/ProjectFluent.dll
	`
     Where `<GamePath>` is the location of your Stardew Valley installation.

3. **Build the project:**

   ```sh
   dotnet build stardew-access.sln
   ```

4. **Run or test the mod:**
   - Place the built files in your Stardew Valley `Mods` folder.
   - Follow instructions in [docs/setup.md](docs/setup.md) for more details.

5. **Compile documentation (optional):**
   ```sh
   cd docs
   ruby compiler_script.rb
   ```

---

## Code Style

- Follow the existing code conventions.
- Write clear, maintainable code.
- Add comments where necessary.
- Update or add documentation as needed.

---

## Reporting Issues & Requesting Features

- Use GitHub Issues for bugs, feature requests, or questions.
- Provide as much detail as possible.

---

## Pull Requests

- Ensure your branch is up to date with the main branch.
- Fill out the PR template.
- Be responsive to feedback and requested changes.

---

## Code of Conduct

By participating, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

---

## License

By contributing, you agree your contributions are licensed under the repository’s license.

---

## Contact

For questions or support, reach out via GitHub Issues or our [Discord server](https://discord.gg/yQjjsDqWQX).
