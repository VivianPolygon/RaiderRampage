using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenShotMenu : MonoBehaviour
{
    //Creates a menu item named "Screenshot" with the drop down "Take Screenshot" 
    //where you can then select multiplier of game window resolution (x1,x2,x3,x4)

    //Taking a screenshot will create a new folder in your project file location named "Screenshots"
    //Screenshot saves to "Screenshots" folder with file name "ScreenshotXXX_Width_Height.jpg

    [MenuItem("Screenshot/Take Screenshot/Resolution x 1", false, 1)]
    public static void Grabx1()
    {
        string filePath = System.IO.Directory.GetCurrentDirectory() + "/Screenshots/";
        string screenShotName = "Screenshot" + GetCh() + GetCh() + GetCh() + "_" + Screen.width + "x" + Screen.height + ".jpg";

        //Creates a folder named "ScreenShots
        if(!System.IO.Directory.Exists("Screenshots"))
        {
            System.IO.Directory.CreateDirectory("Screenshots");
        }
        //Captures screenshot with above file name and puts it into the screenshots folder
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(filePath,screenShotName), 1);
    }

    [MenuItem("Screenshot/Take Screenshot/Resolution x 2", false, 2)]
    public static void Grabx2()
    {
        string filePath = System.IO.Directory.GetCurrentDirectory() + "/Screenshots/";
        string screenShotName = "Screenshot" + GetCh() + GetCh() + GetCh() + "_" + Screen.width * 2 + "x" + Screen.height * 2 + ".jpg";

        //Creates a folder named "ScreenShots
        if (!System.IO.Directory.Exists("Screenshots"))
        {
            System.IO.Directory.CreateDirectory("Screenshots");
        }
        //Captures screenshot with above file name and puts it into the screenshots folder
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(filePath, screenShotName), 2);
    }

    [MenuItem("Screenshot/Take Screenshot/Resolution x 3", false, 3)]
    public static void Grabx3()
    {
        string filePath = System.IO.Directory.GetCurrentDirectory() + "/Screenshots/";
        string screenShotName = "Screenshot" + GetCh() + GetCh() + GetCh() + "_" + Screen.width * 3 + "x" + Screen.height * 3 + ".jpg";

        //Creates a folder named "ScreenShots
        if (!System.IO.Directory.Exists("Screenshots"))
        {
            System.IO.Directory.CreateDirectory("Screenshots");
        }
        //Captures screenshot with above file name and puts it into the screenshots folder
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(filePath, screenShotName), 3);
    }

    [MenuItem("Screenshot/Take Screenshot/Resolution x 4", false, 4)]
    public static void Grabx4()
    {
        string filePath = System.IO.Directory.GetCurrentDirectory() + "/Screenshots/";
        string screenShotName = "Screenshot" + GetCh() + GetCh() + GetCh() + "_" + Screen.width * 4 + "x" + Screen.height * 4 + ".jpg";

        //Creates a folder named "ScreenShots
        if (!System.IO.Directory.Exists("Screenshots"))
        {
            System.IO.Directory.CreateDirectory("Screenshots");
        }
        //Captures screenshot with above file name and puts it into the screenshots folder
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(filePath, screenShotName), 4);
    }

    //Picks random letter for file name to prevent duplicate file names
    public static char GetCh()
    {
        return (char)UnityEngine.Random.Range('A', 'Z');
    }
}