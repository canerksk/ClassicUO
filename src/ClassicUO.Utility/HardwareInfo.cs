using System.Management;
using System.IO;

public static class HardwareInfo
{

    public static string getUniqueID(string drive)
    {
        if (drive == string.Empty)
        {
            foreach (DriveInfo compDrive in DriveInfo.GetDrives())
            {
                if (compDrive.IsReady)
                {
                    drive = compDrive.RootDirectory.ToString();
                    break;
                }
            }
        }

        if (drive.EndsWith(":\\"))
        {
            //C:\ -> C
            drive = drive.Substring(0, drive.Length - 2);
        }

        string volumeSerial = getVolumeSerial(drive);
        string cpuID = getCPUID();
        return cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
    }

    public static string GetHDDID()
    {
        ManagementObject disk = new ManagementObject("Win32_LogicalDisk.DeviceID='C:'");
        string HDDID = disk.GetPropertyValue("VolumeSerialNumber").ToString();
        return HDDID;
    }

    public static string getVolumeSerial(string drive)
    {
        ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
        disk.Get();

        string volumeSerial = disk["VolumeSerialNumber"].ToString();
        disk.Dispose();

        return volumeSerial;
    }

    public static string getCPUID()
    {
        string cpuInfo = "";
        ManagementClass managClass = new ManagementClass("win32_processor");
        ManagementObjectCollection managCollec = managClass.GetInstances();

        foreach (ManagementObject managObj in managCollec)
        {
            if (cpuInfo == "")
            {
                //Get only the first CPU's ID
                cpuInfo = managObj.Properties["processorID"].Value.ToString();
                break;
            }
        }

        return cpuInfo;
    }
}



