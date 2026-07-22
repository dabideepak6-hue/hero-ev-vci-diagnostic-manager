using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// DEEPAK DABI VCI SCAN - LOCAL VCI BRIDGE
// ============================================================

builder.WebHost.UseUrls("http://127.0.0.1:8765");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Register VCI service
builder.Services.AddSingleton<IVciHardwareService, VciHardwareService>();

var app = builder.Build();

app.UseCors();

var vci = app.Services.GetRequiredService<IVciHardwareService>();


// ============================================================
// STATUS
// ============================================================

app.MapGet("/api/status", () =>
{
    return Results.Ok(new
    {
        success = true,
        application = "DEEPAK DABI VCI SCAN",
        bridge = true,
        endpoint = "127.0.0.1:8765",
        connected = vci.IsConnected,
        device = vci.DeviceName,
        mode = vci.Mode
    });
});


// ============================================================
// CONNECT VCI
// ============================================================

app.MapPost("/api/connect", async () =>
{
    try
    {
        var result = await vci.ConnectAsync();

        return Results.Ok(new
        {
            success = result,
            connected = vci.IsConnected,
            device = vci.DeviceName,
            message = result
                ? "VCI connected successfully."
                : "VCI connection failed.",
            mode = vci.Mode
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "VCI Connection Error",
            detail: ex.Message,
            statusCode: 500
        );
    }
});


// ============================================================
// DISCONNECT VCI
// ============================================================

app.MapPost("/api/disconnect", async () =>
{
    await vci.DisconnectAsync();

    return Results.Ok(new
    {
        success = true,
        connected = false,
        message = "VCI disconnected."
    });
});


// ============================================================
// VEHICLE IDENTIFICATION
// ============================================================

app.MapPost("/api/vehicle/identify", async () =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    var vehicle = await vci.IdentifyVehicleAsync();

    return Results.Ok(vehicle);
});


// ============================================================
// FULL VEHICLE SCAN
// ============================================================

app.MapPost("/api/diagnostics/scan", async () =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    var result = await vci.FullScanAsync();

    return Results.Ok(result);
});


// ============================================================
// ECU SCAN
// ============================================================

app.MapPost("/api/ecu/scan", async (EcuScanRequest request) =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    if (string.IsNullOrWhiteSpace(request.Ecu))
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "ECU name is required."
        });
    }

    var result =
        await vci.ScanEcuAsync(request.Ecu);

    return Results.Ok(result);
});


// ============================================================
// READ DTC
// ============================================================

app.MapGet("/api/dtc/read", async () =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    var result =
        await vci.ReadDtcAsync();

    return Results.Ok(result);
});


// ============================================================
// CLEAR DTC
// ============================================================

app.MapPost("/api/dtc/clear", async () =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    var result =
        await vci.ClearDtcAsync();

    return Results.Ok(result);
});


// ============================================================
// LIVE DATA
// ============================================================

app.MapGet("/api/live-data", async () =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    var data =
        await vci.GetLiveDataAsync();

    return Results.Ok(data);
});


// ============================================================
// FIRMWARE COMPATIBILITY CHECK
// ============================================================

app.MapPost(
    "/api/firmware/verify",
    async (FirmwareRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.File))
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Firmware file is required."
        });
    }

    var result =
        await vci.VerifyFirmwareAsync(
            request.File
        );

    return Results.Ok(result);
});


// ============================================================
// FIRMWARE PROGRAMMING
// ============================================================

app.MapPost(
    "/api/firmware/program",
    async (FirmwareRequest request) =>
{
    if (!vci.IsConnected)
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "VCI is not connected."
        });
    }

    if (string.IsNullOrWhiteSpace(request.File))
    {
        return Results.BadRequest(new
        {
            success = false,
            message = "Firmware file is required."
        });
    }

    var result =
        await vci.ProgramFirmwareAsync(
            request.File
        );

    return Results.Ok(result);
});


// ============================================================
// START BRIDGE
// ============================================================

Console.WriteLine(
    "=============================================="
);

Console.WriteLine(
    "       DEEPAK DABI VCI SCAN"
);

Console.WriteLine(
    "       HERO EV LOCAL VCI BRIDGE"
);

Console.WriteLine(
    "=============================================="
);

Console.WriteLine(
    "Bridge URL:"
);

Console.WriteLine(
    "http://127.0.0.1:8765"
);

Console.WriteLine(
    "=============================================="
);

Console.WriteLine(
    "Mode: Simulation / SDK Integration Required"
);

Console.WriteLine(
    "Waiting for browser connection..."
);

Console.WriteLine(
    "=============================================="
);

app.Run();


// ============================================================
// REQUEST MODELS
// ============================================================

public record EcuScanRequest(
    string? Ecu
);

public record FirmwareRequest(
    string? File
);


// ============================================================
// VCI HARDWARE INTERFACE
// ============================================================

public interface IVciHardwareService
{
    bool IsConnected { get; }

    string DeviceName { get; }

    string Mode { get; }

    Task<bool> ConnectAsync();

    Task DisconnectAsync();

    Task<object> IdentifyVehicleAsync();

