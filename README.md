# Hero EV VCI Diagnostic Manager V2.0

GitHub-ready browser application for managing Hero EV diagnostic workflows and a local firmware compatibility database.

## Included
- Dashboard
- VCI connection manager
- Demo / Web Serial / WebUSB connection layer
- ECU diagnostic workflow
- DTC manager
- Live-data demo
- Firmware search and compatibility database
- Flashing workflow UI
- VIN / vehicle information
- Local service history
- PWA manifest and offline cache

## Run locally
Because the app uses ES modules and JSON fetches, serve it from a local web server rather than opening `index.html` directly.

Example:
```bash
python -m http.server 8080
```
Then open `http://localhost:8080`.

## GitHub Pages
1. Create a public repository.
2. Upload the complete project.
3. Open Settings -> Pages.
4. Choose Deploy from branch.
5. Select `main` and `/ (root)`.
6. Save.

## VCI integration
The project contains a Web Serial/WebUSB adapter layer. Actual vehicle communication requires a compatible VCI, its documented protocol/SDK, and authorized diagnostic/ECU procedures. The flashing page is a workflow manager and does not implement proprietary ECU flashing protocols.

## Firmware database
`data/firmware-database.json` contains the firmware mappings supplied for V1, V2 and VX2 model variants.
