// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class TaskDialog
{
    private class WindowSubclassHandler : Forms.WindowSubclassHandler
    {
        private readonly TaskDialog _taskDialog;

        public WindowSubclassHandler(TaskDialog taskDialog)
            : base((HWND)taskDialog.OrThrowIfNull().Handle)
        {
            _taskDialog = taskDialog;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case ContinueButtonClickHandlingMessage:
                    // We received the message which we posted earlier when
                    // handling a TDN_BUTTON_CLICKED notification, so we should
                    // no longer ignore such notifications.
                    // We do not forward the message to the base class.
                    _taskDialog._ignoreButtonClickedNotifications = false;
                    break;
                case PInvoke.WM_CTLCOLORBTN:
                    m.ResultInternal = (LRESULT)PInvoke.CreateSolidBrush(SystemColors.Control).Value;
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override bool CanCatchWndProcException(Exception ex) => CanCatchCallbackException();

        protected override void HandleWndProcException(Exception ex) => HandleCallbackException(ex);
    }

    private class PageSubclassHandler : Forms.WindowSubclassHandler
    {
        private readonly HWND _pageDialog;

        public PageSubclassHandler(HWND taskDialog)
            : base((HWND)taskDialog.OrThrowIfNull().Value)
        {
            _pageDialog = taskDialog;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvoke.WM_ERASEBKGND:
                    m.ResultInternal  = new LRESULT(1);
                    break;
                case PInvoke.WM_CTLCOLORDLG:
                case PInvoke.WM_CTLCOLOREDIT:
                case PInvoke.WM_CTLCOLORSTATIC:
                case PInvoke.WM_CTLCOLORBTN:
                    m.ResultInternal = (LRESULT)PInvoke.CreateSolidBrush(SystemColors.Control).Value;
                    break;
                case PInvoke.WM_PAINT:
                    base.WndProc(ref m);
                    HDC hdc = PInvokeCore.GetDC(m.HWND);
                    RECT rcClint = new RECT();
                    PInvokeCore.GetClientRect(m.HWND, out rcClint);
                    rcClint.top = rcClint.bottom - 50;
                    PInvoke.FillRect(hdc, rcClint, PInvoke.CreateSolidBrush(SystemColors.Control));
                    PInvokeCore.ReleaseDC(m.HWND, hdc);
                    // We received the message which we posted earlier when
                    // handling a TDN_BUTTON_CLICKED notification, so we should
                    // no longer ignore such notifications.
                    // We do not forward the message to the base class.\
                    break;
                case PInvoke.WM_DESTROY:
                    base.WndProc(ref m);
                    Dispose();
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override bool CanCatchWndProcException(Exception ex) => CanCatchCallbackException();

        protected override void HandleWndProcException(Exception ex) => HandleCallbackException(ex);
    }

   

    public class ButtonWindow : NativeWindow
    {
        internal virtual HBRUSH InitializeDCForWmCtlColor(HDC dc, MessageId msg)
        {
            {
                PInvoke.SetTextColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.WindowText));
                PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.Control));
                return PInvoke.CreateSolidBrush(SystemColors.Control);
            }
        }
        private void WmCtlColorControl(ref Message m)
        {
            // We could simply reflect the message, but it's faster to handle it here if possible.
            Control? control = Control.FromHandle(m.LParamInternal);
            if (control is not null)
            {
                m.ResultInternal = (LRESULT)InitializeDCForWmCtlColor((HDC)(nint)m.WParamInternal, m.MsgInternal).Value;
                if (m.ResultInternal != 0)
                {
                    return;
                }
            }

            DefWndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
          //  PInvoke.SetClassLong(m.HWnd)

            string className = GetClassName(m.HWND);

            switch (m.MsgInternal)
            {

                //case PInvoke.WM_ERASEBKGND:
                //    m.ResultInternal = new LRESULT(1);
                //    break;
                case PInvoke.WM_CTLCOLORDLG:
                case PInvoke.WM_CTLCOLOREDIT:
                case PInvoke.WM_CTLCOLORSTATIC:
                case PInvoke.WM_CTLCOLORBTN:
                    WmCtlColorControl(ref m);
                    break;
                case PInvoke.WM_ERASEBKGND:
                case PInvoke.WM_PAINT:
                   // base.WndProc(ref m);
                   PAINTSTRUCT PS = new PAINTSTRUCT();
                    HDC hdc = PInvoke.BeginPaint(m.HWND, out PS);
                    RECT rcClint = new RECT();
                    PInvokeCore.GetClientRect(m.HWND, out rcClint);
                    // rcClint.top = rcClint.bottom - 50;
                    PInvoke.FillRect(hdc, rcClint, PInvoke.CreateSolidBrush(SystemColors.Control));
                    PInvoke.EndPaint(m.HWND, PS);

                 //   HDC hdc = PInvokeCore.GetDC(m.HWND);
                 //   RECT rcClint = new RECT();
                 //   PInvokeCore.GetClientRect(m.HWND, out rcClint);
                 //   // rcClint.top = rcClint.bottom - 50;
                 ////   PInvoke.FillRect(hdc, rcClint, PInvoke.CreateSolidBrush(SystemColors.Control));
                 //   PInvokeCore.ReleaseDC(m.HWND, hdc);
                    break;
                case PInvoke.WM_NCDESTROY:
                    ReleaseHandle();
                    base.WndProc(ref m);
                    break;
                default: base.WndProc(ref m);
                    break;   
            }
          
        }
    }
}
