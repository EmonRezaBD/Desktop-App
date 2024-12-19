#include <stdio.h>
#include <windows.h>
#include<iostream>
#include <string>
//#include <bits/stdc++.h>
using namespace std;

HANDLE hPipe1;
BOOL Finished;

typedef struct {
    int intVal;
    double doubVal;
    string str;
}arup;
int main(int argc, char* argv[]) {
    unsigned char buf[100] = {'0'};
    LPCWSTR Pipename = L"\\\\.\\pipe\\myNamedPipe1";
    DWORD cbWritten;
    DWORD dwBytesToWrite = (DWORD)sizeof(buf)/sizeof(buf[0]);
    BOOL Write_St = TRUE;
    Finished = FALSE;
    hPipe1 = CreateFile(Pipename, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
    if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE)) {
        cout << "Could not open the pipe ";
    }
    else {
        do {
            cout << "Enter the element to be added: ";
            //cin >> buf;
            arup var;
            cin >> var.intVal;
            cin >> var.doubVal;
            cin >> var.str;
            //cin >> var.doubleVal;

           // buf[0] = '0';

            buf[0] = (unsigned char)(var.intVal >> 24);
            buf[1] = (unsigned char)(var.intVal >> 16);
            buf[2] = (unsigned char)(var.intVal >> 8);
            buf[3] = (unsigned char)var.intVal;



            //string tmp = std::to_string(var.intVal);
            //int idx = 4;
            //for (auto i : tmp)
            //{
            //    buf[idx] = i;
            //    idx++;
            //}
            //tmp = to_string(var.doubleVal);
            //cout << "double value is --> " << tmp << endl;
            int idx = 4;
            for (auto i : var.str)
            {
                buf[idx] = i;
                idx++;
            }
            //for (auto i : var.str)
            //{
            //    buf[idx] = i;
            //    idx++;
            //}
            buf[idx] = '\0';
            cout << buf << endl;
            if (strcmp((char*)buf, "quit") == 0) Write_St = FALSE;
            else {
                WriteFile(hPipe1, buf, dwBytesToWrite, &cbWritten, NULL);
                memset(buf, 0xCC, 100);
            }
        } while (Write_St);
        CloseHandle(hPipe1);
        Finished = TRUE;
    }
    getchar();
}