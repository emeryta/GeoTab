// See https://aka.ms/new-console-template for more information
using Geotab.Checkmate.ObjectModel;
using Geotab.Checkmate;
using GeoTab.Models;
using Geotab.Checkmate.ObjectModel.Engine;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        Console.WriteLine("Waiting for next iteration...");

        while (await timer.WaitForNextTickAsync())
        {
            #region process
            Console.WriteLine("Iterate " + DateTime.Now.ToString("yyyyMMdd_HHmmssffff"));

            var api = new API("maria.rius08@gmail.com", "geotabpass", null, "demo_candidates_net", "mypreview.geotab.com");
            await api.AuthenticateAsync();

            try
            {
                IList<Device> devices = await api.CallAsync<IList<Device>>("Get", typeof(Device), new { search = new DeviceSearch { Groups = new List<GroupSearch> { new GroupSearch(KnownId.GroupVehicleId) } } }) ?? new List<Device>();
                foreach (var device in devices)
                {
                    DeviceToCsv deviceToCsv = new DeviceToCsv()
                    {
                        Id = device.Id?.ToString() ?? String.Empty,
                        Name = device.Name ?? String.Empty,
                        TimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmssffff")
                    };

                    #region VIN
                    try
                    {
                        deviceToCsv.VIN = ((XDevice)device).VehicleIdentificationNumber ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while retrieving VIN: {ex.Message}");
                    }
                    #endregion

                    #region odometer
                    try
                    {
                        IList<StatusData> statusData = await api.CallAsync<IList<StatusData>>("Get", typeof(StatusData), new
                        {
                            search = new StatusDataSearch
                            {
                                DeviceSearch = new DeviceSearch(device.Id),
                                DiagnosticSearch = new DiagnosticSearch(KnownId.DiagnosticOdometerAdjustmentId),
                                FromDate = DateTime.MaxValue
                            }
                        });
                        deviceToCsv.Odometer = Math.Truncate((double)statusData[0].Data / 1000).ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while retrieving Odometer: {ex.Message}");
                    }
                    #endregion

                    #region coordinates
                    try
                    {
                        IList<DeviceStatusInfo> deviceStatusInfo = await api.CallAsync<List<DeviceStatusInfo>>("Get", typeof(DeviceStatusInfo), new
                        {
                            search = new DeviceStatusInfoSearch
                            {
                                DeviceSearch = new DeviceSearch
                                {
                                    Id = device.Id
                                }
                            }
                        });
                        deviceToCsv.Coordinates = "Lng " + deviceStatusInfo[0].Longitude.ToString().Replace(',', '.') + " Lat " + deviceStatusInfo[0].Latitude.ToString().Replace(',', '.');
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while retrieving Coordinates: {ex.Message}");
                    }
                    #endregion

                    try
                    {
                        deviceToCsv.WriteDataToCsv();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while downloading the file: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        #endregion
    }
}