    Task<object> FullScanAsync();

    Task<object> ScanEcuAsync(
        string ecu
    );

    Task<object> ReadDtcAsync();

    Task<object> ClearDtcAsync();

    Task<object> GetLiveDataAsync();

    Task<object> VerifyFirmwareAsync(
        string file
    );

    Task<object> ProgramFirmwareAsync(
        string file
    );
}


// ============================================================
// VCI HARDWARE SERVICE
// ============================================================

public class VciHardwareService
    : IVciHardwareService
{
    public bool IsConnected
    {
        get;
        private set;
    }

    public string DeviceName
    {
        get;
        private set;
    } = "NOT CONNECTED";

    public string Mode
    {
        get;
        private set;
    } = "Simulation";


    // ========================================================
    // CONNECT
    // ========================================================

    public async Task<bool> ConnectAsync()
    {
        await Task.Delay(500);

        // ----------------------------------------------------
        // TODO:
        // Replace this section with authorized VCI SDK code.
        //
        // Example:
        //
        // var devices = VciSdk.GetDevices();
        // var device = devices.FirstOrDefault();
        // device.Open();
        //
        // ----------------------------------------------------

        IsConnected = true;

        DeviceName =
            "DEEPAK DABI VCI DEMO";

        Mode =
            "Simulation";

        return true;
    }


    // ========================================================
    // DISCONNECT
    // ========================================================

    public async Task DisconnectAsync()
    {
        await Task.Delay(200);

        // TODO:
        // Close real VCI SDK connection here.

        IsConnected = false;

        DeviceName =
            "NOT CONNECTED";
    }


    // ========================================================
    // VEHICLE IDENTIFY
    // ========================================================

    public async Task<object>
        IdentifyVehicleAsync()
    {
        await Task.Delay(700);

        return new
        {
            success = true,

            model =
                "HERO VIDA EV",

            vin =
                "DEMO-VIN-VCI",

            manufacturer =
                "HERO",

            protocol =
                "VCI",

            mode =
                Mode
        };
    }


    // ========================================================
    // FULL SCAN
    // ========================================================

    public async Task<object>
        FullScanAsync()
    {
        await Task.Delay(1200);

        var ecus =
            new[]
            {
                new
                {
                    ecu = "TCU",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "VCU",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "MCU",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "EHL",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "BMS-1",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "BMS-1B",
                    status = "OK",
                    dtc = 0
                },

                new
                {
                    ecu = "BMS-1X",
                    status = "OK",
                    dtc = 0
                }
            };

        return new
        {
            success = true,

            scanType =
                "FULL VEHICLE SCAN",

            totalEcus =
                ecus.Length,

            totalDtc =
                0,

            ecus,

            timestamp =
                DateTime.Now,

            mode =
                Mode
        };
    }


    // ========================================================
    // ECU SCAN
    // ========================================================

    public async Task<object>
        ScanEcuAsync(
            string ecu
        )
    {
        await Task.Delay(500);

        return new
        {
            success = true,

            ecu =
                ecu.ToUpper(),

            status =
                "OK",

            dtcCount =
                0,

            dtcs =
                Array.Empty<string>(),

            mode =
                Mode
        };
    }


    // ========================================================
    // READ DTC
    // ========================================================

    public async Task<object>
        ReadDtcAsync()
    {
        await Task.Delay(500);

        return new
        {
            success = true,

            count =
                0,

            dtcs =
                Array.Empty<string>(),

            mode =
                Mode
        };
    }


    // ========================================================
    // CLEAR DTC
    // ========================================================

    public async Task<object>
        ClearDtcAsync()
    {
        await Task.Delay(500);

        return new
        {
            success = true,

            message =
                "DTC clear request completed.",

            mode =
                Mode
        };
    }


    // ========================================================
    // LIVE DATA
    // ========================================================

    public async Task<object>
        GetLiveDataAsync()
    {
        await Task.Delay(300);

        return new
        {
            success = true,

            soc =
                82,

            voltage =
                52.4,

            current =
                0.0,

            rpm =
                0,

            speed =
                0,

            batteryTemperature =
                28,

            motorTemperature =
                30,

            mode =
                Mode,

            timestamp =
                DateTime.Now
        };
    }


    // ========================================================
    // FIRMWARE VERIFY
    // ========================================================

    public async Task<object>
        VerifyFirmwareAsync(
            string file
        )
    {
        await Task.Delay(500);

        return new
        {
            success = true,

            file =
                file,

            compatible =
                true,

            message =
                "Firmware metadata verification completed.",

            mode =
                Mode
        };
    }


    // ========================================================
    // FIRMWARE PROGRAM
    // ========================================================

    public async Task<object>
        ProgramFirmwareAsync(
            string file
        )
    {
        await Task.Delay(500);

        // IMPORTANT:
        // Real flashing must NOT be enabled here until
        // the authorized VCI SDK/API and correct vehicle
        // programming sequence are integrated.

        return new
        {
            success =
                false,

            file =
                file,

            message =
                "Real firmware programming is disabled. Integrate the authorized VCI SDK/API before enabling flashing.",

            mode =
                Mode
        };
    }
}
