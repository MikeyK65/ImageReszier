# ImageResize

`ImageResize` is a Blazor web application for resizing all supported images in a selected folder to a target width and height. It provides a simple UI for choosing a source folder, selecting common device-size presets, and processing images in bulk while optionally preserving the original aspect ratio.

The application targets `.NET 10`, uses Blazor with interactive server components, and performs image processing with `SkiaSharp`.

## Features

- Resize all supported images in a folder in one run
- Optional `Keep Aspect Ratio` mode
- Built-in presets for common iPhone and iPad screen sizes
- Progress display showing the file currently being processed
- Results table showing successful and failed files
- Automatic output folder generation to avoid overwriting previous runs
- Windows folder picker integration for selecting the source folder

## Technology Stack

- `.NET 10`
- `ASP.NET Core Blazor` with interactive server rendering
- `SkiaSharp` for image decoding, resizing, and encoding
- `System.Windows.Forms.FolderBrowserDialog` for folder selection

## Requirements

### Runtime / SDK

- `.NET 10 SDK`
- Windows

### Why Windows is required

This project targets `net10.0-windows` and enables Windows Forms:

- `UseWindowsForms` is enabled in the project file
- Folder selection is implemented with `System.Windows.Forms.FolderBrowserDialog`

Because of that, this app is intended to run on Windows.

## Supported Image Formats

The app currently processes files with these extensions:

- `.jpg`
- `.jpeg`
- `.png`
- `.bmp`
- `.gif`
- `.webp`
- `.tiff`
- `.tif`

## How the App Works

1. The user selects or types a source folder.
2. The user enters a target width and height, or clicks a preset.
3. The user chooses whether to preserve aspect ratio.
4. The app scans the selected folder for supported image files.
5. Each image is resized and written to a newly created output folder.
6. The UI displays progress and a final results summary.

### Aspect ratio behavior

When `Keep Aspect Ratio` is enabled, the app scales the image so it fits within the requested width and height without distortion.

When `Keep Aspect Ratio` is disabled, the image is resized directly to the requested dimensions.

### Output folder naming

The output folder is created inside the source folder using this pattern:

- `resized_yyyyMMdd`
- `resized_yyyyMMdd_1`
- `resized_yyyyMMdd_2`
- etc.

This avoids overwriting a previous resize run from the same day.

## Built-in Presets

The UI currently includes these presets:

- `iPhone 15 Pro Max (6.7")` — `1290 x 2796`
- `iPhone 15 (6.1")` — `1179 x 2556`
- `iPhone SE (4.7")` — `750 x 1334`
- `iPad Pro 12.9"` — `2048 x 2732`
- `iPad Pro 11"` — `1668 x 2388`

## Project Structure

```text
ImageResize/
├─ Components/
│  ├─ App.razor
│  ├─ Routes.razor
│  ├─ Layout/
│  └─ Pages/
│     ├─ Home.razor
│     ├─ Home.razor.css
│     ├─ Error.razor
│     └─ NotFound.razor
├─ Services/
│  ├─ FolderPickerService.cs
│  └─ ImageResizeService.cs
├─ Program.cs
├─ ImageResize.csproj
└─ README.md
```

## Key Files

### `Program.cs`
Configures the Blazor app and registers application services:

- `ImageResizeService`
- `FolderPickerService`

It also sets up Razor components with interactive server rendering.

### `Components/Pages/Home.razor`
The main UI for the application. It contains:

- Source folder input
- Browse button
- Width and height inputs
- Aspect ratio toggle
- Preset buttons
- Start and clear actions
- Progress display
- Resize results table

### `Services/FolderPickerService.cs`
Provides folder selection using `FolderBrowserDialog` on an STA thread.

### `Services/ImageResizeService.cs`
Contains the image-processing logic, including:

- supported file filtering
- output folder resolution
- resize execution
- dimension calculation
- format selection for encoded output

## Getting Started

### Clone the repository

```powershell
git clone https://github.com/MikeyK65/ImageReszier.git
cd ImageReszier
```

> Note: the repository remote currently uses the name `ImageReszier`.

### Build the app

```powershell
dotnet build
```

### Run the app

```powershell
dotnet run
```

After the app starts, open the local URL shown in the console, typically something like:

- `https://localhost:xxxx`
- `http://localhost:xxxx`

## Usage

1. Start the application.
2. Open the app in your browser.
3. Enter a folder path manually, or click `Browse…`.
4. Set the target width and height.
5. Optionally enable or disable `Keep Aspect Ratio`.
6. Optionally choose one of the preset device sizes.
7. Click `Go`.
8. Wait for processing to complete.
9. Review the results summary and output folder path.

## Validation Rules

The UI enforces the following checks before processing:

- The source folder must exist and be accessible
- Width must be greater than `0`
- Height must be greater than `0`

If validation fails, the page shows an inline error message.

## Output Behavior

- Resized images are saved using the original file name
- Files are written to the generated output folder inside the selected source folder
- Existing source files are not modified

### Output encoding by format

The app currently encodes output using these rules:

- `.png` → PNG
- `.gif` → GIF
- `.webp` → WebP
- all other supported formats → JPEG

## Development Notes

### Dependency Injection

The app registers its services as scoped services:

- `ImageResizeService`
- `FolderPickerService`

### Rendering model

The home page uses:

- `@page "/"`
- `@rendermode InteractiveServer`

This means the UI is interactive through Blazor server-side interactivity.

## Known Limitations

- The app only scans the top level of the selected folder; it does not recurse into subfolders
- The folder picker is Windows-specific
- Output format selection is based on the file extension
- JPEG quality is currently fixed in code
- Some original formats may be re-encoded as JPEG depending on extension handling

## Troubleshooting

### No images were processed

Possible reasons:

- The selected folder contains no supported image types
- The folder path is invalid
- The app does not have permission to read the folder

### Folder picker does not work

Possible reasons:

- The app is not running on Windows
- Windows Forms support is unavailable in the current environment

### Some files failed to resize

Possible reasons:

- The image file is corrupted
- The file is locked by another process
- The image cannot be decoded by `SkiaSharp`

The UI will display the error message returned for failed files.

## Future Improvement Ideas

- Add recursive folder processing
- Add selectable image quality settings
- Add format conversion options
- Add overwrite / skip behavior controls
- Add drag-and-drop support
- Add automated tests for resize logic
- Add logging and exportable operation summaries

## Package References

The project currently uses:

- `SkiaSharp`
- `SkiaSharp.NativeAssets.Win32`

