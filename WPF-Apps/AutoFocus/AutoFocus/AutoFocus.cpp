#include <iostream>
#include <string>
#include <vector>
#include <cmath>
#include <thread>
#include <chrono>


//Start CalContrast func

void AFCam_CalContrast_t(std::string ArrName, double& Target, double& HighestPeak, double& Noise, double& Minvalue, double& FWHM, bool bVerticalDir = true, double& Width, int TargetFrame = 0) {
    int Count, w, h, Num;
    double Frame;

    Camera_Pylon_StopCapture(IDAFCam);
    Camera_Pylon_FrameBuffer_GetCount(IDAFCam, Count);
    Camera_Pylon_FrameBuffer_GetWidth(IDAFCam, w);
    Camera_Pylon_FrameBuffer_GetHeight(IDAFCam, h);

    int l = 0, t = 0, r = w - 1, b = h - 1;
    AutoFocus_GetScanRegion(1, l, t, r, b);

    if (Count == 0) {
        return; // Exit with error code 1, no images
    }

    // Initialize array for contrast processing
    //ArrayDbl_Init(ArrName, Count, 6);
    double* ArrName = new double(6);

    if (bUpdateXY) {
        IDXYScan.removeAllPoints("IDCurveContrast_l");
        IDXYScan.removeAllPoints("IDCurveIntensity_l");
        IDXYScan.removeAllPoints("IDCurveCursor_l");
        IDXYScan.removeAllPoints("IDCurveCursor_2");
    }
    PeakSearch_Handle ps = PeakSearch_Create();
    double x, Contrast, Intensity, Theory, Difference, Brightness, framecur;
    int CompareLen = ((r + l) / 2 > AFCam_Crosshair_CenterX) ? AFCam_Scan_CompareLen_Right : AFCam_Scan_CompareLen;
    bool bShowReadoutArea = false;

    for (int i = 0; i < Count; i++) 
    {
        if (AFCam_bHistogram) {
            Camera_Pylon_FrameBuffer_GetCellContrastHistogram(IDAFCam, i, l, t, r, b, Contrast, Intensity, Brightness);
        }
        else {
            Camera_Pylon_FrameBuffer_GetCellContrast(IDAFCam, i, l, t, r, b, 0, Contrast, Intensity, CompareLen, Brightness);
        }
        Camera_Pylon_FrameBuffer_GetFrameNumber(IDAFCam, i, Num);

        if (i == 0) {
            Num = 0;
        }

        ArrayDbl_SetX(ArrName, i, Num);
        ArrayDbl_SetY(ArrName, i, 0, Contrast);
        ArrayDbl_SetY(ArrName, i, 1, Intensity);
        ArrayDbl_SetY(ArrName, i, 2, Brightness);
        ArrayDbl_SetY(ArrName, i, 3, Theory);
        ArrayDbl_SetY(ArrName, i, 4, Difference);

        x = PosFrom + i / (Steps) * (PosTo - PosFrom);
        if (bUpdateXY) {
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveContrast_l", x, Contrast);
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveIntensity_l", x, Brightness);
        }

        PeakSearch_AddPoint(ps, Contrast);
    }

    if (bShowReadoutArea) 
    {
        Camera_Pylon_FrameBuffer_GetCellContrast(IDAFCam, -1, l, t, r, b, 1, Contrast, Intensity, CompareLen, Brightness);
        Camera_Pylon_FrameBuffer_Goto(IDAFCam, Count - 1);
        Camera_Pylon_Display_Update(IDAFCam);
        std::this_thread::sleep_for(std::chrono::milliseconds(2000));
    }

    if (AFCam_bHistogram) {
        int rows;
        double c, cl, cr, cmax = 0, posm = 0, cmin = 1e10;
        ArrayDbl_GetRows(ArrName, rows);
        ArrayDbl_GetY(ArrName, 0, 0, cl);
        ArrayDbl_GetY(ArrName, rows - 1, 0, cr);

        for (int i = 0; i < rows; i++) {
            ArrayDbl_GetY(ArrName, i, 0, c);
            if (c > cmax) {
                cmax = c;
                posm = i;
            }
            if (c < cmin) {
                cmin = c;
            }
        }

        if (cmax < cl) return;
        if (cmax < cr) return;
        if ((cmax / cl) < 1.1) return;
        if ((cmax / cr) < 1.1) return;

        Target = PosFrom + posm / Steps * (PosTo - PosFrom);
        TargetFrame = posm;
        HighestPeak = cmax;
        Noise = 1;
        Minvalue = cmin;
        FWHM = 1;
        return;
    }
    PeakSearch_Handle ps = PeakSearch_Create();
    PeakSearch_SetPeakRatio(ps, 0.05);
    PeakSearch_SetPeakFitPoints(ps, 12);
    PeakSearch_FindPeaks(ps);
    int Count2;
    PeakSearch_GetNumPeaks(ps, Count2);
    double framecur = 0;

    double AFPeak1Position = 0, AFPeak2Position = 0;
    bool AFPeak1Valid = false, AFPeak2Valid = false, AFPeak3Valid = false;
    double w, width = 0;

    if (Count2 > 0) 
    {
        double y, ymax = 0;
        for (int i = 0; i < Count2; i++) {
            PeakSearch_GetPeak(ps, i, framecur, y, w);
            if (framecur > Frame) {
                ymax = y;
                Frame = framecur;
                TargetFrame = Frame;
                Width = w / Count * (PosTo - PosFrom);
            }
            if (i == Count2 - 1) {
                AFPeak1Position = framecur;
                AFPeak1Valid = true;
                TargetFrame = framecur;
            }
            if (i == Count2 - 2) {
                AFPeak2Position = framecur;
                AFPeak2Valid = true;
                TargetFrame = framecur;
            }
            if (i == Count2 - 3) {
                AFPeak3Valid = true;
                TargetFrame = framecur;
            }
        }
    }

    PeakSearch_GetNoiseLevel(ps, Noise);
    HighestPeak = ymax;

    double avg, sigma, Amin, Amax;
    ArrayDbl_GetStatistics(ArrName, 0, avg, sigma, Amin, Amax);
    Minvalue = Amin;

    if (bUpdateXY) 
    {
        if (AFMode == AFMode_P1_Target && AFPeak1Valid) {
            double x = PosFrom + AFPeak1Position / Steps * (PosTo - PosFrom);
            Target = x;
            AFPeak1Position = Target;
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100);
        }
        if (AFMode == AFMode_P2_Target && AFPeak2Valid) {
            double x = PosFrom + AFPeak2Position / Steps * (PosTo - PosFrom);
            Target = x;
            AFPeak2Position = Target;
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100);
        }
        if (AFMode == AFMode_P3_Target && AFPeak3Valid) {
            double x = PosFrom + AFPeak3Position / Steps * (PosTo - PosFrom);
            Target = x;
            AFPeak3Position = Target;
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
            XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100);
        }
        if (AFMode == AFMode_P1_Target_Dist12)
        {
            if (AFPeak1Valid==true && AFPeak2Valid=true)
            {
                double x;
                x = PosFrom + AFPeak1Position / Steps * (PosTo - PosFrom);
                Target = x;
                AFPeak1Position = Target;

                double x2;
                x2 = PosFrom + AFPeak2Position / Steps * (PosTo - PosFrom);

                AFPeak2Position = x2;
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x2, 0);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x2, 100);
            }
            else
            {
                if (AFPeak1Valid)
                {
                    x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom);
                    Target = x;
                    AFPeak1Position = Target;
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100); // also eine vertikale Linie
                }
            }
            
        }

        if (AFMode == AFMode_P2_Target_Dist12)
        {
            if ((AFPeak1Valid == True) && (AFPeak2Valid == True))
            {
                x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom);
                AFPeak1Position = x;
                
                double x2;
                x2 = PosFrom + AFPeak2Position / Steps * (PosTo - Posfrom);
                Target = x2;
                AFPeak2Position = x2;

                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l ", x, 100); // also eine vertikale Linie

                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x2, 0);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x2, 100); // also eine vertikale Linie
            }
            else
            {
                if (AFPeak1Valid)
                {
                    x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom);
                    Target = x;
                    AFPeak1Position = Target;
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100); // also eine vertikale Linie
                }
                   
            }
        }

        if (AFMode == AFMode_P3_Target_Dist13)
        {
            if ((AFPeak1Valid == True) && (AFPeak3Valid == True))
            {
                x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom);
                AFPeak1Position = x;

                double x3;
                x3 = PosFrom + AFPeak3Position / Steps * (PosTo - Posfrom);
                Target = x3;
                AFPeak3Position = x3;

                XYPlot_Curve_AddPoint(IDXYScan,IDCurveCursor_l ,x, 0);
                XYPlot_Curve_AddPoint(IDXYScan,IDCurveCursor_l ,x, 100);// also eine vertikale Linie
                                              
                XYPlot_Curve_AddPoint(IDXYScan,IDCurveCursor_2 ,x3 , 0);
                XYPlot_Curve_AddPoint(IDXYScan,IDCurveCursor_2 ,x3 , 100); // also eine vertikale Linie
            }
            else
            {
                if (AFPeak1Valid)
                {
                    x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom);
                    Target = x;
                    AFPeak1Position = Target;
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 0);
                    XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100); // also eine vertikale Linie
                }
                   
            }
        }

        if(AFMode == AFMode_P3_Target_Dist23)
        {
            if ((AFPeak2Valid == True) && (AFPeak3Valid == True))
            {
                x = PosFrom + AFPeak2Position / Steps * (PosTo - Posfrom);
                AFPeak2Position = x;

                double x3;
                x3 = PosFrom + AFPeak3Position / Steps * (PosTo - Posfrom);
                Target = x3;
                AFPeak3Position = x3;

                XYPlot_Curve_AddPoint( IDXYScan, "IDCurveCursor_l", x, 0);
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_l", x, 100);// also eine vertikale Linie

                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x3, 0); // also eine vertikale Linie
                XYPlot_Curve_AddPoint(IDXYScan, "IDCurveCursor_2", x3, 100); // also eine vertikale Linie
            }
            else
            {
                if (AFPeak1Valid)
                {
                    x = PosFrom + AFPeak1Position / Steps * (PosTo - Posfrom)
                    Target = x;
                    AFPeak1Position = Target;
                    XYPlot_Curve_AddPoint( IDXYScan, "IDCurveCursor_l", x, 0);
                    XYPlot_Curve_AddPoint( IDXYScan, "IDCurveCursor_l", x, 100); // also eine vertikale Linie
                }
                   
            }
        }

        PeakSearch.Delete ps; //delete ps;
        if (bUpdateXY)
        {
            XYPlot_update(IDXYScan, 1);
        }
        
        Camera_Pylon_FrameBuffer_GetRawPixelAverage( IDAFCam, Frame, t, l, b, r, g_AFSignal);
        return 0;
    }
    else
    {
        PeakSearch.Delete ps; //delete ps;
        if (bUpdateXY)
        {
            XYPlot_update(IDXYScan, 1);
        }
               
    }

}
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
    double H = 0.0, t;
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

    std::string th, th2;
    int bRunning;

    // Running autofocus scan setup
    AFCam_MovedownThreadProc(PosFrom);
    AFCam_SetupScan(Expected + 20);

    // Configure camera settings
    Camera_Pylon_Feature_SetEnum(IDAFCam, "SensorReadoutMode", 1); // 0 Normal, 1 Fast

    // Start image capture
    Camera_Pylon_StartCapture(IDAFCam);
    std::this_thread::sleep_for(std::chrono::milliseconds(100));

    // Update UI progress dialog
    Dlg_Control_SetText(200, "Scanning ...");

    // Frame buffer handling
    int Num, n;
    Camera_Pylon_FrameBuffer_GetCount(IDAFCam, Num);
    n = Num;

    std::string sti;
    double ms;
    TimeDifference.CreateGetName(sti);

    ProgressBar_SetRange(201, 0, Steps);
    ProgressBar_SetPos(201, 0);
    ProgressBar_SetState(201, "Normal");

    for (int i = 0; i <= Steps; i++) {
        ProgressBar_SetPos(201, i);
        if (i % 10 == 0) {
            std::string scanStatus = "Scanning level " + std::to_string(Level + 1) + ":0\n\nFrame " +
                std::to_string(i) + " of " + std::to_string(Steps) + "\n\nGain = " +
                std::to_string(AFCam_Scan_Gain) + " % / Exposure = " +
                std::to_string(AFCam_Scan_ExposureUs) + " µs";
            Dlg_Control_SetText(200, scanStatus);
        }

        if (AFAbort) {
            Camera_Pylon_StopCapture(IDAFCam);
            AFCam_SetupView();
            AFCam_Scan_GotoPos(OldPos);
            if (bUpdateXY) {
                XYPlot_SetText(IDXYScan, "BottomSubtitle", "FAILED User Abort");
                XYPlot_SetText(IDXYScan, "Subtitle", "AF: Old Target set: " + std::to_string(H) + " mm");
                XYPlot_Update(IDXYScan);
            }
            Target = OldPos;
            AddLog("$Prefix(1)$Autofocus scan failed: user abort");
            TimeDifference.Free(sti);
            return;
        }

        TimeDifference.Start(sti);
        Camera_Pylon_Feature_Execute(IDAFCam, "TriggerSoftware");

        if (IDAFCam.Scan.Step > 2)
        {
            bQuickScan = false;
        }

        if (bQuickScan)
        {
            double waitms = AFCam.Scan.ExposureUs / 1000;
            double waitmsmin = 40;

            if (waitms < waitmsmin)
            {
                waitms = waitmsmin
            }
            AFCam_Scan_GotoPos_NoWait(PosFrom + i * (PosTo - PosFrom) / Steps);
            std::this_thread::sleep_for(std::chrono::milliseconds(static_cast<int>(waitms)));
        }
        else
        {
            while (n == Num)
            {
                Camera_Pylon_FrameBuffer_GetCount(IDAFCam, n);
                TimeDifference.GetMs(sti, ms);
                if (ms > 500) {
                    Camera_Pylon_StopCapture(IDAFCam);
                    TimeDifference.Free(sti);
                    AddLog("$prefix(1)$Autofocus scan received no images, restarting.");
                    i--; // Retry current step
                    continue;
                }
                std::this_thread::sleep_for(std::chrono::milliseconds(1));
            }
            Num = n;
            AFCam_Scan_GotoPos(PosFrom + i * (PosTo - PosFrom) / Steps);
        }

        TimeDifference.Free(sti);
        Camera_Pylon_StopCapture(IDAFCam);
        Camera_Pylon_FrameBuffer_GetCount(IDAFCam, Count);

        if (std::abs(Count - Expected) > 3) {
            AFCam_SetupView();
            AFCam_Scan_GotoPos(H);
            if (bUpdateXY) {
                XYPlot_SetText(IDXYScan, "BottomSubtitle", "FAILED Count = " + std::to_string(Count) + "; Expected = " + std::to_string(Expected) + "; Range = " + std::to_string(PosTo - PosFrom) + " mm; Step = " + std::to_string(AFCam_Scan_Step * 1000) + " µm");
                XYPlot_SetText(IDXYScan, "Subtitle", "AF: Old Target set: " + std::to_string(H) + " mm");
                XYPlot_Update(IDXYScan);
            }
            Target = OldPos;
            AddLog("$Prefix(1)$Autofocus scan failed: not enough frames received (" + std::to_string(Count) + " of " + std::to_string(Expected) + ")");
            return;
        }

        int Ret;
        double OldPos, Frame, pospercent, HighestPeak, Noise, g_AFSNR, Minvalue, FWHM, Width;

        //Contrast calculation
        Ret = AFCam_CalContrast_t(A0, Target, HighestPeak, Noise, Minvalue, FWHM, true, Width);

        if (Ret == 0)
        {
            g_AFWidth = Width;
        }
        else
        {
            g_AFWidth = 0;
        }

        if (bMoveToTarget)
        {
            AFCam_SetupView;
        }
        else
        {
            return;
        }

        double av, sig, min, max;
        //ArrayDbl_GetStatistics(A0, 2, av, sig, min, max);
        double* GetStatistics;
        double SignalLevel = (max / 255.0) * 100.0; // Convert to percentage

        if (Max < 20 || Max>250)
        {
            double PrevGain = AFCam.Scan.Gain;
            int PrevExpUs = AFCam.Scan.ExposureUs;
            if (Level < 10)
            {
                // Underflow Handling
                if (max < 40)
                {
                    if (AFCam_Scan_ExposureUs < AFCam_View_AutoExposureMax) {
                        AFCam_Scan_ExposureUs *= 4;
                        if (AFCam_Scan_ExposureUs > AFCam_View_AutoExposureMax) {
                            AFCam_Scan_ExposureUs = AFCam_View_AutoExposureMax;
                        }
                        AddLog("$Prefix(1)$Underflow: Increase exposure, retrying ...");
                    }
                    else {
                        if (AFCam_Scan_Gain < 80) {
                            AFCam_Scan_Gain = (AFCam_Scan_Gain == 0) ? 10 : AFCam_Scan_Gain * 2;
                            if (AFCam_Scan_Gain > 80) {
                                AFCam_Scan_Gain = 80;
                            }
                            AddLog("$Prefix(1)$Underflow: Increase gain, retrying ...");
                        }
                        else {
                            AddLog("$Prefix(1)$Underflow with largest gain. No light, failed.");
                            return;
                        }
                    }
                    AFCam_MoveTargetThreadProc(OldPos);
                    AFCam_RunScanLoc(Target, bUpdateXY, Level + 1, bMoveToTarget);
                    return;
                }
                // Overflow Handling
                if (max > 250)
                {
                    if (AFCam_Scan_Gain > 4) {
                        AFCam_Scan_Gain /= 4;
                        AddLog("$Prefix(1)$Overflow: Decrease gain, retrying ...");
                    }
                    else {
                        if (AFCam_Scan_ExposureUs > AFCam_View_AutoExposureMin) {
                            AFCam_Scan_ExposureUs = AFCam_View_AutoExposureMin + (AFCam_Scan_ExposureUs - AFCam_View_AutoExposureMin) / 4;
                            AddLog("$Prefix(1)$Overflow: Decrease exposure, retrying ...");
                        }
                        else {
                            AddLog("$Prefix(1)$Overflow with smallest exposure time. Too much light, failed.");
                            return;
                        }
                    }
                    AFCam_MoveTargetThreadProc(OldPos);
                    AFCam_RunScanLoc(Target, bUpdateXY, Level + 1, bMoveToTarget);
                    return;
                }
            }
        }

        if (ret != 0)
        {
            double av, sig, min, max;
            // ArrayDbl_GetStatistics(A0, 0, av, sig, min, max);
            double* GetStatistics;
            if (!AFCam_bHistogram)
            {
                if (max - min > Noise * 4)
                {
                    int rows, cn;
                    double suml = 0, sumr = 0, summ = 0, v;
                    // ArrayDbl_GetRows(A0, rows);
                    double* GetRows;
                    cn = rows / 10;
                    if (cn > 3)
                    {
                        suml = 0;
                        for (int i = 0; i < cn; i++) {
                            //ArrayDbl_GetY(A0, i, 0, v);
                            double* GetY;
                            suml += v;
                        }
                        suml /= cn;

                        sumr = 0;
                        for (int i = rows - cn; i < rows; i++) {
                            //ArrayDbl_GetY(A0, i, 0, v);
                            double* GetY;
                            sumr += v;
                        }
                        sumr /= cn;

                        summ = 0;
                        for (int i = cn; i < rows - cn; i++) {
                            //ArrayDbl_GetY(A0, i, 0, v);
                            double* GetY;

                            summ += v;
                        }
                        summ /= (rows - cn * 2);

                        if (suml > summ && suml < sumr)
                        {
                            if ((2 * suml / (summ + sumr)) > 1.5)
                            {
                                AFCam_MoveTargetThreadProc(PosFrom);
                                AddLog("$Prefix(1)$Left side could have a peak, moving to lower z-pos, retrying...");
                                AFCam_RunScanLoc(Target, bUpdateXY, Level + 1, bMoveToTarget);
                                return;
                            }
                        }

                        if ((sumr > summ) && (sumr > suml))
                        {
                            if ((2 * sumr / (summ + suml)) > 1.5)
                            {
                                AFCam_MoveTargetThreadProc(PosTo);
                                AddLog("$Prefix(1)$Right side could have a peak, moving to upper z-pos, retrying...");
                                AFCam_RunScanLoc(Target, bUpdateXY, Level + 1, bMoveToTarget);
                                return;
                            }
                        }

                        if (Range < 20) {
                            double oldrange = AFCam_Scan_Range, oldstep = AFCam_Scan_Step;
                            AFCam_Scan_Range *= 2;
                            AFCam_Scan_Step = AFCam_Scan_Range / 200;
                            if (AFCam_Scan_Step < AFCam_Scan_StepMin) {
                                AFCam_Scan_Step = AFCam_Scan_StepMin;
                            }
                            if (AFCam_Scan_Step > AFCam_Scan_StepMax) {
                                AFCam_Scan_Step = AFCam_Scan_StepMax;
                            }
                            AddLog("$Prefix(1)$Only constant signal, enlarging range, retrying...");
                            AFCam_MoveTargetThreadProc(OldPos);
                            AFCam_RunScanLoc(Target, bUpdateXY, Level + 1, bMoveToTarget);
                            AFCam_Scan_Range = oldrange;
                            AFCam_Scan_Step = oldstep;
                            return;
                        }
                    }
                }

                Target = OldPos;
                AFCam_MoveTargetThreadProc(Target - 0.1);
                AFCam_MoveTargetThreadProc(Target);
                XYPlot_SetText(IDXYScan, "BottomSubtitle", "FAILED: contrast calculation found no peak");
                XYPlot_Update(IDXYScan);
                AddLog("$Prefix(1)$Autofocus scan failed: contrast calculation found no peak");
                return;
            }

            g_AFPeak = HighestPeak;
            if (Noise > 0)
            {
                g_AFSNR = HighestPeak / Noise;
            }
            else
            {
                g_AFSNR = 0;
            }

            if (g_AFSNR < AFCam_Scan_SNRLimit) {
                XYPlot_SetText(IDXYScan, "BottomSubtitle", "FAILED: SNR too low/ Signal Level: " + std::to_string((HighestPeak / 255) * 100) + "% / SNR = " + std::to_string(HighestPeak / Noise));
                XYPlot_SetText(IDXYScan, "Subtitle", "AF: Target set: " + std::to_string(H));
                Target = OldPos;
                AFCam_MoveTargetThreadProc(Target - 0.1);
                AFCam_MoveTargetThreadProc(Target);
                XYPlot_SetText(IDXYScan, "BottomSubtitle", "FAILED:" + std::to_string(Ret) + " / Count = " + std::to_string(Count) + "; Range = ±" + std::to_string(Range));
                XYPlot_SetText(IDXYScan, "Subtitle", "AF: Target set: " + std::to_string(H));
                XYPlot_Update(IDXYScan);
                AddLog("$Prefix(1)$Autofocus scan failed: SNR too low : " + std::to_string(g_AFSNR) + " < " + std::to_string(AFCam_Scan_SNRLimit));
                return;
            }

            if (Target >= PosFrom && Target < PosTo) {
                if (!bMoveToTarget) {
                    Target = OldPos;
                }
                AFCam_MoveTargetThreadProc(Target);
            }

            XYPlot_SetText(IDXYScan, "BottomSubtitle", "Count=" + std::to_string(Count) + " Expected=" + std::to_string(Expected) + "; Range = " + std::to_string(Range) + " mm; Width = " + std::to_string(Width * 1000) + " um");

            if (AFMode == AFMode_P1_Target)
            {
                if (AFPeak1Valid)
                {
                    XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
            }

            if (AFMode == AFMode_P2_Target)
            {
                if (AFPeak2Valid)
                {
                    XYPlot.SetText IDXYScan Subtitle "AF : Target[2] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Target[2] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak1Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                }
            }

            if (AFMode == AFMode_P3_Target)
            {
                if (AFPeak3Valid)
                {
                    XYPlot.SetText IDXYScan Subtitle "AF : Target[3] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Target[3] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak1Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                }
            }

            if (AFMode == AFMode_P1_Target_Dist12)
            {
                if ((AFPeak1Valid == True) && (AFPeak2Valid == True))
                {
                    dbl Dist = abs(AFPeak1Position - AFPeak2Position) * 1000
                        XYPlot.SetText IDXYScan Subtitle "AF : Distance = $dist:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Distance = $dist:2$ µm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak1Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }

                }
            }

            if (AFMode == AFMode_P2_Target_Dist12)
            {
                if ((AFPeak1Valid == True) && (AFPeak2Valid == True))
                {
                    dbl Dist = abs(AFPeak1Position - AFPeak2Position) * 1000
                        XYPlot.SetText IDXYScan Subtitle "AF : Distance = $dist:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Distance = $dist:2$ µm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak2Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[2] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[2] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        if (AFPeak1Valid)
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                        else
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                    }
                }
            }

            if (AFMode == AFMode_P3_Target_Dist13)
            {
                if ((AFPeak1Valid == True) && (AFPeak3Valid == True))
                {
                    dbl Dist = abs(AFPeak1Position - AFPeak3Position) * 1000
                        XYPlot.SetText IDXYScan Subtitle "AF : Distance = $dist:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Distance = $dist:2$ µm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak3Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[3] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[3] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        if (AFPeak1Valid)
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                        else
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[1] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Invalid Target[1] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                    }
                }

            }

            if (AFMode == AFMode_P3_Target_Dist23)
            {
                if ((AFPeak2Valid == True) && (AFPeak3Valid == True))
                {
                    dbl Dist = abs(AFPeak2Position - AFPeak3Position) * 1000
                        XYPlot.SetText IDXYScan Subtitle "AF : Distance = $dist:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        Addlog "$Prefix(1)$Autofocus success: Distance = $dist:2$ µm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                }
                else
                {
                    if (AFPeak3Valid)
                    {
                        XYPlot.SetText IDXYScan Subtitle "AF : Target[3] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                            Addlog "$Prefix(1)$Autofocus success: Target[3] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                    }
                    else
                    {
                        if (AFPeak2Valid)
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Target[2] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Target[2] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                        else
                        {
                            XYPlot.SetText IDXYScan Subtitle "AF : Invalid Target[2] = $target:5$ mm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                                Addlog "$Prefix(1)$Autofocus success: Invalid Target[2] = $target:5$ mm / Width=$g_AFWidth:2$ µm / Signal Level: $max/255*100:0$ % / SNR = $HighestPeak/Noise:0$"
                        }
                    }
                }
            }

            XYPlot.update  IDXYScan;

            g_AFTime = 0;

            string AFTitle;

            //hwndMain
            std::string sf;
            sf = "{scriptfolder}afscan.bmp";
            ReplacePath(sf);
            Camera.Pylon.FrameBuffer.savebitmap(IDAFCam, sf);
        }
    }
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
