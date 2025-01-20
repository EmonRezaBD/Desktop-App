
// MFCApplication1Dlg.h : header file
//

#pragma once
#include<string>
// CMFCApplication1Dlg dialog
class CMFCApplication1Dlg : public CDialogEx
{
// Construction
public:
	CMFCApplication1Dlg(CWnd* pParent = nullptr);	// standard constructor

// Dialog Data
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_MFCAPPLICATION1_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnBnClickedGetTemp();
public:
	//Pipe Variables
	HANDLE hPipe;

	void openPipe();
	void closePipe();

	//Read operations from C#
	void readDataFromCSharp();
	DWORD bytesRead;
	char buffer[100];

	//Write opeartions to C#
	DWORD bytesWritten; 
	void writeDataToCSharp();
	std::string sendCommand; //Command to initiate temp reading op in C#

	//Update UI
	CStatic tempData;//to update the data in C++ UI
	void updateUI();

};
