#include <iostream>
#include <string>
#include <vector>
#include <cmath>
#include <thread>
#include <chrono>

void AFCam_RunScanLoc(double Target, int bUpdateXY = 1, int Level = 0, bool bMoveToTarget = true) 
{
   // ESP32_Device.AKF_Set(1); // Turn LED on
   // ESP32_Device.Lamp_Set(0); // Block Lamp

    bool bQuickScan = true;

    //if (AFAbort) {
    //    return; // Exit with error code 2
    //}

    int Ret;
    double OldPos;

   /* if (Level == 0) {
        double AFM_InitialStep = AFCam_Scan_Step;
        double AFM_InitialRange = AFCam_Scan_Range;
    }*/

    //TimeDifference.Start(ti);

    //Plot remove all points logic

    //double Range = AFCam_Scan_Range;
    double H=0.0, t;
    int Count, Expected, k;

    //Scan_GetPos(H, k);

    OldPos = H;
    double PosFrom = H - Range / 2;
    double PosTo = H + Range / 2;

    if (PosFrom < Align_Axis_H_Min) {
        PosFrom = Align_Axis_H_Min;
        PosTo = PosFrom + Range;
    }
    if (PosTo > Align_Axis_H_Max) {
        PosTo = Align_Axis_H_Max;
        PosFrom = PosTo - Range;
    }
    if (PosFrom < Align_Axis_H_Min) {
        PosFrom = Align_Axis_H_Min;
    }
    if (PosTo > Align_Axis_H_Max) {
        PosTo = Align_Axis_H_Max;
    }

    H = (PosFrom + PosTo) / 2;
    int Steps = static_cast<int>(Range / AFCam_Scan_Step);

    AFCam_Steps = Steps;
    AFCam_PosFrom = PosFrom;
    AFCam_PosTo = PosTo;

    std::string s;
    s += "$Prefix(1)$";
    s += "Autofocus scan from " + std::to_string(PosFrom) + " mm to " + std::to_string(PosTo) + " mm ";
    s += "with " + std::to_string(Steps) + " steps started ";
    s += "[Range is " + std::to_string(PosTo - PosFrom) + " mm, ";
    s += std::to_string(AFCam_Scan_Step * 1000) + " µm per step, Exp: ";
    s += std::to_string(AFCam_Scan_ExposureUs) + " µs, Gain: ";
    s += std::to_string(AFCam_Scan_Gain) + "%] ...";

    AddLog(s); //send s to log

    Expected = Steps;





}

void AFCam_RunScan(double Target, int bUpdateXY = 1, int Level = 0, bool bMoveToTarget = true) 
{
    int DlgW = 200;
    int DlgH = 115;
    int BtnW = 40;
    int BtnH = 20;
    int PBH = 10;
    int Space = 5;

    int AFResult = 0;
    bool AFAbort = false;
    void* hwndAFProgress = nullptr; // Placeholder for UI handle

    //New dialog will appear
    
    //Caption Autofocus
    //OnInit
    //OnTimer
    //ID101
    //hwndMain
       /* AFCam.RunScanLoc Target bUpdateXY 0 bMoveToTarget
        AFResult = result*/
    //ID100
    //OnDone
        //SavePos WndAutofocus


}


int main()
{
    std::cout << "Hello World!\n";

    AFCam_RunScan(10.5);


    return 0;
}
