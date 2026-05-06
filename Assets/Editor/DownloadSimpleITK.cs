// Name: Paul Lewis Marcos
// Date: February 20, 2026
// Assignment: CS 152 Project - Programming Paradigms
// Description: Provides Unity Editor menu items to download, enable, and check the status
//              of the SimpleITK medical imaging library used for DICOM processing.

using UnityEditor;
using UnityVolumeRendering;

public class DownloadSimpleITK
{
        // Menu item to download the SimpleITK into the project
    [MenuItem("CS152/Download SimpleITK")]
    public static void Download()
    {
        SimpleITKManager.DownloadBinaries();
    }
        // Menu item to enable SimpleITK after it's been downloaded
    [MenuItem("CS152/Enable SimpleITK")]
    public static void Enable()
    {
        SimpleITKManager.EnableSITK(true);
    }
    // Menu item to check the status of SimpleITK
    [MenuItem("CS152/Check SimpleITK Status")]
    public static void CheckStatus()
    {
        bool hasDownloaded = SimpleITKManager.HasDownloadedBinaries();
        bool isEnabled = SimpleITKManager.IsSITKEnabled();
        

         // Print both statuses to the console for debugging
        UnityEngine.Debug.Log("SimpleITK Downloaded: " + hasDownloaded);
        UnityEngine.Debug.Log("SimpleITK Enabled: " + isEnabled);
    }
}