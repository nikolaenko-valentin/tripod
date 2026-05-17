# NanoCAD SDK External Libraries

This folder contains the **NanoCAD SDK** (version 26.0.7228.4926.8429) managed assemblies required for developing plugins and extensions for NanoCAD.

## Contents

The following `.dll` files are included:

- `hostmgd.dll` – Core NanoCAD hosting API
- `hostdbmgd.dll` – Database and document management
- `HostMgdAvalonia.dll` – Avalonia UI integration
- `hostPointCloudsMgd.dll` – Point cloud support
- `imapimgd.dll`, `mapimgd.dll`, `mapiforms.dll`, `mapinet.dll` – Mapping and imagery features
- `MapiBaseTypes.dll`, `MapiBaseTypes2.dll` – Base type definitions for the MAPI (Mapping API)
- `mapiwpf.dll` – WPF-specific mapping controls
- `McUnits.dll` – Unit conversion and measurement utilities
- `NrxGateMgd.dll` – NetReactor (NRX) gate API

## Purpose

These libraries are **external references** for .NET projects that extend NanoCAD. They provide access to:

- Drawing and database operations (`Teigha` namespaces)
- Geometry types (`Point3d`, `Line`, etc.)
- UI elements and commands
- Map/raster functionality

## Usage

1. Add references to the required DLLs in your `.csproj` file:

   ```xml
   <Reference Include="hostmgd">
     <HintPath>path\to\this\folder\hostmgd.dll</HintPath>
     <Private>true</Private>
   </Reference>
   