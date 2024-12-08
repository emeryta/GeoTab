namespace GeoTab.Models
{
    internal class DeviceToCsv
    {
        public string Id = String.Empty;
        public string Name = String.Empty;

        public string Coordinates = String.Empty;
        public string Odometer = String.Empty;
        public string VIN = String.Empty;
        public string TimeStamp = String.Empty;

        public void WriteDataToCsv()
        {
            string csvFileName = "DeviceBackup - " + Name+".csv";
            string csvPath = "GeoTabBackups";
            string downloadFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + csvPath;

            if (!Directory.Exists(downloadFolderPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(downloadFolderPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating backup folder: {ex.Message}");
                }
            }

            string downloadFilePath = Path.Combine(downloadFolderPath, Path.GetFileName(csvFileName));
            if (!File.Exists(downloadFilePath))
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(downloadFilePath))
                    {
                        writer.WriteLine("Id, Timestamp, VIN, Coordinates, Odometer");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while writing the header to the file: {ex.Message}");
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(downloadFilePath, append: true))
                {
                    writer.WriteLine($"{Id}, {TimeStamp}, {VIN}, {Coordinates}, {Odometer}");

                }

                Console.WriteLine(TimeStamp + " Data written to CSV file. - " + Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }
    }
}
