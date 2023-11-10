# SA3D Project Configuration

This repository hosts reused project configurations for all SA3D projects.
<br/> (Greatly inspired by the [Reloaded Project](https://github.com/Reloaded-Project/Reloaded.Project.Configurations))

This includes:

- `.editorconfig` files for configuring code style.
- `NuGet.Build.props` for common project settings that target NuGet.
- `Directory.Build.props` for common project settings.

## Usage

To use this repository, add it as a submodule to your project.

```
git submodule add https://github.com/X-Hax/SA3D.ProjectConfigurations.git ./src/SA3D.ProjectConfigurations
```

Periodically update the submodule to the latest version.

```
git submodule update --remote
```

## Code Style

The code style is defined in the `.editorconfig` file in the root of the repo.

To apply this to your project, do the following:

- Enable symlinks for your current project by running `git config core.symlinks true`.
- Make symlink to `SA3D.ProjectConfigurations/.editorconfig` in the root of your project.
- In cmd you can do `mklink .editorconfig SA3D.ProjectConfigurations\.editorconfig` as admin.
- Or on Linux, you can do `ln -s "SA3D.ProjectConfigurations/.editorconfig" ".editorconfig"`.

## File Layout

The following is the expected file layout for your project:

```
- docs/
- src/
```

The `docs` folder should contain all documentation for your project (if present).
The `src` folder should contain all source code for your project.

## License

This repository is licensed under the GPLv3 license; as per the license of SA3D.
